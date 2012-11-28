using System;

namespace Federation.Core
{
    public static class AccountService
    {
        private static IAccountService _accountService = new AccountServiceImpl();

        #region Настройки

        public static int MinPasswordLength
        {
            get { return _accountService.MinPasswordLength; }
        }

        #endregion

        public static void RemoveOldPasswordRecoveries()
        {
            _accountService.RemoveOldPasswordRecoveries();
        }

        public static void RemoveOldEmailVerifications()
        {
            _accountService.RemoveOldEmailVerifications();
        }

        public static void RemoveOldSecretCodes()
        {
            _accountService.RemoveOldSecretCodes();
        }

        public static string GenerateSecretCode(Guid userId)
        {
            return _accountService.GenerateSecretCode(userId);
        }

        public static bool VerifySecretCode(Guid id, string code)
        {
            return _accountService.VerifySecretCode(id, code);
        }

        public static void SendEmailVerification(Guid userId)
        {
            _accountService.SendEmailVerification(userId);
        }

        public static bool VerifyEmail(Guid userId, string code)
        {
            return _accountService.VerifyEmail(userId, code);
        }

        public static DateTime? IsEmailVerificationAlreadySend(Guid userId)
        {
            return _accountService.IsEmailVerificationAlreadySend(userId);
        }

        public static void DisconnectSocialAccount(User user, SocialType socialType)
        {
            _accountService.DisconnectSocialAccount(user, socialType);
        }

        public static bool ConnectSocialAccount(User user, SocialType socialType, string link, string key)
        {
            return _accountService.ConnectSocialAccount(user, socialType,link, key);
        }

        #region Идентификация

        /// <summary>
        /// Идентификация пользователя по логину/е-мэйлу и паролю
        /// </summary>
        public static bool TrySignIn(string userIdentificator, string password, bool createPersistentCookie, string ip)
        {
            return _accountService.TrySignIn(userIdentificator, password, createPersistentCookie, ip);
        }

        /// <summary>
        /// Идентификация пользователя через внешние идентифицирующие сервисы
        /// </summary>
        public static bool TrySignIn(string key, SocialType socialtype, bool createPersistentCookie, string ip)
        {
            return _accountService.TrySignIn(key, socialtype, createPersistentCookie, ip);
        }

        /// <summary>
        /// Регистрация пользователя по логину, электропочте и паролю
        /// </summary>
        public static BaseUser SignUp(string login, string email, string password, bool saveChanges = true)
        {
            return _accountService.SignUp(login, email, password, saveChanges);
        }

        /// <summary>
        /// Выход из аккаунта пользователя
        /// </summary>
        public static void SignOut()
        {
            _accountService.SignOut();
        }

        #endregion

        #region Восстановление пароля

        /// <summary>
        /// Изменение пароля пользователя. Вызывается, когда известен пользователь. По сути со страницы пользователя
        /// </summary>
        public static void ChangePassword(BaseUser user, string oldPassword, string newPassword)
        {
            _accountService.ChangePassword(user, oldPassword, newPassword);   
        }

        /// <summary>
        /// Изменение пароля пользователся по секретному ключу, который до этого был доставлен пользователю. 
        /// Вызывается когда пользователь относительно неизвестен. Например при восстановлении пароля через почту
        /// </summary>
        public static void ChangePassword(Guid userKey, Guid securityKey, string newPassword)
        {
            _accountService.ChangePassword(userKey, securityKey, newPassword);
        }

        /// <summary>
        /// Инициировать систему возврата пароля. Например отсылки письма с секретной ссылкой на электропочту
        /// </summary>
        public static void RecoverPassword(Guid userId)
        {
            _accountService.RecoverPassword(userId);
        }

        public static void RecoverPassword(Guid recoverKey, string newPassword)
        {
            _accountService.RecoverPassword(recoverKey, newPassword);
        }

        #endregion

        public static void Block(BaseUser user, TimeSpan blockDuration)
        {
            _accountService.Block(user, blockDuration);
        }

    }
}
