using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Federation.Core.Services.MonitoringService;

namespace Federation.Core
{
    public class AccountServiceImpl : IAccountService
    {
        private static readonly ConcurrentDictionary<Guid, PasswordRecoverInfo> _PasswordRecoverRecords = new ConcurrentDictionary<Guid, PasswordRecoverInfo>();
        private static readonly ConcurrentDictionary<Guid, SecretInfo> _SecretCodes = new ConcurrentDictionary<Guid, SecretInfo>();
        private static readonly ConcurrentDictionary<Guid, SecretInfo> _EmailVerificationCodes = new ConcurrentDictionary<Guid, SecretInfo>();

        public int MinPasswordLength
        {
            get { return 6; }
        }

        public void DisconnectSocialAccount(User user, SocialType socialType)
        {
            if (user != null && socialType != SocialType.None)
            {
                var account = user.SocialAccounts.SingleOrDefault(x => x.SocialType == (byte)socialType);
                if (account == null)
                    throw new BusinessLogicException("Аккаунт " + socialType + " не прикреплен к вашему профилю");

                user.SocialAccounts.Remove(account);

                DataService.PerThread.SocialAccountSet.DeleteObject(account);
                DataService.PerThread.SaveChanges();
            }
        }

        public bool ConnectSocialAccount(User user, SocialType socialType, string link, string key)
        {
            if (!string.IsNullOrWhiteSpace(key) && user != null && socialType != SocialType.None)
            {
                if (user.SocialAccounts.Count(x => x.SocialType == (byte)socialType) > 0)
                    throw new BusinessLogicException("Аккаунт " + socialType + " уже прикреплен к вашему профилю");

                var socialAccount = new SocialAccount();
                socialAccount.Id = Guid.NewGuid();
                socialAccount.SocialId = key;
                socialAccount.SocialType = (byte)socialType;
                socialAccount.DirectUrl = link;
                user.SocialAccounts.Add(socialAccount);

                DataService.PerThread.SaveChanges();

                return true;
            }

            return false;
        }

        public string GenerateSecretCode(Guid userId)
        {
            var info = _SecretCodes.AddOrUpdate(userId, key =>
                {
                    return new SecretInfo
                    {
                        Code = new Random().Next(1000, 9999).ToString(CultureInfo.InvariantCulture),
                        Date = DateTime.Now
                    };
                },
                (key, oldValue) =>
                {
                    return new SecretInfo
                    {
                        Code = oldValue.Code,
                        Date = DateTime.Now
                    };
                });

            return info.Code;
        }

        public bool VerifySecretCode(Guid id, string code)
        {
            SecretInfo info;
            if (_SecretCodes.TryGetValue(id, out info))
            {
                if (code != info.Code)
                    return false;

                SecretInfo nothing;
                _SecretCodes.TryRemove(id, out nothing);

                return true;
            }

            return false;
        }

        public DateTime? IsEmailVerificationAlreadySend(Guid userId)
        {
            SecretInfo info;
            if (_EmailVerificationCodes.TryGetValue(userId, out info))
                return info.Date;

            return null;
        }

        public void SendEmailVerification(Guid userId)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            var info = _EmailVerificationCodes.AddOrUpdate(userId, key =>
                {
                    return new SecretInfo
                    {
                        Code = Guid.NewGuid().ToString(),
                        Date = DateTime.Now
                    };
                },
                (key, oldValue) =>
                {
                    return new SecretInfo
                    {
                        Code = oldValue.Code,
                        Date = DateTime.Now
                    };
                });

            _SendEmailVerificationMessage(userId, user.SubscriptionSettings.SubscriptionEmail, user.FullName, info.Code);
        }

        public bool VerifyEmail(Guid userId, string code)
        {
            SecretInfo info;
            if (_EmailVerificationCodes.TryGetValue(userId, out info))
            {
                if (code != info.Code)
                    return false;

                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
                if (user == null)
                    throw new BusinessLogicException("Пользователь не найден");

                user.IsEmailVerified = true;
                user.SubscriptionSettings.IsSubscribed = true;

                DataService.PerThread.SaveChanges();

                SecretInfo nothing;
                _SecretCodes.TryRemove(userId, out nothing);

                return true;
            }

            return false;
        }

        public void RemoveOldEmailVerifications()
        {
            var oldEmailKeys = _EmailVerificationCodes.Where(x => x.Value.Date < DateTime.Now.AddDays(-2)).Select(x => x.Key);
            foreach (var key in oldEmailKeys)
            {
                SecretInfo uslessInfo;
                _EmailVerificationCodes.TryRemove(key, out uslessInfo);
            }
        }

        private void _SendEmailVerificationMessage(Guid userId, string userEmail, string userName, string code)
        {
            var securityLink = "http://democratia2.ru/Account/EmailVerification/" + userId.ToString() + "?key=" + code;

            var message = new MailMessage();
            message.From = new MailAddress("noreply@democratia2.ru");
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Подтверждение почты на Демократия2";
            message.Body = "<p><b>Добрый день!</b></p>" +
                           "<p>Для подтверждения почты на сайте <a href='http://democratia2.ru'>Демократия2</a> пройдите по следующей ссылке (или скопируйте ее в адресную строку вашего браузера) <a href='" + securityLink + "'>" + securityLink + "</a></p>";
            message.IsBodyHtml = true;

            var smtp = new SmtpClient();
            smtp.Send(message);
            smtp.Dispose();
            message.Dispose();
        }

        public void RemoveOldSecretCodes()
        {
            var oldCodesKeys = _SecretCodes.Where(x => x.Value.Date < DateTime.Now.AddDays(-2)).Select(x => x.Key);
            foreach (var codeKey in oldCodesKeys)
            {
                SecretInfo uslessInfo;
                _SecretCodes.TryRemove(codeKey, out uslessInfo);
            }
        }

        public bool TrySignIn(string key, SocialType socialtype, bool createPersistentCookie, string ip)
        {
            var socialAccount = DataService.PerThread.SocialAccountSet.SingleOrDefault(x => x.SocialId == key);
            if (socialAccount != null)
            {
                var user = socialAccount.User;
                if (user.BlockedUserRecord != null)
                {
                    MonitoringService.AsynchLogUserAuthentification(user.Id.ToString(), ip, false);
                    throw new BusinessLogicException("Вы были забанены! Причина: " + user.BlockedUserRecord.Reason);
                }

                MonitoringService.AsynchLogUserAuthentification(user.Id.ToString(), ip, true);

                AuthenticationService.SignIn(user.Id.ToString(), createPersistentCookie);
                return true;
            }

            MonitoringService.AsynchLogUserAuthentification(null, ip, false);
            return false;
        }

        public bool TrySignIn(string userIdentificator, string password, bool createPersistentCookie, string ip)
        {
            if (string.IsNullOrWhiteSpace(userIdentificator) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException();

            if (ValidationHelper.IsCorrectEmail(userIdentificator))
            {
                var email = userIdentificator;
                var user = SecurityService.EmailUserValidation(email, password);

                if (user == null)
                    return false;

                if (user.BlockedUserRecord != null)
                {
                    MonitoringService.AsynchLogUserAuthentification(user.Id.ToString(), ip, false);
                    throw new BusinessLogicException("Вы были забанены! Причина: " + user.BlockedUserRecord.Reason);
                }

                MonitoringService.AsynchLogUserAuthentification(user.Id.ToString(), ip, true);

                AuthenticationService.SignIn(user.Id.ToString(), createPersistentCookie);

                return true;
            }

            if (ValidationHelper.IsCorrectLogin(userIdentificator))
            {
                var login = userIdentificator;
                var user = SecurityService.LoginUserValidation(login, password);

                if (user == null)
                    return false;

                MonitoringService.AsynchLogUserAuthentification(user.Id.ToString(), ip, true);

                AuthenticationService.SignIn(user.Id.ToString(), createPersistentCookie);

                return true;
            }

            MonitoringService.AsynchLogUserAuthentification(null, ip, false);

            throw new ArgumentException();
        }

        public BaseUser SignUp(string login, string email, string password, bool saveChanges)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException();

            Guid hash, salt;
            SecurityService.GeneratePasswordHash(password, out hash, out salt);

            var federation = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == ConstHelper.RootGroupId);
            if (federation == null)
                throw new BusinessLogicException("Не создана группа Федерация");

            var encryptedEmail = CryptographyService.EncryptEmail(email);
            var emailUser = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.EncryptedEmail == encryptedEmail);
            if (emailUser != null)
                throw new BusinessLogicException("Указанная почта уже используется");

            var user = new User // TODO: Регистрация админов должна идти отдельной функцией
                {
                    Login = login,
                    Email = email,
                    Password = hash,
                    Salt = salt,
                    IsVerified = false,
                    RegistrationDate = DateTime.Now,
                    LiveJournalSindication = true,
                    LiveJournalSindicateAsDraft = false,
                    LastActivity = DateTime.Now
                };

            user.SubscriptionSettings = new SubscriptionSettings { SubscriptionEmail = email };

            var gm = new GroupMember
            {
                EntryDate = DateTime.Now,
                GroupId = federation.Id,
                State = (byte)GroupMemberState.Approved,
                UserId = user.Id
            };

            DataService.PerThread.GroupMemberSet.AddObject(gm);
            SubscriptionService.SubscribeToGroup(federation, user);
            VotingService.AnalizeGroupMemberBulletins(gm.Id);

            return user;
        }

        public void SignOut()
        {
            AuthenticationService.SignOut();
        }

        public void ChangePassword(BaseUser baseUser, string oldPassword, string newPassword)
        {
            if (baseUser == null)
                throw new BusinessLogicException("Пользователь пустой!");

            if (!(baseUser is User))
                throw new BusinessLogicException("Вы кому пароль меняете?!");

            var user = baseUser as User;
            var checkedUser = SecurityService.EmailUserValidation(user.Email, oldPassword);
            if (checkedUser == null)
                throw new ValidationException("Введен неправильный пароль!");

            Guid hash, salt;
            SecurityService.GeneratePasswordHash(newPassword, out hash, out salt);
            user.Password = hash;
            user.Salt = salt;
            DataService.PerThread.SaveChanges();
        }

        public void ChangePassword(Guid userKey, Guid securityKey, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void RecoverPassword(Guid userId)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            var recoverKey = Guid.NewGuid();
            var recoverInfo = _PasswordRecoverRecords.AddOrUpdate(recoverKey, key =>
                {
                    return new PasswordRecoverInfo()
                    {
                        UserId = user.Id,
                        UserName = user.FullName,
                        Email = user.Email,
                        CreateDate = DateTime.Now,
                        RecoveryKey = recoverKey
                    };
                },
                (key, oldValue) =>
                {
                    return new PasswordRecoverInfo()
                    {
                        UserId = oldValue.UserId,
                        UserName = oldValue.UserName,
                        Email = oldValue.Email,
                        CreateDate = DateTime.Now,
                        RecoveryKey = oldValue.RecoveryKey
                    };
                });
            _SendRecoverMessage(recoverInfo);
        }

        public void RecoverPassword(Guid recoverKey, string newPassword)
        {
            PasswordRecoverInfo recoverInfo;
            if (_PasswordRecoverRecords.TryGetValue(recoverKey, out recoverInfo))
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == recoverInfo.UserId);
                if (user == null)
                    throw new BusinessLogicException("Ошибка при восстановлении пароля! Указанный пользователь не найден! Попробуйте еще раз, при повторном возникновении данной ошибки обратитесь к администратору");

                Guid hash, salt;
                SecurityService.GeneratePasswordHash(newPassword, out hash, out salt);
                user.Password = hash;
                user.Salt = salt;
                DataService.PerThread.SaveChanges();
            }
            else
                throw new ValidationException("Указанный ключ не верен или устарел!");
        }

        private void _SendRecoverMessage(PasswordRecoverInfo recoverInfo)
        {
            var securityLink = "http://democratia2.ru/Account/SetNewPassword?key=" + recoverInfo.RecoveryKey;

            MailMessage message = new MailMessage();
            message.From = new MailAddress("support@democratia2.ru");
            message.To.Add(new MailAddress(recoverInfo.Email));
            message.Subject = "Восстановление пароля на Демократия2";
            message.Body = "<p><b>Добрый день!</b></p>" +
                           "<p>Для восстановления пароля на сайте <a href='http://democratia2.ru'>Демократия2</a> пройдите по следующей ссылке (или скопируйте ее в адресную строку вашего браузера) <a href='" + securityLink + "'>" + securityLink + "</a></p>" +
                           "<p>" + recoverInfo.UserName + ", если вы не запрашивали востановление пароля, то просто проигнорируйте данное письмо.</p>";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(message);
            smtp.Dispose();
            message.Dispose();
        }

        public void RemoveOldPasswordRecoveries()
        {
            var oldPasswordRecordKeys = _PasswordRecoverRecords.Where(x => x.Value.CreateDate < DateTime.Now.AddDays(-7)).Select(x => x.Key);
            foreach (var codeKey in oldPasswordRecordKeys)
            {
                SecretInfo uslessInfo;
                _SecretCodes.TryRemove(codeKey, out uslessInfo);
            }
        }

        public void Block(BaseUser user, TimeSpan blockDuration)
        {
            throw new NotImplementedException();
        }

        public BaseUser GetBaseUser(Guid baseUserId)
        {
            throw new NotImplementedException();
        }

        public BaseUser GetBaseUserByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
