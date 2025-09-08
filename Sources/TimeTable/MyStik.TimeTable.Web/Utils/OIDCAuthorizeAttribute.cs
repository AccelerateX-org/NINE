using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyStik.TimeTable.Web.Utils
{
    public class OIDCAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            var r = filterContext.ActionDescriptor.GetCustomAttributes(true).Any(x => x is AllowAnonymousAttribute) ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).Any(x => x is AllowAnonymousAttribute);
            if (r) return;

            if (filterContext.HttpContext.Request.IsAuthenticated) return;

            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" }
                });

        }
    }
}