using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _AuthorContentViewModel
    {
        public Guid AuthorId { get; set; }
        public List<_AuthorContent_ContentViewModel> AuthorContent = new List<_AuthorContent_ContentViewModel>();

        public _AuthorContentViewModel()
        {
        }

        public _AuthorContentViewModel(Guid authorId, Guid? excludedContentId = null, int amount = 3)
        {
            AuthorId = authorId;

            var contentList = DataService.PerThread.ContentSet.Where(x => x.AuthorId == authorId && x.Id != excludedContentId && x.State == (byte)ContentState.Approved).Take(amount).ToList();
            foreach (var content in contentList)
                AuthorContent.Add(new _AuthorContent_ContentViewModel(content));
        }

        public _AuthorContentViewModel(Guid authorId, Guid groupId, Guid? excludedContentId = null, int amount = 3)
        {
            AuthorId = authorId;

            var contentList = DataService.PerThread.ContentSet.Where(x => x.AuthorId == authorId && x.Id != excludedContentId && x.GroupId == groupId && x.State == (byte)ContentState.Approved).Take(amount).ToList();
            foreach (var content in contentList)
                AuthorContent.Add(new _AuthorContent_ContentViewModel(content));
        }
    }

    public class _AuthorContent_ContentViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }

        public _AuthorContent_ContentViewModel()
        {
        }

        public _AuthorContent_ContentViewModel(Content content)
        {
            if (content != null)
            {
                Id = content.Id;
                Title = content.Title;
                Url = content.GetUrl();

                if (content is Post)
                    ContentType = "Пост";
                else if (content is Poll)
                    ContentType = "Голосование";
                else if (content is Survey)
                    ContentType = "Опрос";
                else if (content is Petition)
                    ContentType = "Петиция";
                else if (content is Election)
                    ContentType = "Выборы";
            }
        }
    }
}

