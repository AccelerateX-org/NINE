using MyStik.TimeTable.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyStik.TimeTable.Web.Migrations;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TimeTableDbContext, Data.Migrations.Configuration>());

            log4net.Config.XmlConfigurator.Configure();

            // Datenbank für log4net per EF anlegen
            Database.SetInitializer(new CreateDatabaseIfNotExists<LogDbContext>());

            // ob es das noch wirklich braucht?
            // JA, sonst geht der attach nicht, das Hangfire macht Probleme
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
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("~/Error/AntiForgery", true);
            }
        }
    }
}
