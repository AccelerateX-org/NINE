using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using MyStik.TimeTable.Data;
using System;
using System.Linq;
using MyStik.TimeTable.DataServices.IO.GpUntis.Data;

namespace MyStik.TimeTable.DataServices.IO.GpUntis
{
    public class SemesterImport
    {
        private ImportContext _import;

        private Guid _semId;
        private Guid _orgId;
        private DateTime? _firstDate;
        private DateTime? _lastDate;


        private int _numRooms;
        private int _numLecturers;

        private Stopwatch sw = new Stopwatch();

        private readonly ILog _Logger = LogManager.GetLogger("Import");



        public SemesterImport(ImportContext import, Guid semId, Guid orgId, string firstDate=null, string lastDate=null)
        {
            _import = import;
            _semId = semId;
            _orgId = orgId;

            DateTime date;
            if (!string.IsNullOrEmpty(firstDate))
            {
                var isOK = DateTime.TryParse(firstDate, out date);
                if (isOK)
                {
                    _firstDate = date;
                }
            }

            if (!string.IsNullOrEmpty(lastDate))
            {
                var isOK = DateTime.TryParse(lastDate, out date);
                if (isOK)
                {
                    _lastDate = date;
                }
            }
        }


        public string ImportCourse(Kurs k)
        {
            string msg;
            if (!k.IsValid)
            {
                msg = string.Format("Kurs {0} ist nicht konsistent und wird nicht importiert.", k.Id);
                return msg;
            }

            if (k.Fach == null)
            {
                msg = string.Format("Kurs ohne Fach. Id={0}, wird nicht importiert.", k.Id);
                return msg;
            }

            ImportKurs(k);
            msg = string.Format("Kurs {0} mit Terminen importiert", k.Fach.FachID);

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

                // TODO: Was macht eigentlich die OccurrenceGroup?
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
                    // Die Zeiten werden jetzt aus dem Kontext gelesen
                    if (!_import.Stunden.ContainsKey(termin.VonStunde))
                    {
                        _import.AddErrorMessage("Import", string.Format("Stunde {0} nicht definiert", termin.VonStunde), false);
                        continue;
                    }
                    if (!_import.Stunden.ContainsKey(termin.BisStunde))
                    {
                        _import.AddErrorMessage("Import", string.Format("Stunde {0} nicht definiert", termin.BisStunde), false);
                        continue;
                    }

                    // Termin liegt an einem Tag, der nicht importiert werden soll
                    if (!_import.Tage.ContainsKey(termin.Tag))
                    {
                        _import.AddErrorMessage("Import", string.Format("Tag {0} nicht definiert", termin.Tag), false);
                        continue;
                    }

                    if (!_import.Tage[termin.Tag])
                    {
                        _import.AddErrorMessage("Import", string.Format("Tag {0} soll nicht importiert werden", termin.Tag), false);
                        continue;
                    }


                    var occBegin = _import.Stunden[termin.VonStunde].Start;
                    var occEnd = _import.Stunden[termin.BisStunde].Ende;

                    // den wahren zeitraum bestimmen
                    // Hypothese: immer Vorlesungszeitraum des Semesters
                    var semesterAnfang = semester.StartCourses;
                    var semesterEnde = semester.EndCourses;

                    // abweichende Angaben? dann diese nehmen
                    if (_firstDate.HasValue)
                    {
                        semesterAnfang = _firstDate.Value;
                    }

                    if (_lastDate.HasValue)
                    {
                        semesterEnde = _lastDate.Value;
                    }

                    // Tag der ersten Veranstaltung nach Vorlesungsbeginn ermitteln
                    var semesterStartTag = (int)semesterAnfang.DayOfWeek;
                    var day = termin.Tag;
                    int nDays = day - semesterStartTag;
                    if (nDays < 0)
                        nDays += 7;

                    DateTime occDate = semesterAnfang.AddDays(nDays);


                    //Solange neue Termine anlegen bis das Enddatum des Semesters erreicht ist
                    int numOcc = 0;
                    while (occDate <= semesterEnde)
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
                                if (room != null)
                                {
                                    occ.Rooms.Add(room);
                                }
                            }

                            foreach (var dozent in termin.Dozenten)
                            {
                                var lecturer = InitLecturer(db, dozent, organiser);
                                if (lecturer != null)
                                {
                                    occ.Hosts.Add(lecturer);
                                }
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
            var n = organiser.Members.Count(l => l.ShortName.Equals(dozent.DozentID));
            if (n > 1)
                return null;

            var lecturer = organiser.Members.FirstOrDefault(l => l.ShortName.Equals(dozent.DozentID));
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
            var room = db.Rooms.FirstOrDefault(r => r.Number.Equals(raum.Nummer));
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
                db.SaveChanges();

                _numRooms++;
            }


            var assignment = db.RoomAssignments.SingleOrDefault(x => 
                x.Room.Id == room.Id &&
                x.Organiser.Id == organiser.Id);
            if (assignment == null)
            {
                assignment = new RoomAssignment
                {
                    Organiser = organiser,
                    InternalNeedConfirmation = false,   // offen für interne
                    ExternalNeedConfirmation = true     // geschlossen für externe
                };

                room.Assignments.Add(assignment);
                db.RoomAssignments.Add(assignment);
                db.SaveChanges();
            }

            return room;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="gruppenId">Gruppenalias nach dem in der Datenbank gesucht wird</param>
        /// <returns></returns>
        private List<SemesterGroup> InitSemesterGroups(TimeTableDbContext db, string gruppenId)
        {
            var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            var org = db.Organisers.SingleOrDefault(x => x.Id == _orgId);

            // Annahme, die Semestergruppen existieren!
            var semGroupList = new List<SemesterGroup>();

            // suche alle aktuellen Zuordnungen zu dieser gruppenID
                var zuordnungen = _import.GruppenZuordnungen.Where(x => x.Alias.Equals(gruppenId));

                foreach (var zuordnung in zuordnungen)
                {
                    // Studiengang finden
                    var curr = db.Curricula.SingleOrDefault(x =>
                        x.ShortName.Equals(zuordnung.Studiengang) &&
                        x.Organiser.Id == org.Id);
                    if (curr == null)
                    {
                        curr = new TimeTable.Data.Curriculum
                        {
                            Organiser = org,
                            ShortName = zuordnung.Studiengang,
                            Name = zuordnung.Studiengang
                        };
                        db.Curricula.Add(curr);
                    }

                    var sg = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(zuordnung.Studiengruppe));
                    if (sg == null)
                    {
                        sg = new CurriculumGroup
                        {
                            Name = zuordnung.Studiengruppe,
                            IsSubscribable = true,
                            Curriculum = curr
                        };
                        db.CurriculumGroups.Add(sg);
                        curr.CurriculumGroups.Add(sg);
                    }

                    var cg = string.IsNullOrEmpty(zuordnung.Kapazitätsgruppe) ?
                        sg.CapacityGroups.SingleOrDefault(x => string.IsNullOrEmpty(x.Name)) :
                        sg.CapacityGroups.SingleOrDefault(x => x.Name.Equals(zuordnung.Kapazitätsgruppe));
                    if (cg == null)
                    {
                        cg = new CapacityGroup
                        {
                            InSS = true,
                            InWS = true,
                            Name = zuordnung.Kapazitätsgruppe,
                            CurriculumGroup = sg
                        };
                        db.CapacityGroups.Add(cg);
                        sg.CapacityGroups.Add(cg);
                    }

                    // bis hierher habe ich ohne Semesterbezug gearbeitet
                    // jetzt noch die Semestergruppe

                    var semGroup = cg.SemesterGroups.SingleOrDefault(x => x.Semester.Id == semester.Id);
                    if (semGroup == null)
                    {
                        // semestergruppe gibt es nicht => auf jeden Fall anlegen
                        semGroup = new SemesterGroup
                        {
                            CapacityGroup = cg,
                            Semester = semester,
                        };

                        _Logger.InfoFormat("Semestergruppe {0} angelegt {1}", semGroup.FullName, gruppenId);

                        cg.SemesterGroups.Add(semGroup);
                        db.SemesterGroups.Add(semGroup);
                    }

                    semGroupList.Add(semGroup);
                }

                db.SaveChanges();


              return semGroupList;
        }




        /*
        private List<SemesterGroup> InitSemesterGroups(TimeTableDbContext db, string gruppenId)
        {
            var semester = db.Semesters.SingleOrDefault(s => s.Id == _semId);
            var org = db.Organisers.SingleOrDefault(x => x.Id == _orgId);

            // Annahme, die Semestergruppen existieren!
            var semGroupList = new List<SemesterGroup>();
            
            // Annahme, die Semestergruppen existieren nicht alle und müssen ggf. angelegt werden

            // damit man nach den Alias namen in Abhängigkeit der Studiengänge / Fakultät suchen kann
            // so müssen die Namen derzeit auf globale Ebene eindeutig sein
            var aliasList = db.GroupAliases.Where(g => g.Name.ToUpper().Equals(gruppenId.ToUpper())
                && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == _orgId).ToList();

            // falls leer, jetzt in Zuordnungen nachsehen
            if (!aliasList.Any())
            {
                var zuordnungen = _import.GruppenZuordnungen.Where(x => x.Alias.Equals(gruppenId));

                foreach (var zuordnung in zuordnungen)
                {
                    // Studiengang finden
                    var curr = db.Curricula.SingleOrDefault(x =>
                        x.ShortName.Equals(zuordnung.Studiengang) &&
                        x.Organiser.Id == org.Id);
                    if (curr == null)
                    {
                        curr = new TimeTable.Data.Curriculum
                        {
                            Organiser = org,
                            ShortName = zuordnung.Studiengang,
                            Name = zuordnung.Studiengang
                        };
                        db.Curricula.Add(curr);
                    }

                    var sg = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(zuordnung.Studiengruppe));
                    if (sg == null)
                    {
                        sg = new CurriculumGroup
                        {
                            Name = zuordnung.Studiengruppe,
                            IsSubscribable = true,
                            Curriculum = curr
                        };
                        db.CurriculumGroups.Add(sg);
                        curr.CurriculumGroups.Add(sg);
                    }

                    var cg = string.IsNullOrEmpty(zuordnung.Kapazitätsgruppe) ?
                        sg.CapacityGroups.SingleOrDefault(x => string.IsNullOrEmpty(x.Name)) :
                        sg.CapacityGroups.SingleOrDefault(x => x.Name.Equals(zuordnung.Kapazitätsgruppe));
                    if (cg == null)
                    {
                        cg = new CapacityGroup
                        {
                            InSS = true,
                            InWS = true,
                            Name = zuordnung.Kapazitätsgruppe,
                            CurriculumGroup = sg
                        };
                        db.CapacityGroups.Add(cg);
                        sg.CapacityGroups.Add(cg);
                    }

                    var al = cg.Aliases.SingleOrDefault(x => x.Name.Equals(zuordnung.Alias));
                    if (al == null)
                    {
                        al = new GroupAlias
                        {
                            Name = zuordnung.Alias,
                            CapacityGroup = cg
                        };
                        db.GroupAliases.Add(al);
                        cg.Aliases.Add(al);
                    }

                    aliasList.Add(al);
                }
                db.SaveChanges();
            }


            foreach (var groupAlias in aliasList)
            {
                // zugehörige Kapazitätsgruppe
                var capGroup = groupAlias.CapacityGroup;

                // im semester suchen
                var semGroup = semester.Groups.SingleOrDefault(g => g.CapacityGroup.Id == capGroup.Id);

                if (semGroup == null)
                {
                    // semestergruppe gibt es nicht => auf jeden Fall anlegen
                    semGroup = new SemesterGroup
                    {
                        CapacityGroup = capGroup,
                        Semester = semester,
                    };

                    _Logger.InfoFormat("Semestergruppe {0} angelegt {1}", semGroup.FullName, gruppenId);

                    capGroup.SemesterGroups.Add(semGroup);
                    db.SemesterGroups.Add(semGroup);
                    db.SaveChanges();
                }

                semGroupList.Add(semGroup);
            }

            return semGroupList;
        }
        */



        public void CheckRooms()
        {
            var db = new TimeTableDbContext();
            var org = db.Organisers.SingleOrDefault(o => o.Id == _orgId);

            if (org == null)
            {
                _import.AddErrorMessage("Import", "Unbekannte Organisationseinheit", true);
                return;
            }

            foreach (var raum in _import.Raeume.Where(r => r.IsTouched))
                {
                    var room = db.Rooms.SingleOrDefault(r => r.Number.Equals(raum.Nummer));
                    if (room == null)
                    {
                        _import.AddErrorMessage("Import",
                            $"Raum [{raum.Nummer}] existiert nicht in Datenbank. Raum wird bei Import automatisch angelegt und {org.ShortName} zugeordnet", false);
                    }
                    else
                    {
                        if (room.Assignments.All(a => a.Organiser.Id != org.Id))
                        {
                            _import.AddErrorMessage("Import",
                                $"Raum [{raum.Nummer}] existiert hat aber keine Zuordnung zu {org.ShortName}. Zuordnung wird bei Import automatisch angelegt.", false);
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
                _import.AddErrorMessage("Import", "Unbekannte Organisationseinheit", true);
                return;
            }

            foreach (var doz in _import.Dozenten.Where(d => d.IsTouched))
            {
                if (org.Members.Count(m => m.ShortName.Equals(doz.DozentID)) > 1)
                {
                    _import.AddErrorMessage("Import", string.Format("Kurzname {0} existieren mehrfach in Datenbank. Dozent wird keinem Termin zugeordnet", doz.DozentID), true);
                }
                else
                {
                    var lec = org.Members.SingleOrDefault(m => m.ShortName.Equals(doz.DozentID));
                    if (lec == null)
                    {
                        _import.AddErrorMessage("Import", string.Format("Dozent [{0} ({1})] existiert nicht in Datenbank. Wird bei Import automatisch angelegt.", doz.Name, doz.DozentID), false);
                    }

                }
            }
        }

        /// <summary>
        /// es werden alle Zuordnungen durchsucht
        /// ob der Studiengang existiert,
        /// ob Studiengruppe und Kapazitätsgruppe existieren
        /// Fehlendes würde später automatisch angelegt werden
        /// </summary>
        public void CheckGroups()
        {
            var db = new TimeTableDbContext();

            foreach (var zuordnung in _import.GruppenZuordnungen)
            {
                // muss es überhaupt berücksichtigt werden
                var klasse = _import.Gruppen.SingleOrDefault(x => x.GruppenID.ToUpper().Equals(zuordnung.Alias.ToUpper()));
                if (klasse == null)
                {
                    _import.AddErrorMessage("Import", string.Format(
                        "Klasse [{0}]: Zuordnung vorhanden, aber keine zugehörige Klasse in GPU003",
                        zuordnung.Alias), true);
                }
                else
                {
                    if (klasse.IsTouched)
                    {

                        // Nach dem Studiengang suchen
                        var curr = db.Curricula.SingleOrDefault(x =>
                            x.ShortName.ToUpper().Equals(zuordnung.Studiengang.ToUpper()) && x.Organiser.Id == _orgId);

                        if (curr == null)
                        {
                            _import.AddErrorMessage("Import", string.Format(
                                "Klasse [{0}]: Studiengang [{1}] existiert nicht. Wird angelegt",
                                zuordnung.Alias, zuordnung.Studiengang), true);
                        }
                        else
                        {
                            var group = curr.CurriculumGroups.SingleOrDefault(x =>
                                x.Name.ToUpper().Equals(zuordnung.Studiengruppe.ToUpper()));

                            if (group == null)
                            {
                                _import.AddErrorMessage("Import", string.Format(
                                    "Klasse [{0}]: Studiengruppe [{1}] in Studienfgang [{2}] existiert nicht. Wird angelegt",
                                    zuordnung.Alias, zuordnung.Studiengruppe, zuordnung.Studiengang), true);
                            }
                            else
                            {
                                var capGroup = group.CapacityGroups.SingleOrDefault(x =>
                                    x.Name.ToUpper().Equals(zuordnung.Kapazitätsgruppe.ToUpper()));

                                if (capGroup == null)
                                {
                                    _import.AddErrorMessage("Import", string.Format(
                                        "Klasse [{0}]: Kapzitätsgruppe [{1}] der Studiengruppe [{2}] in Studienfgang [{3}] existiert nicht. Wird angelegt",
                                        zuordnung.Alias, zuordnung.Kapazitätsgruppe, zuordnung.Studiengruppe,
                                        zuordnung.Studiengang), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        /* alte variante
        public void CheckGroups()
        {
            var db = new TimeTableDbContext();

            foreach (var gruppe in _import.Gruppen.Where(g => g.IsTouched))
            {
                // alias in den Studiengängen des Veranstalters suchen
                var aliasList = db.GroupAliases.Where(g => g.Name.ToUpper().Equals(gruppe.GruppenID.ToUpper())
                    && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == _orgId).ToList();

                if (!aliasList.Any())
                {
                    // Alias existiert nicht in der Datenbank
                    // wenn es im Context eine Vorgabe gibt, dann wäre diese jetzt zu verwenden!

                    // gibt es eine Zuordnung? in der Curriculumsdati
                    var z = _import.GruppenZuordnungen.Any(x => x.Alias.Equals(gruppe.GruppenID));
                    if (!z)
                    {
                        // auch kein Alias => diese Gruppe passt nicht
                        gruppe.IsValid = false;
                        _import.AddErrorMessage("Import", string.Format("Studiengruppe für Klasse [{0}] existiert nicht! Daten zu dieser Klasse werden nicht importiert.", gruppe.GruppenID), true);
                    }
                    else
                    {
                        // Alias vorhanden => wird angelegt
                        _import.AddErrorMessage("Import", string.Format("Studiengruppe für Klasse [{0}] existiert. Die Klasse wird als Alias in der Studiengruppe ergänzt.", gruppe.GruppenID), false);
                    }

                }
            }
        }
        */

        /// <summary>
        /// Zu jedem Kurs gibt es Klassen
        /// zu jeder Klasse muss mindestens eine Zuordnung existieren
        /// </summary>
            public void CheckCourses()
        {

            foreach (var kurs in _import.Kurse)
            {
                var isValid = false;
                foreach (var gruppe in kurs.Gruppen)
                {
                    // suche alle Zuordnungen
                    var n = _import.GruppenZuordnungen.Count(x => x.Alias.ToUpper().Equals(gruppe.GruppenID.ToUpper()));

                    if (n == 0)
                    {
                        _import.AddErrorMessage("Import", string.Format("Kurs [{0}] die Klasse [{1}] hat keine Zuordnung. Diese Beziehnung wird nicht importiert", kurs.Id, gruppe.GruppenID), false);
                    }
                    else
                    {
                        isValid = true;
                    }
                }

                if (!isValid)
                {
                    _import.AddErrorMessage("Import", string.Format("Kurs [{0}] hat keinerlei Zuordnungen und wird nicht importiert", kurs.Id), true);
                }

                kurs.IsValid = isValid;
            }

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
