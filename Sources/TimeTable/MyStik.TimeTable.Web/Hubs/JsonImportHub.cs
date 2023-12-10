using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonImportHub : Hub
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        /// <param name="formatId"></param>
        public string ImportSemester(Guid semId, Guid orgId, string formatId)
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
                return String.Empty;
            }

            if (!Directory.Exists(tempDir))
            {
                msg = string.Format("Verzeichnis für {0} existiert nicht", semester.Name);
                perc1 = 100;
                Clients.Caller.updateProgress(msg, perc1);
                return String.Empty;
            }

            DataServices.IO.Json.FileReader reader = new DataServices.IO.Json.FileReader();

            try
            {
                reader.ReadFiles(tempDir);


            }
            catch (Exception ex)
            {
                msg = string.Format("FEHLER bei Datei einlesen: {0}", ex.Message);
                perc1 = 100;
                Clients.Caller.updateProgress(msg, perc1);
                return String.Empty;
            }




            // fehler hier ignorieren => muss noch an anderer Stelle besser gelöst werden
            // Annahme: Fehler sind dem Anwender bekannt, da vorher ein Check durchgeführt wurde
            // daher können die Checks oben auch entfallen
            var importer = new DataServices.IO.Json.SemesterImport(reader.Context, semId, orgId);
            importer.CheckFaculty();

            var n = reader.Context.ValidCourses.Count;
            var i = 0;
            var errLog = "";

            foreach (var k in reader.Context.ValidCourses)
            {
                var course = db.Activities.FirstOrDefault(x =>
                    x.ExternalSource.Equals(formatId) &&
                    x.ExternalId.Equals(k.CourseId) &&
                    x.Organiser.Id == org.Id);
                try
                {
                    if (course != null)
                    {
                        if (course is Course)
                        {
                            msg = importer.UpdateCourse(course as Course, k);
                        }

                    }
                    else
                    {
                        msg = importer.ImportActivity(k);
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


            string tmpFileName = "Import_" + semester.Name + "_" + org.ShortName + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");

            var path = Path.Combine(Path.GetTempPath(), tmpFileName);

            var html = importer.GetReport();

            File.WriteAllText(path, html, Encoding.UTF8);

            return tmpFileName;
        }
    }
}