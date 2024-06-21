using RazorEngine;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Utils
{
    /*
    public static class FakeViewExtensions
    {
        public static string MapPath(string filePath)
        {
            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath(filePath) : string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("~", string.Empty).TrimStart('/'));
        }

        public static string RenderPartialToString(string viewPath, object model)
        {
            string viewAbsolutePath = MapPath(viewPath);

            var viewSource = File.ReadAllText(viewAbsolutePath);

            string renderedText = Razor.Parse(viewSource, model);
            return renderedText;
        }


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
    */
}