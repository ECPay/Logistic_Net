namespace ECPay.SDK.Logistics
{
    public static class ServiceDomain
    {
        private static bool _isDebug = false;

        private static void setParameter()
        {
            CVSMAP = APIURL + "Express/map";
            CreateLogisticsOrder = APIURL + "Express/Create";
            ReturnHome = APIURL + "Express/ReturnHome";
            ReturnCVS = APIURL + "Express/ReturnCVS";
            LogisticsCheckAccoounts = APIURL + "Helper/LogisticsCheckAccoounts";
            ReturnHiLifeCVS = APIURL + "Express/ReturnHiLifeCVS";
            UpdateShippingInfo = APIURL + "Helper/UpdateShipmentInfo";
            UpdateStoreInfo = APIURL + "Express/UpdateStoreInfo";
            CancelC2COrder = APIURL + "Express/CancelC2COrder";
            QueryLogisticsTradeInfo = APIURL + "Helper/QueryLogisticsTradeInfo/V2";
            PrintTradeDoc = APIURL + "helper/printTradeDocument";
            PrintUniMartC2COrderInfo = APIURL + "Express/PrintUniMartC2COrderInfo";
            PrintFAMIC2COrderInfo = APIURL + "Express/PrintFAMIC2COrderInfo";
        }

        internal static string APIURL
        {
            get
            {
                if (IsDebug == true)
                {
                    return "https://logistics-stage.ecpay.com.tw/";
                }
                else
                {
                    return "https://logistics.ecpay.com.tw/";
                }
            }
        }

        /// <summary>是否為Debug</summary>
        /// <remarks>
        /// 當使用IsDebug == true 時，呼叫測試機服務
        /// 當使用IsDebug == false 時，呼叫正式機服務
        /// </remarks>
        public static bool IsDebug
        {
            set
            {
                _isDebug = value;
                setParameter();
            }
            get
            {
                return _isDebug;
            }
        }

        /// <summary>電子地圖</summary>
        public static string CVSMAP = APIURL + "Express/map";

        /// <summary>物流訂單產生</summary>
        public static string CreateLogisticsOrder = APIURL + "Express/Create";

        /// <summary>宅配逆物流訂單</summary>
        public static string ReturnHome = APIURL + "Express/ReturnHome";

        /// <summary>超商取貨逆物流訂單</summary>
        public static string ReturnCVS = APIURL + "Express/ReturnCVS";

        /// <summary>逆物流核帳</summary>
        public static string LogisticsCheckAccoounts = APIURL + "Helper/LogisticsCheckAccoounts";

        /// <summary>超商取貨逆物流訂單(萊爾富超商B2C)</summary>
        public static string ReturnHiLifeCVS = APIURL + "Express/ReturnHiLifeCVS";

        /// <summary>廠商修改物流資訊</summary>
        public static string UpdateShippingInfo = APIURL + "Helper/UpdateShipmentInfo";

        /// <summary>更新門市</summary>
        public static string UpdateStoreInfo = APIURL + "Express/UpdateStoreInfo";

        /// <summary>取消訂單</summary>
        public static string CancelC2COrder = APIURL + "Express/CancelC2COrder";

        /// <summary>物流訂單查詢</summary>
        public static string QueryLogisticsTradeInfo = APIURL + "Helper/QueryLogisticsTradeInfo/V2";

        /// <summary>產生托運單(宅配)/一段標(超商取貨)</summary>
        public static string PrintTradeDoc = APIURL + "helper/printTradeDocument";

        /// <summary>列印繳款單(統一超商C2C)</summary>
        public static string PrintUniMartC2COrderInfo = APIURL + "Express/PrintUniMartC2COrderInfo";

        /// <summary>全家列印小白單(全家超商C2C)</summary>
        public static string PrintFAMIC2COrderInfo = APIURL + "Express/PrintFAMIC2COrderInfo";
    }
}