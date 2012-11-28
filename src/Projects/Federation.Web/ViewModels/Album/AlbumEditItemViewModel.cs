using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumEditItemViewModel
    {
        public Guid Id { get; set; }
        public AlbumItemType Type { get; set; }

        public Guid AlbumId { get; set; }
        public string AlbumTitle { get; set; }

        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public AlbumEditItemViewModel()
        {
        }

        public AlbumEditItemViewModel(AlbumItem albumItem)
        {
            if (albumItem != null)
            {
                Id = albumItem.Id;
                Type = (AlbumItemType)albumItem.Type;

                AlbumId = albumItem.AlbumId;
                AlbumTitle = albumItem.Album.Title;

                Title = albumItem.Title;
                Description = albumItem.Description;

                UserId = albumItem.Album.UserId;
                GroupId = albumItem.Album.GroupId;
            }
        }
    }
}