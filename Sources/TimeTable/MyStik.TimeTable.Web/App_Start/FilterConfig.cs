using System.Web.Mvc;

namespace MyStik.TimeTable.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new RequireHttpsAttribute());
        }
    }
}
