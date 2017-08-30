using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Fillter
{
    /// <summary>
    /// 
    /// </summary>
    public class FillterAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// 
        /// </summary>
        public override string AreaName 
        {
            get 
            {
                return "Fillter";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
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