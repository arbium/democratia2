using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class CommentController : MainController
    {
        [HttpPost]
        public ActionResult Add(_AddCommentViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var comment = CommentService.AddComment(model.Text, UserContext.Current.Id, model.ContentId, model.ParentCommentId);

            var returnUrl = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                returnUrl.Append(model.ReturnUrl);
            else
                returnUrl.Append(Request.UrlReferrer.PathAndQuery);

            returnUrl.Append("#" + comment.Id);

            return Redirect(returnUrl.ToString());
        }

        public ActionResult Hide(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var comment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Комментарий по указанному идентификатору не найден");

            if (comment.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не можете удалить комментарий, т.к. не являетесь автором!");

            CommentService.HideComment(id ,true);

            var returnUrl = string.Empty;

            if (comment.Content is Post)
                returnUrl = Core.UrlHelper.GetUrl<Post>(comment.ContentId);
            else if (comment.Content is Voting)
                returnUrl = Core.UrlHelper.GetUrl<Voting>(comment.ContentId);

            if (comment.ParentCommentId.HasValue)
                returnUrl += "#" + comment.ParentCommentId;

            return Redirect(returnUrl);
        }

        public ActionResult Unhide(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var comment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Комментарий по указанному идентификатору не найден");

            if (comment.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не можете удалить комментарий, т.к. не являетесь автором!");

            CommentService.UnhideComment(id, true);

            return Redirect(Request.UrlReferrer.PathAndQuery + "#" + comment.Id);
        }        

        [HttpPost]
        public ActionResult Edit(EditCommentViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View("editcomment", model);

            var comment = DataService.PerThread.CommentSet.SingleOrDefault(x => x.Id == model.Id);
            if (comment == null)
                throw new BusinessLogicException("Указан неверный идентификатор комментария");

            if (comment.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя редактировать чужие комментарии");
            if (DateTime.Now - comment.DateTime > ConstHelper.CommentEditTimeSpan.Add(ConstHelper.CommentEditTimeSpan))
                throw new BusinessLogicException("Время на редактирование вышло");

            comment.Text = model.Text;

            DataService.PerThread.SaveChanges();

            return Redirect(model.ReturnUrl);
        }
    }
}