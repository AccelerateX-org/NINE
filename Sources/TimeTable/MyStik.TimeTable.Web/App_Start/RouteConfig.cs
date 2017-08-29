using System.Web.Mvc;
using System.Web.Routing;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Scheint egal zu sein, wenn was dahinter liegt
            // nur wenn nix da ist, dann unterscheidet sich die Fehlermeldung
            // routes.IgnoreRoute("Analytics");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MyStik.TimeTable.Web.Controllers" }
            );
        }
    }
}
