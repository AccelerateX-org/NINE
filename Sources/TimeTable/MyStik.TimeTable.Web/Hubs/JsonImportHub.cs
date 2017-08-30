using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Json;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonImportHub : Hub
    {
        /// <summary>
        /// Löscht alle Kurse aus dem angegebenen Semester, die aus gpUntis importiert wurden
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        public void DeleteSemester(Guid semId, Guid orgId)
        {
            var db = new TimeTableDbContext();

            var semService = new SemesterService();
            var timeTableService = new TimeTableInfoService(db);

            var msg = "Sammle Daten";
            var perc1 = 0;

            Clients.Caller.updateProgress(msg, perc1);

            var semester = semService.GetSemester(semId);
            if (semester == null)
            {
                msg = "Semester existiert nicht";
                perc1 = 100;

                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            var courses = db.Activities.OfType<Course>().Where(c =>
                    c.Organiser.Id == orgId &&                                  // Veranstalter
                    (c.SemesterGroups.Any(g =>                                  // Mit Semestergruppe
                         g.Semester.Id == semId &&
                         g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == orgId) ||
                     !c.SemesterGroups.Any()                                 // oder ohne Zuordnung
                    ) &&
                    (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("JSON")))    // aus GPUNTIS
                .ToList();

            msg = $"Lösche {courses.Count} von {courses.Count} Kursen";
            perc1 = 0;
            Clients.Caller.updateProgress(msg, perc1);

            var n = courses.Count;
            var i = 0;
            foreach (var course in courses)
            {
                i++;
                msg = string.Format("Lösche {0}", course.Name);
                perc1 = (i * 100) / n;

                Clients.Caller.updateProgress(msg, perc1);

                timeTableService.DeleteCourse(course.Id);
            }


            msg = "Alle Kurse gelöscht";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        public void ImportSemester(Guid semId, Guid orgId)
        {
            var db = new TimeTableDbContext();

            var tempDir = Path.GetTempPath();

            var semester = db.Semesters.SingleOrDefault(s => s.Id == semId);
            var org = db.Organisers.SingleOrDefault(o => o.Id == orgId);

            if (semester != null && org != null)
            {
                tempDir = Path.Combine(tempDir, semester.Name);
                tempDir = Path.Combine(tempDir, org.ShortName);
            }


            var msg = "Sammle Daten";
            var perc1 = 0;

            Clients.Caller.updateProgress(msg, perc1);

            if (semester == null)
            {
                msg = "Semester existiert nicht";
                perc1 = 100;

                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            if (!Directory.Exists(tempDir))
            {
                msg = string.Format("Verzeichnis für {0} existiert nicht", semester.Name);
                perc1 = 100;
                Clients.Caller.updateProgress(msg, perc1);
                return;
            }

            FileReader reader = new FileReader();

            try
            {
                reader.ReadFiles(tempDir);


            }
            catch (Exception ex)
            {
                msg = string.Format("FEHLER bei Datei einlesen: {0}", ex.Message);
                perc1 = 100;
                Clients.Caller.updateProgress(msg, perc1);
                return;
            }




            // fehler hier ignorieren => muss noch an anderer Stelle besser gelöst werden
            // Annahme: Fehler sind dem Anwender bekannt, da vorher ein Check durchgeführt wurde
            // daher können die Checks oben auch entfallen
            var importer = new SemesterImport(reader.Context, semId, orgId);
            importer.CheckFaculty();

            var n = reader.Context.ValidCourses.Count;
            var i = 0;
            var errLog = "";

            foreach (var k in reader.Context.ValidCourses)
            {
                var course = db.Activities.OfType<Course>().FirstOrDefault(x =>
                    x.ExternalSource.Equals("JSON") &&
                    x.ExternalId.Equals(k.CourseId.ToString()) &&
                    x.Organiser.Id == org.Id);
                try
                {
                    if (course != null)
                    {
                        msg = importer.UpdateCourse(course, k);
                    }
                    else
                    {
                        msg = importer.ImportCourse(k);
                    }

                }
                catch (Exception e)
                {
                    msg = "FEHLER bei " + k.ShortName;
                    errLog += k.ShortName + ": " + e.Message;
                    if (e.InnerException != null)
                    {
                        errLog += " Inner: " + e.InnerException.Message;
                        if (e.InnerException.InnerException != null)
                        {
                            errLog += " InnerInner: " + e.InnerException.InnerException.Message;
                        }
                    }
                }
                i++;

                perc1 = (i * 100) / n;

                Clients.Caller.updateProgress(msg, perc1);
            }

            msg = "Alle Kurse importiert";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);


            // Jetzt noch die Lotterien
            msg = "Importie Lotterien";
            perc1 = 0;
            Clients.Caller.updateProgress(msg, perc1);


            if (reader.Context.Model.Lotteries != null)
            {
                n = reader.Context.Model.Lotteries.Count;
                i = 0;

                foreach (var k in reader.Context.Model.Lotteries)
                {
                    msg = importer.ImportLottery(k);
                    i++;

                    perc1 = (i * 100) / n;

                    Clients.Caller.updateProgress(msg, perc1);
                }
            }

            msg = "Alle Kurse und Lotterien importiert. " + errLog;

            

            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);

        }
    }
}