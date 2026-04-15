using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MyStik.TimeTable.DataServices.IO.CSV2
{
    public class FileReader
    {
        private readonly ImportContext ctx = new ImportContext();

        private readonly ILog _Logger = LogManager.GetLogger("CSV2");

        private readonly char seperator = ';';

        public ImportContext Context => ctx;


        public void ProcessFile(string fileName)
        {
            _Logger.Info("Lese CSV");
            ReadCsv(fileName);
        }

        private string[] GetFileContent(string fileName)
        {
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllLines(fileName, Encoding.UTF8);
                ctx.AddErrorMessage(fileName, $"Anzahl Zeilen in {fileName}: {lines.Length}", false);
                // Leerzeilen und Kopfzeile löschen
                var linesWithContent = new List<string>();
                var i = 0;
                foreach (var line in lines)
                {
                    i++;
                    if (i == 1 || string.IsNullOrEmpty(line.Trim()))
                        continue;
                    linesWithContent.Add(line);
                }

                ctx.AddErrorMessage(fileName, $"Anzahl Zeilen mit Inhalt in {fileName}: {linesWithContent.Count}", false);
                return linesWithContent.ToArray();
            }

            ctx.AddErrorMessage(fileName, $"Datei {fileName} nicht vorhanden", true);
            return new string[] { };
        }

        private void ReadCsv(string fileName)
        {
            var lines = GetFileContent(fileName);
            ctx.AddErrorMessage(fileName, $"# Zeilen: {lines.Length}", false);

            foreach (var line in lines)
            {
                // Zeile aufspalten
                var words = line.Split(seperator);

                var fk = words[0].Replace("\"", "").Trim();
                var sem = words[1].Replace("\"", "").Trim();
                var seg = "";
                if (sem.Contains(":"))
                {
                    var semWords = sem.Split(':');
                    sem = semWords[0].Trim();
                    seg = semWords[1].Trim();
                }
                var courseId = words[2].Replace("\"", "").Trim();
                var beginPeriod = words[3].Replace("\"", "").Trim();
                var endPeriod = words[4].Replace("\"", "").Trim();
                var days = words[5].Replace("\"", "").Trim().Split(',');
                var frq = words[6].Replace("\"", "").Trim();
                var begin = words[7].Replace("\"", "").Trim();
                var end = words[8].Replace("\"", "").Trim();
                var rooms = words[9].Replace("\"", "").Trim().Split(',');
                var lecturers = words[10].Replace("\"", "").Trim().Split(',');
                var cohorts = words[11].Replace("\"", "").Trim().Split(',');
                var capacity = words[12].Replace("\"", "").Trim();
                var title = words[13].Replace("\"", "").Trim();
                var modules = words[14].Replace("\"", "").Trim().Split(',');

                // den Kurs suchen
                var course = Context.Courses.Keys.FirstOrDefault(x =>
                    x.Faculty.Equals(fk) && x.Semester.Equals(sem) && x.Id.Equals(courseId));

                if (course == null)
                {
                    course = new ImportCourseId
                    {
                        Faculty = fk,
                        Semester = sem,
                        Id = courseId,
                        IsValid = true
                    };
                    Context.Courses[course] = new List<ImportCourseDataSet>();
                }

                // Titel ergänzen, wenn nicht vorhanden
                if (string.IsNullOrEmpty(course.CourseName) && !string.IsNullOrEmpty(title))
                {
                    course.CourseName = title;
                }

                // neuen Datensatz anlegen
                var dataSet = new ImportCourseDataSet
                {
                    Segment = seg,
                };

                // Beginn und Ende des Zeitraums
                try
                {
                    if (string.IsNullOrEmpty(beginPeriod))
                    {
                        if (string.IsNullOrEmpty(seg))
                        {
                            course.Errors.Add("Invalid period no begin and no segment seg");
                            course.IsValid = false;
                            continue;
                        }
                    }
                    else
                    {
                        dataSet.PeriodBegin = DateTime.Parse(beginPeriod);
                    }
                }
                catch
                {
                    course.Errors.Add($"Invalid period begin {beginPeriod}");
                    course.IsValid = false;
                }

                try
                {
                    if (string.IsNullOrEmpty(endPeriod))
                    {
                        if (string.IsNullOrEmpty(seg))
                        {
                            course.Errors.Add("Invalid period no end and no segment");
                            course.IsValid = false;
                            continue;
                        }
                    }
                    else
                    {
                        dataSet.PeriodEnd = DateTime.Parse(endPeriod);
                    }
                }
                catch
                {
                    course.Errors.Add($"Invalid period end {endPeriod}");
                    course.IsValid = false;
                    continue;
                }

                if (dataSet.PeriodBegin.HasValue && 
                    dataSet.PeriodEnd.HasValue && 
                    dataSet.PeriodBegin.Value > dataSet.PeriodEnd.Value)
                {
                    course.Errors.Add($"Invalid period begin {dataSet.PeriodBegin} > period end {dataSet.PeriodEnd}");
                    course.IsValid = false;
                    continue;
                }

                if (dataSet.PeriodBegin.HasValue && !dataSet.PeriodEnd.HasValue)
                {
                    course.Errors.Add($"Invalid period begin {dataSet.PeriodBegin} and no period end");
                    course.IsValid = false;
                    continue;
                }

                if (!dataSet.PeriodBegin.HasValue && dataSet.PeriodEnd.HasValue)
                {
                    course.Errors.Add($"Invalid period end {dataSet.PeriodEnd} and no period begin");
                    course.IsValid = false;
                    continue;
                }


                foreach (var day in days)
                {
                    var weekDay = day.Trim().ToUpper();
                    switch (weekDay)
                    {
                        case "MO":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Monday);
                            break;
                        case "DI":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Tuesday);
                            break;
                        case "MI":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Wednesday);
                            break;
                        case "DO":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Thursday);
                            break;
                        case "FR":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Friday);
                            break;
                        case "SA":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Saturday);
                            break;
                        case "SO":
                            dataSet.DaysOfWeek.Add(DayOfWeek.Sunday);
                            break;
                        default:
                            course.Errors.Add($"Invalid weekday {day}");
                            course.IsValid = false;
                            continue;
                    }

                }

                try
                {
                    var nFrq = int.Parse(frq);
                    if (nFrq == 1 || nFrq == 7 || nFrq == 14)
                    {
                        dataSet.Frequency = nFrq;
                    }
                    else
                    {
                        course.Errors.Add($"Invalid frequency {frq}");
                        course.IsValid = false;
                        continue;
                    }
                }
                catch
                {
                    course.Errors.Add($"Invalid frequency {frq}");
                    course.IsValid = false;
                    continue;
                }


                // Beginn und Ende der Termine (Uhrzeit)
                try
                {
                    if (string.IsNullOrEmpty(begin))
                    {
                        course.Errors.Add("Invalid date no begin");
                        course.IsValid = false;
                        continue;
                    }
                    else
                    {
                        dataSet.DateBegin = DateTime.Parse(begin);
                    }
                }
                catch
                {
                    course.Errors.Add($"Invalid date begin {begin}");
                    course.IsValid = false;
                    continue;
                }

                try
                {
                    if (string.IsNullOrEmpty(end))
                    {
                        course.Errors.Add("Invalid date no end and no segment");
                        course.IsValid = false;
                        continue;
                    }
                    else
                    {
                        dataSet.DateEnd = DateTime.Parse(end);
                    }
                }
                catch
                {
                    course.Errors.Add($"Invalid date end {end}");
                    course.IsValid = false;
                    continue;
                }

                if (dataSet.DateBegin.Value >= dataSet.DateEnd.Value)
                {
                    course.Errors.Add($"Invalid date begin {dataSet.DateBegin} >= date end {dataSet.DateEnd}");
                    course.IsValid = false;
                    continue;
                }

                // Räume, Dozenten, Kohorten und Module ergänzen
                foreach (var room in rooms)
                {
                    dataSet.Rooms.Add(new ImportItem { Token = room.Trim() });
                }
                foreach (var lecturer in lecturers)
                {
                    dataSet.Lecturers.Add(new ImportItem { Token = lecturer.Trim() });
                }
                foreach (var cohort in cohorts)
                {
                    dataSet.Cohorts.Add(new ImportItem { Token = cohort.Trim() });
                }

                dataSet.Capacity = null;
                if (!string.IsNullOrEmpty(capacity))
                {
                    try
                    {
                        var nCapacity = int.Parse(capacity);
                        if (nCapacity >= 0)
                        {
                            dataSet.Capacity = nCapacity;
                        }
                    }
                    catch
                    {
                        course.Errors.Add($"Invalid capacity {capacity}");
                        course.IsValid = false;
                        continue;
                    }
                }

                foreach (var module in modules)
                {
                    dataSet.Modules.Add(new ImportItem { Token = module.Trim() });
                }

                // der Titel wurde bereits oben ergänzt, wenn er nicht vorhanden war

                ctx.Courses[course].Add(dataSet);
            }
        }

    }
}
