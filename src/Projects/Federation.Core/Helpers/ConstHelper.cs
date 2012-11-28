using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Federation.Core
{
    public static class ConstHelper
    {
        private static Guid _rootGroupId = Guid.Empty;

        public static string HomeUrl;
        public static string LoginUrl;

        public const string DefaultImageName = "noimage.jpg";

        private static string _apiUrl;
        public static string ApiUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_apiUrl))
                    _apiUrl = ConfigurationSettings.AppSettings["ApiUrl"];

                return _apiUrl;
            }
        }

        private static string _apiKey;
        public static string ApiKey
        {
            get
            {
                if (string.IsNullOrEmpty(_apiKey))
                    _apiKey = ConfigurationSettings.AppSettings["ApiKey"];

                return _apiKey;
            }
        }

        /// <summary>
        /// Регулярное выражение на адрес электронной почты
        /// </summary>
        public const string EmailRegexp = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        /// <summary>
        /// Регулярное выражение для ссылки на профиль facebook
        /// </summary>
        public const string FbRegexp = @"^https?://(www\.)?(facebook|fb)\.com/.*$";

        /// <summary>
        /// Регулярное выражение для ссылки на профиль livejournal
        /// </summary>
        public const string LjRegexp = @"^https?://(www\.)?(.*\.)?(livejournal|lj)\.(com|ru)/.*$";

        public const string TelRegexp = @"^\s*((\+?[1-9])(-?|\s?))\(?((-?|\s?)\d){3}\)?((-?|\s?)\d){7,10}\s*$";

        /// <summary>
        /// Время, через которое чувак протухает
        /// </summary>
        public static readonly TimeSpan UserOutdatedTime = new TimeSpan(30, 0, 0, 0);

        /// <summary>
        /// Время, через которое пользователь может выслать инвайт повторно
        /// </summary>
        public static readonly TimeSpan MinInviteResendInterval = new TimeSpan(1, 0, 0, 0);

        /// <summary>
        /// Минимальное количество модераторов в группе для ее создания
        /// </summary>
        public const int MinModeratorsInGroup = 3;

        /// <summary>
        /// Минимально необходимое число членов группы для возможности инициации выборов
        /// </summary>
        public const int MinGMsCountForElection = 50;

        /// <summary>
        /// Необходимая часть от всех членов проголосовавших, для признания выборов состоявшимися
        /// </summary>
        public const double ElectionQuorumGMsPart = 0.3;

        /// <summary>
        /// Необходимая часть от всех членов проголосовавших, для признания голосования состоявшимися
        /// </summary>
        public const double PollQuorumGMsPart = 0.2;

        /// <summary>
        /// Продолжительность стадии голосования на выборах
        /// </summary>
        public const int ElectionDurationDays = 3;

        /// <summary>
        /// Продолжительность предварительной стадии на выборах
        /// </summary>
        public const int ElectionAgitationDurationDays = 7;

        /// <summary>
        /// Количество подписей, которое необходимо собрать кандидату для участия в выборах
        /// </summary>
        public const int CandidatePetitionNecessarySigners = 5;

        /// <summary>
        /// Идентификатор корневой группы
        /// </summary>
        public static Guid RootGroupId
        { 
            get 
            {
                var result = Guid.Empty;

                if (_rootGroupId != Guid.Empty)
                    result = _rootGroupId;
                else
                {
                    var rootGroup = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Type == (byte)GroupType.Root);
                    if (rootGroup != null)
                    {
                        _rootGroupId = rootGroup.Id;
                        result = rootGroup.Id;
                    }
                }

                return result;
            }

            set
            {
                _rootGroupId = value;
            }
        }

        /// <summary>
        /// Длина отображаемой части контента поста в ленте блога
        /// </summary>
        public static int SummaryLength = 1000;

        /// <summary>
        /// Мини summary
        /// </summary>
        public static int MiniSummaryLength = 300;

        /// <summary>
        /// Длина отображаемой части сообщения
        /// </summary>
        public static int MessageSummaryLength = 150;

        /// <summary>
        /// Путь к приложению
        /// </summary>
        public static string AppPath
        {
            get
            {
                if (string.IsNullOrEmpty(_appPath))
                    throw new BusinessLogicException("Не ицициалоизирован AppPath");

                return _appPath;
            }
            set { _appPath = value; }
        }

        /// <summary>
        /// Url приложения
        /// </summary>
        public static string AppUrl
        {
            get 
            {
                if (string.IsNullOrEmpty(_appUrl))
                    throw new BusinessLogicException("Не ицициалоизирован AppUrl");

                return _appUrl; 
            }
            set { _appUrl = value; }
        }

        private static string _appUrl;
        private static string _appPath;

        /// <summary>
        /// "http://federation.ru/<...>/<значение этого поля>=errorcode"
        /// </summary>
        public static string ErrorCode = "error";

        /// <summary>
        /// Список алиасов Url приложения
        /// </summary>
        public static List<string> UrlAliases = new List<string>();

        public static short MaxInvitesCount = 30;

        public static TimeSpan CommentEditTimeSpan = new TimeSpan(0, 0, 3, 0);

        public static int CommentsPerPage = 15;

        /// <summary>
        /// ID группы координационного совета
        /// </summary>
        public static string KsGroupId = "8FC1E353-652F-4340-B02B-4BF278D45943";
    }
}