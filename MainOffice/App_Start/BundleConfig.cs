using System.Web;
using System.Web.Optimization;

namespace MainOffice
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.bundle.min.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-table").Include(
                       "~/Scripts/tableExport.min.js",
                       "~/Scripts/bootstrap-table.js",
                       "~/Scripts/bootstrap-table-locale-all.min.js",
                       "~/Scripts/bootstrap-table-export.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include(
                       "~/Scripts/bootstrap-select.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/bootstrap-table.min.css",
                        "~/Content/bootstrap-select.css",
                        "~/Content/site.css"));
        }
    }
}
