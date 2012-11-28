namespace Federation.Core
{
    public partial class User
    {
        private string _email;
        private string _phoneNumber;

        public string FullName
        {
            get
            {
                var username = string.Empty;

                if (!string.IsNullOrEmpty(SurName))
                    username += SurName + " ";
                if (!string.IsNullOrEmpty(FirstName))
                    username += FirstName + " ";
                if (!string.IsNullOrEmpty(Patronymic))
                    username += Patronymic;

                if (string.IsNullOrWhiteSpace(username))
                    username = Id.ToString();

                return username;
            }
        }

        public string Email
        {
            get
            {
                if (_email == null)
                    _email = CryptographyService.DecryptEmail(EncryptedEmail);
                return _email;
            }
            set
            {
                EncryptedEmail = CryptographyService.EncryptEmail(value);
                _email = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                if (_phoneNumber == null)
                    _phoneNumber = CryptographyService.DecryptPhone(EncryptedPhoneNumber);
                return _phoneNumber;
            }
            set
            {
                EncryptedPhoneNumber = CryptographyService.EncryptPhone(value);
                _phoneNumber = value;
            }
        }
    }
}