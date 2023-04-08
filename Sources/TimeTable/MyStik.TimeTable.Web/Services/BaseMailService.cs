using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using Postal;

namespace MyStik.TimeTable.Web.Services
{
    public class BaseMailService
    {
        protected ILog Logger { get;  }

        protected UserInfoService UserService { get; }

        protected Postal.EmailService EmailService { get; }


        protected BaseMailService()
        {
            /*
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            FromEMailAdress = $"{topic} <{smtpSection.From}>";
            */

            Logger = LogManager.GetLogger("Mail");

            UserService = new UserInfoService();

            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            var engine = new FileSystemRazorViewEngine(viewsPath);
            engines.Add(engine);
            EmailService = new Postal.EmailService(engines);
        }

    }
}