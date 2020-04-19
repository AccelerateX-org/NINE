using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyStik.TimeTable.DataServices.IO.GpUntis.Data;

namespace MyStik.TimeTable.DataServices.IO.GpUntis
{
    public class FileReader
    {
        private string directory;
        private readonly ImportContext ctx = new ImportContext();
        private readonly Dictionary<int, List<Kurs>> unterrichtsMap = new Dictionary<int, List<Kurs>>();

        private readonly ILog _Logger = LogManager.GetLogger("FileReader");

        private readonly char seperator = ';';


        public ImportContext Context 
        {
            get
            {
                return ctx;
            }
        }


        public void ReadFiles(string path)
        {
            directory = path;

            _Logger.Info("Lese Dozenten");
            ReadGPU004("GPU004.txt");   // Dozenten

            _Logger.Info("Lese Räume");
            ReadGPU005("GPU005.txt");   // Räume

            _Logger.Info("Lese Fächer");
            ReadGPU006("GPU006.txt");   // Fächer

            _Logger.Info("Lese Klassen");
            ReadGPU003("GPU003.txt");   // Gruppen

            // GPU002 wird nicht eingelesen
            _Logger.Info("Lese Unterricht");
            ReadGPU001("GPU001.txt");   // Unterricht


            // GPU0016: Zeitrestriktionen => belegte Zeiten von Räumen
            _Logger.Info("Lese Zeitrestriktionen");
            ReadGPU016("GPU016.txt");   // Unterricht



            // ab hier ; separiert
            ReadConfigDays("configDays.txt");

            ReadConfigHours("configHours.txt");

            ReadConfigGroups("configGroups.txt");

            // Komplette Konsistenzprüfung der Dateien
            CheckGroups();
        }

        private string[] GetFileContent(string gpuFile)
        {
            var path = Path.Combine(directory, gpuFile);

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(Path.Combine(directory, gpuFile), Encoding.Default);
                ctx.AddErrorMessage(gpuFile, string.Format("Anzahl Einträge: {0}", lines.Length), false);
                return lines;
            }

            ctx.AddErrorMessage(gpuFile, string.Format("Datei {0} nicht vorhanden", gpuFile), true);
            return new string[]{};
        }


        #region Einlesen der Untis Dateien
        #region GPU006: Fächer
        private void ReadGPU006(string fileName)
        {
            var lines = GetFileContent(fileName);

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(seperator);

                var fachID = words[0].Replace("\"", "");
                var fachName = words[1].Replace("\"", "");
                var mv = words[12].Replace("\"", "");

                var fach = new Fach
                {
                    FachID = fachID,
                    Name = fachName,
                    
                    // Macht keinen Sinn mehr. In der Datei staht da nicht die ID, sondern der Name
                    // und der kann doppelt vorkommen
                    //FachVerantwortlicher = ctx.Dozenten.SingleOrDefault(d => d.Name.Equals(mv)),
                };

                ctx.Faecher.Add(fach);
            }
        }
        #endregion

        #region GPU005: Räume mit Import
        private void ReadGPU005(string fileName)
        {
            // Überprüfen ob Room.Number in GPU null ist oder nicht
            // Einlesen des GPUFachnamens
            var lines = GetFileContent(fileName);

            foreach (var line in lines)
            {
                var words = line.Split(seperator);
                var GPU005RaumID = words[0].Replace("\"", "");
                var GPU005RaumName = words[1].Replace("\"", "");
                var capacity = int.TryParse(words[7], out var result) ? result : 0;
                var desc = words[12].Replace("\"", "");
                var owner = words[11].Replace("\"", "");

                //Falls in GPU nichts steht, id als Namen übernehmen
                if (string.IsNullOrEmpty(GPU005RaumName))
                {
                    GPU005RaumName = GPU005RaumID;
                }

                var room = new Raum()
                {
                    RaumID = GPU005RaumID,
                    Nummer = GPU005RaumName,
                    Kapazitaet = capacity,
                    Beschreibung = desc,
                    Nutzer = owner,
                };

                ctx.Raeume.Add(room);
            }
        }
        #endregion

        #region GPU004: Dozenten mit Import
        private void ReadGPU004(string fileName)
        {
            var lines = GetFileContent(fileName);

            var n = lines.Count();

            foreach (var line in lines)
            {
                var words = line.Split(seperator);
                var GPU004DozID = words[0].Replace("\"", "");
                var GPU004DozName = words[1].Replace("\"", "");
                var GPU004DozTyp = words[22].Replace("\"", "");


                var doz = new Dozent()
                {
                    DozentID = GPU004DozID,
                    Name = GPU004DozName,
                    Typ = GPU004DozTyp,
                };

                ctx.Dozenten.Add(doz);
            }

        }

        #endregion

        #region GPU003: Gruppen
        private void ReadGPU003(string fileName)
        {
            var lines = GetFileContent(fileName);

            var n = lines.Count();

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(seperator);

                var groupID = words[0].Replace("\"", "");
                var desc = words[1].Replace("\"", "");
                int result;
                var gruppenSize = int.TryParse(words[17], out result) ? result : 0;

                var gruppe = new Gruppe
                {
                    GruppenID = groupID,
                    IsValid = true
                };
            
                ctx.Gruppen.Add(gruppe);
            }

        }

        #endregion

        #region GPU002: WIRD NICHTS IMPORTIERT!!!
        #endregion

        #region GPU001: Unterricht mit Import!
        private void ReadGPU001(string fileName)
        {
            var lines = GetFileContent(fileName);

            // Grundannahme: Die Datei ist nach UnterrichtID sortiert
            var lastUnterrichtID = 0;

            foreach (var line in lines)
            {
                // Unterricht einlesen
                var words = line.Split(seperator);

                // Unterrichts-objekt anlegen und befüllen
                var u = new Unterricht
                {
                    UnterrichtID = int.Parse(words[0]),
                    GruppeID = words[1].Replace("\"", ""),
                    DozentID = words[2].Replace("\"", ""),
                    FachID = words[3].Replace("\"", ""),
                    RaumID = words[4].Replace("\"", ""),
                    Tag = int.Parse(words[5]),
                    Stunde = int.Parse(words[6])
                };

                // Kurs hinzufügen, d.h. wir haben jetzt einen passenden Kurs mit mindestens
                // einem CourseEvent
                // Entscheidung an Hand von UnterrichtsID und FachID
                var course = GetCourse(u);

                if (lastUnterrichtID == u.UnterrichtID)
                {
                    // Den Course Event ermitteln lassen
                    // 1. Tag
                    // 2. Zeitraum
                    // 3. Raum
                    CreateCourseEvent(course, u);

                    // ggf. Noch Dozent und Gruppe des Kurses aktualisieren
                    UpdateCourse(course, u);
                }

                lastUnterrichtID = u.UnterrichtID;
            }
        }

        private void ReadGPU016(string fileName)
        {
            var lines = GetFileContent(fileName);

            if (!lines.Any())
                return;


            foreach (var line in lines)
            {
                // Unterricht einlesen
                var words = line.Split(seperator);

                var r = new Restriktion
                {
                    Typ = words[0].Replace("\"", ""),
                    ElementID = words[1].Replace("\"", ""),
                    Tag = int.Parse(words[2]),
                    Stunde = int.Parse(words[3]),
                    Level= int.Parse(words[4])
                };

                if (r.Typ.Equals("R") && r.Level == -3)
                {
                    Context.Restriktionen.Add(r);
                }
            }

            // Alle Resriktionen durchgehen
            var orderedRestrictions = Context.Restriktionen.OrderBy(x => x.ElementID).ThenBy(x => x.Tag)
                .ThenBy(x => x.Stunde).ToList();


            foreach (var restriction in orderedRestrictions)
            {
                var room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(restriction.ElementID));

                if (room != null)
                {

                    // gleicher Tag und am hinteren Ende bisherigen Zeitraums und gleicher Raum und gleicher Dozent
                    var blockade = Context.Blockaden.SingleOrDefault(e =>
                        e.Tag == restriction.Tag &&
                        e.BisStunde + 1 == restriction.Stunde &&
                        e.Raum.RaumID.Equals(restriction.ElementID));
                    if (blockade != null)
                    {
                        blockade.BisStunde++;
                    }
                    else
                    {
                        // nicht vorhanden, dann anderer Tag oder Lücke oder anderer Raum 
                        blockade = new RaumBlockade
                        {
                            Raum = room,
                            Tag = restriction.Tag,
                            VonStunde = restriction.Stunde,
                            BisStunde = restriction.Stunde
                        };

                        Context.Blockaden.Add(blockade);
                    }
                }
            }


        }



        /// <summary>
        /// Anlegen eines neuen Kurstermins
        /// </summary>
        /// <param name="course"></param>
        /// <param name="u"></param>
        private void CreateCourseEvent(Kurs course, Unterricht u)
        {
            // die u.RaumID kann auch mehrere Räume enthalten
            // Trennzeichen mit "~"
            Raum room = null;
            if (u.RaumID.StartsWith("~"))
            {
                // das sind nun mehrere Räume
                var roomIds = u.RaumID.Trim().Split('~');
                foreach (var roomId in roomIds)
                {
                    if (!string.IsNullOrEmpty(roomId))
                    {
                        room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(roomId));
                        // es reicht, wenn ein Raum dabei ist
                        if (room != null)
                            break;
                    }
                }
            }
            else
            {
                room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(u.RaumID));
            }

            
            var lecturer = ctx.Dozenten.SingleOrDefault(l => l.DozentID.Equals(u.DozentID));

            if (lecturer == null)
            {
                //ctx.ErrorMessages.Add(string.Format("Kein Dozent angegeben {0}", u));
                // weitermachen macht keinen Sinn => es wird eh nichts importiert
                return;
            }

            Termin termin;

            if (room != null)
            {
                // gleicher Tag und am hinteren Ende bisherigen Zeitraums und gleicher Raum und gleicher Dozent
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    e.BisStunde + 1 == u.Stunde &&
                    e.Raeume.Any(r => r.RaumID.Equals(room.RaumID)) &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                {
                    termin.BisStunde++;
                    return;
                }

                // gleicher Tag und am vorderen Ende bisherigen Zeitraums und gleicher Raum
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    e.VonStunde - 1 == u.Stunde &&
                    e.Raeume.Any(r => r.RaumID.Equals(room.RaumID)) &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                {
                    termin.VonStunde--;
                    return;
                }

                // gleicher Tag und innerhalb des bisherigen Zeitraums und gleicher Raum und Dozent schon angegeben
                // das können wohl auch mehr werden!
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    (e.VonStunde <= u.Stunde && u.Stunde <= e.BisStunde) &&
                    e.Raeume.Any(r => r.RaumID.Equals(room.RaumID)) &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                    return;

                // gleicher Zeitraum
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    (e.VonStunde <= u.Stunde && u.Stunde <= e.BisStunde));

                if (termin != null)
                {
                    // wenn es ein neuer Raum ist, dann einen neuen Termin machen
                    var kursRaum = termin.Raeume.SingleOrDefault(r => r.RaumID.Equals(room.RaumID));

                    if (kursRaum != null)
                    {
                        // wenn der Raum bekannt ist
                        var kursDozent = termin.Dozenten.SingleOrDefault(d => d.DozentID.Equals(lecturer.DozentID));
                        if (kursDozent != null)
                        {
                            // wenn Dozent schon drin => nix machen
                        }
                        else
                        {
                            // wenn neuer Dozent für diesen Termin => an Termin anhängen
                            termin.Dozenten.Add(lecturer);
                            return;
                        }
                    }
                }
            }
            else
            {
                // es wurde kein Raum angegeben
                // gleicher Tag und am hinteren Ende bisherigen Zeitraums und gleicher Raum und gleicher Dozent
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    e.BisStunde + 1 == u.Stunde &&
                    !e.Raeume.Any() &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                {
                    termin.BisStunde++;
                    return;
                }

                // gleicher Tag und am vorderen Ende bisherigen Zeitraums und gleicher Raum
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    e.VonStunde - 1 == u.Stunde &&
                    !e.Raeume.Any() &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                {
                    termin.VonStunde--;
                    return;
                }

                // gleicher Tag und innerhalb des bisherigen Zeitraums und gleicher Raum
                // das können wohl auch mehr werden!
                termin = course.Termine.SingleOrDefault(e =>
                    e.Tag == u.Tag &&
                    (e.VonStunde <= u.Stunde && u.Stunde <= e.BisStunde) &&
                    !e.Raeume.Any() &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                    return;
            }

            // Ein neuer Termin muss her
            termin = new Termin()
            {
                VonStunde = u.Stunde,
                BisStunde = u.Stunde,
                Tag = u.Tag,
            };

            if (room != null)
                termin.Raeume.Add(room);

            termin.Dozenten.Add(lecturer);

            course.Termine.Add(termin);
        }

        private void UpdateCourse(Kurs course, Unterricht u)
        {
            //var lecturer = ctx.Dozenten.SingleOrDefault(l => l.DozentID.Equals(u.DozentID));
            var group = ctx.Gruppen.SingleOrDefault(s => s.GruppenID.Equals(u.GruppeID));

            if (group != null &&!course.Gruppen.Contains(group))
            {
                group.IsTouched = true;
                course.Gruppen.Add(group);
            }

            if (!course.Unterricht.Contains(u))
            {
                course.Unterricht.Add(u);
            }
        }

        private Kurs GetCourse(Unterricht u)
        {
            // Vorbedingungen prüfen
            // es können auch mehrere Räume sein
            var rooms = new List<Raum>();
            if (u.RaumID.StartsWith("~"))
            {
                // das sind nun mehrere Räume
                var roomIds = u.RaumID.Trim().Split('~');
                foreach (var roomId in roomIds)
                {
                    if (!string.IsNullOrEmpty(roomId))
                    {
                        var room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(roomId));
                        if (room != null)
                        {
                            rooms.Add(room);
                        }
                    }
                }
            }
            else
            {
                var room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(u.RaumID));
                if (room != null)
                {
                    rooms.Add(room);
                }
            }



            var lecturer = ctx.Dozenten.SingleOrDefault(l => l.DozentID.Equals(u.DozentID));

            Kurs c;
            if (!unterrichtsMap.ContainsKey(u.UnterrichtID))
            {
                c = CreateCourse(u, rooms, lecturer);
                if (c != null)
                {
                    unterrichtsMap[u.UnterrichtID] = new List<Kurs> {c};
                }
            }
            else
            {
                if (string.IsNullOrEmpty(u.FachID))
                {
                    c = unterrichtsMap[u.UnterrichtID].SingleOrDefault(co => co.Fach == null);
                }
                else
                {
                    c = unterrichtsMap[u.UnterrichtID].SingleOrDefault(co => co.Fach != null && co.Fach.FachID.Equals(u.FachID));
                }

                if (c == null)
                {
                    c = CreateCourse(u, rooms, lecturer);
                    if (c != null)
                    {
                        unterrichtsMap[u.UnterrichtID].Add(c);
                    }
                }
            }
            return c;
        }

        private Kurs CreateCourse(Unterricht u, List<Raum> rooms, Dozent lecturer)
        {
            var fach = ctx.Faecher.SingleOrDefault(f => f.FachID.Equals(u.FachID));

            if (fach != null)
                fach.IsTouched = true;

            var course = new Kurs
            {
                Id = u.UnterrichtID.ToString(),
                Fach = fach,
            };
            course.Unterricht.Add(u);

            var group = ctx.Gruppen.SingleOrDefault(s => s.GruppenID.Equals(u.GruppeID));
            if (group != null)
            {
                group.IsTouched = true;
                course.Gruppen.Add(group);
            }

            // Die LV
            var ce = new Termin()
            {
                VonStunde = u.Stunde,
                BisStunde = u.Stunde,
                Tag = u.Tag,
            };

            foreach (var room in rooms)
            {
                ce.Raeume.Add(room);
                room.IsTouched = true;
            }

            if (lecturer != null)
            {
                ce.Dozenten.Add(lecturer);
                lecturer.IsTouched = true;
            }

            course.Termine.Add(ce);

            ctx.Kurse.Add(course);

            return course;
        }


        #endregion

        #endregion

        #region Einlesen der Configdateien

        private void ReadConfigDays(string fileName)
        {
            var lines = GetFileContent(fileName);

            foreach (var line in lines)
            {
                if (line == lines.First()) continue;
                // Tag einlesen
                var words = line.Split(';');

                // 
                var dayId = int.Parse(words[0]);
                var weekDay = words[1].Replace("\"", "");
                var import = words[2].Replace("\"", "");

                ctx.Tage[dayId] = !string.IsNullOrEmpty(import);

                ctx.AddErrorMessage(fileName, string.Format("Tag {0} wird {1}", dayId,
                    ctx.Tage[dayId] ? "importiert" : "nicht importiert"), false);
            }
        }

        private void ReadConfigHours(string fileName)
        {
            var lines = GetFileContent(fileName);

            foreach (var line in lines)
            {
                if (line == lines.First()) continue;

                // Stunden einlesen
                var words = line.Split(';');

                // 
                var hourId = int.Parse(words[0]);
                var start = words[1].Replace("\"", "");
                var end = words[2].Replace("\"", "");

                ctx.Stunden[hourId] = new ImportTimeSpan
                {
                    Start = DateTime.Parse(start),
                    Ende = DateTime.Parse(end)
                };

                ctx.AddErrorMessage(fileName, string.Format("Stunde {0}: {1} - {2}", hourId,
                    ctx.Stunden[hourId].Start, ctx.Stunden[hourId].Ende), false);
            }
        }

        private void ReadConfigGroups(string fileName)
        {
            var lines = GetFileContent(fileName);
            if (!lines.Any())
            {
                ctx.AddErrorMessage(fileName,
                    string.Format(
                        "Keine Gruppenzordnungen vorgegeben. Es werden die Studiengänge aus der Datenbank verwendet."), false);
                return;
            }

            foreach (var line in lines)
            {
                if (line == lines.First()) continue;

                var words = line.Split(';');

                var z = new Zuordnung()
                {
                    Studiengang = words[0].Replace("\"", ""),
                    Studiengruppe = words[1].Replace("\"", ""),
                    Kapazitätsgruppe = words[2].Replace("\"", ""),
                    Alias = words[3].Replace("\"", ""),
                };

                
                ctx.GruppenZuordnungen.Add(z);
            }

        }

        #endregion

        private void CheckGroups()
        {
            // jeder Kurs
            foreach (var kurs in ctx.Kurse)
            {
                kurs.IsValid = true;

                if (kurs.Fach == null)
                {
                    kurs.IsValid = false;
                    ctx.AddErrorMessage("allgemein", string.Format("Kurs ohne Fach: {0} - wird nicht importiert", kurs.Id), true);
                }

                if (!kurs.Gruppen.Any())
                {
                    kurs.IsValid = false;
                    ctx.AddErrorMessage("allgemein", string.Format("Kurs ohne Gruppe: {0} - wird nicht importiert", kurs.Id), true);
                }

                // Tage und Stunden der Termine
                foreach (var termin in kurs.Termine)
                {
                    // Die Zeiten werden jetzt aus dem Kontext gelesen
                    if (!ctx.Stunden.ContainsKey(termin.VonStunde))
                    {
                        kurs.IsValid = false;
                        ctx.AddErrorMessage("allgemein", string.Format("Stunde {0} nicht definiert - Kurs {1} wird nicht importiert", termin.VonStunde, kurs.Id), true);
                        continue;
                    }
                    if (!ctx.Tage.ContainsKey(termin.Tag))
                    {
                        kurs.IsValid = false;
                        ctx.AddErrorMessage("allgemein", string.Format("Tag {0} nicht definiert - Kurs {1} wird nicht importiert", termin.Tag, kurs.Id), true);
                        continue;
                    }


                    if (!ctx.Stunden.ContainsKey(termin.BisStunde))
                    {
                        kurs.IsValid = false;
                        ctx.AddErrorMessage("allgemein", string.Format("Stunde {0} nicht definiert - Kurs {1} wird nicht importiert", termin.BisStunde, kurs.Id), true);
                    }

                    // Termin liegt an einem Tag, der nicht importiert werden soll
                    if (!ctx.Tage[termin.Tag])
                    {
                        kurs.IsValid = false;
                        ctx.AddErrorMessage("allgemein",
                            string.Format("Tag {0} soll nicht importiert werden - Kurs {1} wird nicht importiert", termin.Tag, kurs.Id), true);
                    }
                }

            }
        }

    }
}
