using System;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.Controllers
{
    public class ContentController : MainController
    {
        public ActionResult Edit(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == id);
            if (content == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента");

            if (content is Post)
            {
                if (content.GroupId.HasValue)
                    return RedirectToAction("editpost", "group", new { id = content.Id });
                
                return RedirectToAction("editpost", "user", new { id = content.Id });
            }
            if (content is Petition)
            {
                if (content.GroupId.HasValue)
                    return RedirectToAction("editpetition", "group", new { id = content.Id });

                return RedirectToAction("editpetition", "user", new { id = content.Id });
            }
            if (content is Poll)
            {
                return RedirectToAction("editpoll", "group", new { id = content.Id });
            }
            if (content is Survey)
            {
                return RedirectToAction("editsurvey", "group", new { id = content.Id });
            }

            return RedirectToAction("index", "home");
        }

        public ActionResult Attach(Guid id, byte target)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = ContentService.Attach(id, UserContext.Current.Id, (AttachDetachTarget)target);

            return Redirect(content.GetUrl());
        }

        public ActionResult Detach(Guid id, byte target)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = ContentService.Detach(id, UserContext.Current.Id, (AttachDetachTarget)target);

            return Redirect(content.GetUrl());
        }

        public ActionResult Publish(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.Publish(id, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }

        public ActionResult Unpublish(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.Unpublish(id, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }

        public ActionResult ToggleBlock(bool block, Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.ToggleBlock(block, content, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }

        public ActionResult Delete(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.Delete(id, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }

        public ActionResult Restore(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.Restore(id, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }

        public ActionResult ToggleDiscussion(Guid id, bool show)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
            if (content == null)
                throw new BusinessLogicException("Не найден контент с указанным идентификатором");

            ContentService.ToggleDiscussion(show, id, UserContext.Current.Id);

            return Redirect(content.GetUrl());
        }
    }
}
