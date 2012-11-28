using System;
using System.Linq;
using Federation.Core;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Federation.Web.ViewModels
{
    public class _TagContentViewModel
    {
        public Guid ContentId { get; set; }
        public List<_TagContent_ContentViewModel> TagContent = new List<_TagContent_ContentViewModel>();

        public _TagContentViewModel()
        {
        }

        public _TagContentViewModel(Guid contentId, int amount = 3)
        {
            ContentId = contentId;
            var tags = DataService.PerThread.TagSet.Where(t => t.Contents.Any(c => c.Id == contentId)).Select(t => t.Id).ToList();
            var filteredcontent = DataService.PerThread.ContentSet.Where(c => c.Id != contentId && c.Tags.Any(t => tags.Contains(t.Id))).OrderByDescending(x => x.Tags.Select(t => t.Id).Intersect(tags).Count()).Take(amount);
            foreach (var content in filteredcontent)
            {
                TagContent.Add(new _TagContent_ContentViewModel(content));
            }
        }
    }

    public class  _TagContent_ContentViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }

        public _TagContent_ContentViewModel()
        {
        }

        public _TagContent_ContentViewModel(Content content)
        {
            if (content != null)
            {
                Id = content.Id;
                Title = content.Title;
                Url = content.GetUrl();
            }
            if (content is Post)
            {
                ContentType = "Пост";
            }
            else if (content is Poll)
            {
                ContentType = "Голосование";
            }
            else if (content is Survey)
            {
                ContentType = "Опрос";
            }
            else if (content is Petition)
            {
                ContentType = "Петиция";
            }
            else if (content is Election)
            {
                ContentType = "Выборы";
            }
        }
    }
}

