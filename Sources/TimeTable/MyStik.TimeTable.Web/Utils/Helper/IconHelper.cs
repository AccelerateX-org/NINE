using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyStik.TimeTable.Web.Utils.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="iconClass"></param>
        /// <param name="linkText"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
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