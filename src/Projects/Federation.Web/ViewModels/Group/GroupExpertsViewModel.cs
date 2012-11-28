using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupExpertsViewModel
    {        
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public Guid? FilterTagId { get; set; }
        public string FilterTagTitle { get; set; }

        public bool IsExpert { get; set; }

        public List<GroupExperts_ExpertViewModel> Experts { get; private set; }

        public GroupExpertsViewModel(Group group, Guid? filterByTag = null)
        {
            GroupId = group.Id;
            GroupUrl = group.Url;
            FilterTagId = filterByTag;
            Experts = new List<GroupExperts_ExpertViewModel>();

            if (UserContext.Current.IsUserApprovedInGroup(group.Id))
            {
                var uig = GroupService.UserInGroup(UserContext.Current.Id, group);

                if (uig.Expert != null)
                    IsExpert = true;
            }

            List<Expert> experts;
            if(filterByTag!=null)
            {
                var filterTag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == filterByTag);
                if (filterTag != null)
                {
                    experts =
                        DataService.PerThread.ExpertSet.Where(
                            x => x.GroupMember.GroupId == GroupId).ToList().Where(x => x.Tags.Contains(filterTag) && x.Tags.Count > 0).ToList();
                    
                    FilterTagTitle = filterTag.Title;
                }
                else
                {
                    FilterTagId = null;
                    experts = DataService.PerThread.ExpertSet.Where(x => x.GroupMember.GroupId == GroupId && x.Tags.Count > 0 ).ToList();
                }
            }
            else
                experts = DataService.PerThread.ExpertSet.Where(x => x.GroupMember.GroupId == GroupId && x.Tags.Count > 0).ToList();


            foreach (var expert in experts)
                Experts.Add(new GroupExperts_ExpertViewModel(expert));
        }
    }

    public class GroupExperts_ExpertViewModel
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }

        public readonly List<TagViewModel> Tags = new List<TagViewModel>();

        public GroupExperts_ExpertViewModel()
        {
        }

        public GroupExperts_ExpertViewModel(Expert expert)
        {
            Id = expert.Id;
            UserId = expert.GroupMember.UserId;

            var user = expert.GroupMember.User;
            Avatar = ImageService.GetImageUrl<User>(user.Avatar);
            FirstName = user.FirstName;
            SurName = user.SurName;
            Patronymic = user.Patronymic;

            foreach (var tag in expert.Tags)
                Tags.Add(new TagViewModel(tag));
        }
    }
}