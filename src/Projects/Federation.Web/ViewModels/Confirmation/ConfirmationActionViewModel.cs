using System;

namespace Federation.Web.ViewModels
{
    public class ConfirmationActionViewModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string YesUrl { get; set; }
        public string NoUrl { get; set; }
        public bool HttpPost { get; set; }
    }
}