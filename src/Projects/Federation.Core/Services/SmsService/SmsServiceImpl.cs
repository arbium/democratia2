using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Federation.Core
{
    public class SmscServiceImpl : ISmsService
    {
        private string _login = "anonymouse";
        private string _md5Password = "";

        //TODO: сделать потокобезопасным!
        public bool UsePost { get; set; }
        public bool UseHttps { get; set; }
        public string Charset { get; set; }
        public bool Debug { get; set; }
        public string SenderName { get; set; }

        public SmscServiceImpl()
        {
            UsePost = false;
            UseHttps = false;
            Charset = "utf-8";
            Debug = false;
            SenderName = "Democratia2";
        }

        public bool Authorize(string login, string password)
        {
            bool result = true;

            _login = login;
            _md5Password = CryptographyService.EncryptPassword(password);

            try
            {
                GetBalance();
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        public void SmsInfoChangeStatus(int smsId, byte smsStatus, byte? errorCode)
        {
            var currentSmsInfo = DataService.PerThread.SmsInfoSet.FirstOrDefault(x => x.Id == smsId);

            if (currentSmsInfo == null)
                throw new BusinessLogicException("Не найдено смс с таким идентификатором");

            currentSmsInfo.State = smsStatus;
            currentSmsInfo.ResponseError = errorCode;
            currentSmsInfo.ChangeDate = DateTime.Now;

            DataService.PerThread.SaveChanges();
        }

        /// <summary>
        /// Метод отправки SMS
        /// </summary>
        /// <param name="phone">телефон</param>
        /// <param name="message">сообщение</param>
        /// <param name="query">доп. параметры</param>
        /// <returns></returns>
        public SmsInfo SendSms(string phone, string message, Guid? userId = null, string query = "")
        {
            SmsInfo smsInfo = DataService.PerThread.SmsInfoSet.CreateObject();
            smsInfo.Phone = phone;
            smsInfo.Message = message;
            smsInfo.State = Byte.MaxValue;
            smsInfo.Cost = 0;
            smsInfo.CreationDate = DateTime.Now;
            smsInfo.ChangeDate = DateTime.Now;
            smsInfo.PartsCount = 0;
            if (userId != null)
                smsInfo.BaseUserId = userId;
            DataService.PerThread.SmsInfoSet.AddObject(smsInfo);
            DataService.PerThread.SaveChanges();

            if (String.IsNullOrWhiteSpace(phone))
                throw new ArgumentNullException("phone");

            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException("message");

            if (message.Length >= 800)
                throw new ArgumentException("Длина отправляемого сообщения не должна превышать 800 символов", "message");

            string parameters = String.Format("cost=3&charset={0}&phones={1}&mes={2}&id={3}&sender={4}", Charset,
                                              HttpUtility.UrlEncode(phone), HttpUtility.UrlEncode(message),
                                              smsInfo.Id.ToString(CultureInfo.InvariantCulture),
                                              HttpUtility.UrlEncode(SenderName));

            if (String.IsNullOrWhiteSpace(query))
                parameters += ("&" + query);

            var m = SendCommand("send", parameters);
            if (m[0] == "0")
            {
                byte errorcode;
                if (Byte.TryParse(m[1].Substring(1, 1), out errorcode))
                {
                    smsInfo.SendError = errorcode;
                    smsInfo.ChangeDate = DateTime.Now;
                }
            }
            else
            {
                NumberFormatInfo numberInfo = NumberFormatInfo.InvariantInfo;

                double cost;
                if (Double.TryParse(m[2], NumberStyles.Any, numberInfo, out cost))
                    smsInfo.Cost = cost;

                byte partsCount;
                if (Byte.TryParse(m[1], out partsCount))
                    smsInfo.PartsCount = partsCount;

                smsInfo.State = 0;
            }

            return smsInfo;
        }

        /// <summary>
        /// Метод получения баланса
        /// </summary>
        /// <returns>Баланс в виде строки</returns>
        public double GetBalance()
        {
            //TODO: разобраться почему не работает взятие баланса через хттпс и пост
            var usePost = UsePost;
            var useHttps = UseHttps;

            UsePost = false;
            UseHttps = false;

            string[] m = SendCommand("balance", "");      
            
            if (m.Length != 1)
                throw new BusinessLogicException("Ошибка #" + m[1].Substring(1, 1));
            
            NumberFormatInfo numberInfo = NumberFormatInfo.InvariantInfo;
            double cost;
            if (!Double.TryParse(m[0], NumberStyles.Any, numberInfo, out cost))
                throw new BusinessLogicException("Ошибка получения баланса: некорректное значение");

            UsePost = usePost;
            UseHttps = useHttps;

            return cost;
        }

        #region Внутренние методы

        /// <summary>
        /// Метод вызова запроса. Формирует URL и делает 3 попытки чтения 
        /// </summary>
        /// <param name="command">Комманда</param>
        /// <param name="parameters">Параметры комманды</param>
        /// <returns></returns>
        private string[] SendCommand(string command, string parameters)
        {
            if (String.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException("command");

            string allParameters = "";

            allParameters = String.Format("login={0}&psw={1}&fmt=1", HttpUtility.UrlEncode(_login),
                                              HttpUtility.UrlEncode(_md5Password));
            if (!String.IsNullOrWhiteSpace(parameters))
                allParameters += ("&" + parameters);
            

            string protocol = UseHttps ? "https" : "http";

            string url = "";
            if(!UsePost)
                url = String.Format("{0}://smsc.ru/sys/{1}.php{2}", protocol, command, "?"+allParameters);
            else
                url = String.Format("{0}://smsc.ru/sys/{1}.php", protocol, command);
            

            WebRequest request = WebRequest.Create(url);

            if (UsePost)
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = allParameters.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(Encoding.Default.GetBytes(allParameters), 0, allParameters.Length);
                stream.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            string resultResponse;
            int i = 0;

            do
            {
                if (i > 0)
                    System.Threading.Thread.Sleep(2000);

                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    throw new BusinessLogicException("Невозможно получить ответ по адресу: " + url);

                var streamReader = new StreamReader(responseStream);
                resultResponse = streamReader.ReadToEnd();
            }
            while (resultResponse == "" && ++i < 3);

            if (resultResponse == "")
            {
                throw new BusinessLogicException("Невозможно получить ответ по адресу: " + url);
            }

            return resultResponse.Split(',');
        }

        #endregion
    }

    public struct SmsStatus
    {
        public int Status;
        public DateTime LastDate;
        public int d;
    }

    // Examples: 
    // SMSC smsc = new SMSC(); 
    // string[] r = smsc.send_sms("79999999999", "Ваш пароль: 123", 2); 
    // string[] r = smsc.send_sms("79999999999", "http://smsc.ru\nSMSC.RU", 0, "", 0, 0, "", "maxsms=3"); 
    // string[] r = smsc.send_sms("79999999999", "0605040B8423F0DC0601AE02056A0045C60C037761702E736D73632E72752F0001037761702E736D73632E7275000101", 0, "", 0, 5); 
    // string[] r = smsc.send_sms("79999999999", "", 0, "", 0, 3); 
    // string[] r = smsc.get_sms_cost("79999999999", "Вы успешно зарегистрированы!"); 
    // smsc.send_sms_mail("79999999999", "Ваш пароль: 123", 0, "0101121000"); 
    // string[] r = smsc.get_status(12345, "79999999999"); 
    // string balance = smsc.get_balance();
}


//// Метод получения стоимости SMS 
//// 
//// обязательные параметры: 
//// 
//// phones - список телефонов через запятую или точку с запятой 
//// message - отправляемое сообщение 
//// 
//// необязательные параметры: 
//// 
//// translit - переводить или нет в транслит 
//// format - формат сообщения (0 - обычное sms, 1 - flash-sms, 2 - wap-push, 3 - hlr, 4 - bin, 5 - bin-hex, 6 - ping-sms) 
//// sender - имя отправителя (Sender ID) 
//// query - строка дополнительных параметров, добавляемая в URL-запрос ("list=79999999999:Ваш пароль: 123\n78888888888:Ваш пароль: 456") 
//// 
//// возвращает массив (<стоимость>, <количество sms>) либо массив (0, -<код ошибки>) в случае ошибки 

//public string[] get_sms_cost(string phones, string message, int translit = 0, int format = 0, string sender = "", string query = "")
//{
//    string[] formats = { "flash=1", "push=1", "hlr=1", "bin=1", "bin=2", "ping=1" };

//    string[] m = _smsc_send_cmd("send", "cost=1&charset=" + Charset + "&phones=" + _urlencode(phones)
//                    + "&mes=" + _urlencode(message) + translit.ToString() + (format > 0 ? "&" + formats[format - 1] : "")
//                    + (sender != "" ? "&sender=" + _urlencode(sender) : "") + (query != "" ? "&query" : ""));

//    // (cost, cnt) или (0, -error) 

//    if (Debug)
//    {
//        if (Convert.ToInt32(m[1]) > 0)
//            _print_debug("Стоимость рассылки: " + m[0] + " руб. Всего SMS: " + m[1]);
//        else
//            _print_debug("Ошибка №" + m[1].Substring(1, 1));
//    }

//    return m;
//}

// Метод проверки статуса отправленного SMS или HLR-запроса 
// 
// id - ID cообщения 
// phone - номер телефона 
// 
// возвращает массив: 
// для отправленного SMS (<статус>, <время изменения>, <код ошибки sms>) 
// для HLR-запроса (<статус>, <время изменения>, <код ошибки sms>, <код страны регистрации>, <код оператора абонента>, 
// <название страны регистрации>, <название оператора абонента>, <название роуминговой страны>, <название роумингового оператора>, 
// <код IMSI SIM-карты>, <номер сервис-центра>) 
// либо массив (0, -<код ошибки>) в случае ошибки 

//public string[] GetStatus(int id, string phone)
//{
//    //так же можно узнавать о статусе HLR-запроса, но эта функция не реализована
//    //там есть еще развернутый формат, при all = 1

//    string[] m = SendCommand("status", "phone=" + HttpUtility.UrlEncode(phone) + "&id=" + id.ToString(CultureInfo.InvariantCulture));


//        if (m[1] != "" && Convert.ToInt32(m[1]) >= 0)
//        {
//            int timeStamp = Convert.ToInt32(m[1]);
//            DateTime offset = new DateTime(1970, 1, 1, 0, 0, 0, 0);
//            DateTime date = offset.AddSeconds(timeStamp);

//            SmsStatus smsStatus = new SmsStatus() ;
//        }
//        else
//            throw new BusinessLogicException("Ошибка №" + m[1].Substring(1, 1));


//    return m;
//}