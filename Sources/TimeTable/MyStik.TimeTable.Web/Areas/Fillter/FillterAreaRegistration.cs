using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Fillter
{
    public class FillterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Fillter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Fillter_default",
                "Fillter/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "MyStik.TimeTable.Web.Areas.Fillter.Controllers" }
            );
        }
    }
}