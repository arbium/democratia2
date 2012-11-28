using System;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.Controllers
{
    public class CommunicationController : Controller
    {
        [HttpPost]
        public ActionResult FromSmsc(int id, string phone, byte status, DateTime time)
        {
            var errorField = Request.Form["err"];
            byte errorCode;

            if (Byte.TryParse(errorField, out errorCode))
                SmsService.SmsInfoChangeStatus(id, status, errorCode);
            else
                SmsService.SmsInfoChangeStatus(id, status, null);

            return View("Result", (object)"Готово!");
        }
    }
}
