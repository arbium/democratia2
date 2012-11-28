using System;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.Controllers
{
    public class LikeController : MainController
    {
        public ActionResult Apply(Guid id, bool val, byte type)
        {
            if (!Request.IsAuthenticated || UserContext.Current == null)
                throw new AuthenticationException();

            var userId = UserContext.Current.Id;
            Like like = null;

            switch ((WtfLikes)type)
            {
                case WtfLikes.AlbumItem:
                    var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(c => c.Id == id);
                    if (albumItem == null)
                        throw new BusinessLogicException("Указан неверный идентификатор контента альбома");

                    if (albumItem.Album.UserId.HasValue)
                        throw new BusinessLogicException("Нельзя лайкать свое творчество");

                    like = DataService.PerThread.LikeSet.SingleOrDefault(l => l.UserId == userId && l.AlbumItemId == id);
                    if (like != null)
                        throw new BusinessLogicException("Вы уже лайкали это");

                    like = new Like
                    {
                        AlbumItemId = id,
                        UserId = userId,
                        Value = val
                    };
                    break;

                case WtfLikes.Content:
                    var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == id);
                    if (content == null)
                        throw new BusinessLogicException("Указан неверный идентификатор контента");

                    if (content.AuthorId == userId)
                        throw new BusinessLogicException("Нельзя лайкать свое творчество");

                    like = DataService.PerThread.LikeSet.SingleOrDefault(l => l.UserId == userId && l.ContentId == id);
                    if (like != null)
                        throw new BusinessLogicException("Вы уже лайкали это");

                    like = new Like
                    {
                        ContentId = id,
                        UserId = userId,
                        Value = val
                    };
                    break;

                case WtfLikes.Comment:
                    var comment = DataService.PerThread.CommentSet.SingleOrDefault(x => x.Id == id);
                    if (comment == null)
                        throw new BusinessLogicException("Указан неверный идентификатор комментария");

                    if (comment.UserId == userId)
                        throw new BusinessLogicException("Нельзя лайкать свое творчество");

                    like = DataService.PerThread.LikeSet.SingleOrDefault(l => l.UserId == userId && l.CommentId == id);
                    if (like != null)
                        throw new BusinessLogicException("Вы уже лайкали это");

                    like = new Like
                    {
                        CommentId = id,
                        UserId = userId,
                        Value = val
                    };
                    break;
            }

            DataService.PerThread.LikeSet.AddObject(like);
            DataService.PerThread.SaveChanges();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return null;
        }
    }
}