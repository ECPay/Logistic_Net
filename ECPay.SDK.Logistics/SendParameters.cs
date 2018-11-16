using System;
using System.Collections.Generic;

namespace ECPay.SDK.Logistics
{
    /// <summary>產生參數 (並且檢驗參數是否正確)</summary>
    internal class SendParameters
    {
        private string _merchantID;
        private Parameters _send;

        public SendParameters(string merchantID, Parameters send)
        {
            _merchantID = merchantID;
            _send = send;
        }

        /// <summary>取得"電子地圖URI"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GetCvsMapURL()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("MerchantTradeNo", new ParameterValue(_send.MerchantTradeNo, false));
            distParameter.Add("LogisticsType", new ParameterValue(_send.LogisticsType.ToString(), true));
            distParameter.Add("LogisticsSubType", new ParameterValue(_send.LogisticsSubType.ToString(), true));
            distParameter.Add("IsCollection", new ParameterValue(_send.IsCollection.ToString().Substring(0, 1), true));
            distParameter.Add("ServerReplyURL", new ParameterValue(_send.ServerReplyURL, true));
            distParameter.Add("ExtraData", new ParameterValue(_send.ExtraData, false));

            if (_send.Device.HasValue)
            {
                distParameter.Add("Device", new ParameterValue(EnumConvertor.ToString(_send.Device.Value), false));
            }

            return distParameter;
        }

        /// <summary>取得"宅配物流訂單"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GetLogisticsOrderForShipping()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("MerchantTradeNo", new ParameterValue(_send.MerchantTradeNo, true));
            distParameter.Add("MerchantTradeDate", new ParameterValue(_send.MerchantTradeDate, true));
            distParameter.Add("LogisticsType", new ParameterValue(_send.LogisticsType.ToString(), true));

            if (_send.LogisticsSubType != LogisticsSubTypes.TCAT && _send.LogisticsSubType != LogisticsSubTypes.ECAN)
            {
                throw new Exception("LogisticsSubType error! Can only be TCAT or ECAN or refer to the dev document!");
            }

            distParameter.Add("LogisticsSubType", new ParameterValue(_send.LogisticsSubType.ToString(), true));

            if (_send.GoodsAmount < 1 || _send.GoodsAmount > 20000)
            {
                throw new Exception("GoodAmount can not be less than 1 or more than 20000!");
            }

            distParameter.Add("GoodsAmount", new ParameterValue(_send.GoodsAmount.ToString(), true));
            distParameter.Add("CollectionAmount", new ParameterValue(_send.CollectionAmount.Value.ToString(), false));

            if (_send.LogisticsType == LogisticsTypes.HOME && _send.IsCollection == IsCollections.YES)
            {
                throw new Exception("Can not set IsCollection to Yes when LogisticsType is Home");
            }

            distParameter.Add("IsCollection", new ParameterValue(_send.IsCollection.ToString().Substring(0, 1), false));

            if (!string.IsNullOrEmpty(_send.GoodsName))
            {
                distParameter.Add("GoodsName", new ParameterValue(_send.GoodsName, false));
            }

            distParameter.Add("SenderName", new ParameterValue(_send.SenderName, true));

            if (_send.LogisticsType == LogisticsTypes.HOME)
            {
                if (string.IsNullOrEmpty(_send.SenderPhone) && string.IsNullOrEmpty(_send.SenderCellPhone))
                {
                    throw new Exception(@"若為宅配時，則寄件人電話(SenderPhone)與手機擇(SenderCellPhone)一不可為空");
                }
            }

            if (!string.IsNullOrEmpty(_send.SenderPhone))
            {
                distParameter.Add("SenderPhone", new ParameterValue(_send.SenderPhone, false));
            }

            if (!string.IsNullOrEmpty(_send.SenderCellPhone))
            {
                int validCell = 0;
                if (int.TryParse(_send.SenderCellPhone, out validCell) && (_send.SenderCellPhone.StartsWith("10") || _send.SenderCellPhone.StartsWith("09")))
                {
                    distParameter.Add("SenderCellPhone", new ParameterValue(_send.SenderCellPhone, false));
                }
                else
                {
                    throw new Exception(@"收件人手機(SenderCellPhone)只允許數字、10碼、09開頭");
                }
            }

            distParameter.Add("ReceiverName", new ParameterValue(_send.ReceiverName, true));

            if (_send.LogisticsType == LogisticsTypes.HOME)
            {
                if (string.IsNullOrEmpty(_send.ReceiverPhone) && string.IsNullOrEmpty(_send.ReceiverCellPhone))
                {
                    throw new Exception(@"若為宅配時，則收件人電話(ReceiverPhone)與手機(ReceiverCellPhone)擇一不可為空");
                }
            }

            distParameter.Add("ReceiverPhone", new ParameterValue(_send.ReceiverPhone, false));

            if (!string.IsNullOrEmpty(_send.ReceiverCellPhone))
            {
                int validCell = 0;
                if (int.TryParse(_send.ReceiverCellPhone, out validCell) && (_send.ReceiverCellPhone.StartsWith("10") || _send.ReceiverCellPhone.StartsWith("09")))
                {
                    distParameter.Add("ReceiverCellPhone", new ParameterValue(_send.ReceiverCellPhone, false));
                }
                else
                {
                    throw new Exception(@"收件人手機(ReceiverCellPhone)只允許數字、10碼、09開頭");
                }
            }

            distParameter.Add("ReceiverEmail", new ParameterValue(_send.ReceiverEmail, false));
            distParameter.Add("TradeDesc", new ParameterValue(_send.TradeDesc, false));
            distParameter.Add("ServerReplyURL", new ParameterValue(_send.ServerReplyURL, true));
            distParameter.Add("ClientReplyURL", new ParameterValue(_send.ClientReplyURL, false));
            distParameter.Add("Remark", new ParameterValue(_send.Remark, false));
            distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));
            distParameter.Add("SenderZipCode", new ParameterValue(_send.SenderZipCode, true));
            distParameter.Add("SenderAddress", new ParameterValue(_send.SenderAddress, true));
            distParameter.Add("ReceiverZipCode", new ParameterValue(_send.ReceiverZipCode, true));
            distParameter.Add("ReceiverAddress", new ParameterValue(_send.ReceiverAddress, true));

            distParameter.Add("Temperature", new ParameterValue(EnumConvertor.ToString(_send.Temperature.Value), true));

            if (_send.LogisticsSubType == LogisticsSubTypes.ECAN)
            {
                if (_send.Temperature.Value != Temperatures.ROOM)
                {
                    throw new Exception(@"若為宅配通(ECan)時，則溫層(Temperature)只能用常溫(0001)");
                }
            }

            distParameter.Add("Distance", new ParameterValue(EnumConvertor.ToString(_send.Distance.Value), true));
            distParameter.Add("Specification", new ParameterValue(EnumConvertor.ToString(_send.Specification.Value), true));

            if (_send.ScheduledPickupTime.HasValue)
            {
                distParameter.Add("ScheduledPickupTime", new ParameterValue(EnumConvertor.ToString(_send.ScheduledPickupTime.Value), false));
            }

            if (_send.LogisticsSubType == LogisticsSubTypes.TCAT || _send.LogisticsSubType == LogisticsSubTypes.ECAN)
            {
                if (_send.ScheduledDeliveryTime != ScheduledDeliveryTimes.TIME_UNLIMITED &&
                    _send.ScheduledDeliveryTime != ScheduledDeliveryTimes.TIME_0_13 &&
                    _send.ScheduledDeliveryTime != ScheduledDeliveryTimes.TIME_14_18)
                {
                    throw new Exception("TCAT or ECAN can only use ScheduledDeliveryTime 0-13, 14-18, Unlimited!");
                }
            }

            if (_send.ScheduledDeliveryTime.HasValue)
            {
                distParameter.Add("ScheduledDeliveryTime", new ParameterValue(EnumConvertor.ToString(_send.ScheduledDeliveryTime.Value), false));
            }

            if (_send.ScheduledDeliveryDate.HasValue && _send.LogisticsSubType == LogisticsSubTypes.ECAN)
            {
                distParameter.Add("ScheduledDeliveryDate", new ParameterValue(_send.ScheduledDeliveryDate.Value.ToString("yyyy/MM/dd"), false));
            }

            if (_send.PackageCount.HasValue && _send.LogisticsSubType == LogisticsSubTypes.ECAN)
            {
                distParameter.Add("PackageCount", new ParameterValue(_send.PackageCount.Value.ToString(), false));
            }

            return distParameter;
        }

        /// <summary>取得"超商物流訂單"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GetLogisticsOrderForPickUp()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("MerchantTradeNo", new ParameterValue(_send.MerchantTradeNo, true));
            distParameter.Add("MerchantTradeDate", new ParameterValue(_send.MerchantTradeDate, true));
            distParameter.Add("LogisticsType", new ParameterValue(_send.LogisticsType.ToString(), true));

            if (_send.LogisticsSubType != LogisticsSubTypes.UNIMART && _send.LogisticsSubType != LogisticsSubTypes.FAMI && _send.LogisticsSubType != LogisticsSubTypes.HILIFE &&
                _send.LogisticsSubType != LogisticsSubTypes.UNIMARTC2C && _send.LogisticsSubType != LogisticsSubTypes.FAMIC2C && _send.LogisticsSubType != LogisticsSubTypes.HILIFEC2C)
            {
                throw new Exception("LogisticsSubType error! Can only be CVS or refer to the dev document!");
            }

            distParameter.Add("LogisticsSubType", new ParameterValue(_send.LogisticsSubType.ToString(), true));

            if (_send.GoodsAmount < 1 || _send.GoodsAmount > 20000)
            {
                throw new Exception("GoodAmount can not be less than 1 or more than 20000!");
            }

            distParameter.Add("GoodsAmount", new ParameterValue(_send.GoodsAmount.ToString(), true));

            if (_send.CollectionAmount.HasValue)
            {
                if (_send.LogisticsSubType == LogisticsSubTypes.UNIMARTC2C && _send.CollectionAmount.Value != _send.GoodsAmount)
                {
                    throw new Exception("物流子類型為UNIMARTC2C(統一超商交貨便)時，代收金額(CollectionAmount)需要與商品金額(GoodsAmount)一致。");
                }
                //Optional parameter, only add if it has value
                distParameter.Add("CollectionAmount", new ParameterValue(_send.CollectionAmount.Value.ToString(), false));
            }

            distParameter.Add("IsCollection", new ParameterValue(_send.IsCollection.ToString().Substring(0, 1), false));

            //UNIMARTC2C：統一超商交貨便(不可為空); HILIFEC2C：萊爾富店到店(不可為空)
            if (string.IsNullOrEmpty(_send.GoodsName))
            {
                if (_send.LogisticsSubType == LogisticsSubTypes.UNIMARTC2C || _send.LogisticsSubType == LogisticsSubTypes.HILIFEC2C)
                {
                    throw new Exception(@"物流子類型UNIMARTC2C(統一超商交貨便)、HILIFEC2C(萊爾富店到店)時，商品名稱(GoodsName)不可為空");
                }
            }
            else
            {
                distParameter.Add("GoodsName", new ParameterValue(_send.GoodsName, false));
            }

            distParameter.Add("SenderName", new ParameterValue(_send.SenderName, true));

            if (!string.IsNullOrEmpty(_send.SenderPhone))
            {
                distParameter.Add("SenderPhone", new ParameterValue(_send.SenderPhone, false));
            }

            if (!string.IsNullOrEmpty(_send.SenderCellPhone))
            {
                distParameter.Add("SenderCellPhone", new ParameterValue(_send.SenderCellPhone, false));
            }
            else if (_send.LogisticsSubType == LogisticsSubTypes.UNIMARTC2C || _send.LogisticsSubType == LogisticsSubTypes.HILIFEC2C)
            {
                throw new Exception("物流子類型UNIMARTC2C(統一超商交貨便)、HILIFEC2C(萊爾富店到店)時，寄件人手機(SenderCellPhone)不可為空");
            }

            distParameter.Add("ReceiverName", new ParameterValue(_send.ReceiverName, true));

            if (!string.IsNullOrEmpty(_send.ReceiverPhone))
            {
                distParameter.Add("ReceiverPhone", new ParameterValue(_send.ReceiverPhone, false));
            }

            distParameter.Add("ReceiverCellPhone", new ParameterValue(_send.ReceiverCellPhone, true));

            if (!string.IsNullOrEmpty(_send.ReceiverEmail))
            {
                distParameter.Add("ReceiverEmail", new ParameterValue(_send.ReceiverEmail, false));
            }

            distParameter.Add("TradeDesc", new ParameterValue(_send.TradeDesc, false));
            distParameter.Add("ServerReplyURL", new ParameterValue(_send.ServerReplyURL, true));
            distParameter.Add("ClientReplyURL", new ParameterValue(_send.ClientReplyURL, false));

            if (string.IsNullOrEmpty(_send.LogisticsC2CReplyURL))
            {
                if (_send.LogisticsSubType == LogisticsSubTypes.UNIMARTC2C)
                {
                    throw new Exception(@"物流子類型為UNIMARTC2C(統一超商交貨便)時，此欄位LogisticsC2CReplyURL不可為空");
                }
            }
            else
            {
                distParameter.Add("LogisticsC2CReplyURL", new ParameterValue(_send.LogisticsC2CReplyURL, false));
            }

            if (!string.IsNullOrEmpty(_send.Remark))
            {
                distParameter.Add("Remark", new ParameterValue(_send.Remark, false));
            }

            if (!string.IsNullOrEmpty(_send.PlatformID))
            {
                distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));
            }

            distParameter.Add("ReceiverStoreID", new ParameterValue(_send.ReceiverStoreID, false));
            distParameter.Add("ReturnStoreID", new ParameterValue(_send.ReturnStoreID, false));

            return distParameter;
        }

        /// <summary>取得"查詢物流訂單"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GetLogisticsInfo()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("AllPayLogisticsID", new ParameterValue(_send.AllPayLogisticsID, true));
            distParameter.Add("TimeStamp", new ParameterValue(((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString(), true));
            distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));

            return distParameter;
        }

        /// <summary>取得"產生托運單(宅配)/一段標(超商取貨)"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GetTradeDoc()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("AllPayLogisticsID", new ParameterValue(_send.AllPayLogisticsID, true));
            distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));

            return distParameter;
        }

        /// <summary>取得"列印繳款單(統一超商C2C)"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GettUnimartC2CBill()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("AllPayLogisticsID", new ParameterValue(_send.AllPayLogisticsID, true));
            distParameter.Add("CVSPaymentNo", new ParameterValue(_send.CVSPaymentNo, true));
            distParameter.Add("CVSValidationNO", new ParameterValue(_send.CVSValidationNo, true));
            distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));

            return distParameter;
        }

        /// <summary>取得"全家列印小白單(全家超商C2C)"參數</summary>
        /// <returns>回傳：參數</returns>
        public Dictionary<string, ParameterValue> GettFamiC2CBill()
        {
            var distParameter = new Dictionary<string, ParameterValue>();

            distParameter.Add("MerchantID", new ParameterValue(_merchantID, true));
            distParameter.Add("AllPayLogisticsID", new ParameterValue(_send.AllPayLogisticsID, true));
            distParameter.Add("CVSPaymentNo", new ParameterValue(_send.CVSPaymentNo, true));
            distParameter.Add("PlatformID", new ParameterValue(_send.PlatformID, false));

            return distParameter;
        }
    }
}