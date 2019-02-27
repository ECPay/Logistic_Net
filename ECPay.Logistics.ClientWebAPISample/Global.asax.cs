using ECPay.Logistics.ClientWebAPISample.Results;
using Rollbar;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ECPay.Logistics.ClientWebAPISample
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private const string postServerItemAccessToken = "ec24f34cc6e24f56b33986bf0eba32ad";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RollbarLocator.RollbarInstance.Configure(new RollbarConfig(postServerItemAccessToken) { Environment = "SDK-1.0.1" });
        }
    }

    //Rollbar action filter
    public class RollbarExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            Rollbars.Report(filterContext.Exception);
        }
    }
}