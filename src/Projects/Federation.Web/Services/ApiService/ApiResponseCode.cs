namespace Federation.Web.Services
{
    public static class ApiResponseCodeExtensions
    {
        public static string PrepareForUser(this ApiResponseCode code)
        {
            if (code == ApiResponseCode.InvalidTicket)
                return "Неверные данные";

            if (code == ApiResponseCode.UsedTicket)
                return "Вы уже голосовали";

            if (code == ApiResponseCode.TicketIsBlank)
                return "Вы не верифицировались на выборах, поэтому не можете использовать этот метод подтверждения личности на Д2";

            if (code == ApiResponseCode.TicketIsDeactivated)
                return "Этот идентификатор заблокирован";

            if (code == ApiResponseCode.BadPhoneFormat)
                return "Неправильный формат телефона";

            return null;
        }
    }

    public enum ApiResponseCode : int
    {
        OK = 0,

        BadJsonFormat = 10,
        MissingJsonArgument = 11,

        InvalidTicket = 20,
        UsedTicket = 21,
        TicketRequestLimitExceeded = 22,
        TicketIsBlank = 23,
        TicketIsDeactivated = 24,

        UnknownOption = 30,
        DuplicateOption = 31,
        OptionSelectionExceeded = 32,
        WrongBulletinNumber = 33,
        WrongElectionNumber = 34,

        WrongApiKey = 40,
        PlatformRequestLimitExceeded = 41,

        BadPhoneFormat = 50,

        Unknown = 99
    }
}