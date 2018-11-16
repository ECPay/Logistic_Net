using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ECPay.Logistics.ClientWebAPISample.Startup))]

namespace ECPay.Logistics.ClientWebAPISample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}