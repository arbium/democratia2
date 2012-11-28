using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Federation.Core;
using Federation.Web.Services;
using Federation.Web.ViewModels;
using Federation.Core.Services.MonitoringService;

namespace Federation.Web.Controllers
{
    public class AccountController : MainController
    {
        public ActionResult Activation()
        {
            if (UserContext.Current != null && Request.IsAuthenticated)
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
                if (user == null)
                    throw new BusinessLogicException("Возникла ошибка. Система не знает такого пользователя.");

                var justRegistered = user.RegistrationDate > (DateTime.Now - ConstHelper.UserOutdatedTime);

                var phoneNubmer = Regex.Replace(user.PhoneNumber, @"\D*", string.Empty); 

                var model = new AccountActivationViewModel
                {
                    Id = user.Id,
                    AccountMail = user.Email,
                    SubscribtionMail = user.SubscriptionSettings.SubscriptionEmail,
                    Phone = phoneNubmer,
                    ShowCode = false,
                    JustRegistered = justRegistered
                };

                return View(model);
            }

            return RedirectToAction("signin", "account", null);
        }

        [HttpPost]
        public ActionResult Activation(AccountActivationViewModel model)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("signin", "account", null);

            if (ModelState.IsValid)
            {
                if (model.CodeId == Guid.Empty || string.IsNullOrWhiteSpace(model.Code))
                {
                    var phone = UserService.NormalizePhoneNumber(model.Phone);

                    var encryptedPhoneNumber = CryptographyService.EncryptPhone(phone);
                    var usersWithSamePhone = DataService.PerThread.BaseUserSet.OfType<User>().Count(u => u.EncryptedPhoneNumber == encryptedPhoneNumber && (u.Id != model.Id));
                    if (usersWithSamePhone != 0)
                        throw new ValidationException("Пользователь с таким номером телефона уже активирован");  

                    var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
                    if (user == null)
                        throw new BusinessLogicException("Неверный идентификатор пользователя!");

                    user.PhoneNumber = phone;
                    user.SubscriptionSettings.SubscriptionEmail = model.SubscribtionMail;

                    UserService.NormalizePhoneNumber(user);                                      

                    ModelState.Clear();

                    model.CodeId = user.Id;
                    model.ShowCode = true;

                    var code = AccountService.GenerateSecretCode(user.Id);
                    var sms = "Ваш секретный код для верификации на Демократии2: " + code;

                    if (UserContext.Current.SentSmsCount >= 5)
                        throw new BusinessLogicException("Лимит ваших смс исчерпан. Обратитесь в техподдержку");

                    SmsService.SendSms(user.PhoneNumber, sms);
                    UserContext.Current.SentSmsCount++;
                }
                else
                {
                    if (!AccountService.VerifySecretCode(model.CodeId, model.Code))
                        throw new ValidationException("Введен неверный код!");

                    if (UserContext.Current.Id != model.Id)
                        throw new BusinessLogicException("Возникла ошибка. Ключ пользователя начала и завершения активации аккаунта не совпадают.");

                    var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
                    if (user == null)
                        throw new BusinessLogicException("Возникла ошибка. Система не знает такого пользователя.");

                    user.IsOutdated = false;
                    user.IsPhoneVerified = true;

                    UserContext.Abandon();

                    return RedirectToAction("profile", "user", null);
                }
            }

            return View(model);
        }

        public ActionResult TicketVerification()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new Exception("Перезайдите");

            return View(new AccountTicketVerificationViewModel(user));
        }

        [HttpPost]
        public ActionResult TicketVerification(AccountTicketVerificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new Exception("Перезайдите");

            if (!ApiService.CheckPassword(model.TicketCode, model.TempPass))
                throw new ValidationException("Неверный одноразовый пароль");

            user.IsTicketVerified = true;
            DataService.PerThread.SaveChanges();

            BadgeService.AwardUser<TicketVerifiedBadge>(user.Id);

            return RedirectToAction("profile", "user");
        }

        public object SendPassForTicket(string code, string phone)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new Exception("Перезайдите");
            
            try
            {
                ApiService.D2SendPassword(code, phone, user);
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return null;
        }

        public ActionResult CheckPaypal()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            return View();
        }

        public ActionResult SocialDeattach(SocialType social)
        {
            if (Request.IsAuthenticated && UserContext.Current != null)
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
                if (user == null)
                    throw new BusinessLogicException("Данный пользователь не найден");

                AccountService.DisconnectSocialAccount(user, social);
                return RedirectToAction("profile", "user", null);
            }

            throw new AuthenticationException();
        }

        public ActionResult FacebookSignIn(string state, string code)
        {
            string userDataJson;

            if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(code))
                throw new BusinessLogicException("Facebook не подтвердил вашу авторизацию");

            var sessionState = HttpContext.Session["state"];
            if (string.IsNullOrEmpty(state) || sessionState == null || sessionState.ToString() != state)
                throw new BusinessLogicException("Facebook не подтвердил вашу авторизацию");

            var returnUrl = HttpContext.Session["SocialSignIn_ReturnUrl"] == null ? string.Empty : HttpContext.Session["SocialSignIn_ReturnUrl"].ToString();

            try
            {
                //Сервер-Фейсбук авторизация пользователя
                var token_url = "https://graph.facebook.com/oauth/access_token?client_id=301981299827190&client_secret=ef57eaec8473b59fcd2a7177c7f5652b" + "&code=" + code + "&redirect_uri=" + Url.Action("facebooksignin", "account", null, "http");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(token_url);
                request.Method = "GET";
                var a = request.GetResponse();
                var stream = a.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                var match = Regex.Match(responseFromServer, @"(?<=access_token=)\w+(?=&|\n|$)");

                if (!match.Success)
                    throw new Exception("не валидный токен");

                var access_token = match.Value;

                //Сервер-Фейсбук получение данных о пользователе
                var graph_url = "https://graph.facebook.com/me?access_token=" + access_token;
                request = (HttpWebRequest)HttpWebRequest.Create(graph_url);
                request.Method = "GET";
                a = request.GetResponse();
                stream = a.GetResponseStream();
                reader = new StreamReader(stream);
                userDataJson = reader.ReadToEnd();
                reader.Close();
            }
            catch (WebException exp)
            {
                var resultStream = exp.Response.GetResponseStream();
                StreamReader reader = new StreamReader(resultStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();

                throw new BusinessLogicException("Сервис авторизации Facebook не отвечает");
            }
            catch (Exception exp)
            {
                throw new BusinessLogicException("Произошла внутренняя ошибка, авторизация через Facebook не доступна.");
            }

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic userData = serializer.Deserialize(userDataJson, typeof(object));

            if (UserContext.Current != null && Request.IsAuthenticated)//Пользователь уже был залогинен, пытаемся прикрепить аккаунт фейсбука к нему
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);

                if (user == null)
                    throw new BusinessLogicException("Ваш профиль не найден в базе данных.");

                if (user.SocialAccounts.Count(x => x.SocialType == (byte)SocialType.Facebook) == 0)
                {
                    AccountService.ConnectSocialAccount(user, SocialType.Facebook, userData.link, userData.id);
                }
                return RedirectToAction("profile", "user", null);
            }

            if (AccountService.TrySignIn(userData.id, SocialType.Facebook, true, HttpContext.Request.UserHostAddress))//Пытаемся войти с таким аккаунтом Фейсбук
            {
                if (string.IsNullOrWhiteSpace(returnUrl))
                    return RedirectToAction("index", "user", null);

                return Redirect(returnUrl);
            }

            //Регистрируем пользователя в системе
            HttpContext.Session.Add("social_id", userData.id);
            HttpContext.Session.Add("social_type", SocialType.Facebook);
            HttpContext.Session.Add("social_link", userData.link);

            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("signup", "account", new { social = SocialType.Facebook, returnUrl = returnUrl });
            else
                return Redirect(returnUrl);
        }

        public ActionResult SocialSignIn(AccountSocialSignIn model)
        {

            return View(model);
        }

        [RequireHttps]
        public ActionResult SignIn(string returnUrl = null)
        {
            return View(new SignInViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (AccountService.TrySignIn(model.UserIdentificator, model.Password, model.IsPermanent, HttpContext.Request.UserHostAddress))
                {
                    if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                        return RedirectToAction("index", "user", null);

                    MonitoringService.AsynchLogUserAuthentification(null, HttpContext.Request.UserHostAddress, true);

                    return Redirect(model.ReturnUrl);
                }

                MonitoringService.AsynchLogUserAuthentification(null, HttpContext.Request.UserHostAddress, false);
                
                throw new ValidationException("Неверный логин/пароль");
            }

            return View(model);
        }

        [RequireHttps]
        public ActionResult SignUp(bool? isdialog, SocialType social = SocialType.None, string returnUrl = null)
        {
            var model = new AccountSignUpViewModel
            {
                SocialType = social,
                ConnectSocial = true,
                ReturnUrl = returnUrl
            };

            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("SignUp", "_LightLayout", model);


            return View(model);
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult SignUp(AccountSignUpViewModel model, bool? isdialog)
        {
            if (ModelState.IsValid)
            {
                var phone = UserService.NormalizePhoneNumber(model.PhoneNumber);

                var encryptedPhoneNumber = CryptographyService.EncryptPhone(phone);
                var usersWithSamePhone = DataService.PerThread.BaseUserSet.OfType<User>().Count(u => u.EncryptedPhoneNumber == encryptedPhoneNumber);
                if (usersWithSamePhone != 0)
                    throw new ValidationException("Пользователь с таким номером телефона уже зарегистрирован");

                var encryptedEmail = CryptographyService.EncryptEmail(model.Email);
                var usersWithSameEmail = DataService.PerThread.BaseUserSet.OfType<User>().Count(u => u.EncryptedEmail == encryptedEmail);
                if (usersWithSameEmail != 0)
                    throw new ValidationException("Пользователь с такой электропочтой уже зарегистрирован");

                var user = (User)AccountService.SignUp(model.Email, model.Email, model.Password, false);
                user.FirstName = model.Name;
                user.SurName = model.SurName;
                user.Patronymic = model.Patronymic;
                user.PhoneNumber = phone;
                user.Sex = ValueAnalizer.GetGenderFromString(model.Gender);
                user.IsPhoneVerified = false;
                user.IsEmailVerified = false;
                user.IsOutdated = true;

                UserService.NormalizePhoneNumber(user);

                if (model.ConnectSocial && HttpContext.Session["social_type"] != null && HttpContext.Session["social_id"] != null && HttpContext.Session["social_link"] != null)
                {
                    var socialType = (SocialType)HttpContext.Session["social_type"];
                    var socialLink = HttpContext.Session["social_link"].ToString();
                    var socialKey = HttpContext.Session["social_id"].ToString();

                    var usersWithSameSocial = DataService.PerThread.SocialAccountSet.Count(u => u.SocialId == socialKey && u.SocialType == (byte)socialType);
                    if (usersWithSameSocial != 0)
                        throw new ValidationException("Пользователь с таким аккаунтом " + model.SocialType.ToString() + " уже зарегистрирован");

                    AccountService.ConnectSocialAccount(user, socialType, socialLink, socialKey);
                    HttpContext.Session.Remove("social_id");
                    HttpContext.Session.Remove("social_type");
                    HttpContext.Session.Remove("social_link");
                }

                DataService.PerThread.SaveChanges();

                //var code = AccountService.GenerateSecretCode(user.Id);
                //var sms = "Ваш секретный код для верификации на Демократии2: " + code;
                //SmsService.SendMessage("+7" + telephone, sms);

                //var smtp = new SmtpClient();
                //var link = ConstHelper.AppUrl + Url.Action("codeVerification", new { id = user.Id });
                //var message = new MailMessage
                //{
                //    Subject = "Ввод кода подтверждения на Демократии2",
                //    Body = "<p>Если вы еще не ввели код подтверждения, пришедший вам по смс, перейдите по ссылке: <a href='" + link + "'>" + link + "</a>.</p>" +
                //        "<p>Если вам не пришло смс, напишите на <a href='mailto:support@democratia2.ru'>support@democratia2.ru</a></p>",
                //    IsBodyHtml = true
                //};

                //message.To.Add(user.Email);

                //smtp.Send(message);
                //smtp.Dispose();

                AccountService.SendEmailVerification(user.Id);
                AccountService.TrySignIn(model.Email, model.Password, true, HttpContext.Request.UserHostAddress);

                MonitoringService.AsynchLogUserRegistration(user.Id, HttpContext.Request.UserHostAddress);

                if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                    return RedirectToAction("index", "user");

                return Redirect(model.ReturnUrl);
            }

            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("SignUp", "_LightLayout", model);

            return View(model);
        }

        public ActionResult EmailVerification(Guid id, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return RedirectToAction("index", "home", null);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден!");

            if (AccountService.VerifyEmail(id, key))
                ViewBag.Message = "Вы успешно подтвердили почту!";
            else
                ViewBag.Message = "Возникла ошибка! Попробуйте повторно запросить подтверждение почты";

            return View();
        }

        public ActionResult SignUpCvk(string key)
        {
            if (key == null)
                throw new BusinessLogicException("Вы не можете зарегистрироваться, так как вы не указали код приглашения");

            key = key.Replace("`", "");
            var inviteEntity = InviteService.GetInvite(key);

            if (inviteEntity == null)
                throw new BusinessLogicException("Вы не можете зарегистрироваться, так как вы указали неправильный код приглашения");

            if (inviteEntity.State == (byte)InviteState.Requested)
                throw new BusinessLogicException("Вы не можете зарегистрироваться, так как ваш запрос еще не рассмотрели");

            if (inviteEntity.State == (byte)InviteState.Used)
                throw new BusinessLogicException("Вы не можете  зарегистрироваться, так как данный ключ уже был использован");
            
            return View("signupcvk", new SignUpCvkViewModel
                                         {
                                             Email = inviteEntity.Email,
                                             InviteKey = key,
                                             PhoneNumber = inviteEntity.Phone,
                                         });
        }

        [HttpPost]
        public ActionResult SignUpCvk(SignUpCvkViewModel model)
        {
            if (ModelState.IsValid)
            {
                string key = string.IsNullOrWhiteSpace(model.InviteKey) ? null : model.InviteKey.Trim().ToLower();

                if (key != string.Empty)
                {
                    string email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim().ToLower();

                    key = key.Replace("`", "");
                    var inviteEntity = InviteService.GetInvite(key);

                    if (inviteEntity == null)
                        throw new BusinessLogicException("Вы не можете зарегистрироваться, так как вы указали неправильный код приглашения");

                    if (inviteEntity.State == (byte)InviteState.Requested)
                        throw new BusinessLogicException("Вы не можете зарегистрироваться, так как ваш запрос еще не рассмотрели");

                    if (inviteEntity.State == (byte)InviteState.Used)
                        throw new BusinessLogicException("Вы не можете  зарегистрироваться, так как данный ключ уже был использован");

                    if (inviteEntity.State != (byte)InviteState.Used || inviteEntity.State != (byte)InviteState.Requested)
                    {
                        var encryptedPhoneNumber = CryptographyService.DecryptPhone(model.PhoneNumber);
                        var existedUsersCount = DataService.PerThread.BaseUserSet.OfType<User>().Count(u => u.EncryptedPhoneNumber == encryptedPhoneNumber);

                        if (existedUsersCount != 0)
                            throw new ValidationException("Пользователь с таким номером телефона уже зарегистрирован");

                        var user = (User)AccountService.SignUp(email, email, model.Password);
                        user.FirstName = inviteEntity.Name;
                        user.SurName = inviteEntity.Surname;
                        user.Patronymic = inviteEntity.Patronymic;
                        user.InviteTicket = inviteEntity;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Info = inviteEntity.UserInfo;
                        user.UTCOffset = (short)TimeSpan.Parse(model.UTCOffset).TotalMinutes;
                        user.IsEmailVerified = true;
                        user.SubscriptionSettings.IsSubscribed = true;

                        inviteEntity.State = (byte)InviteState.Used;

                        DataService.PerThread.SaveChanges();

                        GroupService.AddMember(user.Id, new Guid(ConstHelper.KsGroupId), true);
                        GroupService.ChangeMemberStatus(user.Id, new Guid(ConstHelper.KsGroupId), GroupMemberState.Approved);
                        DataService.PerThread.SaveChanges();

                        AccountService.TrySignIn(email, model.Password, true, HttpContext.Request.UserHostAddress);

                        MonitoringService.AsynchLogUserRegistration(user.Id, HttpContext.Request.UserHostAddress);

                        return RedirectToAction("index", "user", null);
                    }

                }
            }
            return View(model);
        }

        public ActionResult SignUpWithKey(bool? isdialog)
        {
            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("SignUpWithKey", "_LightLayout");
 
            return View();
        }

        [HttpPost]
        public ActionResult SignUpWithKey(InviteSignUpWithKeyViewModel model, bool? isdialog)
        {
            if (ModelState.IsValid)
            {
                string key = string.IsNullOrWhiteSpace(model.InviteKey) ? null : model.InviteKey.Trim().ToLower();

                if (key != string.Empty)
                {
                    string email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim().ToLower();

                    key = key.Replace("`", "");
                    var inviteEntity = InviteService.GetInvite(key);

                    if (inviteEntity == null)
                        throw new BusinessLogicException("Вы не можете зарегистрироваться, так как вы указали неправильный код приглашения");

                    if (inviteEntity.State == (byte)InviteState.Requested)
                        throw new BusinessLogicException("Вы не можете зарегистрироваться, так как ваш запрос еще не рассмотрели");

                    if (inviteEntity.State == (byte)InviteState.Used)
                        throw new BusinessLogicException("Вы не можете  зарегистрироваться, так как данный ключ уже был использован");

                    if (inviteEntity.State != (byte)InviteState.Used || inviteEntity.State != (byte)InviteState.Requested)
                    {
                        var phone = UserService.NormalizePhoneNumber(model.PhoneNumber);

                        var encryptedPhoneNumber = CryptographyService.DecryptPhone(phone);
                        var existedUsersCount = DataService.PerThread.BaseUserSet.OfType<User>().Count(u => u.EncryptedPhoneNumber == encryptedPhoneNumber);
                        if (existedUsersCount != 0)
                            throw new ValidationException("Пользователь с таким номером телефона уже зарегистрирован");

                        var user = (User)AccountService.SignUp(email, email, model.Password);
                        user.FirstName = inviteEntity.Name;
                        user.SurName = inviteEntity.Surname;
                        user.Patronymic = inviteEntity.Patronymic;
                        user.InviteTicket = inviteEntity;
                        user.PhoneNumber = phone;
                        user.Info = inviteEntity.UserInfo;
                        user.UTCOffset = (short)TimeSpan.Parse(model.UTCOffset).TotalMinutes;

                        UserService.NormalizePhoneNumber(user);

                        inviteEntity.State = (byte)InviteState.Used;

                        DataService.PerThread.SaveChanges();
                        AccountService.TrySignIn(email, model.Password, true, HttpContext.Request.UserHostAddress);

                        MonitoringService.AsynchLogUserRegistration(user.Id, HttpContext.Request.UserHostAddress);

                        return RedirectToAction("index", "user", null);
                    }

                }
            }

            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("SignUpWithKey", "_LightLayout", model);

            return View(model);
        }

        public ActionResult SignOut()
        {
            AccountService.SignOut();

            return RedirectToAction("index", "home", null);
        }

        public ActionResult InviteRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InviteRequest(InviteRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var inviteData = new InviteDataStruct()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Patronymic = model.Patronymic,
                    Email = model.Email,
                    FacebookId = model.FacebookUrl,
                    LiveJournalId = model.LiveJournalUrl,
                    State = InviteState.Requested,
                    UserInfo = model.UserInfo
                };

                InviteService.CreateInvite(inviteData);

                //todo - сделать вьюху для сообщения "вы успешно послали реквест"
                return RedirectToAction("index", "home", null);
            }

            return View(model);
        }

        public ActionResult BlockedInvites()
        {
            var model = new AccountBlockedInvitesViewModel(DataService.PerThread.InviteSet.Where(i => i.State == (byte)InviteState.Blocked).ToList());
            return View(model);
        }

        public ActionResult RequestedInvites()
        {
            var model = new AccountRequestedInvitesViewModel(DataService.PerThread.InviteSet.Where(i => i.State == (byte)InviteState.Requested).ToList());
            return View(model);
        }

        public ActionResult RecoverPassword()
        {
            return View(new AccountRecoverPasswordViewModel());
        }

        [HttpPost]
        public ActionResult RecoverPassword(AccountRecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var encyptedEmail = CryptographyService.EncryptEmail(model.Email);
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.EncryptedEmail == encyptedEmail);
            if (user == null)
                throw new ValidationException("Пользователь с указанным адресом электропочты не найден!");

            AccountService.RecoverPassword(user.Id);

            return View("Result", (object)"Письмо с инструкцией по восстановлению пароля было выслано Вам на почту");
        }

        public ActionResult SetNewPassword(string key)
        {
            Guid recoveryKey;
            if (!Guid.TryParse(key, out recoveryKey))
                throw new MvcActionRedirectException("", "home", "index");//TODO

            return View(new AccountSetNewPasswordViewModel { RecoverKey = recoveryKey });
        }

        [HttpPost]
        public ActionResult SetNewPassword(AccountSetNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            AccountService.RecoverPassword(model.RecoverKey, model.NewPassword);

            return View("Result", (object)"Вы успешно сменили Ваш пароль! Попробуйте войти, используя новый пароль.");
        }

        public ActionResult ChangePassword()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("index", "home", null);

            return View(new AccountChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(AccountChangePasswordViewModel model)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("index", "home", null);

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден!");

            AccountService.ChangePassword(user, model.OldPassword, model.NewPassword);

            return View("Result", (object)"Вы успешно сменили Ваш пароль!");
        }
    }
}
