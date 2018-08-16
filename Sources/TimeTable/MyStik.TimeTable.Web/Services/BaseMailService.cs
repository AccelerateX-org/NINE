using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.DataServices.Drawing;
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