using System;
using System.Linq;

namespace Federation.Core
{
    public static class UrlHelper
    {
        public static string GetUrl(this Comment comment, bool relative = true)
        {
            var result = relative ? string.Empty : ConstHelper.AppUrl;

            if (comment.Content.GroupId == null)
                result += @"/user/replycomment/" + comment.Id;
            else
                result += @"/group/" + comment.Id + "/replycomment";

            return result;
        }

        public static string GetUrl(this Content content, bool relative = true)
        {
            var result = relative ? string.Empty : ConstHelper.AppUrl;

            if (content.GroupId.HasValue)
                result += @"/group/" + content.Id + "/content";
            else
                result += @"/user/content/" + content.Id;

            return result;
        }

        public static string GetUrl(this Content content, Comment comment, bool relative = true)
        {
            var result = content.GetUrl(relative);
            var count = DataService.PerThread.CommentSet.Count(x => x.ContentId == comment.ContentId && x.DateTime > comment.DateTime);
            var page = (int)(count / ConstHelper.CommentsPerPage) + 1;
            result += (page > 1 ? "?page=" + page : "") + "#" + comment.Id;

            return result;
        }

        public static string GetActionUrl(string action, string controller, string id, string otherValues, bool relative = true)
        {
            var result = relative ? string.Empty : ConstHelper.AppUrl;

            if (controller.ToLower() == "group")
                result += string.Format("/{0}/{1}/{2}", controller, id, action);
            else
                result += string.Format("/{0}/{1}/{2}", controller, action, id);

            if (!string.IsNullOrEmpty(otherValues))
                result += "?" + otherValues;

            return result;
        }

        public static string GetTopicUrl(this Tag tag, bool relative = true)
        {
            var result = relative ? string.Empty : ConstHelper.AppUrl;

            if (tag.GroupId.HasValue)
                result += @"/group/" + tag.GroupId + "/topics";
            else
                result = @"";

            return result;
        }

        public static string GetSearchUrl(this Tag tag, bool relative = true)
        {
            var result = relative ? string.Empty : ConstHelper.AppUrl;

            //if (tag.GroupId.HasValue)
            //    result += @"/group/" + tag.GroupId + "/topics";
            //else
            //    result = @"";
            throw new NotImplementedException();

            //return result;
        }

        /// <typeparam name="T">User/Group/Expert/Content</typeparam>
        public static string GetUrl<T>(string id, bool relative = true) where T : class
        {
            string result = string.Empty;

            if (!relative)
                result = ConstHelper.AppUrl;

            if (typeof(T) == typeof(User))
            {
                result += @"/user/id" + id;
            }
            else if (typeof(T) == typeof(Group))
            {
                result += @"/group/" + id;
            }
            else if (typeof(T) == typeof(Expert))
            {
                result += @"/group/" + id + "/expert";
            }
            else if (typeof(T) == typeof(Comment))
            {
                Guid commentId;

                if (!Guid.TryParse(id, out commentId))
                    throw new BusinessLogicException("Неверный формат идентификатора комментария");

                var comment = DataService.PerThread.CommentSet.SingleOrDefault(p => p.Id == commentId);
                if (comment == null)
                    throw new BusinessLogicException("Не найден контент с Id = " + commentId);

                result = comment.GetUrl(relative);
            }
            else if (typeof(T).IsSubclassOf(typeof(Content)) || typeof(T) == typeof(Content))
            {
                Guid contentId;

                if (!Guid.TryParse(id, out contentId))
                    throw new BusinessLogicException("Неверный формат идентификатора поста");

                var content = DataService.PerThread.ContentSet.SingleOrDefault(p => p.Id == contentId);
                if (content == null)
                    throw new BusinessLogicException("Не найден контент с Id = " + contentId);

                result = content.GetUrl(relative);
            }            

            return result;
        }

        /// <typeparam name="T">User/Group/Expert/Content</typeparam>
        public static string GetUrl<T>(Guid id, bool relative = true) where T : class
        {
            return GetUrl<T>(id.ToString(), relative);
        }

        public static string GetUrl(string action, string controller, bool relative = true)
        {
            var result = string.Empty;

            if (!relative)
                result = ConstHelper.AppUrl;

            result += "/" + controller + "/" + action;

            return result;
        }

        public static bool IsInnerUrl(string checkedUrl)
        {
            Uri uri = new Uri(checkedUrl);
            int count = ConstHelper.UrlAliases.Count(x => x.ToLower() == uri.Host.ToLower());

            if (count == 0)
                return true;

            return false;
        }
    }
}
