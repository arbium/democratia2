using System;

namespace Federation.Core
{
    public partial class SubscriptionSettings
    {
        private string _subscriptionEmail;

        public SubscriptionSettings()
        {
            Id = Guid.NewGuid();
        }

        public string SubscriptionEmail
        {
            get
            {
                if (_subscriptionEmail == null)
                    _subscriptionEmail = CryptographyService.DecryptEmail(EncryptedSubscriptionEmail);
                return _subscriptionEmail;
            }
            set
            {
                EncryptedSubscriptionEmail = CryptographyService.EncryptEmail(value);
                _subscriptionEmail = value;
            }
        }
    }
}