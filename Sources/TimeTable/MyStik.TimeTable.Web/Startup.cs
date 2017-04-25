using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyStik.TimeTable.Web.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MyStik.TimeTable.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);

            // Die Datenbank muss existieren!
            GlobalConfiguration.Configuration.UseSqlServerStorage("LogDbContext");



            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new HangFireAuthFilter() }
            });


            app.UseHangfireServer();

        }
    }
}
