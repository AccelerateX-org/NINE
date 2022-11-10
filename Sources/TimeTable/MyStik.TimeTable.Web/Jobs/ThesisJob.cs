using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Hangfire;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using Postal;

namespace MyStik.TimeTable.Web.Jobs
{
    public class ThesisJob
    {
        private ILog logger = LogManager.GetLogger("ThesisJob");


        public void InitJob()
        {
            // Morgens um 10:00
            RecurringJob.AddOrUpdate<ThesisJob>("Thesis.CheckAutoIssue", x => x.CheckIssueDates(), "0 10 * * *");

        }


        /// <summary>
        /// Automatische Anmeldung aller Abschlussarbeiten
        /// Wenn Job unterbrochen, dann kein Problem. Wird nu
        /// Vorgehen
        /// - Alle Arbeiten sammeln
        /// - Jeden einzeln Anschreiben To: Student, cc: Betreuer
        /// - Sammel E-Mail an SysAdmin (fürs erste)
        /// </summary>
        public void CheckIssueDates()
        {
            var db = new TimeTableDbContext();

            var today = DateTime.Today;

            // haben einen geplanten Beginn
            // noch nicht angemeldet
            // noch nicht abgegeben (zur Sicherheit der alten Fälle)
            var allThesis = db.Theses
                .Where(x => 
                    x.PlannedBegin != null && x.PlannedBegin <= today && 
                    x.IssueDate == null && x.DeliveryDate == null &&
                    x.Supervisors.Any(s => s.AcceptanceDate.HasValue))
                .OrderBy(x => x.Student.Curriculum.ShortName)
                .ThenBy(x => x.PlannedBegin.Value)
                .ToList();

            var allCurr = allThesis.Select(x => x.Student.Curriculum).Distinct();


            var allCurrWithPk = allCurr.Where(x =>
                x.Organiser.Autonomy != null && 
                x.Organiser.Autonomy.Committees.Any(c =>
                    c.Name.Equals("PK") &&
                    c.Curriculum != null &&
                    c.Curriculum.Id == x.Id))
                .ToList();


            var allThesisWithPk = allThesis.Where(x => allCurrWithPk.Contains(x.Student.Curriculum)).ToList();



            // Prepare Postal classes to work outside of ASP.NET request
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));

            var emailService = new EmailService(engines);


            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            // Sammel
            var emailTotal = new ThesisAutoIssueSummaryEmail()
            {
                From = smtpSection.From,
                To = "olav.hinz@hm.edu",
                Subject = "Abschlussarbeiten Automatische Anmeldungem",
                Theses = allThesisWithPk
            };


            // Rendern und senden
            emailService.Send(emailTotal);

        }
    }
}