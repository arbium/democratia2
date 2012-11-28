using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupIndexViewModel : CachedViewModel
    {
        public GroupState State { get; set; }
        public Guid GroupId { get; set; }
        public int MembersCount { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public bool IsExpertable { get; set; }
        public bool IsEditEnabled { get; set; }
        public bool IsInvisible { get; set; }
        public int GroupTagsCount { get; set; }
        public Guid MemberId { get; set; }
        public bool IsCreateElectionEnabled { get; set; }

        public Guid? TopicId { get; set; }
        public bool? ByDate { get; set; }

        public IList<GroupIndex_TopicViewModel> Topics = new List<GroupIndex_TopicViewModel>();

        public _ContentFeedViewModel Feed = new _ContentFeedViewModel();
        public Group_ModersViewModel Moders = new Group_ModersViewModel();

        public int RequiredModers { get; set; }

        private readonly Guid _groupId;
        private Guid? _userId;

        public GroupIndexViewModel(Guid groupId, Guid? userId, Guid? topicId = null, bool? byDate = null)
        {
		    _groupId = groupId;
            _userId = userId;
            CachUserRelated = true;
            RelativeModelId = groupId;
            CachDropPeriod = new TimeSpan(0, 15, 0);
            TopicId = topicId;
            ByDate = byDate;
        }

        public override ICachedViewModel Initialize()
        {
            var group = GroupService.GetGroupByLabelOrId(_groupId);
			
            var membersCount = DataService.PerThread.GroupMemberSet.Count(gm => gm.GroupId == group.Id && (gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator));
            var moderatorsCount = DataService.PerThread.GroupMemberSet.Count(gm => gm.GroupId == group.Id && gm.State == (byte)GroupMemberState.Moderator);

            State = (GroupState)group.State;
            GroupId = group.Id;
            MembersCount = membersCount;
            Name = group.Name;
            Summary = TextHelper.CleanTags(group.Summary);
            if (Summary.Length > 500)
                Summary = Summary.Substring(0, 500) + "…";
            Url = group.Url;
            GroupTagsCount = group.Tags.Count;
            RequiredModers = group.ModeratorsCount - moderatorsCount;
            IsExpertable = group.Tags.Count != 0;
            IsEditEnabled = group.Content.OfType<Election>().Count(x => !x.IsFinished) == 0;
            IsInvisible = group.PrivacyEnum.HasFlag(GroupPrivacy.Invisible);
            IsCreateElectionEnabled = GroupService.CanElectionsBeStarted(group);

            Topics = group.Tags.Where(x => x.TopicState == (byte) TopicState.GroupTopic).OrderByDescending(x => x.Weight).Take(5).ToList()
                .Select(x => new GroupIndex_TopicViewModel(x)).ToList();

            IQueryable<Content> feedQueue;

            if (_userId.HasValue && _userId.Value != Guid.Empty)
            {
                var uig = GroupService.UserInGroup(_userId.Value, GroupId);                    

                if (uig != null)
                {
                    MemberId = uig.Id;
                    
                    feedQueue = DataService.PerThread.ContentSet.Where(x => x.GroupId.HasValue && x.GroupId.Value == _groupId
                        && (x.State == (byte)ContentState.Approved || (x.State == (byte)ContentState.Premoderated && (x.AuthorId.HasValue && x.AuthorId.Value == _userId.Value || uig.State == (byte)GroupMemberState.Moderator))));
                }
                else
                    feedQueue = DataService.PerThread.ContentSet.Where(x => x.GroupId.HasValue && x.GroupId.Value == _groupId && x.State == (byte)ContentState.Approved);
            }
            else
                feedQueue = DataService.PerThread.ContentSet.Where(x => x.GroupId.HasValue && x.GroupId.Value == _groupId && x.State == (byte)ContentState.Approved);

            if (ByDate.HasValue && ByDate.Value)
                feedQueue = feedQueue.OrderByDescending(x => x.PublishDate ?? DateTime.Now);
            else
            {
                /*feedQueue = (from content in feedQueue.Where(x => x.PublishDate.HasValue)
                    join comment in DataService.PerThread.CommentSet on content.Id equals comment.ContentId
                    join like in DataService.PerThread.LikeSet on content.Id equals like.ContentId
                    let commentsCount = content.Comments.Count
                    let likesCount = content.Likes.Count
                    let plusesCount = content.Likes.Count(x => x.Value)
                    let minusesCount = content.Likes.Count(x => !x.Value)
                    //orderby (Math.Pow(plusesCount * likesCount, 0.5) + commentsCount / 10) /
                    //    Math.Pow(EntityFunctions.DiffDays(content.PublishDate.Value, DateTime.Now).Value + 0.01, 0.5) /
                    //    Math.Pow(minusesCount + 1, 0.5) descending
                    orderby (Math.Pow(plusesCount * likesCount, 0.5) + commentsCount / 10 + 7) /
                        Math.Pow((EntityFunctions.DiffDays(content.PublishDate.Value, DateTime.Now).Value + 0.01) * (minusesCount + 1), 0.5)
                        descending
                    select content);*/

                feedQueue = feedQueue.OrderByDescending(x => (Math.Pow(x.Likes.Count(y => y.Value) * x.Likes.Count, 0.5) + x.Comments.Count / 10 + 7) /
                        Math.Pow((EntityFunctions.DiffDays(x.PublishDate.Value, DateTime.Now).Value + 0.01) * (x.Likes.Count(y => !y.Value) + 1), 0.5));
            }

            if (TopicId.HasValue)
                feedQueue = feedQueue.Where(x => x.Tags.Count(y => y.Id == TopicId.Value) != 0);

            Feed = new _ContentFeedViewModel(feedQueue, Url);
            Moders = new Group_ModersViewModel(group);

			return this;
        }
    }

    public class GroupIndex_TopicViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public GroupIndex_TopicViewModel(Tag topic)
        {
            Id = topic.Id;
            Title = topic.Title;
        }
    }
}
