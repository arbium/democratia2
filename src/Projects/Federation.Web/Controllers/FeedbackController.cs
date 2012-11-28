using System.Web.Mvc;
using Federation.Core;
using Federation.Web.Services;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class FeedbackController : MainController
    {
        public ActionResult Index(bool? isdialog)
        {
            var model = new FeedbackIndexViewModel();

            if (!Request.IsAuthenticated)
            {
                var keyInfo = AntibotService.GetSefetyKey(!Request.IsAuthenticated);
                var key = keyInfo.Key;
                var question = keyInfo.Question;
                var tip = AntibotService.MakeQuestionTip(question);
                var questionImage = AntibotService.GetCaptureImageUrl(question);

                model.SafetyKey = key;
                model.SafetyImageUrl = questionImage;
                model.Tip = tip;
            }
            else
            {
                model.Answer = "null";
                model.UserId = UserContext.Current.Id;
            }

            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("Index", "_LightLayout", model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FeedbackIndexViewModel model, bool? isdialog)
        {            
            if (!Request.IsAuthenticated && !AntibotService.IsKeyActual(model.SafetyKey, model.Answer))
            {
                ViewBag.ValidationExceptionText =
                    "Вы не прошли проверку на бота. Повторите попытку. <br/> Если ошибка будет повторяться напишите нам на <a href='mailto:support@democratia2.ru'>support@democratia2.ru</a>";
            }
            else if (ModelState.IsValid)
            {                
                if (model.UserId.HasValue)
                    FeedbackService.SendMessage(model.UserId.Value, model.Subject, model.Text);
                else
                    FeedbackService.SendMessage(model.Name, model.Email, model.Subject, model.Text);

                if (isdialog.HasValue)
                    if (isdialog.Value)
                        return View("Result", "_LightLayout", "Готово!");

                return View("Result", (object)"Готово!");
            }

            if (!Request.IsAuthenticated)
            {
                ModelState.Clear();

                var keyInfo = AntibotService.GetSefetyKey(!Request.IsAuthenticated);
                var key = keyInfo.Key;
                var question = keyInfo.Question;
                var tip = AntibotService.MakeQuestionTip(question);
                var questionImage = AntibotService.GetCaptureImageUrl(question);

                model.SafetyKey = key;
                model.SafetyImageUrl = questionImage;
                model.Tip = tip;
            }

            if (isdialog.HasValue)
                if (isdialog.Value)
                    return View("Index", "_LightLayout", model);

            return View(model);
        }
    }
}