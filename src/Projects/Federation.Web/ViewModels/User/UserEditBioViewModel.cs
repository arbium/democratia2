using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditBioViewModel
    {
        private string _cleanInfo;

        [Display(Name = "Краткая биография")]
        [AllowHtml]
        public string Info
        {
            get { return _cleanInfo; }
            set { _cleanInfo = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }
    }
}