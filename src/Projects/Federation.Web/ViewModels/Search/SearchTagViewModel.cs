using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class SearchTagViewModel
    {
        public Guid Id { get; set; }
        public string Tag { get; set; }

        public bool IsEmptyRequest { get; set; }
        public bool IsEmptyResult 
        {
            get
            {
                return Experts.Count == 0 && Groups.Count == 0 && Content.Count == 0;
            }
        }

        public readonly IList<SearchTag_ExpertViewModel> Experts = new List<SearchTag_ExpertViewModel>();
        public readonly IList<SearchTag_GroupViewModel> Groups = new List<SearchTag_GroupViewModel>();
        public readonly IList<SearchTag_ContentViewModel> Content = new List<SearchTag_ContentViewModel>();

        public SearchTagViewModel()
        {
        }

        public SearchTagViewModel(Tag tag)
        {
            if (tag != null)
            {
                Id = tag.Id;
                Tag = tag.Title;
                IsEmptyRequest = false;

                Experts = tag.Experts.Select(e => new SearchTag_ExpertViewModel(e)).ToList();
                if (tag.Group!=null)
                    Groups.Add(new SearchTag_GroupViewModel(tag.Group));
                Content = tag.Contents.Where(p => p.State == (byte)ContentState.Approved).Select(c => new SearchTag_ContentViewModel(c)).ToList();
            }
        }
    }

    public class SearchTag_ContentViewModel
    {
        public string AuthorName { get; set; }
        public Guid? AuthorId { get; set; }
        public string AuthorAvatar { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public ContentViewType ContentType { get; set; }
        public string ContentClass { get; set; }
        public ContentState ContentState { get; set; }

        public SearchTag_ContentViewModel()
        {
        }

        public SearchTag_ContentViewModel(Content content)
        {
            if (content != null)
            {
                if (content.AuthorId.HasValue)
                {
                    AuthorName = content.Author.FullName;
                    AuthorId = content.AuthorId;
                    AuthorAvatar = ImageService.GetImageUrl<User>(content.Author.Avatar);
                }

                if (content.GroupId.HasValue)
                {
                    GroupUrl = content.Group.Url;
                    GroupName = content.Group.Name;
                    GroupLogo = content.Group.Logo;
                }

                ContentState = (ContentState)content.State;

                Url = content.GetUrl();
                Title = content.Title;
                Summary = TextHelper.CleanTags(content.Text);
                if (Summary.Length > 200)
                    Summary = Summary.Substring(0, 200) + "...";

                if (content is Post)
                {
                    if (content.GroupId.HasValue)
                    {
                        ContentClass = "group";
                        ContentType = ContentViewType.GroupPost;
                    }
                    else
                    {
                        ContentClass = "user";
                        ContentType = ContentViewType.UserPost;
                    }

                    ContentClass += " post";
                }
                else if (content is Petition)
                {
                    if (content.GroupId.HasValue)
                    {
                        ContentClass = "group";
                        ContentType = ContentViewType.GroupPetition;
                    }
                    else
                    {
                        ContentClass = "user";
                        ContentType = ContentViewType.UserPetition;
                    }

                    ContentClass += " petition";
                }
                else if (content is Poll)
                {
                    ContentClass = "poll";
                    ContentType = ContentViewType.Poll;
                }
                else if (content is Election)
                {
                    ContentClass = "election";
                    ContentType = ContentViewType.Election;
                }
            }
        }
    }
}