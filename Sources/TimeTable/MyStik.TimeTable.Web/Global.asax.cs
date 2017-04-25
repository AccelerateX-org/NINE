using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Initializers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyStik.TimeTable.Data.DefaultData;
using MyStik.TimeTable.Web.Migrations;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
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

            GlobalSettings.Init(new DateTime(2015, 6, 16));

            // Datenbank anlegen bzw. initialiseren
            TimeTableDbContext dbTT = new TimeTableDbContext();
            if (!dbTT.Organisers.Any())
            {
                InfrastructureData.InitOrganisation(dbTT);
                InfrastructureData.InitCurriculum(dbTT);
                InfrastructureData.InitGroupTemplates(dbTT);
                InfrastructureData.InitSemester(dbTT);
            }
        }
    }
}
