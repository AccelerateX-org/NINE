using System.Web.Mvc;
using MyStik.TimeTable.Web.Utils;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());

            //filters.Add(new OIDCAuthorizeAttribute());

#if !DEBUG
            filters.Add(new RequireHttpsAttribute());
#endif
        }
    }
}
