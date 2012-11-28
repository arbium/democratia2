using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupTagsViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public List<GroupTags_TagViewModel> RecomendedTags = new List<GroupTags_TagViewModel>();
        public List<GroupTags_TagViewModel> Tags = new List<GroupTags_TagViewModel>();

        public GroupTagsViewModel(Guid groupId)
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
                RecomendedTags = group.Tags.Where(x => x.TopicState == (byte)TopicState.NotTopic && x.IsRecommended).OrderByDescending(x=>x.Weight).Select(x => new GroupTags_TagViewModel(x)).ToList();
                Tags = group.Tags.Where(x => x.TopicState == (byte)TopicState.NotTopic && !x.IsRecommended).OrderByDescending(x => x.Weight).Take(50).Select(x => new GroupTags_TagViewModel(x)).ToList();
            }
        }
    }

    public class GroupTags_TagViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsRecomended { get; set; }
        public int Weight { get; set; }

        public GroupTags_TagViewModel(Tag tag)
        {
            Id = tag.Id;
            Title = tag.Title;
            IsRecomended = tag.IsRecommended;
            Weight = tag.Weight;
        }
    }
}
