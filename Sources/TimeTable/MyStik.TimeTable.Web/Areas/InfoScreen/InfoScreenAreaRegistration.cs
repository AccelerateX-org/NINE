using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.InfoScreen
{
    /// <summary>
    /// 
    /// </summary>
    public class InfoScreenAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// 
        /// </summary>
        public override string AreaName 
        {
            get 
            {
                return "InfoScreen";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
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