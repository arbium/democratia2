using System;
using System.Linq;

namespace Federation.Core
{
    public partial class Content
    {
        public string Controller
        {
            get
            {
                return GroupId.HasValue ? "group" : "user";
            }
        }

        public int Rating
        {
            get { return Likes.Count(x => x.Value) - Likes.Count(x => !x.Value); }
        }

        private bool IsAttached
        {
            get { return Attach != null; }
        }

        /// <summary>
        /// Закреплено в группе
        /// </summary>
        public bool IsGroupAttached(Guid groupId)
        {
            if (!IsAttached)
                return false;

            return Attach.Groups.Count(x => x.Id == groupId) != 0;
        }

        /// <summary>
        /// Закреплено в группе
        /// </summary>
        public bool IsGroupAttached(string groupUrl)
        {
            if (!IsAttached)
                return false;

            return Attach.Groups.Count(x => x.Url == groupUrl) != 0;
        }

        /// <summary>
        /// Закреплено у пользователя
        /// </summary>
        public bool IsUserAttached(Guid userId)
        {
            if (!IsAttached)
                return false;

            return Attach.Users.Count(x => x.Id == userId) != 0;
        }

        public Content()
        {
            Id = Guid.NewGuid();
        }
    }
}