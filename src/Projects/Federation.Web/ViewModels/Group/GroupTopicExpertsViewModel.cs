using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupTopicExpertsViewModel
    {
        private Guid _topicId;

        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }

        public Guid TopicId { get; set; }
        public string TopicName { get; set; }
        public string TopicDescription { get; set; }

        public int ContentCount { get; set; }
        public bool IsCurrentUserTopicExpert { get; set; }

        public List<GroupTopicExperts_ItemViewModel> Experts = new List<GroupTopicExperts_ItemViewModel>();

        public GroupTopicExpertsViewModel(Guid topicId)
        {
            _topicId = topicId;
            Initialize();
        }

        public void Initialize()
        {
            var topic = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == _topicId);
            if (topic == null)
                throw new BusinessLogicException("Темы с таким id не обнаружено");

            if (topic.GroupId == null)
                throw new BusinessLogicException("Данная группа не привязана к группе");

            GroupId = topic.Group.Id;
            GroupName = topic.Group.Name;
            GroupUrl = topic.Group.Url;
            var groumMembersCount = topic.Group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator);

            TopicId = _topicId;
            TopicName = topic.Title;

            ContentCount = topic.Contents.Count(x => x.State == (byte)ContentState.Approved);

            IsCurrentUserTopicExpert = false;

            if (!string.IsNullOrEmpty(topic.Description))
                TopicDescription = Regex.Replace(topic.Description, @"(\r?\n\s{0,}){1,}", "<br/>");

            Guid? expertVoteId = null;
            if (UserContext.Current != null)
            {
                if (topic.Experts.Any(x => x.GroupMember.UserId == UserContext.Current.Id))
                    IsCurrentUserTopicExpert = true;
                else
                {
                    var vote =
                        DataService.PerThread.ExpertVoteSet.SingleOrDefault(
                            x => x.TagId == _topicId && x.GroupMember.UserId == UserContext.Current.Id);
                    if (vote != null)
                        expertVoteId = vote.ExpertId;
                }
            }

            Experts = topic.Experts.Select(x => new GroupTopicExperts_ItemViewModel(x, _topicId, groumMembersCount, expertVoteId)).ToList();
        }
    }

    public class GroupTopicExperts_ItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ExpertId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public bool VoteDelegated { get; set; }
        public int VotesCount { get; set; }
        public double VotesRelative { get; set; }

        public GroupTopicExperts_ItemViewModel(Expert expert, Guid topicId, int totalCount, Guid? votedExpertId = null)
        {
            ExpertId = expert.Id;
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.GroupMembers.Any(g => g.Expert.Id == expert.Id));
            if (user != null)
            {
                Id = user.Id;
                FullName = user.FullName;
                Image = ImageService.GetImageUrl<User>(user.Avatar);
            }

            Description = "";
            if (!string.IsNullOrEmpty(expert.Info))
                Description = Regex.Replace(expert.Info, @"(\r?\n\s{0,}){1,}", "<br/>");

            if (Description.Length > 300)
                Description = Description.Substring(0, 300) + "...";
            VotesCount = expert.ExpertVotes.Count(x => x.TagId == topicId);

            if (totalCount > 0)
                VotesRelative = Math.Round(((double)VotesCount) / totalCount, 2);
            else
                VotesRelative = 0;
            
            VoteDelegated = votedExpertId == expert.Id;
        }
    }
}
