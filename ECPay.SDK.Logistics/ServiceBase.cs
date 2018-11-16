using System.Threading.Tasks;

namespace ECPay.SDK.Logistics
{
    public class ServiceBase
    {
        /// <summary>物流介接 HashKey</summary>
        public string HashKey;

        /// <summary>物流介接 HashIV</summary>
        public string HashIV;

        /// <summary>商店代號(特店編號)</summary>
        public string MerchantID;

        /// <summary>要呼叫的服務URL</summary>
        public string ServiceURL;

        /// <summary>呼叫服務的Http方式</summary>
        public ServiceMethods ServiceMethod;

        public ServiceBase()
        {
        }

        public ServiceBase(string HashKey, string HashIV, string MerchantID, string ServiceURL)
        {
            this.HashKey = HashKey;
            this.HashIV = HashIV;
            this.MerchantID = MerchantID;
            this.ServiceURL = ServiceURL;
        }

        protected virtual string genPostHtml(string SubmitURL, string ButtonText)
        {
            string strHtml = "<div style='text-align:center;'>" +
                                "<form id='ecpayForm' method='POST' action=" + SubmitURL + ">" +
                                    "<input type='submit' id='__paymentButton' value='" + ButtonText + "' />" +
                                "</form>" +
                             "</div>";
            return strHtml;
        }
    }

    public enum ServiceMethods { Get, Post }

    public interface ServiceCall
    {
        Response<string> Send();

        Task<Response<string>> SendAsync();
    }
}