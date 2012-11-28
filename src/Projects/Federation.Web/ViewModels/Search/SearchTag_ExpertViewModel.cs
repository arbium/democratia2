using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class SearchTag_ExpertViewModel
    {
        public string Avatar { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public string GroupLogo { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }
        public IList<TagViewModel> Tags { get; set; }
        public Guid UserId { get; set; }

        public SearchTag_ExpertViewModel()
        {
            Tags = new List<TagViewModel>();
        }

        public SearchTag_ExpertViewModel(Expert expert)
        {
            if (expert != null)
            {
                Avatar = ImageService.GetImageUrl<User>(expert.GroupMember.User.Avatar);
                GroupName = expert.GroupMember.Group.Name;
                GroupUrl = expert.GroupMember.Group.Url;
                GroupLogo = ImageService.GetImageUrl<Group>(expert.GroupMember.Group.Logo);
                Name = expert.GroupMember.User.FullName;
                FirstName = expert.GroupMember.User.FirstName;
                SurName = expert.GroupMember.User.SurName;
                Patronymic = expert.GroupMember.User.Patronymic;
                Tags = expert.Tags.Select(x => new TagViewModel(x)).ToList();
                UserId = expert.GroupMember.UserId;
            }
        }
    }
}