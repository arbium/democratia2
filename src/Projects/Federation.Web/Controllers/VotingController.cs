using System;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class VotingController : MainController
    {
        [HttpPost]
        public ActionResult CreatePetition(_CreatePetitionViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (ModelState.IsValid)
            {
                var data = new PetitionContainer
                {
                    GroupId = model.GroupId,
                    IsPrivate = model.IsPrivate,
                    Tags = model.TagTitles,
                    Text = model.Text,
                    Title = model.Title
                };

                var petition = VotingService.CreatePetition(data, UserContext.Current.Id);
                UserContext.Abandon();

                return RedirectToAction("petition", petition.Controller, new { id = petition.Id });
            }

            if (model.GroupId != null)
                View("../group/createpetition", model);

            return View("../user/createpetition", model);
        }

        [HttpPost]
        public ActionResult EditPetition(_EditPetitionViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (ModelState.IsValid)
            {
                PetitionContainer data = new PetitionContainer
                {
                    GroupId = model.GroupId,
                    Id = model.Id,
                    IsPrivate = model.IsPrivate,
                    Tags = model.TagTitles,
                    Text = model.Text,
                    Title = model.Title
                };

                Petition petition = VotingService.EditPetition(data, UserContext.Current.Id);

                return RedirectToAction("petition", petition.Controller, new { id = petition.Id });
            }

            if (model.GroupId != null)
                View("../group/editpetition", model);

            return View("../user/editpetition", model);
        }

        public ActionResult SignPetition(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (ModelState.IsValid)
            {
                Petition petition = VotingService.SignPetition(id, UserContext.Current.Id);
                UserContext.Abandon();
                return RedirectToAction("petition", petition.Controller, new { id = petition.Id });
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult PublishPetition(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            Petition petition = VotingService.PublishPetition(id, UserContext.Current.Id);

            return RedirectToAction("petition", petition.Controller, new { id = petition.Id });
        }

        [HttpPost]
        public ActionResult InvitePetitionCoauthor(_EditPetitionCoauthorsViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            PetitionCoauthorContainer data = new PetitionCoauthorContainer
            {
                PetitionId = model.PetitionId,
                UserName = model.UserNameForInvite
            };
            
            VotingService.InvitePetitionCoauthor(data, UserContext.Current.Id);            

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult DeletePetitionCoauthor(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            Coauthor coauthor = DataService.PerThread.CoauthorSet.SingleOrDefault(x => x.Id == id);

            if (coauthor == null)
                throw new Exception("Неверный идентификатор соавтора");

            VotingService.DeletePetitionCoauthor(id, UserContext.Current.Id);

            return RedirectToAction("editpetitioncoauthors", "user", new { id = coauthor.PetitionId });
        }

        public ActionResult RespondToPetitionInvite(Guid id, bool accept)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            Petition petition = VotingService.RespondToPetitionInvite(id, UserContext.Current.Id, accept);

            if (accept)
                return RedirectToAction("petition", petition.Controller, new { id = petition.Id });
            else
                return RedirectToAction("petitionnotices", "user");
        }
    }
}