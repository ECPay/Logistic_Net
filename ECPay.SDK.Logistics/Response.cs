using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ECPay.SDK.Logistics
{
    /// <summary>
    /// 6.  物流訂單產生
    /// 19. 產生托運單(宅配)/一段標(超商取貨)格式(B2C)
    /// 20. 列印繳款單(統一超商 C2C)
    /// 21. 全家列印小白單(全家超商 C2C)
    /// 22. 萊爾富列印小白單(萊爾富超商 C2C)
    /// 23. 產生萊爾富測標資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        private T rawData;

        public T Data
        {
            get
            {
                return rawData;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                rawData = value;

                if (typeof(string) == rawData.GetType())
                {
                    if (string.IsNullOrEmpty(rawData.ToString()))
                    {
                        return;
                    }
                    if (rawData.ToString().Contains("<html") ||
                        rawData.ToString().Contains("<form") ||
                        rawData.ToString().Contains("<div"))
                    {
                        return;
                    }

                    var _strData = rawData.ToString().Split('|');

                    if (_strData != null && _strData.Length > 1)
                    {
                        Code = _strData[0];
                        DictionData = HttpUtility.ParseQueryString(_strData[1]);
                    }
                    else
                    {
                        DictionData = HttpUtility.ParseQueryString(_strData[0]);
                    }
                    if (DictionData["LogisticsType"] != null && DictionData["LogisticsType"].Contains('_'))
                    {
                        var _strValueAry = DictionData["LogisticsType"].Split('_');
                        if (_strValueAry.Length > 1)
                        {
                            DictionData["LogisticsType"] = _strValueAry[0];
                            DictionData["LogisticsSubType"] = _strValueAry[1];
                        }
                    }
                }
                ParsedData = new Parameters();

                foreach (var property in ParsedData.GetType().GetProperties())
                {
                    var valueAsString = DictionData[property.Name];//HttpContext.Current.Request.QueryString[property.Name];
                    if (!string.IsNullOrEmpty(valueAsString))
                    {
                        TypeConverter obj = TypeDescriptor.GetConverter(property.PropertyType);
                        if (property.Name.ToLower() == "iscollection")
                        {
                            switch (valueAsString.ToLower())
                            {
                                case "n":
                                    valueAsString = "NO";
                                    break;

                                case "y":
                                    valueAsString = "YES";
                                    break;

                                default:
                                    break;
                            }
                        }

                        object pValue = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueAsString);
                        if (pValue != null)
                            property.SetValue(ParsedData, pValue, null);
                    }
                }
            }
        }

        public string Code { get; set; }
        public NameValueCollection DictionData { get; set; }
        public Parameters ParsedData { get; set; }

        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public Exception ErrorException { get; set; }
    }
}