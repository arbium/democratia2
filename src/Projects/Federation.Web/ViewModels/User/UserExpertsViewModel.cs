using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserExpertsViewModel
    {
        public Guid UserId { get; set; }
        public readonly IList<UserExperts_ExpertViewModel> Experts = new List<UserExperts_ExpertViewModel>();

        public UserExpertsViewModel()
        {
        }

        public UserExpertsViewModel(User currentUser)
        {
            if (currentUser != null)
            {
                UserId = currentUser.Id;
                var getUserExpertIdQuery = from votes in DataService.PerThread.ExpertVoteSet
                         join tag in DataService.PerThread.TagSet on votes.TagId equals tag.Id
                         join groupMember in DataService.PerThread.GroupMemberSet on votes.GroupMemberId equals groupMember.Id
                         join grp in DataService.PerThread.GroupSet on groupMember.GroupId equals grp.Id
                         join user in DataService.PerThread.BaseUserSet.OfType<User>() on groupMember.UserId equals user.Id
                         where user.Id == UserId
                         select new
                                    {
                                        TagTitle = tag.LowerTitle,
                                        TagId = tag.Id,
                                        GroupName = grp.Name,
                                        GroupId = grp.Id,
                                        ExpertId = votes.ExpertId,
                                        GroupLabel = grp.Label
                                    };

                var getUserFullExpertInfosQuery = from votes in getUserExpertIdQuery
                        join expert in DataService.PerThread.ExpertSet on votes.ExpertId equals expert.Id
                        join groupMember in DataService.PerThread.GroupMemberSet on expert.Id equals groupMember.Expert.Id
                        join user in DataService.PerThread.BaseUserSet.OfType<User>() on groupMember.UserId equals user.Id
                        select
                            new
                                {
                                    TagTitle = votes.TagTitle,
                                    TagId = votes.TagId,
                                    UserFirstName = user.FirstName,
                                    UserSurName = user.SurName,
                                    UserPatronymic = user.Patronymic,
                                    UserAvatar = user.Avatar,
                                    UserId = user.Id,
                                    GroupName = votes.GroupName,
                                    GroupId = votes.GroupId,
                                    GroupLabel = votes.GroupLabel,
                                    ExpertId = votes.ExpertId
                                };
                var expertInfos = getUserFullExpertInfosQuery.GroupBy(x => new { x.UserId, x.UserFirstName, x.UserSurName, x.UserPatronymic, x.UserAvatar }).ToList();

                foreach (var expertInfo in expertInfos)
                {
                    var expert = new UserExperts_ExpertViewModel
                                     {
                                         Id = expertInfo.Key.UserId.ToString(),
                                         FirstName = expertInfo.Key.UserFirstName,
                                         SurName = expertInfo.Key.UserSurName,
                                         Patronymic = expertInfo.Key.UserPatronymic,
                                         Avatar = ImageService.GetImageUrl<User>(expertInfo.Key.UserAvatar)
                                     };

                    var expertGroupInfos = expertInfo.GroupBy(x => new {x.GroupId, x.GroupName, x.GroupLabel, x.ExpertId});
                    foreach (var expertGroupInfo in expertGroupInfos)
                    {
                        var group = new UserExperts_ExpertGroupViewModel
                                        {
                                            Url = expertGroupInfo.Key.GroupLabel ?? expertGroupInfo.Key.GroupId.ToString(),
                                            Name = expertGroupInfo.Key.GroupName,
                                            ExpertId = expertGroupInfo.Key.ExpertId.ToString()
                                        };
                        
                        foreach (var expertGroupTagInfo in expertGroupInfo)
                        {
                            var tagViewModel = new TagViewModel
                                                   {
                                                       Id = expertGroupTagInfo.TagId, 
                                                       Title = expertGroupTagInfo.TagTitle
                                                   };
                            group.Tags.Add(tagViewModel);
                        }

                        expert.Groups.Add(group);
                    }

                    Experts.Add(expert);
                }
            }
        }
    }

    public class UserExperts_ExpertViewModel
    {
        public string Id { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }

        public readonly List<UserExperts_ExpertGroupViewModel> Groups = new List<UserExperts_ExpertGroupViewModel>();

        public UserExperts_ExpertViewModel()
        {
        }
    }

    public class UserExperts_ExpertGroupViewModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string ExpertId { get; set; }

        public readonly List<TagViewModel> Tags = new List<TagViewModel>();

        public UserExperts_ExpertGroupViewModel()
        {
        }
    }
}