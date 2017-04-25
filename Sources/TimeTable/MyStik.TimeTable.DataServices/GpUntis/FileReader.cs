using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.DataServices.GpUntis.Data;

namespace MyStik.TimeTable.DataServices.GpUntis
{
    public class FileReader
    {
        private string directory;
        private readonly ImportContext ctx = new ImportContext();
        private readonly Dictionary<int, List<Kurs>> unterrichtsMap = new Dictionary<int, List<Kurs>>();

        private readonly ILog _Logger = LogManager.GetLogger("FileReader");


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
            ReadGPU004();   // Dozenten

            _Logger.Info("Lese Räume");
            ReadGPU005();   // Räume

            _Logger.Info("Lese Fächer");
            ReadGPU006();   // Fächer

            _Logger.Info("Lese Klassen");
            ReadGPU003();   // Gruppen

            // GPU002 wird nicht eingelesen
            _Logger.Info("Lese Unterricht");
            ReadGPU001();   // Unterricht

            CheckGroups();
        }

        private string[] GetFileContent(string gpuFile)
        {
            var lines = File.ReadAllLines(Path.Combine(directory, gpuFile), Encoding.Default);

            return lines;
        }


        #region Einlesen der Dateien
        #region GPU006: Fächer
        private void ReadGPU006()
        {
            var lines = GetFileContent("GPU006.txt");

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(',');

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
        private void ReadGPU005()
        {
            // Überprüfen ob Room.Number in GPU null ist oder nicht
            // Einlesen des GPUFachnamens
            var lines = GetFileContent("GPU005.txt");

            foreach (var line in lines)
            {
                var words = line.Split(',');
                var GPU005RaumID = words[0].Replace("\"", "");
                var GPU005RaumName = words[1].Replace("\"", "");
                int result;
                var capacity = int.TryParse(words[7], out result) ? result : 0;
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
        private void ReadGPU004()
        {
            var lines = GetFileContent("GPU004.txt");

            var n = lines.Count();

            foreach (var line in lines)
            {
                var words = line.Split(',');
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
        private void ReadGPU003()
        {
            var lines = GetFileContent("GPU003.txt");

            var n = lines.Count();

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(',');

                var groupID = words[0].Replace("\"", "");
                var desc = words[1].Replace("\"", "");
                int result;
                var gruppenSize = int.TryParse(words[17], out result) ? result : 0;

                var gruppe = new Gruppe
                {
                    GruppenID = groupID,
                };
            
                ctx.Gruppen.Add(gruppe);
            }

        }

        #endregion

        #region GPU002: WIRD NICHTS IMPORTIERT!!!
        #endregion

        #region GPU001: Unterricht mit Import!
        private void ReadGPU001()
        {
            var lines = GetFileContent("GPU001.txt");

            // Grundannahme: Die Datei ist nach UnterrichtID sortiert
            var lastUnterrichtID = 0;

            foreach (var line in lines)
            {
                // Unterricht einlesen
                var words = line.Split(',');

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

        /// <summary>
        /// Anlegen eines neuen Kurstermins
        /// </summary>
        /// <param name="course"></param>
        /// <param name="u"></param>
        private void CreateCourseEvent(Kurs course, Unterricht u)
        {
            var room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(u.RaumID));
            var lecturer = ctx.Dozenten.SingleOrDefault(l => l.DozentID.Equals(u.DozentID));

            if (lecturer == null)
            {
                ctx.ErrorMessages.Add(string.Format("Kein Dozent angegeben {0}", u));
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
                    e.Raeume.Any(r => r.RaumID.Equals(u.RaumID)) &&
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
                    e.Raeume.Any(r => r.RaumID.Equals(u.RaumID)) &&
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
                    e.Raeume.Any(r => r.RaumID.Equals(u.RaumID)) &&
                    e.Dozenten.Any(d => d.DozentID.Equals(u.DozentID)));
                if (termin != null)
                    return;
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
            var room = ctx.Raeume.SingleOrDefault(r => r.RaumID.Equals(u.RaumID));
            var lecturer = ctx.Dozenten.SingleOrDefault(l => l.DozentID.Equals(u.DozentID));

            Kurs c;
            if (!unterrichtsMap.ContainsKey(u.UnterrichtID))
            {
                c = CreateCourse(u, room, lecturer);
                if (c != null)
                {
                    unterrichtsMap[u.UnterrichtID] = new List<Kurs> {c};
                }
            }
            else
            {
                c = unterrichtsMap[u.UnterrichtID].SingleOrDefault(co => co.Fach.FachID.Equals(u.FachID));
                if (c == null)
                {
                    c = CreateCourse(u, room, lecturer);
                    if (c != null)
                    {
                        unterrichtsMap[u.UnterrichtID].Add(c);
                    }
                }
            }
            return c;
        }

        private Kurs CreateCourse(Unterricht u, Raum room, Dozent lecturer)
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

            if (room != null)
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

        private void CheckGroups()
        {
            // jeder Kurs
            foreach (var kurs in ctx.Kurse)
            {
                if (!kurs.Gruppen.Any())
                {
                    ctx.ErrorMessages.Add(string.Format("Kurs ohne Gruppe: {0}", kurs.Fach.FachID));
                }
            }
        }

    }
}
