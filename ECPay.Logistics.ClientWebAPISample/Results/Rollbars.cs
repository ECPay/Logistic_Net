using Rollbar;
using System;

namespace ECPay.Logistics.ClientWebAPISample.Results
{
    public static class Rollbars
    {
        public static void Report(string message)
        {
            RollbarLocator.RollbarInstance.Info(message);
        }

        public static void Report(Exception ex)
        {
            RollbarLocator.RollbarInstance.Error(ex);
        }
    }
}