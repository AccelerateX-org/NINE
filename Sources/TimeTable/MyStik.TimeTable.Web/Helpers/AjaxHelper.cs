using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MyStik.TimeTable.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class AjaxHelperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ajaxHelper"></param>
        /// <param name="linkIcon"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="ajaxOptions"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionButton(this AjaxHelper ajaxHelper, string linkIcon, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var repId = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repId, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);


            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(linkIcon))
            {
                var linkBuilder = new TagBuilder("i");
                linkBuilder.AddCssClass(linkIcon);
                sb.Append(linkBuilder.ToString());
            }

            if (!string.IsNullOrEmpty(linkText))
            {
                sb.Append(" " + linkText);
            }

            if (string.IsNullOrEmpty(linkIcon) && string.IsNullOrEmpty(linkText))
            {
                sb.Append("???");
            }

            return MvcHtmlString.Create(lnk.ToString().Replace(repId, sb.ToString()));
        }
    }
}