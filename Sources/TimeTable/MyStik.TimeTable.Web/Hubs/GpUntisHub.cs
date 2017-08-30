using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.GpUntis;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class GpUntisHub : Hub
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
                (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("GPUNTIS")))    // aus GPUNTIS
                .ToList();

            msg = string.Format("Lösche {0} von {1} Kursen", courses.Count, courses.Count);
            perc1 = 0;
            Clients.Caller.updateProgress(msg, perc1);

            var n = courses.Count;
            var i = 0;
            foreach (var course in courses)
            {
                i++;
                msg = string.Format("Lösche {0}", course.Name);
                perc1 = (i*100) / n;

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
        /// <returns></returns>
        public string CheckConsistency(Guid semId, Guid orgId)
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


            var msg = "Lese Dateien";
            var perc1 = 0;

            Clients.Caller.updateProgress(msg, perc1);

            FileReader reader = new FileReader();

            try
            {
                reader.ReadFiles(tempDir);

                var importer = new SemesterImport(reader.Context, semId, orgId);

                msg = "prüfe Gruppen";
                perc1 = 25;
                Clients.Caller.updateProgress(msg, perc1);
                // Gruppen müssen existieren! => Fehler, wenn nicht
                importer.CheckGroups();

                msg = "prüfe Räume";
                perc1 = 50;
                Clients.Caller.updateProgress(msg, perc1);
                // Räume sollten existieren => Warnung
                // Zuordnungen zu Räumen sollten existieren => Warnung
                importer.CheckRooms();

                msg = "prüfe Dozenten";
                perc1 = 75;
                Clients.Caller.updateProgress(msg, perc1);
                // Dozenten sollten existieren => Warnung
                importer.CheckLecturers();

            }
            catch (Exception ex)
            {
                reader.Context.AddErrorMessage("Import", ex.Message, true);
            }


            msg = "beendet";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);


            if (reader.Context.ErrorMessages["Import"] == null)
                return null;

            var sb = new StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td>Import</td>");
            sb.Append("<td>");
            foreach (var message in reader.Context.ErrorMessages["Import"])
            {
                sb.AppendFormat("<div>{0}</li>", message);
            }
            sb.Append("</td>");
            sb.Append("</tr>");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <param name="orgId"></param>
        /// <param name="firstDate"></param>
        /// <param name="lastDate"></param>
        public void ImportSemester(Guid semId, Guid orgId, string firstDate, string lastDate)
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
            var importer = new SemesterImport(reader.Context, semId, orgId, firstDate, lastDate);
            
            var n = reader.Context.Kurse.Count;
            var i = 0;

            foreach (var k in reader.Context.Kurse)
            {
                msg = importer.ImportCourse(k);
                i++;

                perc1 = (i * 100) / n;

                Clients.Caller.updateProgress(msg, perc1);
            }

            msg = "Alle Kurse importiert";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);
        }
    }
}