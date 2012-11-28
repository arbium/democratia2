using System;

namespace Federation.Core
{
    public static class SmsService
    {
        private static ISmsService _current = new SmscServiceImpl();

        public static bool UsePost
        {
            get { return _current.UsePost; }
            set { _current.UsePost = value; }
        }

        public static bool UseHttps
        {
            get { return _current.UseHttps; }
            set { _current.UseHttps = value; }
        }

        public static string Charset
        {
            get { return _current.Charset; }
            set { _current.Charset = value; }
        }

        public static string SenderName
        {
            get { return _current.SenderName; }
            set { _current.SenderName = value; }
        }

        public static bool Authorize(string login, string password)
        {
            return _current.Authorize(login, password);
        }

        public static void SendSms(string phone, string message, Guid? userId = null, string query = "")
        {
            _current.SendSms( phone,  message, userId,  query);
        }

        public static void SmsInfoChangeStatus(int smsId, byte smsStatus, byte? errorCode)
        {
            _current.SmsInfoChangeStatus(smsId, smsStatus, errorCode);
        }

        public static double GetBalance()
        {
            return _current.GetBalance();
        }
    }
}
