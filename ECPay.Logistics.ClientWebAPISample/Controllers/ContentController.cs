using ECPay.Logistics.ClientWebAPISample.Results;
using ECPay.SDK.Logistics;
using System;
using System.Web.Mvc;

namespace ECPay.Logistics.ClientWebAPISample.Controllers
{
    public class ContentController : Controller
    {
        private const string _logPath = @"~/App_Data/";          //Log位置
        private const string _errorWord = "回傳失敗，請檢查";    //按下submit按鈕後，若失敗且無例外訊息，顯示此訊息

        public ContentController()
        {
            // 當使用IsDebug == true 時，呼叫測試機服務
            // 當使用IsDebug == false 時，呼叫正式機服務
#if (DEBUG)
            ServiceDomain.IsDebug = true;
#else
            ServiceDomain.IsDebug = false;
#endif
        }

        /// <summary>基本輸入測試</summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>基本輸入測試</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult Index(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string merchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                using (var oLogistics = new LogisticsProvider(hashKey, hashIV, merchantID)) { };
                contentResult = "測試成功!";
            }
            catch (Exception ex)
            {
                Rollbars.Report(ex);
                contentResult = $"測試失敗，訊息：{ex.Message}";
            }
            return Content(contentResult);
        }

        /// <summary>查詢-物流訂單</summary>
        /// <returns></returns>
        public ActionResult QueryLogisticsInfo()
        {
            return View();
        }

        /// <summary>查詢-物流訂單</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult QueryLogisticsInfo(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    //呼叫物流訂單查詢
                    response = oLogistics.QueryLogisticsInfo();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.QueryLogisticsTradeInfo;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //呼叫物流訂單查詢
                    response = oLogistics.QueryLogisticsInfo();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>新增超商物流訂單</summary>
        /// <returns></returns>
        public ActionResult CreateLogisticsOrderForPickUp()
        {
            return View();
        }

        /// <summary>新增超商物流訂單</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CreateLogisticsOrderForPickUp(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();
            try
            {
                #region 頁面參數轉換

                decimal goodsAmount = 0;
                decimal.TryParse(collection["GoodsAmount"], out goodsAmount);
                decimal collectionAmount = 0;
                decimal.TryParse(collection["CollectionAmount"], out collectionAmount);

                #endregion 頁面參數轉換

                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.MerchantTradeNo = "<<您此筆訂單交易編號>>";
                    oLogistics.Send.MerchantTradeDate = DateTime.Parse("yyyy/MM/dd");// "<<您此筆訂單交易日期>>";
                    oLogistics.Send.LogisticsType = LogisticsTypes.CVS;// "<<您此筆訂單的物流類型>>";
                    oLogistics.Send.LogisticsSubType = LogisticsSubTypes.UNIMART; //"<<您此筆訂單的物流子類型>>";
                    oLogistics.Send.ServerReplyURL = "<<您要呼叫的服務位址>>";
                    oLogistics.Send.IsCollection = IsCollections.NO;// "<<您此筆訂單是否有代收付款>>";
                    oLogistics.Send.CollectionAmount = decimal.Parse("999");// "<<您此筆訂單代收付款金額>>";
                    oLogistics.Send.GoodsName = "<<您此筆訂單商品名稱>>";
                    oLogistics.Send.GoodsAmount = decimal.Parse("999"); //"<<您此筆訂單商品金額>>";
                    oLogistics.Send.SenderName = "<<您此筆訂單寄件者名稱>>";
                    oLogistics.Send.SenderPhone = "<<您此筆訂單寄件者電話>>";
                    oLogistics.Send.SenderCellPhone = "<<您此筆訂單寄件者手機>>";
                    oLogistics.Send.ReceiverName = "<<您此筆訂單收件者名稱>>";
                    oLogistics.Send.ReceiverPhone = "<<您此筆訂單收件者電話>>";
                    oLogistics.Send.ReceiverCellPhone = "<<您此筆訂單收件者手機>>";
                    oLogistics.Send.ReceiverEmail = "<<您此筆訂單收件者Email>>";
                    oLogistics.Send.TradeDesc = "<<您此筆訂單交易敘述>>";
                    oLogistics.Send.ClientReplyURL = "<<您要綠界科技返回按鈕導向的瀏覽器端網址>>";
                    oLogistics.Send.LogisticsC2CReplyURL = "<<您要綠界科技呼叫您的主機端網址>>";
                    oLogistics.Send.Remark = "<<您要填寫的其他備註>>";
                    oLogistics.Send.ReturnStoreID = "<<您要填寫的收件人門市代號>>";
                    oLogistics.Send.ReceiverStoreID = "<<您要填寫的退貨門市代號(當為店到店的退貨時，要退回的門市代號)>>";
                    oLogistics.Send.PlatformID = "<<您專案合作平台商代號>>";
                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForPickUp();
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForPickUpSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.CreateLogisticsOrder;
                    oLogistics.Send.MerchantTradeNo = collection["MerchantTradeNo"];
                    oLogistics.Send.MerchantTradeDate = collection["MerchantTradeDate"];
                    oLogistics.Send.LogisticsType = (LogisticsTypes)Enum.Parse(typeof(LogisticsTypes), collection["LogisticsType"]);
                    oLogistics.Send.LogisticsSubType = (LogisticsSubTypes)Enum.Parse(typeof(LogisticsSubTypes), collection["LogisticsSubType"]);
                    oLogistics.Send.ServerReplyURL = collection["ServerReplyURL"];
                    oLogistics.Send.IsCollection = (IsCollections)Enum.Parse(typeof(IsCollections), collection["IsCollection"]);
                    oLogistics.Send.CollectionAmount = collectionAmount;
                    oLogistics.Send.GoodsName = collection["GoodsName"];
                    oLogistics.Send.GoodsAmount = goodsAmount;
                    oLogistics.Send.SenderName = collection["SenderName"];
                    oLogistics.Send.SenderPhone = collection["SenderPhone"];
                    oLogistics.Send.SenderCellPhone = collection["SenderCellPhone"];
                    oLogistics.Send.ReceiverName = collection["ReceiverName"];
                    oLogistics.Send.ReceiverPhone = collection["ReceiverPhone"];
                    oLogistics.Send.ReceiverCellPhone = collection["ReceiverCellPhone"];
                    oLogistics.Send.ReceiverEmail = collection["ReceiverEmail"];
                    oLogistics.Send.TradeDesc = collection["TradeDesc"];
                    oLogistics.Send.ClientReplyURL = collection["ClientReplyURL"];
                    oLogistics.Send.LogisticsC2CReplyURL = collection["LogisticsC2CReplyURL"];
                    oLogistics.Send.Remark = collection["Remark"];
                    oLogistics.Send.ReturnStoreID = collection["ReturnStoreID"];
                    oLogistics.Send.ReceiverStoreID = collection["ReceiverStoreID"];
                    oLogistics.Send.PlatformID = collection["PlateformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForPickUp();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data.Replace("\r\n", "").Replace("\"", "'");
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>產生-新增超商物流訂單 HTML按鈕</summary>
        /// <returns></returns>
        public ActionResult CreateLogisticsOrderForPickUpSubmitButton()
        {
            return View();
        }

        /// <summary>產生-新增超商物流訂單 HTML按鈕</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CreateLogisticsOrderForPickUpSubmitButton(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 頁面參數轉換

                decimal goodsAmount = 0;
                decimal.TryParse(collection["GoodsAmount"], out goodsAmount);
                decimal collectionAmount = 0;
                decimal.TryParse(collection["CollectionAmount"], out collectionAmount);

                #endregion 頁面參數轉換

                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.MerchantTradeNo = "<<您此筆訂單交易編號>>";
                    oLogistics.Send.MerchantTradeDate = "<<您此筆訂單交易日期>>";
                    oLogistics.Send.LogisticsType = "<<您此筆訂單的物流類型>>";
                    oLogistics.Send.LogisticsSubType = "<<您此筆訂單的物流子類型>>";
                    oLogistics.Send.ServerReplyURL = "<<您要呼叫的服務位址>>";
                    oLogistics.Send.IsCollection = "<<您此筆訂單是否有代收付款>>";
                    oLogistics.Send.CollectionAmount = "<<您此筆訂單代收付款金額>>";
                    oLogistics.Send.GoodsName = "<<您此筆訂單商品名稱>>";
                    oLogistics.Send.GoodsAmount = "<<您此筆訂單商品金額>>";
                    oLogistics.Send.SenderName = "<<您此筆訂單寄件者名稱>>";
                    oLogistics.Send.SenderPhone = "<<您此筆訂單寄件者電話>>";
                    oLogistics.Send.SenderCellPhone = "<<您此筆訂單寄件者手機>>";
                    oLogistics.Send.ReceiverName = "<<您此筆訂單收件者名稱>>";
                    oLogistics.Send.ReceiverPhone = "<<您此筆訂單收件者電話>>";
                    oLogistics.Send.ReceiverCellPhone = "<<您此筆訂單收件者手機>>";
                    oLogistics.Send.ReceiverEmail = "<<您此筆訂單收件者Email>>";
                    oLogistics.Send.TradeDesc = "<<您此筆訂單交易敘述>>";
                    oLogistics.Send.ClientReplyURL = <<您要綠界科技返回按鈕導向的瀏覽器端網址>>;
                    oLogistics.Send.LogisticsC2CReplyURL = <<您要綠界科技呼叫您的主機端網址>>;
                    oLogistics.Send.Remark = "<<您要填寫的其他備註>>";
                    oLogistics.Send.ReturnStoreID = "<<您要填寫的收件人門市代號>>";
                    oLogistics.Send.ReceiverStoreID = "<<您要填寫的退貨門市代號(當為店到店的退貨時，要退回的門市代號)>>";
                    oLogistics.Send.PlatformID = "<<您專案合作平台商代號>>";
                    oLogistics.Send.ButtonText = "<<您的按鈕文案>>"
                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForPickUp();
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForPickUpSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.CreateLogisticsOrder;
                    oLogistics.Send.MerchantTradeNo = collection["MerchantTradeNo"];
                    oLogistics.Send.MerchantTradeDate = collection["MerchantTradeDate"];
                    oLogistics.Send.LogisticsType = (LogisticsTypes)Enum.Parse(typeof(LogisticsTypes), collection["LogisticsType"]);
                    oLogistics.Send.LogisticsSubType = (LogisticsSubTypes)Enum.Parse(typeof(LogisticsSubTypes), collection["LogisticsSubType"]);
                    oLogistics.Send.ServerReplyURL = collection["ServerReplyURL"];
                    oLogistics.Send.IsCollection = (IsCollections)Enum.Parse(typeof(IsCollections), collection["IsCollection"]);
                    oLogistics.Send.CollectionAmount = collectionAmount;
                    oLogistics.Send.GoodsName = collection["GoodsName"];
                    oLogistics.Send.GoodsAmount = goodsAmount;
                    oLogistics.Send.SenderName = collection["SenderName"];
                    oLogistics.Send.SenderPhone = collection["SenderPhone"];
                    oLogistics.Send.SenderCellPhone = collection["SenderCellPhone"];
                    oLogistics.Send.ReceiverName = collection["ReceiverName"];
                    oLogistics.Send.ReceiverPhone = collection["ReceiverPhone"];
                    oLogistics.Send.ReceiverCellPhone = collection["ReceiverCellPhone"];
                    oLogistics.Send.ReceiverEmail = collection["ReceiverEmail"];
                    oLogistics.Send.TradeDesc = collection["TradeDesc"];
                    oLogistics.Send.ClientReplyURL = collection["ClientReplyURL"];
                    oLogistics.Send.LogisticsC2CReplyURL = collection["LogisticsC2CReplyURL"];
                    oLogistics.Send.Remark = collection["Remark"];
                    oLogistics.Send.ReturnStoreID = collection["ReturnStoreID"];
                    oLogistics.Send.ReceiverStoreID = collection["ReceiverStoreID"];
                    oLogistics.Send.PlatformID = collection["PlateformID"];
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "產生物流訂單" : collection["ButtonText"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForPickUpSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        public ActionResult PrintTradeDoc()
        {
            return View();
        }

        /// <summary>新增-宅配物流訂單</summary>
        /// <returns></returns>
        public ActionResult CreateLogisticsOrderForShipping()
        {
            return View();
        }

        /// <summary>新增-宅配物流訂單</summary>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CreateLogisticsOrderForShipping(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 頁面參數轉換

                decimal goodsAmount = 0;
                decimal.TryParse(collection["GoodsAmount"], out goodsAmount);
                decimal collectionAmount = 0;
                decimal.TryParse(collection["CollectionAmount"], out collectionAmount);
                DateTime? scheduledDeliveryDate = null;
                DateTime _scheduledDeliveryDate;
                if (DateTime.TryParse(collection["ScheduledDeliveryDate"], out _scheduledDeliveryDate))
                    scheduledDeliveryDate = _scheduledDeliveryDate;
                int? packageCount = null;
                int _packageCount;
                if (int.TryParse(collection["PackageCount"], out _packageCount))
                {
                    packageCount = _packageCount;
                }

                ScheduledPickupTimes? scheduledPickupTime = null;
                ScheduledPickupTimes _scheduledPickupTime;
                if (Enum.TryParse(collection["ScheduledPickupTime"], out _scheduledPickupTime))
                {
                    scheduledPickupTime = _scheduledPickupTime;
                }

                #endregion 頁面參數轉換

                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.MerchantTradeNo = "<<您此筆訂單交易編號>>";
                    oLogistics.Send.MerchantTradeDate = DateTime.Parse("yyyy/MM/dd");// "<<您此筆訂單交易日期>>";
                    oLogistics.Send.LogisticsType = LogisticsTypes.HOME;// "<<您此筆訂單的物流類型>>";
                    oLogistics.Send.LogisticsSubType = LogisticsSubTypes.TCAT;// "<<您此筆訂單的物流子類型>>";
                    oLogistics.Send.ServerReplyURL = "<<您要呼叫的服務位址>>";
                    oLogistics.Send.IsCollection = IsCollections.NO;// "<<您此筆訂單是否有代收付款>>";
                    oLogistics.Send.CollectionAmount = decimal.Parse("999");// "<<您此筆訂單代收付款金額>>";
                    oLogistics.Send.GoodsName = "<<您此筆訂單商品名稱>>";
                    oLogistics.Send.GoodsAmount = decimal.Parse("999"); //"<<您此筆訂單商品金額>>";
                    oLogistics.Send.SenderName = "<<您此筆訂單寄件者名稱>>";
                    oLogistics.Send.SenderPhone = "<<您此筆訂單寄件者電話>>";
                    oLogistics.Send.SenderCellPhone = "<<您此筆訂單寄件者手機>>";
                    oLogistics.Send.SenderZipCode = "<<您此筆訂單寄件者郵遞區號>>";
                    oLogistics.Send.SenderAddress = "<<您此筆訂單寄件者地址>>";
                    oLogistics.Send.ReceiverName = "<<您此筆訂單收件者名稱>>";
                    oLogistics.Send.ReceiverPhone = "<<您此筆訂單收件者電話>>";
                    oLogistics.Send.ReceiverZipCode = "<<您此筆訂單收件者郵遞區號>>";
                    oLogistics.Send.ReceiverAddress = "<<您此筆訂單收件者地址>>";
                    oLogistics.Send.ReceiverCellPhone = "<<您此筆訂單收件者手機>>";
                    oLogistics.Send.ReceiverEmail = "<<您此筆訂單收件者Email>>";
                    oLogistics.Send.TradeDesc = "<<您此筆訂單交易敘述>>";
                    oLogistics.Send.ClientReplyURL = "<<您要綠界科技返回按鈕導向的瀏覽器端網址>>";
                    oLogistics.Send.LogisticsC2CReplyURL = "<<您要綠界科技呼叫您的主機端網址>>";
                    oLogistics.Send.Remark = "<<您要填寫的其他備註>>";
                    oLogistics.Send.Temperature = Temperatures.ROOM;//"<<您此筆訂單的溫層>>";
                    oLogistics.Send.Distance = Distances.SAME;// "<<您此筆訂單的寄送距離>>";
                    oLogistics.Send.Specification = Specifications.CM_60;// "<<您此筆訂單的商品規格>>";
                    oLogistics.Send.ScheduledPickupTime = ScheduledPickupTimes.TIME_17_20;// "<<您此筆訂單的預計取件時段>>";
                    oLogistics.Send.ScheduledDeliveryTime = ScheduledDeliveryTimes.TIME_14_18;// "<<您此筆訂單的預定送達時段>>";
                    oLogistics.Send.ScheduledDeliveryDate = DateTime.Parse("yyyy/MM/dd"); //"<<您此筆訂單的指定送達日>>";
                    oLogistics.Send.PackageCount = int.Parse("1");// "<<您此筆訂單的包裹件數>>";
                    oLogistics.Send.PlatformID = "<<您專案合作平台商代號>>";

                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForShipping();
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForShippingSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.CreateLogisticsOrder;
                    oLogistics.Send.MerchantTradeNo = collection["MerchantTradeNo"];
                    oLogistics.Send.MerchantTradeDate = collection["MerchantTradeDate"];
                    oLogistics.Send.LogisticsType = (LogisticsTypes)Enum.Parse(typeof(LogisticsTypes), collection["LogisticsType"]);
                    oLogistics.Send.LogisticsSubType = (LogisticsSubTypes)Enum.Parse(typeof(LogisticsSubTypes), collection["LogisticsSubType"]);
                    oLogistics.Send.ServerReplyURL = collection["ServerReplyURL"];
                    oLogistics.Send.IsCollection = (IsCollections)Enum.Parse(typeof(IsCollections), collection["IsCollection"]);
                    oLogistics.Send.CollectionAmount = collectionAmount;
                    oLogistics.Send.GoodsName = collection["GoodsName"];
                    oLogistics.Send.GoodsAmount = goodsAmount;
                    oLogistics.Send.SenderName = collection["SenderName"];
                    oLogistics.Send.SenderPhone = collection["SenderPhone"];
                    oLogistics.Send.SenderCellPhone = collection["SenderCellPhone"];
                    oLogistics.Send.SenderZipCode = collection["SenderZipCode"];
                    oLogistics.Send.SenderAddress = collection["SenderAddress"];
                    oLogistics.Send.ReceiverName = collection["ReceiverName"];
                    oLogistics.Send.ReceiverPhone = collection["ReceiverPhone"];
                    oLogistics.Send.ReceiverZipCode = collection["ReceiverZipCode"];
                    oLogistics.Send.ReceiverAddress = collection["ReceiverAddress"];
                    oLogistics.Send.ReceiverCellPhone = collection["ReceiverCellPhone"];
                    oLogistics.Send.ReceiverEmail = collection["ReceiverEmail"];
                    oLogistics.Send.TradeDesc = collection["TradeDesc"];
                    oLogistics.Send.ClientReplyURL = collection["ClientReplyURL"];
                    oLogistics.Send.LogisticsC2CReplyURL = collection["LogisticsC2CReplyURL"];
                    oLogistics.Send.Remark = collection["Remark"];
                    oLogistics.Send.Temperature = (Temperatures)Enum.Parse(typeof(Temperatures), collection["Temperature"]);
                    oLogistics.Send.Distance = (Distances)Enum.Parse(typeof(Distances), collection["Distance"]);
                    oLogistics.Send.Specification = (Specifications)Enum.Parse(typeof(Specifications), collection["Specification"]);
                    oLogistics.Send.ScheduledPickupTime = scheduledPickupTime;
                    oLogistics.Send.ScheduledDeliveryTime = (ScheduledDeliveryTimes)Enum.Parse(typeof(ScheduledDeliveryTimes), collection["ScheduledDeliveryTime"]);
                    oLogistics.Send.ScheduledDeliveryDate = scheduledDeliveryDate;
                    oLogistics.Send.PackageCount = packageCount;
                    oLogistics.Send.PlatformID = collection["PlateformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForShipping();
                    //產生定單Html Code方法
                    //response = oLogistics.CreateLogisticsOrderForShippingSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }

            return Content(contentResult);
        }

        /// <summary>產生-新增宅配物流訂單 HTML按鈕</summary>
        /// <returns></returns>
        public ActionResult CreateLogisticsOrderForShippingSubmitButton()
        {
            return View();
        }

        /// <summary>產生-新增宅配物流訂單 HTML按鈕</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CreateLogisticsOrderForShippingSubmitButton(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 頁面參數轉換

                decimal goodsAmount = 0;
                decimal.TryParse(collection["GoodsAmount"], out goodsAmount);
                decimal collectionAmount = 0;
                decimal.TryParse(collection["CollectionAmount"], out collectionAmount);
                DateTime? scheduleDeliveryDate = null;
                DateTime _scheduleDeliveryDate;
                if (DateTime.TryParse(collection["ScheduleDeliveryDate"], out _scheduleDeliveryDate))
                    scheduleDeliveryDate = _scheduleDeliveryDate;
                int? packageCount = null;
                int _packageCount;
                if (int.TryParse(collection["PackageCount"], out _packageCount))
                {
                    packageCount = _packageCount;
                }

                #endregion 頁面參數轉換

                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.MerchantTradeNo = "<<您此筆訂單交易編號>>";
                    oLogistics.Send.MerchantTradeDate = "<<您此筆訂單交易日期>>";
                    oLogistics.Send.LogisticsType = "<<您此筆訂單的物流類型>>";
                    oLogistics.Send.LogisticsSubType = "<<您此筆訂單的物流子類型>>";
                    oLogistics.Send.ServerReplyURL = "<<您要呼叫的服務位址>>";
                    oLogistics.Send.IsCollection = "<<您此筆訂單是否有代收付款>>";
                    oLogistics.Send.CollectionAmount = "<<您此筆訂單代收付款金額>>";
                    oLogistics.Send.GoodsName = "<<您此筆訂單商品名稱>>";
                    oLogistics.Send.GoodsAmount = "<<您此筆訂單商品金額>>";
                    oLogistics.Send.SenderName = "<<您此筆訂單寄件者名稱>>";
                    oLogistics.Send.SenderPhone = "<<您此筆訂單寄件者電話>>";
                    oLogistics.Send.SenderCellPhone = "<<您此筆訂單寄件者手機>>";
                    oLogistics.Send.SenderZipCode = "<<您此筆訂單寄件者郵遞區號>>";
                    oLogistics.Send.SenderAddress = "<<您此筆訂單寄件者地址>>";
                    oLogistics.Send.ReceiverName = "<<您此筆訂單收件者名稱>>";
                    oLogistics.Send.ReceiverPhone = "<<您此筆訂單收件者電話>>";
                    oLogistics.Send.ReceiverZipCode = "<<您此筆訂單收件者郵遞區號>>";
                    oLogistics.Send.ReceiverAddress = "<<您此筆訂單收件者地址>>";
                    oLogistics.Send.ReceiverCellPhone = "<<您此筆訂單收件者手機>>";
                    oLogistics.Send.ReceiverEmail = "<<您此筆訂單收件者Email>>";
                    oLogistics.Send.TradeDesc = "<<您此筆訂單交易敘述>>";
                    oLogistics.Send.ClientReplyURL = <<您要綠界科技返回按鈕導向的瀏覽器端網址>>;
                    oLogistics.Send.LogisticsC2CReplyURL = <<您要綠界科技呼叫您的主機端網址>>;
                    oLogistics.Send.Remark = "<<您要填寫的其他備註>>";
                    oLogistics.Send.Temperature = "<<您此筆訂單的溫層>>";
                    oLogistics.Send.Distance = "<<您此筆訂單的寄送距離>>";
                    oLogistics.Send.Specification = "<<您此筆訂單的商品規格>>";
                    oLogistics.Send.ScheduledPickupTime = "<<您此筆訂單的預計取件時段>>";
                    oLogistics.Send.ScheduledDeliveryTime = "<<您此筆訂單的預定送達時段>>";
                    oLogistics.Send.ScheduledDeliveryDate = "<<您此筆訂單的指定送達日>>";
                    oLogistics.Send.PackageCount = "<<您此筆訂單的包裹件數>>";
                    oLogistics.Send.PlatformID = "<<您專案合作平台商代號>>";
                    oLogistics.Send.ButtonText = "<<您的按鈕文案>>"

                    //產生訂單
                    response = oLogistics.CreateLogisticsOrderForShipping();
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForShippingSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.CreateLogisticsOrder;
                    oLogistics.Send.MerchantTradeNo = collection["MerchantTradeNo"];
                    oLogistics.Send.MerchantTradeDate = collection["MerchantTradeDate"];
                    oLogistics.Send.LogisticsType = (LogisticsTypes)Enum.Parse(typeof(LogisticsTypes), collection["LogisticsType"]);
                    oLogistics.Send.LogisticsSubType = (LogisticsSubTypes)Enum.Parse(typeof(LogisticsSubTypes), collection["LogisticsSubType"]);
                    oLogistics.Send.ServerReplyURL = collection["ServerReplyURL"];
                    oLogistics.Send.IsCollection = (IsCollections)Enum.Parse(typeof(IsCollections), collection["IsCollection"]);
                    oLogistics.Send.CollectionAmount = collectionAmount;
                    oLogistics.Send.GoodsName = collection["GoodsName"];
                    oLogistics.Send.GoodsAmount = goodsAmount;
                    oLogistics.Send.SenderName = collection["SenderName"];
                    oLogistics.Send.SenderPhone = collection["SenderPhone"];
                    oLogistics.Send.SenderCellPhone = collection["SenderCellPhone"];
                    oLogistics.Send.SenderZipCode = collection["SenderZipCode"];
                    oLogistics.Send.SenderAddress = collection["SenderAddress"];
                    oLogistics.Send.ReceiverName = collection["ReceiverName"];
                    oLogistics.Send.ReceiverPhone = collection["ReceiverPhone"];
                    oLogistics.Send.ReceiverZipCode = collection["ReceiverZipCode"];
                    oLogistics.Send.ReceiverAddress = collection["ReceiverAddress"];
                    oLogistics.Send.ReceiverCellPhone = collection["ReceiverCellPhone"];
                    oLogistics.Send.ReceiverEmail = collection["ReceiverEmail"];
                    oLogistics.Send.TradeDesc = collection["TradeDesc"];
                    oLogistics.Send.ClientReplyURL = collection["ClientReplyURL"];
                    oLogistics.Send.LogisticsC2CReplyURL = collection["LogisticsC2CReplyURL"];
                    oLogistics.Send.Remark = collection["Remark"];
                    oLogistics.Send.Temperature = (Temperatures)Enum.Parse(typeof(Temperatures), collection["Temperature"]);
                    oLogistics.Send.Distance = (Distances)Enum.Parse(typeof(Distances), collection["Distance"]);
                    oLogistics.Send.Specification = (Specifications)Enum.Parse(typeof(Specifications), collection["Specification"]);
                    oLogistics.Send.ScheduledPickupTime = (ScheduledPickupTimes)Enum.Parse(typeof(ScheduledPickupTimes), collection["ScheduledPickupTime"]);
                    oLogistics.Send.ScheduledDeliveryTime = (ScheduledDeliveryTimes)Enum.Parse(typeof(ScheduledDeliveryTimes), collection["ScheduledDeliveryTime"]);
                    oLogistics.Send.ScheduledDeliveryDate = scheduleDeliveryDate;
                    oLogistics.Send.PackageCount = packageCount;
                    oLogistics.Send.PlatformID = collection["PlateformID"];
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "產生物流訂單" : collection["ButtonText"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生定單Html Code方法
                    response = oLogistics.CreateLogisticsOrderForShippingSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }

            return Content(contentResult);
        }

        /// <summary>列印-托運單(宅配)/一段標(超商取貨)</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult PrintTradeDoc(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    //產生託運單網址
                    response = oLogistics.PrintTradeDoc();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintTradeDoc;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生託運單網址
                    response = oLogistics.PrintTradeDoc();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>產生-托運單(宅配)/一段標(超商取貨) HTML按鈕</summary>
        /// <returns></returns>
        public ActionResult GeneratePrintTradeDocSubmitButton()
        {
            return View();
        }

        /// <summary>產生-托運單(宅配)/一段標(超商取貨) HTML按鈕</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult GeneratePrintTradeDocSubmitButton(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    oLogistics.Send.ButtonText = "<<您的按鈕文案>>";
                    //產生託運單html button
                    response = oLogistics.GeneratePrintTradeDocSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintTradeDoc;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "產生託運單" : collection["ButtonText"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生託運單html button
                    response = oLogistics.GeneratePrintTradeDocSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>列印-繳款單(統一超商C2C)</summary>
        /// <returns></returns>
        public ActionResult PrintUnimartC2CBill()
        {
            return View();
        }

        /// <summary>列印-繳款單(統一超商C2C)</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult PrintUnimartC2CBill(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.CVSPaymentNo = "<<您的寄貨編號>>";
                    oLogistics.Send.CVSValidationNo = "<<您的驗證碼>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    //產生列印繳款單網址
                    response = oLogistics.PrintUnimartC2CBill();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintUniMartC2COrderInfo;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.CVSPaymentNo = collection["CVSPaymentNo"];
                    oLogistics.Send.CVSValidationNo = collection["CVSValidationNo"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生列印繳款單網址
                    response = oLogistics.PrintUnimartC2CBill();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>產生-繳款單(統一超商C2C) HTML按鈕</summary>
        /// <returns></returns>
        public ActionResult GeneratePrintUnimartC2CBillSubmitButton()
        {
            return View();
        }

        /// <summary>產生-繳款單(統一超商C2C) HTML按鈕</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult GeneratePrintUnimartC2CBillSubmitButton(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();
            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.CVSPaymentNo = "<<您的寄貨編號>>";
                    oLogistics.Send.CVSValidationNo = "<<您的驗證碼>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    oLogistics.Send.ButtonText = "<<您的按鈕文案>>"
                    //產生列印繳款單網址Html Button
                    response = oLogistics.GeneratePrintUnimartC2CBillSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintUniMartC2COrderInfo;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.CVSPaymentNo = collection["CVSPaymentNo"];
                    oLogistics.Send.CVSValidationNo = collection["CVSValidationNo"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "產生列印繳款單" : collection["ButtonText"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生列印繳款單網址Html Button
                    response = oLogistics.GeneratePrintUnimartC2CBillSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>列印-小白單(全家超商C2C)</summary>
        /// <returns></returns>
        public ActionResult PrintFamilyC2CBill()
        {
            return View();
        }

        /// <summary>列印-小白單(全家超商C2C)</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult PrintFamilyC2CBill(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();
            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.CVSPaymentNo = "<<您的寄貨編號>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    //產生全家列印小白單網址
                    response = oLogistics.PrintFamilyC2CBill();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintFAMIC2COrderInfo;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.CVSPaymentNo = collection["CVSPaymentNo"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生全家列印小白單網址
                    response = oLogistics.PrintFamiC2CBill();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>產生-小白單(全家超商C2C) HTML按鈕</summary>
        /// <returns></returns>
        public ActionResult GeneratePrintFamilyC2CBillSubmitButton()
        {
            return View();
        }

        /// <summary>產生-小白單(全家超商C2C) HTML按鈕</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult GeneratePrintFamilyC2CBillSubmitButton(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();
            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.CVSPaymentNo = "<<您的寄貨編號>>";
                    oLogistics.Send.AllPayLogisticsID = "<<您的物流交易編號>>";
                    oLogistics.Send.PlatformID = "<<您的專案合作平台商代號>>";
                    oLogistics.Send.ButtonText = "<<您的按鈕文案>>"
                    //產生全家列印小白單Html Button
                    response = oLogistics.GeneratePrintFamilyC2CBillSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.PrintFAMIC2COrderInfo;
                    oLogistics.Send.AllPayLogisticsID = collection["AllPayLogisticsID"];
                    oLogistics.Send.CVSPaymentNo = collection["CVSPaymentNo"];
                    oLogistics.Send.PlatformID = collection["PlatformID"];
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "全家列印小白單" : collection["ButtonText"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));
                    //產生全家列印小白單Html Button
                    response = oLogistics.GeneratePrintFAMIC2CBillSubmitButton();
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }

        /// <summary>取得-超商電子地圖</summary>
        /// <returns></returns>
        public ActionResult CVSMap()
        {
            return View();
        }

        /// <summary>取得-超商電子地圖</summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult CVSMap(FormCollection collection)
        {
            string contentResult = string.Empty;

            string hashKey = collection["HashKey"];
            string hashIV = collection["HashIV"];
            string MerchantID = collection["MerchantID"];
            Response<string> response = new Response<string>();

            try
            {
                #region 參數說明

                /*
                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = "<<ECPay提供給您的Hash Key>>";
                    oLogistics.HashIV = "<<ECPay提供給您的Hash IV>>";
                    oLogistics.MerchantID = "<<ECPay提供給您的特店編號>>";
                    oLogistics.ServiceURL = "<<您呼叫的API URL>>";
                    oLogistics.Send.MerchantTradeNo = "<<您此筆訂單交易編號>>";
                    oLogistics.Send.LogisticsType = LogisticsTypes.HOME;//"<<您此筆訂單的物流類型>>";
                    oLogistics.Send.LogisticsSubType = LogisticsSubTypes.TCAT; //"<<您此筆訂單的物流子類型>>";
                    oLogistics.Send.IsCollection = IsCollections.NO;// "<<您此筆訂單是否有代收付款>>";
                    oLogistics.Send.ServerReplyURL = "<<您要呼叫的服務位址>>";
                    oLogistics.Send.Device = Devices.PC;// "<<您的裝置類型>>";
                    oLogistics.Send.ButtonText = "<<您HTML按鈕的文字>>";
                    oLogistics.Send.ExtraData = "<<您此筆訂單的額外資訊>>";;
                    //取得電子地圖網址
                    response = oLogistics.GetCvsMapURL();
                    //取得電子地圖Html Code方法
                    response = oLogistics.GetCVSMapSubmitButton();
                }
                */

                #endregion 參數說明

                #region 呼叫範例

                using (var oLogistics = new LogisticsProvider())
                {
                    oLogistics.HashKey = hashKey;
                    oLogistics.HashIV = hashIV;
                    oLogistics.MerchantID = MerchantID;
                    oLogistics.ServiceURL = ServiceDomain.CVSMAP;
                    oLogistics.Send.MerchantTradeNo = collection["MerchantTradeNo"];
                    oLogistics.Send.LogisticsType = (LogisticsTypes)Enum.Parse(typeof(LogisticsTypes), collection["LogisticsType"]);
                    oLogistics.Send.LogisticsSubType = (LogisticsSubTypes)Enum.Parse(typeof(LogisticsSubTypes), collection["LogisticsSubType"]);
                    oLogistics.Send.IsCollection = (IsCollections)Enum.Parse(typeof(IsCollections), collection["IsCollection"]);
                    oLogistics.Send.ServerReplyURL = collection["ServerReplyURL"];
                    oLogistics.Send.Device = (Devices)Enum.Parse(typeof(Devices), collection["Device"]);
                    oLogistics.Send.ButtonText = string.IsNullOrEmpty(collection["ButtonText"]) ? "CVS電子地圖" : collection["ButtonText"];
                    oLogistics.Send.ExtraData = collection["ExtraData"];
                    //啟動log，會在帶入的路徑產生log檔案
                    oLogistics.EnableLogging(HttpContext.Server.MapPath(_logPath));

                    if (string.IsNullOrEmpty(collection["ButtonText"]) == false)
                    {
                        //取得電子地圖Html Code方法
                        response = oLogistics.GetCVSMapSubmitButton();
                    }
                    else
                    {
                        //取得電子地圖網址
                        response = oLogistics.GetCvsMapURL();
                    }
                }

                #endregion 呼叫範例

                if (response.IsSuccess)
                {
                    contentResult = response.Data;
                }
                else
                {
                    contentResult = response.ErrorMessage;
                    if (response.ErrorException == null)
                    {
                        contentResult = _errorWord;
                    }
                    else
                    {
                        Rollbars.Report(response.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                contentResult = ex.Message;
                Rollbars.Report(ex);
            }
            return Content(contentResult);
        }
    }
}