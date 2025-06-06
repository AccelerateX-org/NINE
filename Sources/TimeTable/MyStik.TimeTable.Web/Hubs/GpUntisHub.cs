﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO.GpUntis;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class GpUntisHub : Hub
    {

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

                perc1 = (i * 90) / n;

                Clients.Caller.updateProgress(msg, perc1);
            }

            msg = "Alle Kurse importiert - Importiere Raumreservierungen";
            perc1 = 90;
            Clients.Caller.updateProgress(msg, perc1);


            n = reader.Context.Blockaden.Count;
            i = 0;

            foreach (var k in reader.Context.Blockaden)
            {
                msg = importer.ImportReservation(k);
                i++;

                perc1 = 90 + (i * 10) / n;

                Clients.Caller.updateProgress(msg, perc1);
            }

            msg = "Alle Daten importiert";
            perc1 = 100;
            Clients.Caller.updateProgress(msg, perc1);


        }
    }
}