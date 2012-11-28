using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    [Flags]
    public enum SearchBy : byte
    {
        FullText = 1,
        Tag = 2
    }

    [Flags]
    public enum SearchWhat : byte
    {
        Groups = 1,
        Content = 2
    }

    public class SearchIndexViewModel
    {
        public string Query { get; set; }

        public readonly IList<SearchIndex_ResultViewModel> Content = new Collection<SearchIndex_ResultViewModel>();
        public readonly IList<SearchIndex_ResultViewModel> Groups = new Collection<SearchIndex_ResultViewModel>();

        public SearchIndexViewModel()
        {
        }

        public SearchIndexViewModel(string query, SearchBy searchBy = SearchBy.FullText | SearchBy.Tag, SearchWhat searchWhat = SearchWhat.Groups | SearchWhat.Content)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                Query = query;

                if (searchBy.HasFlag(SearchBy.FullText))
                {
                    if (searchWhat.HasFlag(SearchWhat.Content))
                    {
                        var comments = DataService.PerThread.GetCommentsByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x));
                        var posts = DataService.PerThread.GetPostsByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x));
                        var petitions = DataService.PerThread.GetPetitionsByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x));
                        var polls = DataService.PerThread.GetPollsByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x));
                        var surveys = DataService.PerThread.GetSurveysByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x));

                        Content = comments.Union(posts.Union(petitions.Union(polls.Union(surveys)))).ToList();
                    }

                    if (searchWhat.HasFlag(SearchWhat.Groups))
                        Groups = DataService.PerThread.GetGroupsByFullText(query, 100).Select(x => new SearchIndex_ResultViewModel(x)).ToList();
                }
                if (searchBy.HasFlag(SearchBy.Tag))
                {
                    var tags = DataService.PerThread.TagSet.Where(x => x.Title.Contains(query) || query.Contains(x.Title)).ToList();

                    if (searchWhat.HasFlag(SearchWhat.Content))
                    {
                        IEnumerable<SearchIndex_ResultViewModel> tagedContent = new List<SearchIndex_ResultViewModel>();
                        foreach (var c in tags.Where(x => x.Contents.Count != 0).Select(x => x.Contents))
                            tagedContent = tagedContent.Union(c.Select(x => new SearchIndex_ResultViewModel(x)).ToList());

                        Content = Content.Union(tagedContent).ToList();
                    }

                    if (searchWhat.HasFlag(SearchWhat.Groups))
                    {
                        var tagedGroups = tags.Where(x => x.GroupId.HasValue).Select(x => new SearchIndex_ResultViewModel(x.Group)).ToList();
                        Groups = Groups.Union(tagedGroups).ToList();
                    }
                }
            }
        }
    }

    public class SearchIndex_ResultViewModel
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        public readonly IList<TagViewModel> Tags = new List<TagViewModel>();

        public SearchIndex_ResultViewModel()
        {
        }

        /// <param name="result">Контент/Комментарий/Группа</param>
        public SearchIndex_ResultViewModel(object result)
        {
            if (result != null)
            {
                if (result is Comment)
                {
                    var comment = result as Comment;

                    Type = "комментарий";
                    Image = ImageService.GetImageUrl<User>(comment.User.Avatar);
                    Title = comment.Content.Title;

                    Text = TextHelper.CleanTags(comment.Text);
                    if (Text.Length > ConstHelper.MiniSummaryLength)
                        Text = Text.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                    Url = comment.Content.GetUrl(comment);

                    Tags = comment.Content.Tags.Select(x => new TagViewModel(x)).ToList();
                }
                else if (result is Post)
                {
                    var post = result as Post;

                    Type = "пост";
                    Title = post.Title;
                    Url = UrlHelper.GetUrl<Post>(post.Id);

                    Text = TextHelper.CleanTags(post.Text);
                    if (Text.Length > ConstHelper.MiniSummaryLength)
                        Text = Text.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                    if (post.GroupId.HasValue)
                        Image = ImageService.GetImageUrl<Group>(post.Group.Logo);
                    else if (post.AuthorId.HasValue)
                        Image = ImageService.GetImageUrl<User>(post.Author.Avatar);

                    Tags = post.Tags.Select(x => new TagViewModel(x)).ToList();
                }
                else if (result is Voting)
                {
                    var voting = result as Voting;

                    Type = "голосование";
                    Title = voting.Title;
                    Url = UrlHelper.GetUrl<Voting>(voting.Id);

                    Text = TextHelper.CleanTags(voting.Text);
                    if (Text.Length > ConstHelper.MiniSummaryLength)
                        Text = Text.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                    if (voting.GroupId.HasValue)
                        Image = ImageService.GetImageUrl<Group>(voting.Group.Logo);
                    else if (voting.AuthorId.HasValue)
                        Image = ImageService.GetImageUrl<User>(voting.Author.Avatar);

                    Tags = voting.Tags.Select(x => new TagViewModel(x)).ToList();
                }
                else if (result is Group)
                {
                    var group = result as Group;

                    Type = "группа";
                    Title = group.Name;
                    Image = ImageService.GetImageUrl<Group>(group.Logo);
                    Url = UrlHelper.GetUrl<Group>(group.Url);

                    Text = TextHelper.CleanTags(group.Summary);
                    if (Text.Length > ConstHelper.MiniSummaryLength)
                        Text = Text.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                    Tags = group.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic).Select(x => new TagViewModel(x)).ToList();
                }
            }
        }
    }
}