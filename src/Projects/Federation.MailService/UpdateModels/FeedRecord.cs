using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.MailService
{
    public class FeedRecord
    {
        private DateTime _beginDate;
        private DateTime _endDate;

        private Guid _userId;

        private List<SubscriptionContent> _content = new List<SubscriptionContent>();
        private List<MessageUpdate> _commentNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _groupMemberNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _groupModeratorNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _pollNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _electionNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _surveyNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _petitionNotices = new List<MessageUpdate>();
        private List<MessageUpdate> _systemMessages = new List<MessageUpdate>();
        private List<MessageUpdate> _privateMessages = new List<MessageUpdate>();

        public bool PackFailed { get; set; }

        public IList<GroupSchema> Groups { get; set; }

        public List<SubscriptionContent> Content { get { return _content; } }
        public List<MessageUpdate> CommentNotices { get { return _commentNotices; } }
        public List<MessageUpdate> GroupMemberNotices { get { return _groupMemberNotices; } }
        public List<MessageUpdate> GroupModeratorNotices { get { return _groupModeratorNotices; } }
        public List<MessageUpdate> PollNotices { get { return _pollNotices; } }
        public List<MessageUpdate> ElectionNotices { get { return _electionNotices; } }
        public List<MessageUpdate> SurveyNotices { get { return _surveyNotices; } }
        public List<MessageUpdate> PetitionNotices { get { return _petitionNotices; } }
        public List<MessageUpdate> SystemMessages { get { return _systemMessages; } }
        public List<MessageUpdate> PrivateMessages { get { return _privateMessages; } }

        public bool IsThereUpdates
        {
            get
            {
                return (Content.Count > 0 || CommentNotices.Count > 0 || GroupModeratorNotices.Count > 0 || PollNotices.Count > 0 || PetitionNotices.Count > 0 || SystemMessages.Count > 0 || PrivateMessages.Count > 0);
            }
        }

        public FeedRecord(Guid userId, DateTime beginDate, DateTime endDate)
        {
            if (userId == Guid.Empty)
                throw new Exception("Нельзя создать почтовую запись для Guid.Empty");

            if (beginDate >= endDate)
                throw new Exception("Неверные параметры диапазона выборки рассылаемого контента");

            _userId = userId;
            _beginDate = beginDate;
            _endDate = endDate;
        }

        public void PackMailRecord()
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == _userId);
            if (user == null)
                return;

            var groupfeed = (from _group in user.SubscriptionGroups
                             join content in DataService.PerThread.ContentSet on _group.Id equals content.GroupId
                             where content.State == (byte)ContentState.Approved && _beginDate <= content.PublishDate && content.PublishDate < _endDate && !user.BlackList.Contains(content.Author)
                             where user.Id != content.AuthorId
                             select content).ToList();
            var userfeed = (from _user in user.SubscriptionUsers
                            join content in DataService.PerThread.ContentSet on _user.Id equals content.AuthorId
                            where content.State == (byte)ContentState.Approved && _beginDate <= content.PublishDate && content.PublishDate < _endDate
                            where user.Id != content.AuthorId
                            select content).ToList();

            var feed = groupfeed.Union(userfeed).Distinct().Where(x => x.PublishDate.HasValue).OrderBy(x => x.GroupId).ThenByDescending(c => c.PublishDate);

            _content.AddRange(feed.Select(x => new SubscriptionContent(x)));

            Groups = _content.Select(x => new GroupSchema { Name = x.GroupName, Url = x.GroupUrl }).Distinct().OrderBy(x => x.Name).ToList();

            var messages = user.InboxMessages.Where(m => !m.IsRead && !m.IsDeletedByRecipient && _beginDate <= m.Date && m.Date < _endDate).ToList();

            CommentNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.CommentNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            GroupMemberNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.GroupMemberNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            GroupModeratorNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.GroupModeratorNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            PollNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.PollNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            SurveyNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.SurveyNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            ElectionNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.ElectionNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            PetitionNotices.AddRange(messages.Where(m => m.Type == (byte)MessageType.PetitionNotice).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            SystemMessages.AddRange(messages.Where(m => m.Type == (byte)MessageType.SystemMessage).Select(x => new MessageUpdate(x)).OrderByDescending(x => x.Date));
            PrivateMessages.AddRange(messages.Where(m => m.Type == (byte)MessageType.PrivateMessage).Select(x => new PrivateMessageUpdate(x)).OrderByDescending(x => x.Date));
        }
    }

    public class GroupSchema
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class MessageUpdate
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }

        public MessageUpdate(Message message)
        {
            Id = message.Id;
            Date = message.Date;
            Text = message.Text;
        }
    }

    public class PrivateMessageUpdate : MessageUpdate
    {
        public string AuthorAvatar { get; set; }
        public string AuthorUrl { get; set; }
        public string AuthorName { get; set; }
        public string AnswerUrl { get; set; }

        public PrivateMessageUpdate(Message message) : base(message)
        {
            var author = (User)message.Author;
            AuthorAvatar = ImageService.GetImageUrl<User>(author.Avatar, false);
            AuthorUrl = UrlHelper.GetUrl<User>(author.Id, false);
            AuthorName = author.SurName + " " + author.FirstName;
            AnswerUrl = ConstHelper.AppUrl + "/user/sendmessage?replyTo=" + Id;
        }
    }

    public class SubscriptionContent
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

        public string CommentsCount { get; set; }
        public bool IsDiscussionClosed { get; set; }

        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public SubscriptionContentType ContentType { get; set; }
        public string ContentTypeName { get; set; }
        public ContentState ContentState { get; set; }
        public string ContentStateAlert { get; set; }

        public DateTime PostDate { get; set; }

        public SubscriptionContent(Content content)
        {
            if (content != null)
            {
                AuthorId = content.AuthorId;
                AuthorFirstName = content.Author.FirstName;
                AuthorSurname = content.Author.SurName;
                AuthorPatronymic = content.Author.Patronymic;
                AuthorAvatar = ImageService.GetImageUrl<User>(content.Author.Avatar, false);
                if (content.Author.Address != null)
                    AuthorCity = content.Author.Address.City.Title;
                CommentsCount = DeclinationService.OfNumber(content.Comments.Count(c => !c.IsHidden), "комментарий", "комментария", "комментариев");
                Id = content.Id;
                IsDiscussionClosed = content.IsDiscussionClosed;
                ContentState = (ContentState)content.State;

                if (content.State == (byte)ContentState.Premoderated)
                    ContentStateAlert = "ожидает модерации";

                if (content is Post)
                {
                    var post = (content as Post);

                    if (post.GroupId.HasValue)
                    {
                        GroupId = post.Group.Id;
                        GroupUrl = post.Group.Url;
                        GroupName = post.Group.Name;
                        GroupLogo = ImageService.GetImageUrl<Group>(post.Group.Logo, false);
                        ContentType = SubscriptionContentType.GroupPost;
                        Url = content.GetUrl(false);
                        ContentTypeName = "";
                    }
                    else
                    {
                        ContentType = SubscriptionContentType.UserPost;
                        Url = content.GetUrl(false);
                        ContentTypeName = "";
                    }
                }
                else if (content is Voting)
                {
                    var voting = (content as Voting);

                    if (voting.GroupId != null)
                    {
                        GroupId = voting.Group.Id;
                        GroupUrl = voting.Group.Url;
                        GroupName = voting.Group.Name;
                        GroupLogo = ImageService.GetImageUrl<Group>(voting.Group.Logo, false);

                        if (voting is Petition)
                        {
                            ContentType = SubscriptionContentType.GroupPetition;
                            Url = content.GetUrl(false);
                            ContentTypeName = "Петиция";
                        }
                        else if (voting is Poll)
                        {
                            ContentType = SubscriptionContentType.Poll;
                            Url = content.GetUrl(false);
                            ContentTypeName = "Голосование";
                        }
                        else if (voting is Survey)
                        {
                            ContentType = SubscriptionContentType.Survey;
                            Url = content.GetUrl(false);
                            ContentTypeName = "Опрос";
                        }
                    }
                    else
                    {
                        if (voting is Petition)
                        {
                            ContentType = SubscriptionContentType.UserPetition;
                            Url = content.GetUrl(false);
                            ContentTypeName = "Петиция";
                        }
                    }
                }

                if (content.PublishDate.HasValue)
                    PostDate = content.PublishDate.Value;
                else
                    PostDate = content.CreationDate;

                Summary = TextHelper.CleanTags(content.Text);
                if (Summary.Length > ConstHelper.SummaryLength)
                    Summary = Summary.Substring(0, ConstHelper.SummaryLength) + "...";

                Title = content.Title;
            }
        }
    }
}