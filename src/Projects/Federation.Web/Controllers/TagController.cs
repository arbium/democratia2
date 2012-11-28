using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.Controllers
{
    public class TagController : MainController
    {
        public ActionResult FindMatchingTags(string term, string id = "", bool showTopics = true)
        {
            var foundTags = TagService.FindMatchingTags(term, id, showTopics);

            var json = Json(foundTags);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return json;
        }
    }
}
