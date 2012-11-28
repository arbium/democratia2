using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class BadgeViewModel
    {
        public string Description { get; set; }
        public Guid? GroupId { get; set; }
        public Guid Id { get; set; }
        public string Image { get; set; }
        public bool IsAcquired { get; set; }
        public string Title { get; set; }
        public BadgeType? Type { get; set; }
        public Guid? UserId { get; set; }
        public int? Value { get; set; }
        public DateTime? AcquireDate { get; set; }

        public BadgeViewModel()
        {
        }

        public BadgeViewModel(Badge badge)
        {
            if (badge != null)
            {
                Description = badge.Description;
                GroupId = badge.GroupId;
                Id = badge.Id;
                Image = ImageService.GetImageUrl<Badge>(badge.Image);
                IsAcquired = badge.IsAcquired;
                Title = badge.Title;
                if (badge.Type.HasValue)
                    Type = (BadgeType)badge.Type.Value;
                UserId = badge.UserId;
                Value = badge.Value;
                AcquireDate = badge.AcquireDate;
            }
        }
    }
}