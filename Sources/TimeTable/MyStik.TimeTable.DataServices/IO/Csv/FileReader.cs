using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Csv.Data;
using Newtonsoft.Json;

namespace MyStik.TimeTable.DataServices.IO.Csv
{
    public class FileReader
    {
        private string directory;
        private readonly ImportContext ctx = new ImportContext();

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

            _Logger.Info("Lese CSV");
            ReadCsv("import.csv");

            // Komplette Konsistenzprüfung der Dateien
            Check();
        }

        private string[] GetFileContent(string gpuFile)
        {
            var path = Path.Combine(directory, gpuFile);

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(Path.Combine(directory, gpuFile), Encoding.Default);
                ctx.AddErrorMessage(gpuFile, string.Format("Anzahl Zeilen in {0}: {1}", gpuFile, lines.Length), false);
                // Leerzeilen löschen
                var linesWithContent = new List<string>();
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line.Trim()))
                        continue;
                    linesWithContent.Add(line);
                }

                ctx.AddErrorMessage(gpuFile, string.Format("Anzahl Zeilen mit Inhalt in {0}: {1}", gpuFile, linesWithContent.Count), false);
                return linesWithContent.ToArray();
            }

            ctx.AddErrorMessage(gpuFile, string.Format("Datei {0} nicht vorhanden", gpuFile), true);
            return new string[] { };
        }

        private void ReadCsv(string fileName)
        {
            var lines = GetFileContent(fileName);
            ctx.AddErrorMessage("CSV", $"# Zeilen: {lines.Length}", false);

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(seperator);


                var course = new SimpleCourse
                {
                    CourseId = words[0].Replace("\"", "").Trim(),
                    SubjectId = words[1].Replace("\"", "").Trim(),
                    Title = words[2].Replace("\"", "").Trim(),
                    DayOfWeek = null,
                    Begin = null,
                    End = null,
                    Lecturer = words[6].Replace("\"", "").Trim(),
                    Room = words[7].Replace("\"", "").Trim(),
                    LabelLevel = words[8].Replace("\"", "").Trim(),
                    Label = words[9].Replace("\"", "").Trim()
                };

                // Date and Time
                var day = words[3].Replace("\"", "").Trim().ToUpper();
                var begin = words[4].Replace("\"", "").Trim();
                var end = words[5].Replace("\"", "").Trim();

                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(begin) && !string.IsNullOrEmpty(end))
                {
                    switch (day)
                    {
                        case "MO":
                            course.DayOfWeek = DayOfWeek.Monday;
                            break;
                        case "DI":
                            course.DayOfWeek = DayOfWeek.Tuesday;
                            break;
                        case "MI":
                            course.DayOfWeek = DayOfWeek.Wednesday;
                            break;
                        case "DO":
                            course.DayOfWeek = DayOfWeek.Thursday;
                            break;
                        case "FR":
                            course.DayOfWeek = DayOfWeek.Friday;
                            break;
                        case "SA":
                            course.DayOfWeek = DayOfWeek.Saturday;
                            break;
                        case "SO":
                            course.DayOfWeek = DayOfWeek.Sunday;
                            break;
                    }

                    try
                    {
                        course.Begin = DateTime.Parse(begin);
                        course.End = DateTime.Parse(end);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (!ctx.ValidCourses.ContainsKey(course.CourseId))
                {
                    ctx.ValidCourses[course.CourseId] = new List<SimpleCourse>();
                }

                ctx.ValidCourses[course.CourseId].Add(course);
                ctx.AllCourseEntries.Add(course);
            }

        }
        private void Check()
        {
            ctx.AddErrorMessage("CSV", $"# Kurse: {ctx.ValidCourses.Count}", false);
        }
    }
}
