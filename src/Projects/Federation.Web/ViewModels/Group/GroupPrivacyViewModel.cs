using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupPrivacyViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }

        [Display(Name = "Модерация участников группы")]
        public bool IsMemberModeration { get; set; }

        [Display(Name = "Модерация контента группы")]
        public bool IsContentModeration { get; set; }

        [Display(Name = "Комментирование только участниками группы")]
        public bool IsPrivateDiscussion { get; set; }

        [Display(Name = "Невидимая группа")]
        public bool IsInvisible { get; set; }

        public GroupPrivacyViewModel()
        {
        }

        public GroupPrivacyViewModel(Group group)
        {
            GroupId = group.Id;
            GroupUrl = group.Url;
            GroupName = group.Name;
            var groupPrivacy = (GroupPrivacy)group.Privacy;
            IsMemberModeration = groupPrivacy.HasFlag(GroupPrivacy.MemberModeration);
            IsContentModeration = groupPrivacy.HasFlag(GroupPrivacy.ContentModeration);
            IsPrivateDiscussion = groupPrivacy.HasFlag(GroupPrivacy.PrivateDiscussion);
            IsInvisible = groupPrivacy.HasFlag(GroupPrivacy.Invisible);
        }
    }
}