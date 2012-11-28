using System;
using System.Web.Mvc;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class HomeController : MainController
    {
        public ActionResult Index()
        {
            //bool showAllPosts = UserContext.Current == null ? false : UserContext.Current.ShowAllPostsOnIndex;           
            //if (UserContext.Current != null && Request.IsAuthenticated)
            //    return RedirectToAction("Today");

            return View(CachService.GetViewModel(new HomeIndexViewModel()));
        }

        public ActionResult Groups(byte? GroupStateFilter, byte? OrderByFilter)
        {
            return View(CachService.GetViewModel(new HomeGroupsViewModel((GroupState?)GroupStateFilter, (GroupOrderByType?)OrderByFilter)));
        }

        public ActionResult GroupsHub(string filter)
        {
            return View(new HomeGroupsHubViewModel(filter));
        }

        public ActionResult Today(string id)
        {
            DateTime date;

            if (!DateTime.TryParse(id, out date))
                date = DateTime.Now;

            return View(CachService.GetViewModel(new HomeTodayViewModel(date)));
        }

        public ActionResult Roadmap()
        {
            return View(CachService.GetViewModel(new HomeRoadmapViewModel()));
        }

        public ActionResult Rss()
        {
            return View("_Rss", "_EmptyLayout", CachService.GetViewModel(new HomeRssFeedViewModel()));
        }

        //public ActionResult GetMeFailedGroup()
        //{
        //    List<Group> groups = DataService.PerThread.GroupSet.ToList();

        //    foreach (var @group in groups)
        //    {
        //        try
        //        {
        //            ImageService.GetImageUrl<Group>(@group.Logo);
        //        }
        //        catch (Exception e)
        //        {      
        //            throw new BusinessLogicException(@group.Logo, e);

        //        }

        //    }

        //    return new EmptyResult();
        //}

        //public ActionResult MakeAdminDima()
        //{
        //    //var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Email == "dr.arbium@gmail.com");
        //    Group gGroup =
        //        DataService.PerThread.GroupSet.Where(x => x.Id == ConstHelper.FederationGroupId).SingleOrDefault();

        //    var gm = gGroup.Members.Where(x => x.User.Email == "dr.arbium@gmail.com").SingleOrDefault();
        //    gm.IsModerator = true;

        //    DataService.PerThread.SaveChanges();

        //    return new EmptyResult();
        //}
    }
}
