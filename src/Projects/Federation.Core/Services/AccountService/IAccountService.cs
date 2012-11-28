using System;

namespace Federation.Core
{
    public interface IAccountService
    {
        #region Настройки

        int MinPasswordLength { get; }

        #endregion

        void RemoveOldPasswordRecoveries();
        void RemoveOldEmailVerifications();
        void RemoveOldSecretCodes();
        string GenerateSecretCode(Guid userId);
        bool VerifySecretCode(Guid id, string code);
        void SendEmailVerification(Guid userId);
        bool VerifyEmail(Guid userId, string code);
        DateTime? IsEmailVerificationAlreadySend(Guid userId);
        void DisconnectSocialAccount(User user, SocialType socialType);
        bool ConnectSocialAccount(User user, SocialType socialType, string link, string key);

        #region Идентификация

        /// <summary>
        /// Идентификация пользователя по логину/е-мэйлу и паролю
        /// </summary>
        bool TrySignIn(string userIdentificator, string password, bool createPersistentCookie, string ip);

        /// <summary>
        /// Идентификация пользователя через внешние идентифицирующие сервисы
        /// </summary>
        bool TrySignIn(string key, SocialType socialtype, bool createPersistentCookie, string ip);

        /// <summary>
        /// Регистрация пользователя по логину, электропочте и паролю
        /// </summary>
        BaseUser SignUp(string login, string email, string password, bool saveChanges);

        /// <summary>
        /// Выход из аккаунта пользователя
        /// </summary>
        void SignOut();

        #endregion

        #region Восстановление пароля

        /// <summary>
        /// Изменение пароля пользователя. Вызывается, когда известен пользователь. По сути со страницы пользователя
        /// </summary>
        void ChangePassword(BaseUser user, string oldPassword, string newPassword);

        /// <summary>
        /// Изменение пароля пользователся по секретному ключу, который до этого был доставлен пользователю. 
        /// Вызывается когда пользователь относительно неизвестен. Например при восстановлении пароля через почту
        /// </summary>
        void ChangePassword(Guid userKey, Guid securityKey, string newPassword);

        /// <summary>
        /// Инициировать систему возврата пароля. Например отсылки письма с секретной ссылкой на электропочту
        /// </summary>
        void RecoverPassword(Guid user);
        void RecoverPassword(Guid recoverKey, string newPassword);

        #endregion

        //Блокировка(бан) пользователя с указанием длительности
        void Block(BaseUser user, TimeSpan blockDuration);
    }
}
