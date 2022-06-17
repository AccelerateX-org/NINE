using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using Microsoft.Owin;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Areas.Admin.Models;
using MyStik.TimeTable.Web.Migrations;
using MyStik.TimeTable.Web.Models;
using Owin;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

[assembly: OwinStartup(typeof(MyStik.TimeTable.Web.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MyStik.TimeTable.Web
{
    public partial class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();

            app.MapSignalR();
            ConfigureAuth(app);
            ConfigureOAuth(app);

            HttpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(HttpConfiguration);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(HttpConfiguration);


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

            // Die Datenbank muss existieren!
            GlobalConfiguration.Configuration.UseSqlServerStorage("LogDbContext");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthFilter() }
            });


            app.UseHangfireServer();

        }
    }
}
