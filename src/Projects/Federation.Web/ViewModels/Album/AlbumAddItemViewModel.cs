using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumAddItemViewModel
    {
        public Guid AlbumId { get; set; }
        public string AlbumTitle { get; set; }

        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Ссылка")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Display(Name = "Код вставки")]
        [DataType(DataType.Html)]
        [AllowHtml]
        public string EmbedCode { get; set; }

        public AlbumAddItemViewModel()
        {
        }

        public AlbumAddItemViewModel(Album album)
        {
            if (album != null)
            {
                AlbumId = album.Id;
                AlbumTitle = album.Title;

                UserId = album.UserId;
                GroupId = album.GroupId;
            }
        }
    }
}