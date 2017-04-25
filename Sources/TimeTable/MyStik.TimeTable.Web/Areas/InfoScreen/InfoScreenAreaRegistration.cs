using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.InfoScreen
{
    public class InfoScreenAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InfoScreen";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InfoScreen_default",
                "InfoScreen/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "MyStik.TimeTable.Web.Areas.InfoScreen.Controllers" }
            );
        }
    }
}