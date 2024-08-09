using Postal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Web.Models;
using log4net;

namespace MyStik.TimeTable.Web.Jobs
{
    public class BaseJob
    {
        protected Postal.EmailService EmailService { get; }
        protected SmtpSection SmtpSection { get; }


        private ILog _logger = null;
        protected ILog Logger
        {
            get => _logger ?? (_logger = LogManager.GetLogger("BaseJob"));
            set => _logger = value;
        }


        public BaseJob()
        {
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection { new FileSystemRazorViewEngine(viewsPath) };

            EmailService = new EmailService(engines);
            SmtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }

        protected void SendMail(BaseEmail email)
        {
            email.From = SmtpSection.From;

            try
            {
                EmailService.Send(email);
                Logger.InfoFormat("Mail with subject {0} sent", email.Subject);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Erroe sending mail with subject {0} to {1}", email.Subject, email.To);
            }
        }

    }
}