using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _ContentLayoutViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public DateTime? PublishDate { get; set; }
        public ContentState State { get; set; }
        public IList<TagViewModel> Tags { get; set; }        
        public ContentType Type { get; set; }
        public string TypeName { get; set; }
        public object Body { get; set; }

        public bool IsDiscussionClosed { get; set; }
        public bool IsAttached { get; set; }
        public bool IsAuthor { get; set; }
        public bool IsPMAllow { get; set; }

        public bool IsGroupMember { get; set; }
        public bool IsApprovedMember { get; set; }
        public bool IsModerator { get; set; }
        public bool IsContentModerated { get; set; }

        public _LikesViewModel Likes { get; set; }
        public _CommentsBlockViewModel Comments { get; set; }

        public bool IsDelegateButtonEnabled { get; set; }

        public _ContentLayoutViewModel()
        {           
        }

        public _ContentLayoutViewModel(Content content, Guid? userId, bool? invert = null)
        {
            if (content != null)
            {
                Id = content.Id;
                Title = content.Title;
                PublishDate = content.PublishDate;
                IsDiscussionClosed = content.IsDiscussionClosed;
                State = (ContentState)content.State;
                Tags = content.Tags.Select(x => new TagViewModel(x)).ToList();
                Likes = new _LikesViewModel(content, userId);
                Comments = new _CommentsBlockViewModel(content, invert);

                if (content is Post)
                {
                    Type = ContentType.Post;
                    Body = new _PostViewModel(content as Post);
                    TypeName = "Пост";
                }
                else if (content is Petition)
                {
                    Type = ContentType.Petition;
                    Body = new _PetitionViewModel(content as Petition, userId);
                    TypeName = "Петиция";
                }
                else if (content is Poll)
                {
                    Type = ContentType.Poll;
                    Body = new Group_PollViewModel(content as Poll);
                    TypeName = "Голосование";
                }
                else if (content is Election)
                {
                    Type = ContentType.Election;
                    Body = new Group_ElectionViewModel(content as Election, userId);
                    TypeName = "Выборы";
                }
                else if (content is Survey)
                {
                    Type = ContentType.Survey;
                    Body = new Group_SurveyViewModel(content as Survey, userId);
                    TypeName = "Опрос";
                }

                if (content.GroupId.HasValue)
                {
                    GroupId = content.GroupId.Value;
                    GroupUrl = content.Group.Url;
                    IsAttached = content.IsGroupAttached(content.GroupId.Value);

                    if (UserContext.Current != null)
                    {
                        IsGroupMember = UserContext.Current.IsUserInGroup(GroupId.Value);
                        IsApprovedMember = UserContext.Current.IsUserApprovedInGroup(GroupId.Value);
                        IsModerator = UserContext.Current.IsUserModeratorInGroup(GroupId.Value);
                        IsContentModerated = content.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);

                        if (content.AuthorId.HasValue && UserContext.Current.IsUserApprovedInGroup(GroupId.Value))
                        {
                            var groupMemberAuthor = GroupService.UserInGroup(content.AuthorId.Value, content.Group);

                            Expert expert = null;
                            if (groupMemberAuthor != null)
                                expert = groupMemberAuthor.Expert;

                            var groupMember = GroupService.UserInGroup(UserContext.Current.Id, content.Group);

                            if (expert != null)
                            {                                
                                var contentTags = content.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic);
                                var expertTags = expert.Tags.Intersect(contentTags); // Тэги поста, по которым автор эксперт
                                var delegatedTags = groupMember.ExpertVotes.Select(x => x.Tag);
                                var opportunityDelegateTags = expertTags.ToList().Except(delegatedTags);

                                if (opportunityDelegateTags.Any())
                                    IsDelegateButtonEnabled = true;
                            }
                        }
                    }
                }

                if (content.AuthorId.HasValue)
                {
                    AuthorId = content.AuthorId;
                    AuthorName = content.Author.FullName;
                    AuthorAvatar = ImageService.GetImageUrl<User>(content.Author.Avatar);

                    if (userId.HasValue)
                    {
                        IsAuthor = userId == content.AuthorId;
                        IsPMAllow = content.Author.BlackList.Count(x => x.Id == userId.Value) != 0;
                    }
                }
            }
        }
    }
}