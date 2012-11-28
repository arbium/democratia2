using System;
using System.Net;
using System.Text;
using Federation.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Federation.Web.Services
{
    public static class ApiService
    {
        public static void D2SendPassword(string code, string phone, User user)
        {
            var url = ConstHelper.ApiUrl + "d2sendpassword";
            var data = new
            {
                ApiKey = ConstHelper.ApiKey,
                Phone = phone,
                Code = code,
                FirstName = user.FirstName,
                Patronymic = user.Patronymic,
                SurName = user.SurName,
                BirthDate = user.BirthDate
            };

            SendJson(url, data);
        }

        public static void SendPassword(string code, string phone, User user)
        {
            var url = ConstHelper.ApiUrl + "sendpassword";
            var data = new
                {
                    ApiKey = ConstHelper.ApiKey, 
                    Phone = phone, 
                    Code = code
                };

            SendJson(url, data);
        }

        public static bool CheckPassword(string code, string password)
        {
            var url = ConstHelper.ApiUrl + "checkpassword";
            var data = new { ApiKey = ConstHelper.ApiKey, Code = code, Password = password };

            try
            {
                SendJson(url, data);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static JObject SendJson(string url, object data)
        {
            try
            {
                string response;

                using (var webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

                    var stringData = JsonConvert.SerializeObject(data);
                    var responseBytes = webClient.UploadData(url, Encoding.UTF8.GetBytes(stringData));
                    response = Encoding.UTF8.GetString(responseBytes);
                }

                var result = JObject.Parse(response);
                var code = (int?)result["Code"];
                var message = (string)result["Message"];

                if (!code.HasValue || message == null)
                    throw new JsonException();

                if (code != (int)ApiResponseCode.OK)
                    throw new ValidationException(ApiResponse.ApiError((ApiResponseCode)code.Value).Message);

                return result;
            }
            catch (WebException)
            {
                throw new ValidationException(ApiResponse.UnableToConnect.Message);
            }
            catch (JsonException)
            {
                throw new ValidationException(ApiResponse.BadResponse.Message);
            }
        }
    }
}
