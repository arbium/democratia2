using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupTopicsViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public List<GroupTopics_TopicViewModel> Topics = new List<GroupTopics_TopicViewModel>();
        public List<GroupTopics_TopicViewModel> ProposedTopics = new List<GroupTopics_TopicViewModel>();

        public GroupTopicsViewModel(Guid groupId)
        {
            GroupId = groupId;
            Initialize();
        }

        public void Initialize()
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == GroupId);

            if (group != null)
            {
                GroupUrl = group.Url;
                GroupName = group.Name;
                Topics = group.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic).Select(x => new GroupTopics_TopicViewModel(x)).ToList();
                ProposedTopics = group.Tags.Where(x => x.TopicState == (byte)TopicState.ProposedTopic).Select(x => new GroupTopics_TopicViewModel(x)).ToList();
            }
        }
    }

    public class GroupTopics_TopicViewModel
    {
        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ContentCount { get; set; }
        public int ExpertCount { get; set; }

        public GroupTopics_TopicViewModel(Tag tag)
        {
            Id = tag.Id;
            Title = tag.Title;
            Description = tag.Description;
            GroupId = tag.GroupId;
            ContentCount = tag.Contents.Count(x=>x.State == (byte)ContentState.Approved);
            ExpertCount = tag.Experts.Count();
        }
    }
}
