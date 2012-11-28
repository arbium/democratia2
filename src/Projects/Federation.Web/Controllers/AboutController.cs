using System.Web.Mvc;

namespace Federation.Web.Controllers
{
    public class AboutController : MainController
    {        
        public ActionResult Members()
        {
            return View();
        }

        public ActionResult Partners()
        {
            return View();
        }

        public ActionResult Target()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View("Manifest");
        }
    }
}
