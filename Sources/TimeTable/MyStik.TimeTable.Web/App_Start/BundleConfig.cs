using System.Web;
using System.Web.Optimization;

namespace MyStik.TimeTable.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/i18n/jquery.ui.datepicker-de.js",
                        "~/Scripts/globalize/globalize.js",
                        "~/Scripts/globalize/cultures/globalize.culture.de-DE.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery-migrate*",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                      "~/Scripts/fullcalendar.js",
                      "~/Scripts/nine.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap/plugins").Include(
                        "~/Scripts/jquery.cleditor.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/fullcalendar/css").Include(
                      "~/Content/fullcalendar.css",
                      "~/Content/fullcalendar.print.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap/plugins").Include(
                        "~/Content/jquery.cleditor.css"));
            */


            // Warum auch immer das zuerst separat geladen werden muus
            // wenn man es unten dazu gibt, dann wird es in anderer Reihenfolge geladen
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                        "~/Scripts/jquery.signalr-{version}.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));



            /*
            bundles.Add(new ScriptBundle("~/Assets/global/plugins/js").Include(
                "~/Assets/global/plugins/respond.min.js",
                "~/Assets/global/plugins/excanvas.min.js",
                "~/Assets/global/plugins/jquery-migrate.min.js",
                "~/Assets/global/plugins/jquery-ui/jquery-ui.min.js",
                "~/Assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/Assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                "~/Assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/Assets/global/plugins/jquery.blockui.min.js",
                "~/Assets/global/plugins/jquery.cokie.min.js",
                "~/Assets/global/plugins/uniform/jquery.uniform.min.js",
                "~/Assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js"));


            bundles.Add(new ScriptBundle("~/Template/Layout/js").Include(
                "~/Assets/global/scripts/metronic.js",
                "~/Assets/admin/layout/scripts/layout.js",
                "~/Assets/admin/layout/scripts/quick-sidebar.js"));
            */



            /*
            bundles.Add(new StyleBundle("~/Assets/global/plugins/css").Include(
                "~/Assets/global/plugins/font-awesome/css/font-awesome.min.css",
                "~/Assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
                "~/Assets/global/plugins/bootstrap/css/bootstrap.min.css",
                "~/Assets/global/plugins/uniform/css/uniform.default.css",
                "~/Assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css"));

            bundles.Add(new StyleBundle("~/Assets/global/css").Include(
                "~/Assets/global/css/components.css",
                "~/Assets/global/css/plugins.css"));


            bundles.Add(new StyleBundle("~/Assets/admin/layout/css").Include(
                "~/Assets/admin/layout/css/layout.css",
                "~/Assets/admin/layout/css/themes/fillter.css",
                "~/Assets/admin/layout/css/custom.css"));

            bundles.Add(new StyleBundle("~/Assets/fillter/css").Include(
                "~/Assets/fillter/css/fillter.css"));
            */
        }
    }
}
