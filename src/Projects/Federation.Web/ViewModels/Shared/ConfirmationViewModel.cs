using System;
using System.Web.Routing;

namespace Federation.Web.ViewModels
{
    public class ConfirmationViewModel
    {
        public RouteValueDictionary RouteValues { get; private set; }
        public string Message { get; set; }
        public string YesUrlAction { get; set; }
        public string YesUrlController { get; set; }
        public string NoUrl { get; set; }
        public string Query { get; set; }

        public ConfirmationViewModel()
        {
            RouteValues = new RouteValueDictionary();
        }
    }
}