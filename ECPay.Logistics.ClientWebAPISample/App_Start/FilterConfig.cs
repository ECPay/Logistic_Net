using System.Web.Mvc;

namespace ECPay.Logistics.ClientWebAPISample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RollbarExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}