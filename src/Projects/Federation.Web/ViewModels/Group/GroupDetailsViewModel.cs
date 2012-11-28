using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupDetailsViewModel
    {
        public Guid GroupId { get; set; }
        public string Url { get; set; }

        public string Logo { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Category { get; set; }  // TODO: можно переделать на много категорий

        public bool IsContentModeration { get; set; }
        public bool IsMemberModeration { get; set; }
        public bool IsPrivateDiscussion { get; set; }

        public int MembersCount { get; set; }
        public int MaxModeratorsCount { get; set; }
        public byte PollQuorum { get; set; }
        public byte ElectionQuorum { get; set; }

        public _BadgesViewModel Badges = new _BadgesViewModel();
        public Group_ModersViewModel ModeratorList = new Group_ModersViewModel();
        public IList<TagViewModel> Topics = new List<TagViewModel>();
        public IList<TagViewModel> Tags = new List<TagViewModel>();
        public IList<TagViewModel> ProposedTags = new List<TagViewModel>();

        public GroupDetailsViewModel()
        {
        }

        public GroupDetailsViewModel(Group group, Guid? userId = null)
        {
            if (group != null)
            {
                GroupId = group.Id;
                Url = group.Url;

                Logo = group.Logo;
                Name = group.Name;
                Summary = group.Summary;
                PollQuorum = group.PollQuorum;
                ElectionQuorum = group.ElectionQuorum;

                if (group.Categories.Count > 0)
                    Category = group.Categories.First().Title;

                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
                IsMemberModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration);
                IsPrivateDiscussion = group.PrivacyEnum.HasFlag(GroupPrivacy.PrivateDiscussion);

                Badges = new _BadgesViewModel(group);
                MembersCount = group.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator);
                MaxModeratorsCount = group.ModeratorsCount;

                ModeratorList = new Group_ModersViewModel(group);

                Topics = group.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic).OrderByDescending(x=>x.LowerTitle).Select(x=>new TagViewModel(x)).ToList();
                Tags = group.Tags.Where(x => x.TopicState != (byte)TopicState.GroupTopic).OrderByDescending(x => x.Weight).Select(x => new TagViewModel(x)).Take(50).ToList();

                if (userId.HasValue)
                {
                    var uig = GroupService.UserInGroup(userId.Value, group.Id);

                    if (uig != null && uig.State == (byte)GroupMemberState.Moderator)
                        ProposedTags = group.Tags.Where(x => x.TopicState == (byte)TopicState.ProposedTopic).Select(x => new TagViewModel(x)).ToList();
                }
            }
        }
    }
}