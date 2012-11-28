using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountSocialSignIn
    {
        public string ReturnUrl { get; set; }

        public SocialType SocialType { get; set; }
    }
}