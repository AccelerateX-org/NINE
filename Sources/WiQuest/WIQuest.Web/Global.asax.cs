using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WIQuest.Web.Data;
using WIQuest.Web.Models;

namespace WIQuest.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            Database.SetInitializer(new CreateDatabaseIfNotExists<LogDbContext>());
            var db = new LogDbContext();
            if (!db.Log.Any())
            {
                var log = new Log
                {
                    Date = DateTime.Now,
                    Level = "INIT",
                    Logger = "INIT",
                    Message = "Initial Entry",
                    Thread = "xxx",
                };
                db.Log.Add(log);
                db.SaveChanges();
            }
            
            // bei jedem Start der Anwendung wird die Datenbank neu angelegt
            var path = Server.MapPath("~/images");
            Database.SetInitializer(new TestDataInitializer(path));
        }
    }
}
