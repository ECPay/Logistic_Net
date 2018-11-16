using System.Web.Optimization;

namespace ECPay.Logistics.ClientWebAPISample
{
    public class BundleConfig
    {
        // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/AdminLte/css")
                   .Include("~/Content/bootstrap.css",
                            "~/Content/css/font-awesome.css",
                            "~/admin-lte/css/fonts/font-awesome.min.css",
                            "~/admin-lte/css/fonts/ionicons.min.css",
                            "~/admin-lte/css/AdminLTE.css",
                            "~/admin-lte/css/skins/_all-skins.css",
                            "~/Content/gwsdk.css"));

            bundles.Add(new ScriptBundle("~/AdminLte/js")
                   .Include("~/Scripts/jquery-{version}.js",
                            "~/Scripts/jquery.validate*",
                            "~/Scripts/jquery.unobtrusive-ajax*",
                            "~/Scripts/bootstrap.js",
                            "~/admin-lte/js/app.js",
                            "~/Scripts/gwsdk.js"));
        }
    }
}