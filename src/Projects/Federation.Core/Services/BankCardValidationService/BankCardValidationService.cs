using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using PayPal.Platform.SDK;
using PayPal.Services.Private.AA;
using AdaptiveAccounts = PayPal.Platform.SDK.AdaptiveAccounts;


namespace Federation.Core.Services
{
    public static class BankCardValidationService
    {
        private static BaseAPIProfile profile;

        private static bool HaveEmail(string email)
        {
            return DataService.PerThread.PayPalVerificationSet.Any(x => x.Email == email);
        }

        public static bool Validate(string email, string firstName, string lastName, User user)
        {
            GetVerifiedStatusResponse getVerifiedStatusResponse = null;

            if (HaveEmail(email))
                throw new ValidationException("На данную электропочту PayPal уже зарегестирован на Д2 пользователь");

            try
            {

                if (profile == null)
                    profile = CreateProfile();

                var getVerifiedStatusRequest = new GetVerifiedStatusRequest();

                getVerifiedStatusRequest.emailAddress = email;
                getVerifiedStatusRequest.firstName = firstName;
                getVerifiedStatusRequest.lastName = lastName;
                getVerifiedStatusRequest.matchCriteria = "NAME"; // or optional! (name/none)
                var aa = new AdaptiveAccounts();
                aa.APIProfile = profile;
                getVerifiedStatusResponse = aa.GetVerifiedStatus(getVerifiedStatusRequest);
            }
            catch (FATALException FATALEx)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", FATALEx);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", ex);
            }

            if (getVerifiedStatusResponse == null)
                return false;

            if (getVerifiedStatusResponse.accountStatus.ToUpper() == "VERIFIED")
            {
                DataService.PerThread.PayPalVerificationSet.AddObject(new PayPalVerification()
                                                                          {
                                                                              Email = email,
                                                                              FirstName = firstName,
                                                                              LastName = lastName,
                                                                              VerificationDate = DateTime.Now,
                                                                              User = user
                                                                          });
                DataService.PerThread.SaveChanges();
                return true;
            }
            
            return false;
        }

        public static bool ValidateTest(string email, string firstName, string lastName)
        {
            GetVerifiedStatusResponse getVerifiedStatusResponse = null;

            try
            {

                if (profile == null)
                    profile = CreateProfile();

                var getVerifiedStatusRequest = new GetVerifiedStatusRequest();

                getVerifiedStatusRequest.emailAddress = email;
                getVerifiedStatusRequest.firstName = firstName;
                getVerifiedStatusRequest.lastName = lastName;
                getVerifiedStatusRequest.matchCriteria = "NAME"; // or optional! (name/none)
                var aa = new AdaptiveAccounts();
                aa.APIProfile = profile;
                getVerifiedStatusResponse = aa.GetVerifiedStatus(getVerifiedStatusRequest);
            }
            catch (FATALException FATALEx)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", FATALEx);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", ex);
            }

            if (getVerifiedStatusResponse == null)
                return false;

            if (getVerifiedStatusResponse.accountStatus.ToUpper() == "VERIFIED")
            {
                return true;
            }

            return false;
        }

        private static BaseAPIProfile CreateProfile()
        {
            BaseAPIProfile profile = null;
            byte[] bCert = null;
            string filePath = string.Empty;
            FileStream fs = null;

            try
            {
                if (ConfigurationManager.AppSettings["API_AUTHENTICATION_MODE"] == "3TOKEN")
                {
                    ////Three token 
                    profile = new BaseAPIProfile();
                    profile.APIProfileType = ProfileType.ThreeToken;
                    profile.ApplicationID = ConfigurationManager.AppSettings["APPLICATION-ID"];
                    profile.APIUsername = ConfigurationManager.AppSettings["API_USERNAME"];
                    profile.APIPassword = ConfigurationManager.AppSettings["API_PASSWORD"];
                    profile.APISignature = ConfigurationManager.AppSettings["API_SIGNATURE"];
                    profile.DeviceIpAddress = ConfigurationManager.AppSettings["ipAddress"];
                    profile.Environment = ConfigurationManager.AppSettings["ENDPOINT"];
                    profile.RequestDataformat = ConfigurationManager.AppSettings["API_REQUESTFORMAT"];
                    profile.ResponseDataformat = ConfigurationManager.AppSettings["API_RESPONSEFORMAT"];
                    
                    profile.IsTrustAllCertificates = Convert.ToBoolean(ConfigurationManager.AppSettings["TrustAll"]);
                }
                else
                {
                    ////Certificate
                    profile = new BaseAPIProfile();
                    profile.APIProfileType = ProfileType.Certificate;
                    profile.APIUsername = ConfigurationManager.AppSettings["API_USERNAME"];
                    profile.APIPassword = ConfigurationManager.AppSettings["API_PASSWORD"];
                    profile.ApplicationID = ConfigurationManager.AppSettings["APPLICATION-ID"];
                    profile.RequestDataformat = ConfigurationManager.AppSettings["API_REQUESTFORMAT"];
                    profile.ResponseDataformat = ConfigurationManager.AppSettings["API_RESPONSEFORMAT"];
                    profile.IsTrustAllCertificates = Convert.ToBoolean(ConfigurationManager.AppSettings["TrustAll"]);
                    ///loading the certificate file into profile.           
                    filePath =  ConstHelper.AppPath + ConfigurationManager.AppSettings["CERTIFICATE"].ToString();
                    fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    bCert = new byte[fs.Length];
                    fs.Read(bCert, 0, int.Parse(fs.Length.ToString()));
                    fs.Close();
                    
                    profile.Certificate = bCert;
                    profile.PrivateKeyPassword = ConfigurationManager.AppSettings["PRIVATE_KEY_PASSWORD"];
                    profile.APISignature = "";
                    profile.Environment = ConfigurationManager.AppSettings["ENDPOINT"];
                }

            }
            catch (FATALException FATALEx)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", FATALEx);
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException("Ошибка в модуле работы с PayPal", ex);
            }
            
            return profile;
        }
    }
}
