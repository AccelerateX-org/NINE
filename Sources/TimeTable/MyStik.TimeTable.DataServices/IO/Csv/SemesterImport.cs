using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Csv.Data;
using static System.String;

namespace MyStik.TimeTable.DataServices.IO.Csv
{
    internal class DateCone
    {
        internal DateTime Begin { get; set; }
        internal DateTime End { get; set; }
    }

    public class SemesterImport
    {
        private ImportContext _import;

        private TimeTableDbContext db;

        private Guid _semId;
        private Guid _orgId;
        private Guid _segmentId;


        private int _numRooms;
        private int _numLecturers;

        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");


        private ActivityOrganiser _organiser;
        private Semester _semester;
        private SemesterDate _segment;

        private bool _isValid = true;

        private StringBuilder _report = new StringBuilder();


        public SemesterImport(ImportContext import, Guid semId, Guid orgId, Guid segmentId)
        {
            _import = import;
            _semId = semId;
            _orgId = orgId;
            _segmentId = segmentId;

            db = new TimeTableDbContext();

            _report.AppendLine("<html>");
            _report.AppendLine("<body>");
        }


        public void CheckModules()
        {
            _organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            _semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            _segment = db.SemesterDates.SingleOrDefault(s => s.Id == _segmentId);

            foreach (var validCourse in _import.ValidCourses)
            {
                var course = validCourse.Value.First();
                if (course == null)
                {
                    _import.AddErrorMessage("Import",  string.Format("Lehrveranstaltung ohne Inhalt: {0}", validCourse.Key), true);
                    continue;
                }

                if (course.SubjectId.Contains("::"))
                {
                    var words = course.SubjectId.Split(':');

                    if (words.Length == 7)
                    {
                        var orgTag = words[0];
                        var catTag = words[2];
                        var modTag = words[4];
                        var subTag = words[6];

                        var subjects = db.ModuleCourses.Where(x =>
                            x.Module.Catalog != null &&
                            x.Module.Catalog.Organiser.ShortName.Equals(orgTag) &&
                            x.Module.Catalog.Tag.Equals(catTag) &&
                            x.Module.Tag.Equals(modTag) &&
                            x.Tag.Equals(subTag)
                        ).ToList();

                        if (subjects.Any())
                        {
                        }
                        else
                        {
                            _import.AddErrorMessage("Import", string.Format("Modul nicht vorhanden: {0} - {1}", validCourse.Key, course.SubjectId), true);
                        }
                    }
                }
                else
                {
                    _import.AddErrorMessage("Import", string.Format("Kein Modul angegeben: {0}", validCourse.Key), false);
                }

            }





            if (!_isValid)
                return;
        }

        public void CheckRooms()
        {
            var rooms = _import.AllCourseEntries.Where(x => !string.IsNullOrEmpty(x.Room)).Select(x => x.Room).Distinct().ToList();
            var nNew = 0;
            var nExist = 0;


            var db = new TimeTableDbContext();
            var org = db.Organisers.SingleOrDefault(o => o.Id == _orgId);

            if (org == null)
            {
                _import.AddErrorMessage("Import", "Unbekannte Organisationseinheit", true);
                return;
            }

            foreach (var raum in rooms)
            {
                if (string.IsNullOrEmpty(raum))
                {

                }
                else
                {
                    var dbRooms = db.Rooms.Where(r => r.Number.Equals(raum)).ToList();
                    if (dbRooms.Any())
                    {
                        nExist++;
                        foreach (var dbRoom in dbRooms)
                        {
                            if (dbRoom.Assignments.All(a => a.Organiser.Id != org.Id))
                            {
                                _import.AddErrorMessage("Import",
                                    $"Raum [{raum}] existiert hat aber keine Zuordnung zu {org.ShortName}. Zuordnung wird bei Import automatisch angelegt.",
                                    false);
                            }
                        }
                    }
                    else
                    {
                        _import.AddErrorMessage("Import",
                            $"Raum [{raum}] existiert nicht in Datenbank. Raum wird bei Import automatisch angelegt und {org.ShortName} zugeordnet",
                            false);
                        nNew++;
                    }
                }
            }

            _import.AddErrorMessage("Import", string.Format("Anzahl Räueme: {0}: {1} vorhanden - {2} werden angelegt", 
                rooms.Count, nExist, nNew), false);

            if (!_isValid)
                return;
        }

        public void CheckLecturers()
        {
            var lecturer = _import.AllCourseEntries.Where(x => !string.IsNullOrEmpty(x.Lecturer)).Select(x => x.Lecturer).Distinct().ToList();
            var nNew = 0;
            var nExist = 0;

            var db = new TimeTableDbContext();
            var org = db.Organisers.SingleOrDefault(o => o.Id == _orgId);

            if (org == null)
            {
                _import.AddErrorMessage("Import", "Unbekannte Organisationseinheit", true);
                return;
            }

            foreach (var doz in lecturer)
            {
                if (string.IsNullOrEmpty(doz))
                {
                }
                else
                {
                    if (org.Members.Count(m => m.ShortName.Equals(doz)) > 1)
                    {
                        _import.AddErrorMessage("Import", string.Format("Kurzname {0} existieren mehrfach in Datenbank. Dozent wird keinem Termin zugeordnet", doz), true);
                    }
                    else
                    {
                        var lec = org.Members.SingleOrDefault(m => m.ShortName.Equals(doz));
                        if (lec == null)
                        {
                            _import.AddErrorMessage("Import", string.Format("Dozent [{0}] existiert nicht in Datenbank. Wird bei Import automatisch angelegt.", doz), false);
                            nNew++;
                        }
                        else
                        {
                            nExist++;
                        }
                    }
                }
            }

            _import.AddErrorMessage("Import", string.Format("Anzahl lehrende: {0}: {1} vorhanden - {2} werden angelegt", lecturer.Count, nExist, nNew), false);


            if (!_isValid)
                return;
        }

        public string GetReport()
        {
            _report.AppendLine("</body>");
            _report.AppendLine("</html>");

            return _report.ToString();
        }


        public string ImportCourse(List<SimpleCourse> simpleCourseList)
        {
            string msg;

            var referenceCourse = simpleCourseList.FirstOrDefault();
            if (referenceCourse == null)
            {
                msg = $"Lehreranstaltung ohne Inhalt";
                return msg;
            }

            var course = db.Activities.OfType<Course>().FirstOrDefault(x =>
                x.ExternalSource.Equals("CSV") &&
                x.ExternalId.Equals(referenceCourse.CourseId) &&
                x.Organiser.Id == _organiser.Id &&
                x.Semester.Id == _semester.Id &&
                x.Segment.Id == _segment.Id);


            _Logger.DebugFormat("Importiere Fach: {0}", referenceCourse.Title);
            _report.AppendFormat("<h1>Erzeuge LV \"{0} ({1})\" - [{2}]</h1>", referenceCourse.Title, referenceCourse.SubjectId,
                referenceCourse.CourseId);
            _report.AppendLine();


            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);
            var sem = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            var segment = db.SemesterDates.SingleOrDefault(s => s.Id == _segmentId);

            var shortName = referenceCourse.SubjectId;
            List<ModuleSubject> subjects = new List<ModuleSubject>();

            // ggf mit Subject verbinden
            if (shortName.Contains("::"))
            {
                var words = shortName.Split(':');

                if (words.Length == 7)
                {
                    var orgTag = words[0];
                    var catTag = words[2];
                    var modTag = words[4];
                    var subTag = words[6];

                    subjects.AddRange(db.ModuleCourses.Where(x =>
                        x.Module.Catalog != null &&
                        x.Module.Catalog.Organiser.ShortName.Equals(orgTag) &&
                        x.Module.Catalog.Tag.Equals(catTag) &&
                        x.Module.Tag.Equals(modTag) &&
                        x.Tag.Equals(subTag)
                    ).ToList());

                    // Kurzname nur dann ändern, wenn es auch ein Fach dazu gibt
                    if (subjects.Any())
                    {
                        shortName = $"{words[4]} ({words[6]})";
                    }
                }
            }




            long msStart = sw.ElapsedMilliseconds;
            if (course == null)
            {
                course = new Course
                {
                    ExternalSource = "CSV",
                    ExternalId = referenceCourse.CourseId,
                    Organiser = organiser,
                    Semester = sem,
                    Segment = segment,
                    ShortName = shortName,
                    Name = referenceCourse.Title,
                    Description = Empty,
                    Occurrence = CreateDefaultOccurrence(),
                    IsInternal = true,
                    IsProjected = true,
                    LabelSet = new ItemLabelSet()
                };

                // Kurs sofort speichern, damit die ID gesichert ist
                db.Activities.Add(course);
                db.SaveChanges();
            }

            if (course.SubjectTeachings == null)
            {
                course.SubjectTeachings = new List<SubjectTeaching>();
            }

            foreach (var moduleSubject in subjects)
            {
                if (moduleSubject.SubjectTeachings == null)
                {
                    moduleSubject.SubjectTeachings = new List<SubjectTeaching>();
                }

                if (moduleSubject.SubjectTeachings.All(x => x.Course.Id != course.Id))
                {
                    var subjectTeaching = new SubjectTeaching
                    {
                        Course = course,
                        Subject = moduleSubject
                    };

                    course.SubjectTeachings.Add(subjectTeaching);
                    moduleSubject.SubjectTeachings.Add(subjectTeaching);
                }
            }

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

            // jetzt durch alle Einträge durchgehen
            foreach (var simpleCourse in simpleCourseList)
            {
                // CourseId => brauche ich nicht prüfen, weil über den Referenzkurs gemacht
                // SubjectID => Die Verbindung zu den Modulen => wurde über Referenzkurs gemacht
                // Titel => wurde über Referenzkurs gemacht
                // Terminliste
                var dateList = GetDates(simpleCourse);
                // Dozent anlegen
                var lecturer = GetLecturer(db, simpleCourse.Lecturer);
                // Raum anlegen
                var room = GetRoom(db, simpleCourse.Room);

                // Levelset suchen
                // Suche den Studiengang
                // Suche die Fakultät
                // Suche die Institution
                // Ausgangslage: die Eirichtung, die gerade importiert
                // Annahme 1: es ist die Institution
                ItemLabelSet labelSet = null;
                var levelId = simpleCourse.LabelLevel;
                if (!IsNullOrEmpty(levelId))
                {
                    if (organiser.Institution != null && levelId.ToUpper().Equals(organiser.Institution.Tag.ToUpper()))
                    {
                        labelSet = organiser.Institution.LabelSet;
                    }
                    else if (levelId.ToUpper().Equals(organiser.ShortName.ToUpper()))
                    {
                        labelSet = organiser.LabelSet;
                    }
                    else
                    {
                        var curr = organiser.Curricula.FirstOrDefault(x => x.ShortName.ToUpper().Equals(levelId));
                        if (curr != null)
                        {
                            labelSet = curr.LabelSet;
                        }
                    }
                }

                // wenn kein Labelset identifiziert werden kann, dann wird das Label auch nicht importiert
                // Label suchen ggf. anlegen
                if (labelSet != null)
                {
                    var label = labelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(simpleCourse.Label));
                    if (label == null)
                    {
                        label = new ItemLabel
                        {
                            Name = simpleCourse.Label,
                            Description = Empty
                        };

                        labelSet.ItemLabels.Add(label);
                        db.ItemLabels.Add(label);
                        db.SaveChanges();
                    }

                    if (course.LabelSet == null)
                    {
                        var ls = new ItemLabelSet();
                        course.LabelSet = ls;
                        db.ItemLabelSets.Add(ls);
                        db.SaveChanges();
                    }

                    course.LabelSet.ItemLabels.Add(label);
                }

                // Owner ergönzen
                if (lecturer != null)
                {
                    var owner = course.Owners.FirstOrDefault(x => x.Member.Id == lecturer.Id);

                    if (owner == null)
                    {
                        owner = new ActivityOwner
                        {
                            Activity = course,
                            Member = lecturer,
                            IsLocked = false
                        };

                        db.ActivityOwners.Add(owner);
                        db.SaveChanges();
                    }
                }
                


                // jetzt die Termine anlegen
                // jeden Termin im Kurs überprüfen, ob schon vorhanden
                // wenn ja => dann nur DOzent und Raum ergänzen => falls nicht schon da
                // wenn nein => dann Termin anlegen und Dozent und Raum ergänzen
                foreach (var dateCone in dateList)
                {
                    var courseDate =
                        course.Dates.FirstOrDefault(x => x.Begin == dateCone.Begin && x.End == dateCone.End);

                    if (courseDate == null)
                    {
                        courseDate = new ActivityDate
                        {
                            Begin = dateCone.Begin,
                            End = dateCone.End,
                            Activity = course,
                            Occurrence = CreateDefaultOccurrence(),
                        };

                        db.ActivityDates.Add(courseDate);
                        course.Dates.Add(courseDate);
                    }

                    if (lecturer != null && courseDate.Hosts.All(x => x.Id != lecturer.Id))
                    {
                        courseDate.Hosts.Add(lecturer);
                    }

                    if (room != null && courseDate.Rooms.All(x => x.Id != room.Id))
                    {
                        courseDate.Rooms.Add(room);
                    }
                }

            }

            db.SaveChanges();

            msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);

            msg = $"Kurs {course.ShortName} mit Terminen importiert";

            return msg;
        }

        private OrganiserMember GetLecturer(TimeTableDbContext db, string lecturerId)
        {
            OrganiserMember lecturer = null;
            if (IsNullOrEmpty(lecturerId)) return null;

            lecturer = _organiser.Members.SingleOrDefault(l => l.ShortName.Equals(lecturerId));
            if (lecturer != null) return lecturer;

            lecturer = new OrganiserMember
            {
                ShortName = lecturerId,
                Name = Empty,
                Role = Empty,
                Description = Empty
            };
            _organiser.Members.Add(lecturer);
            db.Members.Add(lecturer);
            db.SaveChanges();
            _numLecturers++;
            return lecturer;
        }

        private Room GetRoom(TimeTableDbContext db, string roomId)
        {
            Room room = null;
            if (IsNullOrEmpty(roomId)) return null;

            room = db.Rooms.SingleOrDefault(r => r.Number.Equals(roomId));
            if (room == null)
            {
                room = new Room
                {
                    Number = roomId,
                    Capacity = 0,
                    Description = Empty,
                    Owner = Empty,
                };
                db.Rooms.Add(room);
                db.SaveChanges();

                _numRooms++;
            }

            var assignment = db.RoomAssignments.SingleOrDefault(x =>
                x.Room.Id == room.Id &&
                x.Organiser.Id == _organiser.Id);
            if (assignment == null)
            {
                assignment = new RoomAssignment
                {
                    Organiser = _organiser,
                    InternalNeedConfirmation = false, // offen für interne
                    ExternalNeedConfirmation = true // geschlossen für externe
                };

                room.Assignments.Add(assignment);
                db.RoomAssignments.Add(assignment);
                db.SaveChanges();
            }

            return room;
        }

        private List<DateCone> GetDates(SimpleCourse course)
        {
            var dateList = new List<DateCone>();

            if (course.DayOfWeek == null || course.Begin == null || course.End == null)
                return dateList;


            // den wahren zeitraum bestimmen
            // Hypothese: immer Vorlesungszeitraum des Semesters
            var semesterAnfang = _segment.From;
            var semesterEnde = _segment.To;


            // Tag der ersten Veranstaltung nach Vorlesungsbeginn ermitteln
            var semesterStartTag = (int)semesterAnfang.DayOfWeek;
            var day = (int)course.DayOfWeek.Value;
            int nDays = day - semesterStartTag;
            if (nDays < 0)
                nDays += 7;

            DateTime occDate = semesterAnfang.AddDays(nDays);


            //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
            int numOcc = 0;
            while (occDate <= semesterEnde)
            {
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
                    var ocStart = new DateTime(occDate.Year, occDate.Month, occDate.Day, course.Begin.Value.Hour,
                        course.Begin.Value.Minute, course.Begin.Value.Second);
                    var ocEnd = new DateTime(occDate.Year, occDate.Month, occDate.Day, course.End.Value.Hour,
                        course.End.Value.Minute, course.End.Value.Second);

                    var occ = new DateCone
                    {
                        Begin = ocStart,
                        End = ocEnd,
                    };

                    dateList.Add(occ);
                    numOcc++;
                }

                occDate = occDate.AddDays(7);
            }


            return dateList;
        }


        private static Occurrence CreateDefaultOccurrence(int c=-1)
        {
            return new Occurrence
            {
                Capacity = c,
                IsAvailable = false,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
                UseGroups = false,
            };
        }
    }
  }
