using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyStik.TimeTable.Web.Utils.Helper
{
    public static class IconHelper
    {
        public static IHtmlString IconActionLink(this HtmlHelper htmlHelper, string iconClass, string linkText, string action, string controller, object htmlAttributes)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var iconTag = new TagBuilder("i");
            iconTag.Attributes.Add("class", iconClass);

            var builder = new TagBuilder("a") {InnerHtml = iconTag + " " + linkText};
            builder.Attributes["href"] = urlHelper.Action(action, controller);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(builder.ToString());
        }

    }
}