using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ECPay.SDK.Logistics
{
    public class LogisticsProvider : IDisposable
    {
        public string HashKey;
        public string HashIV;
        public string MerchantID;
        public string ServiceURL;
        private string logPath;
        public HttpMethod ServiceMethod;
        private bool EnableLog = false;
        public Parameters Send { get; set; }

        /// <summary>建立物流SDK呼叫物件，請到「廠商後台」>「系統開發管理」>「系統介接設定」>「介接資訊」查詢系統介接相關資訊</summary>
        /// <param name="HashKey">物流介接 HashKey</param>
        /// <param name="HashIV">物流介接 HashIV</param>
        /// <param name="MerchantID">商店代號(特店編號)</param>
        public LogisticsProvider(string HashKey, string HashIV, string MerchantID)
        {
            if (string.IsNullOrEmpty(MerchantID) || MerchantID.Length > 10)
            {
                throw new Exception("MerchantID can't be null or length > 10!");
            }
            if (string.IsNullOrEmpty(HashKey) || string.IsNullOrEmpty(HashIV))
            {
                throw new Exception("HashKey or HashIV can't be null or empty!");
            }
            this.HashKey = HashKey;
            this.HashIV = HashIV;
            this.MerchantID = MerchantID;
            this.Send = new Parameters();
        }

        public LogisticsProvider()
        {
            this.Send = new Parameters();
        }

        private SendParameters GenerateSendParameters()
        {
            return new SendParameters(this.MerchantID, this.Send);
        }

        /// <summary>產生電子地圖URI(CvsMap)</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字)</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="ServerReplyURL">伺服器端回復網址(取得超商店鋪代號等資訊後，會回傳到此網址)</param>
        /// <param name="ExtraData">額外資訊(供廠商傳遞保留的資訊，在回傳參數中，會原值回傳)</param>
        /// <param name="Device">使用裝置(根據Device類型，來顯示相對應知電子地圖)</param>
        public Response<string> GetCvsMapURL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GetCvsMapURL();

                var queryString = ParameterValidator.Validate(distParameter);
                response.Data = ServiceURL + "?" + queryString;
                response.IsSuccess = true;
                WriteLog(logPath, "Logistics.CVSMAP", queryString);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }

            return response;
        }

        /// <summary>產生電子地圖(CvsMap)HTML Button</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字)</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="ServerReplyURL">伺服器端回復網址(取得超商店鋪代號等資訊後，會回傳到此網址)</param>
        /// <param name="Device">使用設備(根據Device類型，來顯示相對應知電子地圖)</param>
        /// <param name="ExtraData">額外資訊(供廠商傳遞保留的資訊，在回傳參數中，會原值回傳)</param>
        /// <param name="ButtonText">按鈕顯示名稱</param>
        /// <returns></returns>
        public Response<string> GetCVSMapSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            Response<string> response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                #region Function 參數邏輯驗證

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GetCvsMapURL();

                #endregion Function 參數邏輯驗證

                var queryString = ParameterValidator.Validate(distParameter);
                string submitUrl = ServiceURL + "?" + queryString;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);

                response.IsSuccess = true;
                response.Data = _strHtml;
                WriteLog(logPath, "Logistics.CVSMAP", queryString);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }
            return response;
        }

        /// <summary>呼叫宅配物流訂單產生API。使用者在特店(賣家會員)消費後，特店(賣家會員)將結帳完成的出貨資訊Post至ECPay進行物流出貨作業。</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字。)</param>
        /// <param name="MerchantTradeDate">廠商交易時間</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="GoodsAmount">商品金額(物流子類型為UNIMARTC2C(統一超商交貨便)時，商品金額範圍為1~19,999元其餘子類型為1~20,000元)</param>
        /// <param name="CollectionAmount">代收金額(物流子類型為UNIMARTC2C(統一超商交貨便)時，代收金額範圍為1~19,999元其餘子類型為1~20,000元)</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="GoodsName">商品名稱：物流子類型為全家店到店、統一超商交貨便店到店時不可為空。</param>
        /// <param name="SenderName">寄件人姓名</param>
        /// <param name="SenderPhone">寄件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人手機擇一不可為空</param>
        /// <param name="SenderCellPhone">寄件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人電話擇一不可為空。</param>
        /// <param name="ReceiverName">收件人姓名</param>
        /// <param name="ReceiverPhone">收件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人手機擇一不可為空。</param>
        /// <param name="ReceiverCellPhone">收件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人電話擇一不可為空。物流子類型為統一超商交貨便店到店時不可為空。</param>
        /// <param name="ReceiverEmail">收件人電子郵件</param>
        /// <param name="TradeDesc">交易敘述</param>
        /// <param name="ServerReplyURL">伺服器端回覆網址：取得超商店鋪代號等資訊後，會回傳到此網址。</param>
        /// <param name="ClientReplyURL">客戶端回覆網址：此參數不為空時，於物流訂單建立後，會將頁面導至此網址。幕後建立訂單時，請勿設定此參數。</param>
        /// <param name="LogisticsC2CReplyURL">伺服器端物流回傳網址：當使用者選擇取貨門市有問題時，會透過此網址通知特店，請特店通知使用者重新選擇門市。物流子類型為統一超商交貨便店到店時，不可為空</param>
        /// <param name="Remark">備註</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="SenderZipCode">寄件人郵遞區號</param>
        /// <param name="SenderAddress">寄件人地址</param>
        /// <param name="ReceiverZipCode">收件人郵遞區號</param>
        /// <param name="ReceiverAddress">收件人地址</param>
        /// <param name="Temperature">溫層(0001:常溫(預設值)0002:冷藏0003:冷凍)</param>
        /// <param name="Distance">距離(00:同縣市(預設值)01:外縣市02:離島)</param>
        /// <param name="Specification">規格(0001: 60cm (預設值),0002: 90cm, 0003: 120cm, 0004: 150cm)</param>
        /// <param name="ScheduledPickupTime">預定取件時段(1: 9~12, 2: 12~17, 3: 17~20, 4: 不限時(固定4不限時)(當子物流選擇宅配通時，該參數可不填)</param>
        /// <param name="ScheduledDeliveryTime">預定送達時段(1: 13前, 2: 14~18, 4:不限時)</param>
        /// <param name="ScheduledDeliveryDate">指定送達日(當子物流選擇宅配通時，此參數才有作用日期指定限制D+3(D:該訂單建立時間))</param>
        /// <param name="PackageCount">包裹件數(當子物流選擇宅配通時，此參數才有作用，作用於同訂單編號，包裹件數。)</param>
        /// <returns>API回傳訊息</returns>
        public Response<string> CreateLogisticsOrderForShipping()
        {
            //768或3072(SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();

                try
                {
                    #region Function 參數邏輯驗證

                    //因為本function為呼叫[產生宅配物流訂單]，強制設定LogisticsType = LogisticsTypes.HOME
                    Send.LogisticsType = LogisticsTypes.HOME;

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GetLogisticsOrderForShipping();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.CreateOrderForShipping", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>宅配物流訂單Html按鈕產生。使用者在特店(賣家會員)消費後，特店(賣家會員)將結帳完成的出貨資訊Post至ECPay進行物流出貨作業。</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字。)</param>
        /// <param name="MerchantTradeDate">廠商交易時間</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="GoodsAmount">商品金額(物流子類型為UNIMARTC2C(統一超商交貨便)時，商品金額範圍為1~19,999元其餘子類型為1~20,000元)</param>
        /// <param name="CollectionAmount">代收金額(物流子類型為UNIMARTC2C(統一超商交貨便)時，代收金額範圍為1~19,999元其餘子類型為1~20,000元)</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="GoodsName">商品名稱：物流子類型為全家店到店、統一超商交貨便店到店時不可為空。</param>
        /// <param name="SenderName">寄件人姓名</param>
        /// <param name="SenderPhone">寄件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人手機擇一不可為空</param>
        /// <param name="SenderCellPhone">寄件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人電話擇一不可為空。</param>
        /// <param name="ReceiverName">收件人姓名</param>
        /// <param name="ReceiverPhone">收件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人手機擇一不可為空。</param>
        /// <param name="ReceiverCellPhone">收件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人電話擇一不可為空。物流子類型為統一超商交貨便店到店時不可為空。</param>
        /// <param name="ReceiverEmail">收件人電子郵件</param>
        /// <param name="TradeDesc">交易敘述</param>
        /// <param name="ServerReplyURL">伺服器端回覆網址：取得超商店鋪代號等資訊後，會回傳到此網址。</param>
        /// <param name="ClientReplyURL">客戶端回覆網址：此參數不為空時，於物流訂單建立後，會將頁面導至此網址。幕後建立訂單時，請勿設定此參數。</param>
        /// <param name="LogisticsC2CReplyURL">伺服器端物流回傳網址：當使用者選擇取貨門市有問題時，會透過此網址通知特店，請特店通知使用者重新選擇門市。物流子類型為統一超商交貨便店到店時，不可為空</param>
        /// <param name="Remark">備註</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="SenderZipCode">寄件人郵遞區號</param>
        /// <param name="SenderAddress">寄件人地址</param>
        /// <param name="ReceiverZipCode">收件人郵遞區號</param>
        /// <param name="ReceiverAddress">收件人地址</param>
        /// <param name="Temperature">溫層(0001:常溫(預設值)0002:冷藏0003:冷凍)</param>
        /// <param name="Distance">距離(00:同縣市(預設值)01:外縣市02:離島)</param>
        /// <param name="Specification">規格(0001: 60cm (預設值),0002: 90cm, 0003: 120cm, 0004: 150cm)</param>
        /// <param name="ScheduledPickupTime">預定取件時段(1: 9~12, 2: 12~17, 3: 17~20, 4: 不限時(固定4不限時)(當子物流選擇宅配通時，該參數可不填)</param>
        /// <param name="ScheduledDeliveryTime">預定送達時段(1: 13前, 2: 14~18, 4:不限時)</param>
        /// <param name="ScheduleDeliveryDate">指定送達日(當子物流選擇宅配通時，此參數才有作用日期指定限制D+3(D:該訂單建立時間))</param>
        /// <param name="PackageCount">包裹件數(當子物流選擇宅配通時，此參數才有作用，作用於同訂單編號，包裹件數。)</param>
        /// <param name="ButtonText">Html Button 文案</param>
        /// <returns>Html Form with Submit Button</returns>
        public Response<string> CreateLogisticsOrderForShippingSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                #region Function 參數邏輯驗證

                //因為本function為呼叫[產生宅配物流訂單]，強制設定LogisticsType = LogisticsTypes.HOME
                Send.LogisticsType = LogisticsTypes.HOME;

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GetLogisticsOrderForShipping();

                #endregion Function 參數邏輯驗證

                //寫入呼叫Log
                var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);
                WriteLog(logPath, "Logistics.CreateOrderForShippingHtmlForm", httpContent.ReadAsStringAsync().Result);

                string submitUrl = ServiceURL + "?" + httpContent.ReadAsStringAsync().Result;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);
                response.IsSuccess = true;
                response.Data = _strHtml;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }
            return response;
        }

        /// <summary>呼叫超商物流訂單產生API。使用者在特店(賣家會員)消費後，特店(賣家會員)將結帳完成的出貨資訊Post至ECPay進行物流出貨作業。</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字。)</param>
        /// <param name="MerchantTradeDate">廠商交易時間</param>
        /// <param name="LogisticsType">物流類型</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="GoodsAmount">商品金額</param>
        /// <param name="CollectionAmount">代收金額</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="GoodsName">商品名稱：物流子類型為全家店到店、統一超商交貨便店到店時不可為空。</param>
        /// <param name="SenderName">寄件人姓名</param>
        /// <param name="SenderPhone">寄件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人手機擇一不可為空</param>
        /// <param name="SenderCellPhone">寄件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人電話擇一不可為空。</param>
        /// <param name="ReceiverName">收件人姓名</param>
        /// <param name="ReceiverPhone">收件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人手機擇一不可為空。</param>
        /// <param name="ReceiverCellPhone">收件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人電話擇一不可為空。物流子類型為統一超商交貨便店到店時不可為空。</param>
        /// <param name="ReceiverEmail">收件人電子郵件</param>
        /// <param name="TradeDesc">交易敘述</param>
        /// <param name="ServerReplyURL">伺服器端回覆網址：取得超商店鋪代號等資訊後，會回傳到此網址。</param>
        /// <param name="ClientReplyURL">客戶端回覆網址：此參數不為空時，於物流訂單建立後，會將頁面導至此網址。幕後建立訂單時，請勿設定此參數。</param>
        /// <param name="LogisticsC2CReplyURL">伺服器端物流回傳網址：當使用者選擇取貨門市有問題時，會透過此網址通知特店，請特店通知使用者重新選擇門市。物流子類型為統一超商交貨便店到店時，不可為空</param>
        /// <param name="Remark">備註</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="ReceiverStoreID">收件人門市代號</param>
        /// <param name="ReturnStoreID">退貨門市代號(當為店到店的退貨時，要退回的門市代號)</param>
        /// <returns>API回傳訊息</returns>
        public Response<string> CreateLogisticsOrderForPickUp()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();

                try
                {
                    //因為是新增取貨物流訂單，強制設定type為LogisticsTypes.CVS
                    Send.LogisticsType = LogisticsTypes.CVS;

                    #region Function 參數邏輯驗證

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GetLogisticsOrderForPickUp();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.CreateOrderForPickUp", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>產生超商物流訂單產生Html Submit Button。使用者在特店(賣家會員)消費後，特店(賣家會員)將結帳完成的出貨資訊Post至ECPay進行物流出貨作業。</summary>
        /// <param name="MerchantTradeNo">廠商交易編號(僅可使用大小寫英文與數字。)</param>
        /// <param name="MerchantTradeDate">廠商交易時間</param>
        /// <param name="LogisticsType">物流類型</param>
        /// <param name="LogisticsSubType">物流子類型</param>
        /// <param name="GoodsAmount">商品金額</param>
        /// <param name="CollectionAmount">代收金額</param>
        /// <param name="IsCollection">是否代收貨款</param>
        /// <param name="GoodsName">商品名稱：物流子類型為全家店到店、統一超商交貨便店到店時不可為空。</param>
        /// <param name="SenderName">寄件人姓名</param>
        /// <param name="SenderPhone">寄件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人手機擇一不可為空</param>
        /// <param name="SenderCellPhone">寄件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與寄件人電話擇一不可為空。</param>
        /// <param name="ReceiverName">收件人姓名</param>
        /// <param name="ReceiverPhone">收件人電話：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人手機擇一不可為空。</param>
        /// <param name="ReceiverCellPhone">收件人手機：僅可使用0~9為字串內容，長度限制為7~20。物流類型為宅配時，與收件人電話擇一不可為空。物流子類型為統一超商交貨便店到店時不可為空。</param>
        /// <param name="ReceiverEmail">收件人電子郵件</param>
        /// <param name="TradeDesc">交易敘述</param>
        /// <param name="ServerReplyURL">伺服器端回覆網址：取得超商店鋪代號等資訊後，會回傳到此網址。</param>
        /// <param name="ClientReplyURL">客戶端回覆網址：此參數不為空時，於物流訂單建立後，會將頁面導至此網址。幕後建立訂單時，請勿設定此參數。</param>
        /// <param name="LogisticsC2CReplyURL">伺服器端物流回傳網址：當使用者選擇取貨門市有問題時，會透過此網址通知特店，請特店通知使用者重新選擇門市。物流子類型為統一超商交貨便店到店時，不可為空</param>
        /// <param name="Remark">備註</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="ReceiverStoreID">收件人門市代號</param>
        /// <param name="ReturnStoreID">退貨門市代號(當為店到店的退貨時，要退回的門市代號)</param>
        /// <param name="ButtonText">按鈕顯示文案</param>
        /// <returns>API回傳訊息</returns>
        public Response<string> CreateLogisticsOrderForPickUpSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                //因為是新增取貨物流訂單，強制設定type為LogisticsTypes.CVS
                Send.LogisticsType = LogisticsTypes.CVS;

                #region Function 參數邏輯驗證

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GetLogisticsOrderForPickUp();

                #endregion Function 參數邏輯驗證

                //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                //寫入呼叫Log
                WriteLog(logPath, "Logistics.CreateOrderForPickUpHtmlForm", httpContent.ReadAsStringAsync().Result);

                string submitUrl = ServiceURL + "?" + httpContent.ReadAsStringAsync().Result;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);
                response.IsSuccess = true;
                response.Data = _strHtml;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }

            return response;
        }

        /// <summary>查詢物流訂單</summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <returns></returns>
        public Response<string> QueryLogisticsInfo()
        {
            //768或3072(SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();

                try
                {
                    #region Function 參數邏輯驗證

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GetLogisticsInfo();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.QueryLogisticsInfo", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>
        /// 產生托運單(宅配)/一段標(超商取貨)，可利用此功能來產生物流廠商所使用的托運單(宅配)或一段標(全家、統一超商)。
        /// </summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <returns></returns>
        public Response<string> PrintTradeDoc()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();

                try
                {
                    #region Function 參數邏輯驗證

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GetTradeDoc();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.PrintTradeDoc", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(response.Data) == false)
                        {
                            response.Data = response.Data.Replace("<link href=\"/Content", $"<link href=\"{ServiceDomain.APIURL}Content")
                                                         .Replace("<script src=\"/Scripts", $"<script src=\"{ServiceDomain.APIURL}Scripts");
                        }
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>
        /// 產生托運單(宅配)/一段標(超商取貨)Submit Button，可利用此功能來產生物流廠商所使用的托運單(宅配)或一段標(全家、統一超商)。
        /// </summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="ButtonText">按鈕顯示文案</param>
        /// <returns></returns>
        public Response<string> GeneratePrintTradeDocSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                #region Function 參數邏輯驗證

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GetTradeDoc();

                #endregion Function 參數邏輯驗證

                //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                //寫入呼叫Log
                WriteLog(logPath, "Logistics.GeneratePrintTradeDocSubmitButton", httpContent.ReadAsStringAsync().Result);

                string submitUrl = ServiceURL + "?" + httpContent.ReadAsStringAsync().Result;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);
                response.IsSuccess = true;
                response.Data = _strHtml;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }
            return response;
        }

        /// <summary>列印繳款單(統一超商C2C)</summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="CVSPaymentNo">寄貨編號</param>
        /// <param name="CVSValidationNO">驗證碼</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <returns></returns>
        public Response<string> PrintUnimartC2CBill()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();
                try
                {
                    #region Function 參數邏輯驗證

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GettUnimartC2CBill();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);
                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.PrintUnimartC2CBill", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>產生列印繳款單(統一超商C2C) Submit Button(Html)</summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="CVSPaymentNo">寄貨編號</param>
        /// <param name="CVSValidationNO">驗證碼</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <param name="ButtonText">按鈕顯示文案</param>
        /// <returns></returns>
        public Response<string> GeneratePrintUnimartC2CBillSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                #region Function 參數邏輯驗證

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GettUnimartC2CBill();

                #endregion Function 參數邏輯驗證

                //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                //寫入呼叫Log
                WriteLog(logPath, "Logistics.GeneratePrintUnimartC2CBillSubmitButton", httpContent.ReadAsStringAsync().Result);

                string submitUrl = ServiceURL + "?" + httpContent.ReadAsStringAsync().Result;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);
                response.IsSuccess = true;
                response.Data = _strHtml;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }
            return response;
        }

        /// <summary>全家列印小白單(全家超商C2C)</summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="CVSPaymentNo">寄貨編號</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <returns></returns>
        public Response<string> PrintFamiC2CBill()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            using (var client = new HttpClient())
            {
                var distParameter = new Dictionary<string, ParameterValue>();

                try
                {
                    #region Function 參數邏輯驗證

                    //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                    //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                    distParameter = this.GenerateSendParameters().GettFamiC2CBill();

                    #endregion Function 參數邏輯驗證

                    //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                    var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                    //寫入呼叫Log
                    WriteLog(logPath, "Logistics.PrintFamiC2CBill", httpContent.ReadAsStringAsync().Result);

                    client.BaseAddress = new Uri(ServiceURL);
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage httpResponse = client.PostAsync("", httpContent).Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        response.IsSuccess = true;
                        response.Data = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        response.ErrorMessage = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.ErrorException = ex;
                }
            }
            return response;
        }

        /// <summary>產生全家列印小白單(全家超商C2C)Html Submit Button</summary>
        /// <param name="AllPayLogisticsID">綠界科技的物流交易編號：僅可使用0~9為字串內容。請保存綠界科技的交易編號與AllPayLogisticsID的關連。</param>
        /// <param name="CVSPaymentNo">寄貨編號</param>
        /// <param name="PlatformID">專案合作平台商代號：由綠界科技提供，此參數為專案合作的平台商使用，一般廠商介接請忽略此參數。若為專案合作的平台商使用時，MerchantID請帶賣家所綁定的MerchantID。</param>
        /// <returns></returns>
        public Response<string> GeneratePrintFAMIC2CBillSubmitButton()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)768 | (SecurityProtocolType)3072;

            var response = new Response<string>();
            var distParameter = new Dictionary<string, ParameterValue>();

            try
            {
                #region Function 參數邏輯驗證

                //這裡會先做數值驗證，因為每個function都有自己的使用情境，數值邏輯有可能不同
                //如果為非必填且為空值得參數請勿加入，型態驗證會檢查參數值是否為空
                distParameter = this.GenerateSendParameters().GettFamiC2CBill();

                #endregion Function 參數邏輯驗證

                //驗證參數。這邊只會驗證已帶入的參數符不符合型態限制
                var httpContent = ParameterValidator.ValidateURLContent(distParameter, this.HashKey, this.HashIV);

                //寫入呼叫Log
                WriteLog(logPath, "Logistics.GeneratePrintFamiC2CBillSubmitButton", httpContent.ReadAsStringAsync().Result);

                string submitUrl = ServiceURL + "?" + httpContent.ReadAsStringAsync().Result;
                string _strHtml = genPostHtml(submitUrl, Send.ButtonText, distParameter);
                response.IsSuccess = true;
                response.Data = _strHtml;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ErrorException = ex;
            }

            return response;
        }

        /// <summary>產生回傳Submit button</summary>
        /// <param name="SubmitURL"></param>
        /// <param name="ButtonText"></param>
        /// <returns></returns>
        private string genPostHtml(string SubmitURL, string ButtonText)
        {
            string strHtml = "<div style='text-align:center;'>" +
                                "<form id='ecpayForm' method='POST' action=" + SubmitURL + ">" +
                                    "<input type='submit' id='__logisticsButton' value='" + ButtonText + "' />" +
                                "</form>" +
                             "</div>";
            return strHtml;
        }

        private string genPostHtml(string SubmitURL, string ButtonText, Dictionary<string, ParameterValue> Params)
        {
            string strHtml = @"<div style='text-align:center;'>" +
                                "<form id='ecpayForm' method='POST' action=" + SubmitURL + ">";

            foreach (var item in Params)
            {
                strHtml += @"<input type='hidden' name ='" + item.Key + "' value='" + item.Value.Value + "' />";
            }
            strHtml += @"<input type='submit' id='__logisticsButton' value='" + ButtonText + "' />" +
                               "</form>" +
                            "</div>";

            return strHtml;
        }

        public void Dispose()
        {
            this.Send = null;
        }

        private void WriteLog(string path, string funcName, string queryString)
        {
            if (EnableLog)
            {
                using (StreamWriter sw = new StreamWriter(path + @"\sdk.log", true))
                {
                    sw.WriteLine("INFO\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\tOUTPUT\t" + funcName + ": " + queryString);
                }
            }
        }

        public void EnableLogging(string logPath)
        {
            this.EnableLog = true;
            this.logPath = logPath;
        }
    }

    /// <summary>物流類型</summary>
    public enum LogisticsTypes
    {
        /// <summary>超商取貨(CVS)</summary>
        CVS,

        /// <summary>宅配(Home)</summary>
        HOME
    }

    /// <summary>物流子類型</summary>
    public enum LogisticsSubTypes
    {
        /// <summary>無</summary>
        None,

        /// <summary>黑貓宅配(TCAT)</summary>
        TCAT,

        /// <summary>宅配通(ECAN)</summary>
        ECAN,

        /// <summary>全家B2C(FAMI)</summary>
        FAMI,

        /// <summary>統一超商B2C(UNIMART)</summary>
        UNIMART,

        /// <summary>全家店到店C2C(FAMIC2C)</summary>
        FAMIC2C,

        /// <summary>統一超商交貨便店到店C2C(UNIMARTC2C)</summary>
        UNIMARTC2C,

        /// <summary>萊爾富店到店(HILIFEC2C)</summary>
        HILIFEC2C,

        /// <summary>萊爾富物流(B2C)</summary>
        HILIFE
    }

    /// <summary>是否代收貨款</summary>
    public enum IsCollections
    {
        /// <summary>代收貨款(Y)</summary>
        YES,

        /// <summary>不代收貨款(N)</summary>
        NO
    }

    /// <summary>使用裝置</summary>
    public enum Devices
    {
        /// <summary>PC(0)</summary>
        PC,

        /// <summary>行動裝置(1)</summary>
        Mobile
    }

    /// <summary>溫層</summary>
    public enum Temperatures
    {
        /// <summary>常溫(0001)</summary>
        ROOM,

        /// <summary>冷藏(0002)</summary>
        REFRIGERATION,

        /// <summary>冷凍(0003)</summary>
        FREEZE
    }

    /// <summary>距離</summary>
    public enum Distances
    {
        /// <summary>同縣市(01)</summary>
        SAME,

        /// <summary>外縣市(02)</summary>
        OTHER,

        /// <summary>離島(03)</summary>
        ISLAND
    }

    /// <summary>規格</summary>
    public enum Specifications
    {
        /// <summary>60CM(0001)</summary>
        CM_60,

        /// <summary>90CM(0002)</summary>
        CM_90,

        /// <summary>120CM(0003)</summary>
        CM_120,

        /// <summary>150CM(0004)</summary>
        CM_150
    }

    /// <summary>預計取件時段</summary>
    public enum ScheduledPickupTimes
    {
        /// <summary>9~12時(1)</summary>
        TIME_9_12,

        /// <summary>12~17時(2)</summary>
        TIME_12_17,

        /// <summary>17~20(3)</summary>
        TIME_17_20,

        /// <summary>不限時(4)</summary>
        TIME_UNLIMITED
    }

    /// <summary>預定送達時段</summary>
    public enum ScheduledDeliveryTimes
    {
        /// <summary>不限時(4)</summary>
        TIME_UNLIMITED,

        /// <summary>13前</summary>
        TIME_0_13,

        /// <summary>14~18</summary>
        TIME_14_18
    }

    /// <summary>門市類型</summary>
    public enum StoreTypes
    {
        /// <summary>取件門市(01)</summary>
        RECIVE_STORE,

        /// <summary>退件門市(02)</summary>
        RETURN_STORE
    }
}