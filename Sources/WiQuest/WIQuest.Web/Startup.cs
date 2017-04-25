using Hangfire;
using Microsoft.Owin;
using Owin;
using WIQuest.Web.App_Start;

[assembly: OwinStartupAttribute(typeof(WIQuest.Web.Startup))]
namespace WIQuest.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
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
