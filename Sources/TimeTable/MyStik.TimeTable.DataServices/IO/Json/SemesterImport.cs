﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Json.Data;

namespace MyStik.TimeTable.DataServices.IO.Json
{
    public class SemesterImport
    {
        private ImportContext _import;

        private Guid _semId;
        private Guid _orgId;


        private int _numRooms;
        private int _numLecturers;

        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");


        private ActivityOrganiser _organiser;
        private Semester _semester;

        private bool _isValid = true;

        private StringBuilder _report = new StringBuilder();


        public SemesterImport(ImportContext import, Guid semId, Guid orgId)
        {
            _import = import;
            _semId = semId;
            _orgId = orgId;

            _report.AppendLine("<html>");
            _report.AppendLine("<body>");
        }


        public void CheckFaculty()
        {
            var db = new TimeTableDbContext();

            _organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            _semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            // Anzahl der Semestergruppen, die zu meiner Fakultät passen
            var countFacultyFit = _import.Model.Courses.Count(x => 
                x.Groups != null &&     
                x.Groups.Any(g => 
                    !string.IsNullOrEmpty(g.FacultyName) &&
                    g.FacultyName.Equals(_organiser.ShortName)));

            var countSemesterFit = _import.Model.Courses.Count(x =>
                x.Groups != null &&
                x.Groups.Any(g => 
                    !string.IsNullOrEmpty(g.SemesterName) &&
                    g.SemesterName.Equals(_semester.Name)));


            // 0 geht gar nicht!
            if (countFacultyFit == 0)
            {
                _import.AddErrorMessage("Import", $"keine Datem für {_organiser.ShortName}", true);
                _isValid = false;
            }

            if (countSemesterFit == 0)
            {
                _import.AddErrorMessage("Import", $"keine Datem für {_semester.Name}", true);
                _isValid = false;
            }

            if (_isValid)
            {
                _import.ValidCourses.AddRange(_import.Model.Courses);
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


        public string ImportActivity(ScheduleCourse scheduleCourse)
        {
            if (scheduleCourse.Groups.Any())
            {
                bool isCourse = !scheduleCourse.Groups.Any(g => string.IsNullOrEmpty(g.CurriculumShortName));
                if (isCourse)
                    return ImportCourse(scheduleCourse);
            }

            return ImportReservation(scheduleCourse);
        }



        public string ImportCourse(ScheduleCourse scheduleCourse)
        {
            string msg;

            _Logger.DebugFormat("Importiere Fach: {0}", scheduleCourse.Name);
            _report.AppendFormat("<h1>Erzeuge LV \"{0} ({1})\" - [{2}]</h1>", scheduleCourse.Name, scheduleCourse.ShortName,
                scheduleCourse.CourseId);
            _report.AppendLine();

            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            long msStart = sw.ElapsedMilliseconds;
            var course = new Course
            {
                ExternalSource = "JSON",
                ExternalId = scheduleCourse.CourseId,
                Organiser = organiser,
                ShortName = scheduleCourse.ShortName,
                Name = scheduleCourse.Name,
                Description = scheduleCourse.Description,
                Occurrence = CreateDefaultOccurrence(scheduleCourse.SeatRestriction ?? 0),
                IsInternal = true,
            };
            // Kurs sofort speichern, damit die ID gesichert ist
            db.Activities.Add(course);
            db.SaveChanges();
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
            foreach (var scheduleGroup in scheduleCourse.Groups)
            {
                // Fakultät ermitteln
                var org = db.Organisers.SingleOrDefault(x => x.ShortName.Equals(scheduleGroup.FacultyName));

                // Studiengang innerhalb der Fakultät ermitteln
                var curr = org.Curricula.SingleOrDefault(x => x.ShortName.Equals(scheduleGroup.CurriculumShortName));
                if (curr == null)
                {
                    curr = new TimeTable.Data.Curriculum
                    {
                        ShortName = scheduleGroup.CurriculumShortName,
                        Name = scheduleGroup.CurriculumName,
                        Organiser = org
                    };
                    db.Curricula.Add(curr);
                    db.SaveChanges();
                }

                // Studiengruppe innerhalb des Studiengangs ermitteln
                var groupName = scheduleGroup.GroupName;

                // Sonderlocke FK 11
                // aus der LV-Nummer das Semester raussuchen
                /* OHI 20180720: wieder ausgebaut, weil Schnittstelle jetzt sauber befüllt ist
                if (org.ShortName.Equals("FK 11"))
                {
                    if (!string.IsNullOrEmpty(course.ShortName))
                    {
                        groupName = course.ShortName[1].ToString();

                    }
                    else
                    {
                        groupName = "#N.V.";
                    }
                }
                */

                var currGroup = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(groupName));
                if (currGroup == null)
                {
                    currGroup = new CurriculumGroup
                    {
                        Curriculum = curr,
                        Name = groupName,
                        IsSubscribable = true,
                    };
                    db.CurriculumGroups.Add(currGroup);
                    db.SaveChanges();
                }

                // Kapazitätsgruppe innerhalb der Studiengruppe ermitteln
                CapacityGroup capGroup = null;
                if (string.IsNullOrEmpty(scheduleGroup.SubGroupName))
                {
                    capGroup = currGroup.CapacityGroups.SingleOrDefault(x => string.IsNullOrEmpty(x.Name));
                    if (capGroup == null)
                    {
                        capGroup = new CapacityGroup
                        {
                            CurriculumGroup = currGroup,
                            Name = string.Empty,
                            InSS = scheduleGroup.SemesterName.StartsWith("SS"),
                            InWS = scheduleGroup.SemesterName.StartsWith("WS")
                        };
                        db.CapacityGroups.Add(capGroup);
                        db.SaveChanges();
                    }
                }
                else
                {
                    capGroup = currGroup.CapacityGroups.SingleOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(scheduleGroup.SubGroupName));
                    if (capGroup == null)
                    {
                        capGroup = new CapacityGroup
                        {
                            CurriculumGroup = currGroup,
                            Name = scheduleGroup.SubGroupName,
                            InSS = scheduleGroup.SemesterName.StartsWith("SS"),
                            InWS = scheduleGroup.SemesterName.StartsWith("WS")
                        };
                        db.CapacityGroups.Add(capGroup);
                        db.SaveChanges();
                    }

                }

                // Semester ermitteln
                var semester = db.Semesters.SingleOrDefault(x => x.Name.Equals(scheduleGroup.SemesterName));

                if (semester.Id == sem.Id)
                {
                    // jetzt können wir die Semestergruppe ermitteln
                    var semGroup =
                        db.SemesterGroups.SingleOrDefault(x => x.Semester.Id == semester.Id &&
                                                               x.CapacityGroup.Id == capGroup.Id);
                    if (semGroup == null)
                    {
                        semGroup = new SemesterGroup
                        {
                            Semester = semester,
                            CapacityGroup = capGroup,
                            IsAvailable = false // zu Beginn nicht freigegeben
                        };
                        db.SemesterGroups.Add(semGroup);
                        db.SaveChanges();
                    }

                    course.SemesterGroups.Add(semGroup);
                    semGroup.Activities.Add(course);

                    // zu jeder Semestergruppe gibt es dann noch eine 
                    // Gruppe für Platzverlosung
                    var occGroup =
                        course.Occurrence.Groups.SingleOrDefault(
                            gg => gg.SemesterGroups.Any(s => s.Id == semGroup.Id));

                    if (occGroup == null)
                    {
                        occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semGroup);
                        semGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        db.OccurrenceGroups.Add(occGroup);
                    }

                    // NEU: Chapter und Topics
                    if (!string.IsNullOrEmpty(scheduleGroup.ChapterName) &&
                        !string.IsNullOrEmpty(scheduleGroup.TopicName))
                    {
                        var chapter = curr.Chapters.FirstOrDefault(x => x.Name.Equals(scheduleGroup.ChapterName));
                        if (chapter == null)
                        {
                            chapter = new CurriculumChapter
                            {
                                Curriculum = curr,
                                Name = scheduleGroup.ChapterName
                            };
                            db.CurriculumChapters.Add(chapter);
                        }

                        var topic = chapter.Topics.FirstOrDefault(x => x.Name.Equals(scheduleGroup.TopicName));
                        if (topic == null)
                        {
                            topic = new CurriculumTopic
                            {
                                Chapter = chapter,
                                Name = scheduleGroup.TopicName
                            };
                            db.CurriculumTopics.Add(topic);
                        }

                        var semTopic = db.SemesterTopics.FirstOrDefault(x =>
                            x.Semester.Id == sem.Id && x.Topic.Id == topic.Id);

                        if (semTopic == null)
                        {
                            semTopic = new SemesterTopic
                            {
                                Semester = sem,
                                Topic = topic,
                            };
                            db.SemesterTopics.Add(semTopic);
                        }

                        semTopic.Activities.Add(course);
                    }
                }
                // else => Semestergruppe wird nicht angelegt
            }

            db.SaveChanges();

            if (!course.SemesterGroups.Any())
            {
                _Logger.ErrorFormat("Kurs {0} ohne Gruppe", scheduleCourse.CourseId);
            }

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

            msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);

            msg = $"Kurs {course.ShortName} mit Terminen importiert";

            return msg;
        }


        public string ImportReservation(ScheduleCourse scheduleCourse)
        {
            string msg;

            _Logger.DebugFormat("Importiere Raumreservierung: {0}", scheduleCourse.Name);
            _report.AppendFormat("<h1>Erzeuge LV \"{0} ({1})\" - [{2}]</h1>", scheduleCourse.Name, scheduleCourse.ShortName,
                scheduleCourse.CourseId);
            _report.AppendLine();

            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            long msStart = sw.ElapsedMilliseconds;
            var course = new Reservation
            {
                ExternalSource = "JSON",
                ExternalId = scheduleCourse.CourseId,
                Organiser = organiser,
                ShortName = scheduleCourse.ShortName,
                Name = scheduleCourse.Name,
                Description = scheduleCourse.Description,
                Occurrence = null,
                IsInternal = true,
            };
            // Kurs sofort speichern, damit die ID gesichert ist
            db.Activities.Add(course);
            db.SaveChanges();
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
            // Eine Raumreservierung hat keine Gruppen

            db.SaveChanges();

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

                if (isVorlesung)
                {
                    var occ = new ActivityDate
                    {
                        Begin = scheduleDate.Begin,
                        End = scheduleDate.End,
                        Activity = course,
                        Occurrence = null,
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

            msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);

            msg = $"Raumreservierung {course.ShortName} mit Terminen importiert";

            return msg;
        }



        public string UpdateCourse(Course c, ScheduleCourse scheduleCourse)
        {
            string msg;

            _Logger.DebugFormat("Aktualisiere Fach: {0}", scheduleCourse.Name);

            _report.AppendFormat("<h1>Aktualisiere LV \"{0} ({1})\" - [{2}]</h1>", scheduleCourse.Name, scheduleCourse.ShortName,
                scheduleCourse.CourseId);
            _report.AppendLine();

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
            course.Occurrence.Capacity = scheduleCourse.SeatRestriction ?? 0;

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

            msg = $"Kurs {course.ShortName} mit Terminen aktualisiert";

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

        public string ImportLottery(CourseLottery lottery)
        {
            string msg;

            _Logger.DebugFormat("Importiere Lotterie: {0}", lottery.Name);
            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            var dbLottery = db.Lotteries.FirstOrDefault(x =>
                x.Name.Equals(lottery.Name) &&
                x.Semester != null && x.Semester.Id == sem.Id &&
                x.Organiser != null && x.Organiser.Id == organiser.Id);

            if (dbLottery == null)
            {
                dbLottery = new TimeTable.Data.Lottery
                {
                    Name = lottery.Name,
                    DrawingFrequency = DrawingFrequency.Daily,
                    DrawingTime = lottery.FirstDrawing.TimeOfDay,
                    FirstDrawing = lottery.FirstDrawing,
                    LastDrawing = lottery.LastDrawing,
                    JobId = string.Empty,
                    Organiser = organiser,
                    Semester = sem,
                    MaxConfirm = lottery.SlotCount,
                    Description = string.Empty
                };

                db.Lotteries.Add(dbLottery);
                db.SaveChanges();
            }

            // ok: im Augenblick gehen wir davon aus, dass als externe ID die LehrveranID aus vdb_t07 kommt
            // damit müssten alle importieren Kurse eindeutig identifizierbar sein
            foreach (var lotteryLot in lottery.Lots)
            {
                var course = db.Activities.OfType<Course>().FirstOrDefault(x =>
                    x.ExternalId.Equals(lotteryLot.CourseId) &&
                    x.SemesterGroups.Any(g => g.Semester.Id == sem.Id && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id));

                if (course != null)
                {
                    dbLottery.Occurrences.Add(course.Occurrence);
                }

            }
            db.SaveChanges();


            msg = $"Lotterie {lottery.Name} importiert";
            return msg;
        }
    }
    }
