using System;

namespace Federation.Core
{
    public interface ISmsService
    {
        bool UsePost { get; set; }
        bool UseHttps { get; set; }
        string Charset { get; set; }
        string SenderName { get; set; }

        bool Authorize(string login, string password);
        SmsInfo SendSms(string phone, string message, Guid? userId, string query);
        void SmsInfoChangeStatus(int smsId, byte smsStatus, byte? errorCode);
        double GetBalance();
    }
}
