using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using log4net;
using log4net.Repository.Hierarchy;
using MyStik.TimeTable.Data;
using System;
using System.Linq;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.GpUntis.Data;
using Spire.Pdf.General.Render.Decode.Jpeg2000.j2k.util;

namespace MyStik.TimeTable.DataServices.GpUntis
{
    public class SemesterImport
    {
        private ImportContext _import;

        //private Semester _semester;
        //private ActivityOrganiser _organiser;
        //private TimeTableDbContext db = new TimeTableDbContext();

        private Guid _semId;
        private Guid _orgId;


        // Nur die Uhrzeit der ersten Stunde
        // Tagesdatum ist egal
        DateTime _firstHour = new DateTime(2013, 1, 1, 8, 15, 0);

        private int _numCourse;
        private int _numRooms;
        private int _numLecturers;

        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");

        public SemesterImport(ImportContext import)
        {
            _import = import;
        }

        public SemesterImport(ImportContext import, Guid semId, Guid orgId)
        {
            _import = import;
            _semId = semId;
            _orgId = orgId;
        }


        public string ImportCourse(Kurs k)
        {
            string msg;
            // Konvention: Wochenendtermine = Blockunterricht
            // wird nicht importiert
            var isWeekEnd = k.Termine.Any(t => t.Tag == 6 || t.Tag == 7);

            if (isWeekEnd)
            {
                msg = string.Format("Blockunterricht {0} wird nicht importiert.", k.Fach.FachID);
            }
            else
            {
                ImportKurs(k);
                msg = string.Format("Kurs {0} mit Terminen importiert", k.Fach.FachID);
            }

            return msg;
        }



        private void ImportKurs(Kurs kurs)
        {
            _Logger.DebugFormat("Importiere Fach: {0}", kurs.Fach.FachID);
            var db = new TimeTableDbContext();

            var organiser = db.Organisers.SingleOrDefault(s => s.Id == _orgId);

            long msStart = sw.ElapsedMilliseconds;
            var course = new Course
            {
                ExternalSource = "GPUNTIS",
                ExternalId = kurs.Id,
                Organiser = organiser,
                ShortName = kurs.Fach.FachID,
                Name = kurs.Fach.Name,
                Occurrence = CreateDefaultOccurrence(),
                IsInternal = false,
            };
            // Kurs sofort speichern, damit die ID gesichert ist
            db.Activities.Add(course);
            db.SaveChanges();
            long msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer: {0}ms", msEnd - msStart);
            msStart = msEnd;

            // Alle Gruppen durchgehen, die im gpUntis zugeordnet wurden
            var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            foreach (var g in kurs.Gruppen)
            {
                // diesen Semestergruppen soll der Kurs zugeordnet werden
                var semGroups = InitSemesterGroups(db, g.GruppenID);

                foreach (var semesterGroup in semGroups)
                {
                    course.SemesterGroups.Add(semesterGroup);
                    semesterGroup.Activities.Add(course);

                    var occGroup =
                        course.Occurrence.Groups.SingleOrDefault(
                            gg => gg.SemesterGroups.Any(s => s.Id == semesterGroup.Id));

                    if (occGroup == null)
                    {
                        occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semesterGroup);
                        semesterGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        db.OccurrenceGroups.Add(occGroup);
                    }
                }
            }

            if (!course.SemesterGroups.Any())
            {
                _Logger.ErrorFormat("Kurs {0} ohne Gruppe", kurs);
            }


            if (semester != null)
            {
                // jetzt die Termine für diesen Kurs anlegen
                foreach (var termin in kurs.Termine)
                {
                    // Die Zeiten
                    // Generelle Regel: Nach jeder Doppelstunde (=90 min.) 15 min. Pause - keine explizite Mittagspause!
                    // Berechnung des Startzeitpunkts
                    //    Anzahl der Stunden + Anzahl der Pausen aus Start
                    // Berechnung des Endzeitpunkts:
                    //    Startzeitpunkt + Anzahl der Stunden aus Dauer + Anzahl der Pausen aus Dauer

                    var anzStunden = termin.BisStunde - termin.VonStunde + 1;
                    var anzPausen = ((anzStunden + 1) /2) - 1;

                    var occBegin = _firstHour.AddMinutes((termin.VonStunde - 1)*45 + ((termin.VonStunde - 1)/2)*15);
                    // Bisher: hat nicht immer gepasst
                    //DateTime occEnd = _firstHour.AddMinutes((termin.BisStunde) * 45 + ((termin.BisStunde - 1) / 2) * 15);
                    var occEnd = occBegin.AddMinutes(anzStunden*45 + anzPausen*15);

                    // Tag der ersten Veranstaltung nach Vorlesungsbeginn ermitteln
                    var semesterStartTag = (int) semester.StartCourses.DayOfWeek;
                    var day = termin.Tag;

                    int nDays = day - semesterStartTag;
                    if (nDays < 0)
                        nDays += 7;

                    DateTime occDate = semester.StartCourses.AddDays(nDays);


                    //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
                    int numOcc = 0;
                    while (occDate <= semester.EndCourses)
                    {
                        bool isVorlesung = true;
                        foreach (var sd in semester.Dates)
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
                            var ocStart = new DateTime(occDate.Year, occDate.Month, occDate.Day, occBegin.Hour,
                                occBegin.Minute, occBegin.Second);
                            var ocEnd = new DateTime(occDate.Year, occDate.Month, occDate.Day, occEnd.Hour,
                                occEnd.Minute, occEnd.Second);

                            var occ = new ActivityDate
                            {
                                Begin = ocStart,
                                End = ocEnd,
                                Activity = course,
                                Occurrence = CreateDefaultOccurrence(),
                            };

                            foreach (Raum raum in termin.Raeume)
                            {
                                var room = InitRoom(db, raum, organiser);
                                occ.Rooms.Add(room);
                            }

                            foreach (var dozent in termin.Dozenten)
                            {
                                var lecturer = InitLecturer(db, dozent, organiser);
                                occ.Hosts.Add(lecturer);
                            }

                            db.ActivityDates.Add(occ);
                            numOcc++;
                        }

                        occDate = occDate.AddDays(7);
                    }

                    _Logger.DebugFormat("Veranstaltung {0} hat {1} Termine", termin, numOcc);

                }
            }

            db.SaveChanges();

            msEnd = sw.ElapsedMilliseconds;
            _Logger.DebugFormat("Dauer {0}ms", msEnd - msStart);
        }

        private static Occurrence CreateDefaultOccurrence()
        {
            return new Occurrence
            {
                Capacity = -1,
                IsAvailable = true,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
                UseGroups = false,
            };
        }

        private OrganiserMember InitLecturer(TimeTableDbContext db, Dozent dozent, ActivityOrganiser organiser)
        {
            var lecturer = organiser.Members.SingleOrDefault(l => l.ShortName.Equals(dozent.DozentID));
            if (lecturer == null)
            {
                string profileUrl = null;
                if (dozent.Typ.ToUpper().Equals("PROF"))
                {
                    profileUrl = string.Format("http://wi.hm.edu/dozenten/{0}/index.de.html", dozent.Name.ToLowerInvariant());
                }

                lecturer = new OrganiserMember
                {
                    ShortName = dozent.DozentID,
                    Name = dozent.Name,
                    Role = dozent.Typ,
                    Description = dozent.Name,
                    UrlProfile = profileUrl,
                };
                organiser.Members.Add(lecturer);
                db.Members.Add(lecturer);
                db.SaveChanges();
                _numLecturers++;
            }
            return lecturer;
        }

        private Room InitRoom(TimeTableDbContext db, Raum raum, ActivityOrganiser organiser)
        {
            var room = db.Rooms.SingleOrDefault(r => r.Number.Equals(raum.Nummer));
            if (room == null)
            {
                room = new Room
                {
                    Number = raum.Nummer,
                    Capacity = raum.Kapazitaet,
                    Description = raum.Beschreibung,
                    Owner = raum.Nutzer,
                };
                db.Rooms.Add(room);

                var assignment = new RoomAssignment
                {
                    Organiser = organiser,
                    InternalNeedConfirmation = true,
                    ExternalNeedConfirmation = true
                };

                room.Assignments.Add(assignment);

                db.SaveChanges();
                _numRooms++;
            }

            return room;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fachId">Untis</param>
        /// <returns></returns>
        private List<SemesterGroup> InitSemesterGroups(TimeTableDbContext db, string gruppenId)
        {
            // Annahme, die Semestergruppen existieren!
            var semGroupList = new List<SemesterGroup>();
            
            // Annahme, die Semestergruppen existieren nicht alle und müssen ggf. angelegt werden



            // TODO: hier muss irgenwann noch die Fakultätsinfo rein
            // damit man nach den Alias namen in Abhängigkeit der Studiengänge / Fakultät suchen kann
            // so müssen die Namen derzeit auf globale Ebene eindeutig sein
            var aliasList = db.GroupAliases.Where(g => g.Name.ToUpper().Equals(gruppenId.ToUpper())
                && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == _orgId).ToList();

            
            var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);


            var isSS = semester.Name.StartsWith("SS");
            var isWS = semester.Name.StartsWith("WS");



            foreach (var groupAlias in aliasList)
            {
                // zugehörige Kapazitätsgruppe
                var capGroup = groupAlias.CapacityGroup;

                // im semester suchen
                var semGroup = semester.Groups.SingleOrDefault(g => g.CapacityGroup.Id == capGroup.Id);

                if (semGroup == null)
                {
                    // fehlt sie wirklich?
                    if ((isSS && capGroup.InSS) || (isWS && capGroup.InWS))
                    {
                        _import.ErrorMessages.Add("Unbekannte Semestergruppe");


                        // Gruppe anlegen
                        /*
                        semGroup = new SemesterGroup
                        {
                            CurriculumGroup = currGroup,
                            Semester = semester,
                            Name = groupTemplate.SemesterGroupName
                        };

                        _Logger.InfoFormat("Semestergruppe {0} angelegt {1}", semGroup.FullName, aliasName);

                        currGroup.SemesterGroups.Add(semGroup);
                        db.SemesterGroups.Add(semGroup);
                        db.SaveChanges();
                     */
                    }
                }

                if (semGroup != null)
                {
                    semGroupList.Add(semGroup);
                }

            }



            return semGroupList;
        }

        public void CheckRooms()
        {
            var db = new TimeTableDbContext();
            var org = db.Organisers.SingleOrDefault(o => o.Id == _orgId);

            if (org == null)
            {
                _import.ErrorMessages.Add("Unbekannte Organisationseinheit");
                return;
            }

            foreach (var raum in _import.Raeume.Where(r => r.IsTouched))
                {
                    var room = db.Rooms.SingleOrDefault(r => r.Number.Equals(raum.Nummer));
                    if (room == null)
                    {
                        _import.ErrorMessages.Add(
                            string.Format(
                                "Raum [{0}] existiert nicht in Datenbank. Raum wird bei Import automatisch angelegt und {1} zugeordnet",
                                raum.Nummer, org.ShortName));
                    }
                    else
                    {
                        if (room.Assignments.All(a => a.Organiser.Id != org.Id))
                        {
                            _import.ErrorMessages.Add(string.Format("Raum [{0}] hat keine Zurodnung zu {1}. Zuordnung wird bei Import automatisch angelegt.",
                                raum.Nummer, org.ShortName));
                        }
                    }

                }
        }


        public void CheckLecturers()
        {
            var db = new TimeTableDbContext();
            var org = db.Organisers.SingleOrDefault(o => o.Id == _orgId);

            if (org == null)
            {
                _import.ErrorMessages.Add("Unbekannte Organisationseinheit");
                return;
            }

            foreach (var doz in _import.Dozenten.Where(d => d.IsTouched))
            {
                var lec = org.Members.SingleOrDefault(m => m.ShortName.Equals(doz.DozentID));
                if (lec == null)
                {
                    _import.ErrorMessages.Add(string.Format("Dozent [{0} ({1})] existiert nicht in Datenbank. Wird bei Import automatisch angelegt.", doz.Name, doz.DozentID));
                }
            }
        }

        /// <summary>
        /// Überprüfung der externen Gruppen
        /// Innerhalb einer Untis Datei ist die GruppenId eindeutig
        /// In der Datenbank muss sie innerhalb der Studiengänge bzw. deren Gruppen eindeutig sein
        /// </summary>
        public void CheckGroupConsistency()
        {
            var db = new TimeTableDbContext();

            foreach (var gruppe in _import.Gruppen.Where(g => g.IsTouched))
            {
                // gibt es einen Alias?
                var aliasList = db.GroupAliases.Where(g => g.Name.ToUpper().Equals(gruppe.GruppenID.ToUpper())
                    && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == _orgId).ToList();

                // für jeden Alias prüfen, ob es die Semestergruppe gibt
                if (!aliasList.Any())
                {
                    // Alias existiert nicht
                    _import.ErrorMessages.Add(string.Format("Gruppenalias [{0}] existiert nicht! Diese Gruppe wird nicht zugeordnet", gruppe.GruppenID));
                }
                else
                {
                    var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);

                    var isSS = semester.Name.StartsWith("SS");
                    var isWS = semester.Name.StartsWith("WS");

                    foreach (var groupAlias in aliasList)
                    {
                        if (!semester.Groups.Any(
                            g => g.CapacityGroup.Id == groupAlias.CapacityGroup.Id))
                        {
                            // Semestergruppe existiert nicht
                            // muss sie existieren?
                            if (isSS && groupAlias.CapacityGroup.InSS ||
                                isWS && groupAlias.CapacityGroup.InWS)
                            {
                                // Semestergruppe existiert nicht - muss angelegt werden
                                _import.ErrorMessages.Add(
                                    string.Format(
                                        "Semestergruppe [{0}] für Gruppenalias [{1}] existiert nicht! Semestergruppe wird automatisch angelegt.",
                                        groupAlias.CapacityGroup.FullName, gruppe.GruppenID));
                            }
                        }
                    }
                   
                }
            }
        }


        public void InitWPMs()
        {
            var db = new TimeTableDbContext();

            var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);

            var wpmList = db.Activities.OfType<Course>()
                .Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.SemesterGroups.Any(g => g.CapacityGroup.Name.Equals("WPM")) &&
                    !string.IsNullOrEmpty(a.ExternalSource) && a.ExternalSource.Equals("GPUNTIS"))
                .OrderBy(a => a.Name)
                .ToList();


            foreach (var wpm in wpmList)
            {
                // Standard: 15 Plätze, ohne Gruppen
                wpm.Occurrence.Capacity = 15;
                wpm.Occurrence.LotteryEnabled = true;
                wpm.Occurrence.UseGroups = false;
                wpm.Occurrence.UseExactFit = false;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Verschiebt alle Kurse von einem Datum zum anderen
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void MoveDates(DateTime source, DateTime target)
        {
            // Alle Termine am Ziel löschen
            var db = new TimeTableDbContext();

            var nextDay = target.AddDays(1);

            var dates2delete = db.ActivityDates.Where(d => d.Begin >= target && d.Begin < nextDay).ToList();

            _Logger.InfoFormat("Lösche {0} Termine am {1}", dates2delete.Count, source.Date);

            foreach (var date in dates2delete)
            {
                _Logger.InfoFormat("Lösche {0} am {1}", date.Activity.ShortName, date.Begin);
            }

            dates2delete.ForEach(d => db.ActivityDates.Remove(d));
            db.SaveChanges();

            // Alle Termine von der Quelle zum Ziel verschieben
            nextDay = source.AddDays(1);
            var dates2move = db.ActivityDates.Where(d => d.Begin >= source && d.Begin < nextDay).ToList();
            _Logger.InfoFormat("Verschiebe {0} Termine vom {1} zum {2}", dates2move.Count, source.Date, target.Date);

            foreach (var date in dates2move)
            {
                var start = target.Date.Add(date.Begin.TimeOfDay);
                var end = target.Date.Add(date.End.TimeOfDay);

                _Logger.InfoFormat("Verschiebe {0} von {1} nach {2}", date.Activity.ShortName, date.Begin, start);

                date.Begin = start;
                date.End = end;
            }

            db.SaveChanges();

        }
    }
}
