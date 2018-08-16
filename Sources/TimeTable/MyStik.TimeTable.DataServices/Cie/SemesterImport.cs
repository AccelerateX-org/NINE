using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Cie.Data;

namespace MyStik.TimeTable.DataServices.Cie
{
    public class SemesterImport
    {
        private ImportContext _import;

        private Guid _semId;


        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");


        private Semester _semester;

        private bool _isValid = true;

        private StringBuilder _report = new StringBuilder();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="import"></param>
        /// <param name="semId">Für dieses Semester werden die Semestergruppen gesucht</param>
        public SemesterImport(ImportContext import, Guid semId)
        {
            _import = import;
            _semId = semId;

            _report.AppendLine("<html>");
            _report.AppendLine("<body>");
        }

        /// <summary>
        /// Suche nach unbekannten Fakultäten
        /// </summary>
        public void CheckFaculty()
        {
            var db = new TimeTableDbContext();

            _semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            if (_semester == null)
            {
                _import.AddErrorMessage("Import", $"Semester {_semId} existiert nicht", true);
                _isValid = false;
            }

            // Jede Fakultät suchen
            foreach (var course in _import.Model.Courses)
            {
                var org = db.Organisers.SingleOrDefault(x => x.ShortName.Equals(course.department));

                if (org != null)
                {
                    // Studiengang suchen => mache ich erst beim Import

                    _import.ValidCourses.Add(course);
                }
                else
                {
                    if (string.IsNullOrEmpty(course.department))
                    {
                        _import.AddErrorMessage("Import", $"Für Kurs {course.name} wurde kein Veranstalter angegeben", true);
                    }
                    else
                    {
                        _import.AddErrorMessage("Import", $"Für Kurs {course.name} gibt es den Veranstalter {course.department} nicht", true);
                    }
                    _isValid = false;
                }
            }
        }

        public void CheckRooms()
        {
            if (!_isValid)
                return;
        }

        public void CheckLecturers()
        {
            if (!_isValid)
                return;
        }

        public string GetReport()
        {
            _report.AppendLine("</body>");
            _report.AppendLine("</html>");

            return _report.ToString();
        }

        public string ImportCourse(CieCourse scheduleCourse)
        {
            string msg;

            _Logger.DebugFormat("Importiere Fach: {0}", scheduleCourse.name);
            _report.AppendFormat("<h1>Erzeuge LV \"{0}\" - [{1}]</h1>", scheduleCourse.name,
                scheduleCourse.id);
            _report.AppendLine();

            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.ShortName.Equals(scheduleCourse.department));
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            var cieSemGroup = db.SemesterGroups.SingleOrDefault(x =>
                x.Semester.Id == sem.Id &&
                x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE") &&
                x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE") &&
                x.CapacityGroup.CurriculumGroup.Name.Equals("CIE"));

            long msStart = sw.ElapsedMilliseconds;
            // suche den Kurs im bisherigen Angebot auf Grundlage des Namens
            var course = GetCourse(db, sem, organiser, scheduleCourse);

            if (course == null)
            {
                course = new Course
                {
                    ExternalSource = "CIE",
                    ExternalId = scheduleCourse.id,
                    Organiser = organiser,
                    ShortName = "",
                    Name = scheduleCourse.name,
                    Description = scheduleCourse.description,
                    Occurrence = CreateDefaultOccurrence(scheduleCourse.availableSlots),
                    IsInternal = true,
                };
                // Kurs sofort speichern, damit die ID gesichert ist
                db.Activities.Add(course);
                db.SaveChanges();
            }
            long msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer: {0}ms", msEnd - msStart);
            msStart = msEnd;

            _report.AppendLine("<h2>Bezeichnungen</h2>");
            _report.AppendLine("<table>");
            _report.AppendFormat("<tr><td>Name</td><td>{0}</td></tr>", course.Name);
            _report.AppendFormat("<tr><td>Kurzname</td><td>{0}</td></tr>", course.ShortName);
            _report.AppendFormat("<tr><td>Beschreibung</td><td>{0}</td></tr>", course.Description);
            _report.AppendLine("</table>");


            // jetzt die Gruppen
            // Studiengänge ermitteln
            // Den Kurs gibt es nur einmal
            // Zuordnungen pro Studiengang
            // Modul => d.h. duplizieren von ECTS, SWS
            var curricula= GetCurricula(db, sem, scheduleCourse.department, scheduleCourse.level);

            // Semestergruppe => auch hier eine "intelligente" Zuordnung
            // nur machen, wenn Kurs keine Gruppen hat
            if (!course.SemesterGroups.Any())
            {
                foreach (var curriculum in curricula)
                {
                    // Annahme: ein CIE sind immer WPM
                    var wpmCurrciculumGroup = curriculum.CurriculumGroups.SingleOrDefault(x => x.Name.Equals("WPM"));

                    var wpmCapacityGroup = wpmCurrciculumGroup.CapacityGroups.FirstOrDefault();

                    var wpmSemesterGroup = db.SemesterGroups.FirstOrDefault(x =>
                        x.CapacityGroup.Id == wpmCapacityGroup.Id && x.Semester.Id == sem.Id);

                    course.SemesterGroups.Add(wpmSemesterGroup);


                    // zu jeder Semestergruppe gibt es dann noch eine 
                    // Gruppe für Platzverlosung
                    var occGroup =
                        course.Occurrence.Groups.SingleOrDefault(
                            gg => gg.SemesterGroups.Any(s => s.Id == wpmSemesterGroup.Id));

                    if (occGroup == null)
                    {
                        occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(wpmSemesterGroup);
                        wpmSemesterGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        db.OccurrenceGroups.Add(occGroup);
                    }


                }
            }

            // Die Semestergruppe FK 13 / CIE / CIE muss immer dran
            if (!course.SemesterGroups.Contains(cieSemGroup))
            {
                course.SemesterGroups.Add(cieSemGroup);
            }
            db.SaveChanges();

            if (!course.SemesterGroups.Any())
            {
                _Logger.ErrorFormat("Kurs {0} ohne Gruppe", scheduleCourse.id);
            }

            // jetzt die Module
            // wieder pro Studiengang
            foreach (var curriculum in curricula)
            {
                var module = GetModule(db, curriculum, scheduleCourse);

                var nexus = new CourseModuleNexus();
                nexus.Requirement = module;
                nexus.Course = course;
                // nexus.ModuleCourse = // fehlt noch

                course.Nexus.Add(nexus);

                db.CourseNexus.Add(nexus);
            }
            db.SaveChanges();


            // Das Ampelsystem
            if (scheduleCourse.courseStatus.Equals("RED"))
            {
                course.Occurrence.IsCoterie = true;
                course.Occurrence.HasHomeBias = true; // Bedeutungslos
            }
            else if (scheduleCourse.courseStatus.Equals("YELLOW"))
            {
                course.Occurrence.IsCoterie = false;
                course.Occurrence.HasHomeBias = true;

            }
            else //(scheduleCourse.courseStatus.Equals("GREEN"))
            {
                course.Occurrence.IsCoterie = false;
                course.Occurrence.HasHomeBias = false;
            }

            db.SaveChanges();

            // zum Schluss die Termine
            // Dummy Termin mit Dozent
            _report.AppendLine("<h2>Neue Termine</h2>");
            _report.AppendLine("<table>");

            // Der Dummy Termin ist Vorlesungsbeginn 08:00-10:00
            var occDate = sem.StartCourses.Date;

            var occ = new ActivityDate
            {
                Begin = occDate.AddHours(8),
                End = occDate.AddHours(10),
                Activity = course,
                Occurrence = CreateDefaultOccurrence(),
            };
            _report.AppendLine("<tr>");
                _report.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", occ.Begin.ToShortDateString(), occ.Begin.ToShortTimeString(), occ.End.ToShortTimeString());

                // Kein Raum
                _report.AppendFormat("<td>");
                _report.AppendFormat("</td>");

                // Dozent => der angegebene
                var lec = scheduleCourse.lecturer.Split(',');
                _report.AppendFormat("<td>");

            foreach (var s in lec)
            {
                var scheduleDateLecturer = s.Trim();
                _report.AppendFormat("<p>{0} ({1})", scheduleDateLecturer, scheduleDateLecturer);

                var lecturer = organiser.Members.SingleOrDefault(l => l.ShortName.Equals(scheduleDateLecturer) || l.Name.Equals(scheduleDateLecturer));
                if (lecturer == null)
                {
                    lecturer = new OrganiserMember
                    {
                        ShortName = scheduleDateLecturer,
                        Name = scheduleDateLecturer,
                        Role = String.Empty,
                        Description = String.Empty
                    };
                    organiser.Members.Add(lecturer);
                    db.Members.Add(lecturer);
                    db.SaveChanges();
                    _report.AppendFormat(" !!!NEUER DOZENT!!!");
                }

                occ.Hosts.Add(lecturer);
                _report.AppendFormat("</p>");

            }
            _report.AppendFormat("</td>");

                db.ActivityDates.Add(occ);

                _report.AppendLine();
            _report.AppendLine("</tr>");

            _report.AppendLine("</table>");
            db.SaveChanges();

            msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);

            msg = $"Kurs {course.ShortName} mit Terminen importiert";

            return msg;
        }


        public string UpdateCourse(Course c, CieCourse scheduleCourse)
        {
            string msg;

            _Logger.DebugFormat("Aktualisiere Fach: {0}", scheduleCourse.name);

            _report.AppendFormat("<h1>Aktualisiere LV \"{0} ({1})\" - [{2}]</h1>", scheduleCourse.name, scheduleCourse.name,
                scheduleCourse.id);
            _report.AppendLine();

            /*
            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            long msStart = sw.ElapsedMilliseconds;

            var course = db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == c.Id);

            // Ober sticht Unter => zuerst alle termine raus!
            _report.AppendLine("<h2>Gelöschte Termine</h2>");
            _report.AppendLine("<table>");
            foreach (var date in course.Dates.ToList())
            {
                _report.AppendLine("<tr>");
                _report.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", date.Begin.ToShortDateString(), date.Begin.ToShortTimeString(), date.End.ToShortTimeString());

                _report.AppendLine("<td>");
                foreach (var room in date.Rooms)
                {
                    _report.AppendFormat("<p>{0}</p>", room.Number);
                }
                _report.AppendLine("</td>");

                _report.AppendLine("<td>");
                foreach (var host in date.Hosts)
                {
                    _report.AppendFormat("<p>{0} ({1})</p>", host.Name, host.ShortName);
                }
                _report.AppendLine("</td>");

                _report.AppendFormat("<td>{0}</td>", date.Title);
                _report.AppendFormat("<td>{0}</td>", date.Description);
                _report.AppendFormat("<td>{0}</td>", date.Occurrence.Information);

                _report.AppendLine();
                _report.AppendLine("</tr>");


                db.Occurrences.Remove(date.Occurrence);

                foreach (var change in date.Changes.ToList())
                {
                    foreach (var notificationState in change.NotificationStates.ToList())
                    {
                        db.NotificationStates.Remove(notificationState);
                        change.NotificationStates.Remove(notificationState);
                    }
                    date.Changes.Remove(change);
                    db.DateChanges.Remove(change);
                }

                course.Dates.Remove(date);

                db.ActivityDates.Remove(date);
            }
            db.SaveChanges();

            _report.AppendLine("</table>");


            // Die Bezeichnung
            _report.AppendLine("<h2>Bezeichnungen</h2>");
            _report.AppendLine("<h3>Alt</h3>");
            _report.AppendLine("<table>");
            _report.AppendFormat("<tr><td>Name</td><td>{0}</td></tr>", course.Name);
            _report.AppendFormat("<tr><td>Kurzname</td><td>{0}</td></tr>", course.ShortName);
            _report.AppendFormat("<tr><td>Beschreibung</td><td>{0}</td></tr>", course.Description);
            _report.AppendLine("</table>");

            course.Name = scheduleCourse.Name;
            course.ShortName = scheduleCourse.ShortName;
            course.Description = scheduleCourse.Description;

            _report.AppendLine("<h3>Neu</h3>");
            _report.AppendLine("<table>");
            _report.AppendFormat("<tr><td>Name</td><td>{0}</td></tr>", course.Name);
            _report.AppendFormat("<tr><td>Kurzname</td><td>{0}</td></tr>", course.ShortName);
            _report.AppendFormat("<tr><td>Beschreibung</td><td>{0}</td></tr>", course.Description);
            _report.AppendLine("</table>");

            // Platzkontingent
            course.Occurrence.Capacity = scheduleCourse.SeatRestriction;

            // Termine einfügen
            // zum Schluss die Termine
            _report.AppendLine("<h2>Neue Termine</h2>");
            _report.AppendLine("<table>");

            foreach (var scheduleDate in scheduleCourse.Dates)
            {
                // Der Tag
                var occDate = scheduleDate.Begin.Date;

                bool isVorlesung = true;
                foreach (var sd in _semester.Dates)
                {
                    // Wenn der Termin in eine vorlesungsfreie Zeit fällt, dann nicht importieren
                    if (sd.From.Date <= occDate.Date &&
                        occDate.Date <= sd.To.Date &&
                        sd.HasCourses == false)
                    {
                        isVorlesung = false;
                    }
                }

                // Es muss Vorlesung sein und das Datum darf nicht existieren
                if (isVorlesung)
                {
                    var occ = new ActivityDate
                    {
                        Begin = scheduleDate.Begin,
                        End = scheduleDate.End,
                        Activity = course,
                        Occurrence = CreateDefaultOccurrence(),
                    };

                    _report.AppendLine("<tr>");
                    _report.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", occ.Begin.ToShortDateString(), occ.Begin.ToShortTimeString(), occ.End.ToShortTimeString());

                    _report.AppendFormat("<td>");
                    foreach (var scheduleDateRoom in scheduleDate.Rooms)
                    {
                        _report.AppendFormat("<p>{0}", scheduleDateRoom.RoomNumber);
                        if (!string.IsNullOrEmpty(scheduleDateRoom.RoomNumber))
                        {
                            var room = db.Rooms.SingleOrDefault(r => r.Number.Equals(scheduleDateRoom.RoomNumber));
                            if (room == null)
                            {
                                room = new Room
                                {
                                    Number = scheduleDateRoom.RoomNumber,
                                    Capacity = 0,
                                    Description = string.Empty,
                                    Owner = string.Empty,
                                };
                                db.Rooms.Add(room);
                                db.SaveChanges();

                                _numRooms++;

                                _report.AppendFormat(" !!!NEUER RAUM!!!");
                            }


                            var assignment = db.RoomAssignments.SingleOrDefault(x =>
                                x.Room.Id == room.Id &&
                                x.Organiser.Id == organiser.Id);
                            if (assignment == null)
                            {
                                assignment = new RoomAssignment
                                {
                                    Organiser = organiser,
                                    InternalNeedConfirmation = false, // offen für interne
                                    ExternalNeedConfirmation = true // geschlossen für externe
                                };

                                room.Assignments.Add(assignment);
                                db.RoomAssignments.Add(assignment);
                                db.SaveChanges();
                            }

                            occ.Rooms.Add(room);

                            _report.AppendFormat("</p>");
                        }
                    }
                    _report.AppendFormat("</td>");

                    _report.AppendFormat("<td>");
                    foreach (var scheduleDateLecturer in scheduleDate.Lecturers)
                    {
                        _report.AppendFormat("<p>{0} ({1})", scheduleDateLecturer.Name, scheduleDateLecturer.ShortName);
                        var lecturer = organiser.Members.SingleOrDefault(l => l.ShortName.Equals(scheduleDateLecturer.ShortName));
                        if (lecturer == null)
                        {
                            lecturer = new OrganiserMember
                            {
                                ShortName = scheduleDateLecturer.ShortName,
                                Name = scheduleDateLecturer.Name,
                                Role = String.Empty,
                                Description = String.Empty
                            };
                            organiser.Members.Add(lecturer);
                            db.Members.Add(lecturer);
                            db.SaveChanges();
                            _numLecturers++;

                            _report.AppendFormat(" !!!NEUER DOZENT!!!");
                        }

                        occ.Hosts.Add(lecturer);

                        _report.AppendFormat("</p>");

                    }
                    _report.AppendFormat("</td>");


                    db.ActivityDates.Add(occ);

                    _report.AppendLine();
                    _report.AppendLine("</tr>");

                }
            }
            _report.AppendLine("</table>");

            db.SaveChanges();

            long msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);
            */

            msg = $"Kurs {scheduleCourse.name} mit Terminen aktualisiert";

            return msg;
        }

        private static Occurrence CreateDefaultOccurrence(int c=-1)
        {
            return new Occurrence
            {
                Capacity = c,
                IsAvailable = true,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
                UseGroups = false,
            };
        }

        private List<TimeTable.Data.Curriculum> GetCurricula(TimeTableDbContext db, Semester semester, string orgName, string level)
        {
            var list = new List<TimeTable.Data.Curriculum>();

            if (orgName.Equals("FK 01") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAAR"));
            }
            if (orgName.Equals("FK 01") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAAR"));
            }


            if (orgName.Equals("FK 02") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BIB"));
            }
            if (orgName.Equals("FK 02") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BIM"));
            }


            if (orgName.Equals("FK 03") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAMB"));
            }
            if (orgName.Equals("FK 03") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAMB"));
            }

            if (orgName.Equals("FK 04") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAET"));
            }
            if (orgName.Equals("FK 04") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAET"));
            }

            if (orgName.Equals("FK 05") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BADMT"));
            }
            if (orgName.Equals("FK 05") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MADMT"));
            }

            if (orgName.Equals("FK 06") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAMT"));
            }
            if (orgName.Equals("FK 06") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAMT"));
            }

            if (orgName.Equals("FK 07") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAINF"));
            }
            if (orgName.Equals("FK 07") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAINF"));
            }

            if (orgName.Equals("FK 08") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BAGEO"));
            }
            if (orgName.Equals("FK 08") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MAGEO"));
            }

            if (orgName.Equals("FK 09") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "WI"));
            }
            if (orgName.Equals("FK 09") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "WIM"));
            }

            if (orgName.Equals("FK 10") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BABW"));
            }
            if (orgName.Equals("FK 10") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MD"));
            }

            if (orgName.Equals("FK 11") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BASA Präsenz"));
            }
            if (orgName.Equals("FK 11") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "AFSA"));
            }

            if (orgName.Equals("FK 12") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "DES"));
            }
            if (orgName.Equals("FK 12") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "DES-M"));
            }

            if (orgName.Equals("FK 13") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "IPM"));
            }
            if (orgName.Equals("FK 13") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "IKK"));
            }

            if (orgName.Equals("FK 14") && (level.Equals("BACHELOR") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "BATM"));
            }
            if (orgName.Equals("FK 14") && (level.Equals("MASTER") || level.Equals("ANY")))
            {
                list.Add(GetCurriculum(db, semester,orgName, "MATM"));
            }



            return list;
        }

        private TimeTable.Data.Curriculum GetCurriculum(TimeTableDbContext db, Semester semester, string orgName, string currName)
        {
            var org = db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));
            var curr = org.Curricula.SingleOrDefault(x => x.ShortName.Equals(currName));

            if (curr == null)
            {
                curr = new TimeTable.Data.Curriculum
                {
                    ShortName = currName,
                    Name = currName,
                    Organiser = org
                };
                db.Curricula.Add(curr);
                db.SaveChanges();
            }

            // Gleich alles notwendige anlegen
            var wpmCurrciculumGroup = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals("WPM"));
            if (wpmCurrciculumGroup == null)
            {
                wpmCurrciculumGroup = new CurriculumGroup
                {
                    Curriculum = curr,
                    Name = "WPM",
                    IsSubscribable = true,
                };
                db.CurriculumGroups.Add(wpmCurrciculumGroup);
                db.SaveChanges();

            }

            var wpmCapacityGroup = wpmCurrciculumGroup.CapacityGroups.FirstOrDefault();
            if (wpmCapacityGroup == null)
            {
                wpmCapacityGroup = new CapacityGroup
                {
                    CurriculumGroup = wpmCurrciculumGroup,
                    Name = string.Empty,
                    InSS = true,
                    InWS = true
                };
                db.CapacityGroups.Add(wpmCapacityGroup);
                db.SaveChanges();
            }

            var wpmSemesterGroup = db.SemesterGroups.FirstOrDefault(x =>
                x.CapacityGroup.Id == wpmCapacityGroup.Id && x.Semester.Id == semester.Id);

            if (wpmSemesterGroup == null)
            {
                wpmSemesterGroup = new SemesterGroup
                {
                    Semester = semester,
                    CapacityGroup = wpmCapacityGroup,
                    IsAvailable = false // zu Beginn nicht freigegeben
                };
                db.SemesterGroups.Add(wpmSemesterGroup);



                db.SaveChanges();

            }


            return curr;
        }

        private Course GetCourse(TimeTableDbContext db, Semester sem, ActivityOrganiser org, CieCourse course)
        {
            // alle Kurse im Semester des Organisers nach dem Namen durchsuchen


            return null;
        }


        private CurriculumRequirement GetModule(TimeTableDbContext db, TimeTable.Data.Curriculum curriculum, CieCourse scheduleCourse)
        {
            var pck = curriculum.Packages.FirstOrDefault();
            if (curriculum.ShortName.Equals("WI") && pck != null)
            {
                pck = curriculum.Packages.SingleOrDefault(x => x.Name.Equals("Wahlpflicht"));
            }

            if (pck == null)
            {
                pck = new CurriculumPackage();
                pck.Name = "Studium";
                curriculum.Packages.Add(pck);
                db.CurriculumPackages.Add(pck);
            }

            var option = pck.Options.FirstOrDefault();

            if (option == null)
            {
                option = new PackageOption();
                option.Name = "Gesamt";
                option.Package = pck;
                db.PackageOptions.Add(option);
            }

            var module =
                option.Requirements.FirstOrDefault(x => x.Name.ToLower().Equals(scheduleCourse.name.ToLower()));

            if (module == null)
            {
                module = new CurriculumRequirement();
                module.Name = scheduleCourse.name;
                module.ECTS = scheduleCourse.ects;
                module.USCredits = scheduleCourse.usCredits;
                module.SWS = scheduleCourse.semesterWeekHours;

                option.Requirements.Add(module);
            }

            db.SaveChanges();

            return module;
        }
    }
}

