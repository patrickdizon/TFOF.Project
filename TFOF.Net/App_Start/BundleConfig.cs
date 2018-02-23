using System.Web;
using System.Web.Optimization;

namespace TFOF
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"
            ));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                //Angular
                "~/Scripts/angular.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/js").Include(

                "~/Scripts/date.js",

                // "~/Scripts/materialize/materialize.min.js",
                // "~/Scripts/respond.js",
                //jQuery-UI
                "~/Scripts/jquery-ui.min.js",

                //UndescoreJS
                "~/Scripts/underscore/underscore-min.js",

                //Pluralize
                "~/Scripts/pluralize/pluralize.js",

                //Moment
                "~/Scripts/moment/moment-with-locales.min.js",

                //Chart.js
                "~/Scripts/Chart.min.js",

                //InputMask.js
                "~/Scripts/inputmask/dist/min/jquery.inputmask.bundle.min.js",
                "~/Scripts/inputmask/extra/bindings/inputmask.binding.js",


                //Angular
                "~/Scripts/angular.min.js",
                "~/Scripts/angular-cookies.js",
                "~/Scripts/angular-resource.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-chart/dist/angular-chart.min.js",
                "~/Scripts/angular-filter/angular-filter.js",
                "~/Scripts/angular-hotkeys/build/hotkeys.min.js",
                "~/Scripts/angular-local-storage/angular-local-storage.js",
                "~/Scripts/angular-moment/angular-moment.js",
                "~/Scripts/angular-odataresources/odataresources.js",
                "~/Scripts/angular-ui-bootstrap/ui-bootstrap-tpls-1.3.3.min.js",
                "~/Scripts/angular-ui-mask/dist/mask.js",
                "~/Scripts/angular-ui-router/angular-ui-router.min.js",
                "~/Scripts/angular-xeditable/xeditable.min.js",

                "~/Scripts/selectize/dist/js/standalone/selectize.min.js",
                "~/Scripts/tfof/angular-prompt.js",
                //App Angular Libraries
                "~/Scripts/tfof/app.js",
                "~/Scripts/tfof/config.js",
                "~/Scripts/tfof/directives.js",
                "~/Scripts/tfof/filters.js",
                "~/Scripts/tfof/services.js",
                "~/Scripts/tfof/controllers.js",
                "~/Scripts/tfof/init.js",

                //App Angular Libraries
                "~/Scripts/angular-google-maps.min.js",
                "~/Scripts/angular-google-maps-street-view.min.js",
                "~/Scripts/lodash.min.js",
                "~/Scripts/ng-map.min.js",                

                //Finally Bootstrap
                "~/Scripts/bootstrap.min.js"
                //"~/Scripts/fixed-header-table/jquery.fixedheadertable.min.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/font-awesome.min.css",
                "~/Content/angular*",
                //"~/Scripts/selectize/dist/css/selectize.css",
                "~/Scripts/selectize/dist/css/selectize.bootstrap3.css",
                "~/Scripts/angular-hotkeys/build/hotkeys.min.css",
                //"~/Content/materialize/css/materialize.min.css",
                "~/Content/Site.css"
            ));
        }
    }
}
