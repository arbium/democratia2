using System;
using System.Linq;

namespace Federation.Core
{
    public enum BadgeType
    {
        Alpha = 0,
        BugCollector = 1,
        Quest = 2,
        TicketVerified = 3
    }

    //public static class BadgeTypeExtension
    //{
    //    public static Type GetBadgeType(this BadgeType badgeType)
    //    {
    //        switch (badgeType)
    //        {
    //            case BadgeType.First1000:
    //                return typeof(First1000Badge);
    //        }

    //        return null;
    //    }
    //}

    public class BaseBadge
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public BadgeType? Type { get; set; }
        public string Description { get; set; }
        public int? Value { get; set; }
        public bool IsAcquired { get; set; }
        public DateTime? AcquireDate { get; set; }

        public virtual Badge AwardUser(Guid userId)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            Badge badge = null;

            if (Type.HasValue)
                badge = user.Badges.SingleOrDefault(x => x.Type == (int)Type.Value);

            if (badge != null)
            {
                badge.Value = badge.Value ?? 1;
                badge.Value++;
            }
            else
            {
                badge = new Badge();
                DataService.PerThread.BadgeSet.AddObject(badge);
            }

            badge.Description = Description;
            badge.Image = Image;
            badge.IsAcquired = IsAcquired;
            badge.Title = Title;
            badge.UserId = userId;

            if (Type.HasValue)
                badge.Type = (int)Type.Value;

            if (IsAcquired)
                badge.AcquireDate = DateTime.Now;
            
            DataService.PerThread.SaveChanges();

            return badge;
        }

        public virtual Badge AwardGroup(string groupUrl)
        {
            var group = GroupService.GetGroupByLabelOrId(groupUrl);
            if (group == null)
                throw new BusinessLogicException("Группа не найдена");

            var badge = new Badge
            {
                Description = Description,
                GroupId = group.Id,
                Image = Image,
                IsAcquired = IsAcquired,
                Title = Title,
                Value = Value
            };

            if (Type.HasValue)
                badge.Type = (int)Type.Value;

            if (IsAcquired)
                badge.AcquireDate = DateTime.Now;

            DataService.PerThread.BadgeSet.AddObject(badge);
            DataService.PerThread.SaveChanges();

            return badge;
        }
    }    
}