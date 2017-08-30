using System;
using System.IO;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class FakeViewExtensions
    {
        /// <summary>Renders a view to string.</summary> 
        public static string RenderPartialToString(this HtmlHelper html, string viewName, object viewData)
        {
            return RenderViewToString(html.ViewContext.Controller.ControllerContext, viewName, viewData);
        }

        /// <summary>Renders a view to string.</summary> 
        public static string RenderViewToString(this Controller controller, string viewName, object viewData)
        {
            return RenderViewToString(controller.ControllerContext, viewName, viewData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String RenderViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}