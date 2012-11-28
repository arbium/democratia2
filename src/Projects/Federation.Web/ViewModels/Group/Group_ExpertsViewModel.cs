using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_ExpertsViewModel
    {
        public bool IsMember { get; set; }
        public bool IsExpert { get; set; }
        public string GroupUrl { get; set; }

        public IList<TagViewModel> SelfExpertTags { get; private set; }
        public IList<Group_Experts_ExpertViewModel> ExpertsList { get; private set; }

        public Group_ExpertsViewModel()
        {
            SelfExpertTags = new List<TagViewModel>();
            ExpertsList = new List<Group_Experts_ExpertViewModel>();
        }

        public Group_ExpertsViewModel(Guid groupId, Guid? userId)
        {
            SelfExpertTags = new List<TagViewModel>();
            ExpertsList = new List<Group_Experts_ExpertViewModel>();

            var group = GroupService.GetGroupByLabelOrId(groupId);
            GroupUrl = group.Url;
            
            if (userId.HasValue)
            {
                var uig = GroupService.UserInGroup(userId.Value, group);

                if (uig != null)
                {
                    IsMember = true;

                    if (uig.Expert != null)
                    {
                        IsExpert = true;
                        SelfExpertTags = uig.Expert.Tags.Select(x => new TagViewModel(x)).ToList();
                    }                            
                }
            }

            var currenUserId = Guid.Empty;
            if (UserContext.Current != null)
                currenUserId = UserContext.Current.Id;

            //TODO: дописать нормальный join, пока эксперты выводяться хаотично
            var experts = (from expert in DataService.PerThread.ExpertSet
                join groupMember in DataService.PerThread.GroupMemberSet
                on expert.GroupMember.Id equals groupMember.Id
                where groupMember.GroupId == groupId && groupMember.UserId != currenUserId
                select new { expert.Id, groupMember.User }).ToList();
            
            //join vote in DataService.PerRequest.ExpertVoteSet
            //on mergeExpert.Id equals vote.ExpertId
            //group vote by vote.ExpertId into d
            //select new
            //{
            //    Expert = d.Key,
            //    Count = d.Count()
            //};

            //var experts = DataService.PerRequest.GroupMemberSet.Where(x => x.Id == group.Id && x.Expert).ToList();
            //DataService.PerRequest.ExpertSet.Where(x => x.GroupMember

            ExpertsList = new List<Group_Experts_ExpertViewModel>();
            foreach (var expert in experts.Take(3))
            {
                var user = expert.User;

                var expertViewModel = new Group_Experts_ExpertViewModel
                {
                    Id = expert.Id,
                    UserId = user.Id,
                    Name = user.FirstName,
                    Surname = user.SurName,
                    City = null,
                    Avatar = ImageService.GetImageUrl<User>(user.Avatar),
                    Weight = 0
                };
                ExpertsList.Add(expertViewModel);
            }
        }
    }

    public class Group_Experts_ExpertViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }
        public int Weight { get; set; }

        public Group_Experts_ExpertViewModel()
        {
        }

        public Group_Experts_ExpertViewModel(Expert expert)
        {
            if (expert != null)
            {
                var user = expert.GroupMember.User;

                Id = expert.Id;
                UserId = user.Id;
                Name = user.FirstName;
                Surname = user.SurName;
                if (user.Address != null)
                    City = user.Address.City.Title;
                Avatar = ImageService.GetImageUrl<User>(user.Avatar);
                Weight = DataService.PerThread.ExpertVoteSet.Count(x => x.ExpertId == expert.Id);
            }
        }
    }
}
