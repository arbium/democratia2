using System;

namespace Federation.Core
{
    public static class DateTimeExtension
    {
        public static DateTime ToUserTime(this DateTime dateTime, UserContainer currUserContext)
        {
            if (currUserContext == null || dateTime == DateTime.MinValue)
                return dateTime;

            return dateTime + currUserContext.UTCOffset - TimeZoneInfo.Local.BaseUtcOffset;
        }

        public static string ToFormattedUserTime(this DateTime dateTime, UserContainer currUserContext, string timeFormat = "HH:mm:ss", bool timeOnly = false)
        {
            var date = string.Empty;
            var time = string.Empty;

            dateTime = dateTime.ToUserTime(currUserContext);

            if (dateTime.Date == DateTime.Now.Date)
            {
                if (!timeOnly)
                    date = "Сегодня";
                if (!string.IsNullOrEmpty(timeFormat))
                    time = dateTime.ToString(timeFormat);
            }
            else if (dateTime.Year == DateTime.Now.Year)
            {
                if (!timeOnly)
                    date = dateTime.ToString("d MMMM");
                if (!string.IsNullOrEmpty(timeFormat))
                    time = dateTime.ToString(timeFormat);
            }
            else
            {
                if (!timeOnly)
                    date = dateTime.ToString("dd.MM.yy");
                if (!string.IsNullOrEmpty(timeFormat))
                    time = dateTime.ToString(timeFormat);
            }

            return (date + " " + time).Trim();
        }
    }
}