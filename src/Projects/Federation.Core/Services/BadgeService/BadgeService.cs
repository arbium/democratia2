using System;

namespace Federation.Core
{
    public static class BadgeService
    {
        public static Badge AwardUser<T>(Guid userId) where T : BaseBadge
        {
            BaseBadge badge = (BaseBadge)Activator.CreateInstance(typeof(T));

            return badge.AwardUser(userId);
        }

        public static Badge AwardUserCustom(Guid userId, string image, string title, string description = null)
        {
            BaseBadge badge = new BaseBadge
            {
                Image = image,
                Title = title,
                Description = description,
                IsAcquired = true
            };

            return badge.AwardUser(userId);
        }

        public static Badge AwardGroup<T>(string groupUrl) where T : BaseBadge
        {
            BaseBadge badge = (BaseBadge)Activator.CreateInstance(typeof(T));

            return badge.AwardGroup(groupUrl);
        }

        public static Badge AwardGroupCustom(string groupUrl, string image, string title, string description = null)
        {
            BaseBadge badge = new BaseBadge
            {
                Image = image,
                Title = title,
                Description = description,
                IsAcquired = true
            };

            return badge.AwardGroup(groupUrl);
        }
    }
}