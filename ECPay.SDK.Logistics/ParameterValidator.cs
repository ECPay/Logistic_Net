using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace ECPay.SDK.Logistics
{
    public static class ParameterValidator
    {
        /// <summary>驗證輸入的參數。如果正確無誤，會輸出照英文字母排序但不包含CheckMacValue的QueryString</summary>
        /// <param name="parameters"></param>
        /// <returns>QueryString as string</returns>
        public static string Validate(Dictionary<string, string> parameters)
        {
            String result = "";
            //Validate parameters
            if (!parameters.All(kp => validateParameter(kp)))
            {
                throw new Exception("There is an invalid parameter!");
            }

            //Reorder parameters
            parameters = parameters.OrderBy(kp => kp.Key).ToDictionary(d => d.Key, d => d.Value);

            //Generate QueryString
            using (var content = new FormUrlEncodedContent(parameters))
            {
                result = content.ReadAsStringAsync().Result;
            }

            return result;
        }

        /// <summary>驗證輸入的參數。如果正確無誤，會輸出照英文字母排序但不包含CheckMacValue的QueryString</summary>
        /// <param name="parameters"></param>
        /// <returns>QueryString as string</returns>
        public static string Validate(Dictionary<string, ParameterValue> parameters)
        {
            String result = "";
            //Validate parameters
            if (!parameters.All(kp => validateParameter(kp)))
            {
                throw new Exception("There is an invalid parameter!");
            }

            //Reorder parameters
            var orderedParameters = parameters.OrderBy(kp => kp.Key).ToDictionary(d => d.Key, d => d.Value.Value);

            //Generate QueryString
            using (var content = new FormUrlEncodedContent(orderedParameters))
            {
                result = content.ReadAsStringAsync().Result;
            }

            return result;
        }

        /// <summary>驗證輸入的參數。如果正確無誤，會輸出照英文字母排序且包含CheckMacValue的QueryString</summary>
        /// <param name="parameters"></param>
        /// <param name="HashKey"></param>
        /// <param name="HashIV"></param>
        /// <returns></returns>
        public static FormUrlEncodedContent ValidateURLContent(Dictionary<string, string> parameters, string HashKey, string HashIV)
        {
            //Validate parameters
            if (!parameters.All(kp => validateParameter(kp)))
            {
                throw new Exception("There is an invalid parameter!");
            }

            //Reorder parameters
            parameters = parameters.OrderBy(kp => kp.Key).ToDictionary(d => d.Key, d => d.Value);

            var result = attachCheckMacValue(parameters, HashKey, HashIV);

            return result;
        }

        /// <summary>驗證輸入的參數。如果正確無誤，會輸出照英文字母排序且包含CheckMacValue的QueryString</summary>
        /// <param name="parameters"></param>
        /// <param name="HashKey"></param>
        /// <param name="HashIV"></param>
        /// <returns></returns>
        public static FormUrlEncodedContent ValidateURLContent(Dictionary<string, ParameterValue> parameters, string HashKey, string HashIV)
        {
            //Validate parameters
            if (!parameters.All(kp => validateParameter(kp)))
            {
                throw new Exception("There is an invalid parameter!");
            }

            //Reorder parameters
            var orderedParameters = parameters.OrderBy(kp => kp.Key).ToDictionary(d => d.Key, d => d.Value.Value);

            var result = attachCheckMacValue(orderedParameters, HashKey, HashIV);

            return result;
        }

        private static bool validateParameter(KeyValuePair<string, string> parameter)
        {
            switch (parameter.Key.ToLower())
            {
                case "merchantid":
                    validateLegth(parameter.Value, 10, parameter.Key);
                    break;

                case "merchanttradeno":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "merchanttradedate":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    validateDateForm(parameter.Value, parameter.Key);
                    break;

                case "logisticstype":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    validateEnumValue(parameter.Value, typeof(LogisticsTypes), parameter.Key);
                    break;

                case "logisticssubtype":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    validateEnumValue(parameter.Value, typeof(LogisticsSubTypes), parameter.Key);
                    break;

                case "goodsamount":
                    validateMoneyValue(parameter.Value, parameter.Key);
                    break;

                case "collectionamount":
                    validateMoneyValue(parameter.Value, parameter.Key);
                    break;

                case "iscollection":
                    validateLegth(parameter.Value, 1, parameter.Key);
                    validateEnumValue(parameter.Value, typeof(IsCollections), parameter.Key);
                    break;

                case "goodsname":
                    validateLegth(parameter.Value, 60, parameter.Key);
                    break;

                case "sendername":
                    validateLegth(parameter.Value.Replace(" ", ""), 10, parameter.Key);
                    break;

                case "sendercellphone":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "receivername":
                    validateLegth(parameter.Value.Replace(" ", ""), 10, parameter.Key);
                    break;

                case "receiverphone":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "receiveremail":
                    validateLegth(parameter.Value, 100, parameter.Key);
                    break;

                case "receivercellphone":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "serverreplyurl":
                    validateLegth(parameter.Value, 200, parameter.Key);
                    break;

                case "clientreplyurl":
                    validateLegth(parameter.Value, 200, parameter.Key);
                    break;

                case "logisticsc2creplyurl":
                    validateLegth(parameter.Value, 200, parameter.Key);
                    break;

                case "remark":
                    validateLegth(parameter.Value, 200, parameter.Key);
                    break;

                case "platformid":
                    validateLegth(parameter.Value, 10, parameter.Key);
                    break;

                case "senderzipcode":
                    validateLegth(parameter.Value, 5, parameter.Key);
                    break;

                case "senderaddress":
                    validateMinMaxLength(parameter.Value, 6, 60, parameter.Key);
                    break;

                case "receiveraddress":
                    validateLegth(parameter.Value, 200, parameter.Key);
                    break;

                case "receiverzipcode":
                    validateLegth(parameter.Value, 5, parameter.Key);
                    break;

                case "temperature":
                    validateLegth(parameter.Value, 4, parameter.Key);
                    break;

                case "distance":
                    validateLegth(parameter.Value, 2, parameter.Key);
                    break;

                case "specification":
                    validateLegth(parameter.Value, 4, parameter.Key);
                    break;

                case "scheduledpickuptime":
                    validateLegth(parameter.Value, 1, parameter.Key);
                    break;

                case "scheduleddeliverytime":
                    validateLegth(parameter.Value, 2, parameter.Key);
                    break;

                case "scheduleddeliverydate":
                    validateLegth(parameter.Value, 10, parameter.Key);
                    validateDateForm(parameter.Value, parameter.Key);
                    break;

                case "packagecount":
                    validateNumberValue(parameter.Value, parameter.Key);
                    break;

                case "receiverstoreid":
                    validateLegth(parameter.Value, 6, parameter.Key);
                    break;

                case "returnstoreid":
                    validateLegth(parameter.Value, 6, parameter.Key);
                    break;

                case "allpaylogisticsid":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "senderphone":
                    validateLegth(parameter.Value, 20, parameter.Key);
                    break;

                case "collectionAmount":
                    validateMoneyValue(parameter.Value, parameter.Key);
                    break;

                case "quantity":
                    validateLegth(parameter.Value, 50, parameter.Key);
                    break;

                case "cost":
                    validateLegth(parameter.Value, 50, parameter.Key);
                    break;

                case "rtnmerchanttradeno":
                    validateLegth(parameter.Value, 13, parameter.Key);
                    break;

                case "cvspaymentno":
                    validateLegth(parameter.Value, 15, parameter.Key);
                    break;

                case "cvsvalidationno":
                    validateLegth(parameter.Value, 10, parameter.Key);
                    break;

                default:
                    break;
            }
            return true;
        }

        private static bool validateParameter(KeyValuePair<string, ParameterValue> parameter)
        {
            //validate required field is null
            if (parameter.Value.Required)
            {
                validateNull(parameter.Value.Value, parameter.Key);
            }

            //if field is not required and is null then skips validate
            if (!string.IsNullOrEmpty(parameter.Value.Value))
            {
                var testValue = new KeyValuePair<string, string>(parameter.Key, parameter.Value.Value);

                validateParameter(testValue);
            }

            return true;
        }

        private static void validateNull(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(name + " is null");
            }
        }

        private static void validateLegth(string value, int maxLength, string name)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                throw new Exception(name + " length can not be more than " + maxLength.ToString() + "!");
            }
        }

        private static void validateMinMaxLength(string value, int minLength, int maxLength, string name)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                throw new Exception(name + " length can not be more than " + maxLength.ToString() + "!");
            }

            if (!string.IsNullOrEmpty(value) && value.Length < minLength)
            {
                throw new Exception(name + " length can not be less than " + minLength.ToString() + "!");
            }
        }

        private static void validateDateForm(string value, string name)
        {
            validateNull(value, name);
            DateTime result;
            if (!string.IsNullOrEmpty(value) && !DateTime.TryParse(value, out result))
            {
                throw new Exception(name + " is not valid DateTime form(yyyy/MM/dd HH:mm:ss)!");
            }
        }

        private static void validateEnumValue(string value, Type EnumType, string name)
        {
            if (!Enum.GetNames(EnumType).Contains(value))
            {
                //throw new Exception(name + " value error, please check " + name + " Enum!");
            }
        }

        private static void validateMoneyValue(string value, string name)
        {
            decimal result;
            if (!string.IsNullOrEmpty(value) && !decimal.TryParse(value, out result))
            {
                throw new Exception(name + " can only be numbers!");
            }
        }

        private static void validateNumberValue(string value, string name)
        {
            int result;
            if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out result))
            {
                throw new Exception(name + " can only be integers!");
            }
        }

        private static FormUrlEncodedContent attachCheckMacValue(Dictionary<string, string> parameters, string HashKey, string HashIV)
        {
            string strQuery = "";
            //Generate QueryString

            strQuery = "HashKey=" + HashKey + "&" + ToQueryString(parameters) + "&HashIV=" + HashIV;
            var encodedRaw = System.Web.HttpUtility.UrlEncode(strQuery).ToLower();
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, encodedRaw).ToUpper();

                Console.WriteLine("The MD5 hash of " + encodedRaw + " is: " + hash + ".");

                Console.WriteLine("Verifying the hash...");

                if (VerifyMd5Hash(md5Hash, encodedRaw, hash))
                {
                    Console.WriteLine("The hashes are the same.");
                    parameters.Add("CheckMacValue", hash);
                    return new FormUrlEncodedContent(parameters);
                }
                else
                {
                    throw new Exception("The hashes are not same.");
                }
            }
            throw new Exception();
        }

        public static string generateCheckMacValue(string QueryString, string HashKey, string HashIV)
        {
            string strRaw = "HashKey=" + HashKey + "&" + QueryString + "&HashIV=" + HashIV;
            var encodedRaw = System.Web.HttpUtility.UrlEncode(strRaw).ToLower();
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, encodedRaw);

                Console.WriteLine("The MD5 hash of " + encodedRaw + " is: " + hash + ".");

                Console.WriteLine("Verifying the hash...");

                if (VerifyMd5Hash(md5Hash, encodedRaw, hash))
                {
                    Console.WriteLine("The hashes are the same.");
                    return hash.ToUpper();
                }
                else
                {
                    Console.WriteLine("The hashes are not same.");
                }
            }
            throw new Exception("The hashes are not same or fail to generate.");
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string encode(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length * 2);//VERY rough estimate
            byte[] arr = Encoding.UTF8.GetBytes(s);

            for (int i = 0; i < arr.Length; i++)
            {
                byte c = arr[i];

                if (c >= 0x41 && c <= 0x5A)//alpha
                    sb.Append((char)c);
                else if (c >= 0x61 && c <= 0x7A)//ALPHA
                    sb.Append((char)c);
                else if (c >= 0x30 && c <= 0x39)//123456789
                    sb.Append((char)c);
                else if (c == '-' || c == '.' || c == '_' || c == '~')
                    sb.Append((char)c);
                else
                {
                    sb.Append('%');
                    sb.Append(Convert.ToString(c, 16).ToUpper());
                }
            }
            return sb.ToString();
        }

        public static string ToQueryString(this IDictionary<string, string> dict)
        {
            if (dict.Count == 0) return string.Empty;

            var buffer = new StringBuilder();
            int count = 0;
            bool end = false;

            foreach (var key in dict.Keys)
            {
                if (count == dict.Count - 1) end = true;

                if (end)
                    buffer.AppendFormat("{0}={1}", key, dict[key]);
                else
                    buffer.AppendFormat("{0}={1}&", key, dict[key]);

                count++;
            }

            return buffer.ToString();
        }
    }
}