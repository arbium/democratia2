using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _ContentFeedViewModel
    {
        public Guid? AuthorId { get; set; }
        public string GroupUrl { get; set; }

        public PaginationList<_ContentFeed_ContentViewModel, Content> FeedContent { get; set; }

        public _ContentFeedViewModel()
        {
            FeedContent = new PaginationList<_ContentFeed_ContentViewModel, Content>(new List<Content>(), 0, x => new _ContentFeed_ContentViewModel(x));
        }

        public _ContentFeedViewModel(IEnumerable<Content> content, Guid? authorId, string groupUrl, int contentPerPage = 5)
        {
            AuthorId = authorId;
            GroupUrl = groupUrl;

            if (authorId.HasValue)
                content = content.OrderByDescending(x => x.IsUserAttached(authorId.Value));
            if (!string.IsNullOrEmpty(groupUrl))
                content = content.OrderByDescending(x => x.IsGroupAttached(groupUrl));

            FeedContent = new PaginationList<_ContentFeed_ContentViewModel, Content>(content, contentPerPage, x => new _ContentFeed_ContentViewModel(x));
        }

        public _ContentFeedViewModel(IEnumerable<Content> content, int contentPerPage = 5)
            : this(content, null, string.Empty, contentPerPage)
        {
        }

        public _ContentFeedViewModel(IEnumerable<Content> content, Guid authorId, int contentPerPage = 10)
            : this(content, authorId, string.Empty, contentPerPage)
        {
        }

        public _ContentFeedViewModel(IEnumerable<Content> content, string groupUrl, int contentPerPage = 10)
            : this(content, null, groupUrl, contentPerPage)
        {
        }
    }

    public class _ContentFeed_ContentViewModel
    {
        public Guid? AuthorId { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorSurname { get; set; }
        public string AuthorPatronymic { get; set; }
        public string AuthorAvatar { get; set; }
        public string AuthorCity { get; set; }

        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }

        //public string CommentsCount { get; set; }
        //убрали надпись, числа достаточно
        public int CommentsCountNum { get; set; }
        public bool IsDiscussionClosed { get; set; }
        public bool IsGroupAttached { get; set; }
        public bool IsUserAttached { get; set; }

        public bool IsFinished { get; set; }

        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public ContentViewType ContentType { get; set; }
        public string ContentTypeName { get; set; }
        public string ContentClass { get; set; }
        public ContentState ContentState { get; set; }
        public string ContentStateAlert { get; set; }

        public DateTime PostDate { get; set; }

        public IList<TagViewModel> Tags { get; set; }

        // Голосования
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public VoteOption? Vote { get; set; }
        public VoteOption? Result { get; set; }

        public _ContentFeed_ContentViewModel()
        {
        }

        public _ContentFeed_ContentViewModel(Content record)
        {
            if (record != null)
            {
                if (record.GroupId.HasValue)
                {
                    GroupId = record.Group.Id;
                    GroupUrl = record.Group.Url;
                    GroupName = record.Group.Name;
                    GroupLogo = ImageService.GetImageUrl<Group>(record.Group.Logo);
                    ContentClass = "group";
                }
                else
                    ContentClass = "user";

                AuthorId = record.AuthorId;
                if (AuthorId.HasValue)
                {
                    AuthorFirstName = record.Author.FirstName;
                    AuthorSurname = record.Author.SurName;
                    AuthorPatronymic = record.Author.Patronymic;
                    AuthorAvatar = ImageService.GetImageUrl<User>(record.Author.Avatar);

                    if (record.Author.Address != null)
                        AuthorCity = record.Author.Address.City.Title;
                }
                /* убрали надпись, теперь числа достаточно
                 * CommentsCount = DeclinationService.OfNumber(record.Comments.Count(c => !c.IsHidden), "комментарий", "комментария", "комментариев");*/
                CommentsCountNum = record.Comments.Count(c => !c.IsHidden);
                Id = record.Id;
                IsDiscussionClosed = record.IsDiscussionClosed;
                if (GroupId.HasValue)
                    IsGroupAttached = record.IsGroupAttached(GroupId.Value);
                if (AuthorId.HasValue)
                    IsUserAttached = record.IsUserAttached(AuthorId.Value);
                ContentState = (ContentState)record.State;
                if (record.State == (byte)ContentState.Premoderated)
                    ContentStateAlert = "ожидает модерации";
                Url = record.GetUrl();

                if (record is Post)
                {
                    ContentClass += " post";
                    ContentType = record.GroupId.HasValue ? ContentViewType.GroupPost : ContentViewType.UserPost;
                }
                else if (record is Voting)
                {
                    IsFinished = (record as Voting).IsFinished;

                    if (record.PublishDate.HasValue)
                    {
                        var duration = (record as Voting).Duration;

                        StartDate = record.PublishDate.Value;
                        if (duration.HasValue)
                            EndDate = record.PublishDate.Value.AddDays(duration.Value);
                    }

                    if (record is Petition)
                    {
                        ContentClass += " petition";
                        ContentType = record.GroupId.HasValue ? ContentViewType.GroupPetition : ContentViewType.UserPetition;
                        ContentTypeName = "Петиция";
                    }
                    else if (record is Poll)
                    {
                        ContentClass += " poll";
                        ContentType = ContentViewType.Poll;
                        ContentTypeName = "Голосование";

                        var poll = record as Poll;
                        if (poll.PublishDate.HasValue)
                        {
                            if (UserContext.Current != null)
                            {
                                var blt = DataService.PerThread.BulletinSet.OfType<PollBulletin>()
                                    .SingleOrDefault(x => x.Owner.UserId == UserContext.Current.Id && x.PollId == poll.Id);
                                if (blt != null)
                                    Vote = (VoteOption)blt.Result;

                                if (poll.IsFinished)
                                    Result = (VoteOption)poll.Result;
                            }
                        }
                    }
                    else if (record is Election)
                    {
                        ContentClass += " election";
                        ContentType = ContentViewType.Election;
                        ContentTypeName = "Выборы";
                    }
                    else if (record is Survey)
                    {
                        ContentClass += " survey";
                        ContentType = ContentViewType.Survey;
                        ContentTypeName = "Опрос";
                    }
                }

                if (record.PublishDate.HasValue)
                    PostDate = record.PublishDate.Value;
                else
                    PostDate = record.CreationDate;

                Summary = TextHelper.CleanTags(record.Text);
                if (Summary.Length > ConstHelper.MiniSummaryLength)
                    Summary = Summary.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                Title = record.Title;

                Tags = new List<TagViewModel>();
                foreach (var tag in record.Tags.Take(10))
                    Tags.Add(new TagViewModel(tag));
            }
        }
    }
}