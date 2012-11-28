using System.Web.Mvc;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class SearchController : MainController
    {
        public ActionResult Index(string id)
        {
            return View(new SearchIndexViewModel(id));
        }

        public ActionResult Group(string id)
        {
            return View(new SearchIndexViewModel(id, SearchBy.FullText, SearchWhat.Groups));
        }

        public ActionResult Tag(string id)
        {
            //if (string.IsNullOrWhiteSpace(id))
            //    return View(new SearchTagViewModel() { IsEmptyRequest = true });

            //var tag = TagService.GetTag(id, );
            //if (tag == null)
            //    return View(new SearchTagViewModel() { Tag = id });

            //return View(new SearchTagViewModel(tag));
            return View(new SearchTagViewModel());
        }
    }
}