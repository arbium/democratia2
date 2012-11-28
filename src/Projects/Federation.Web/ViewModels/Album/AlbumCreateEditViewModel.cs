using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumCreateEditViewModel
    {
        public Guid AlbumId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }

        [Display(Name = "Заголовок")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Члены группы могут добавлять контент")]
        public bool IsOpen { get; set; }

        public AlbumCreateEditViewModel()
        {
        }

        public AlbumCreateEditViewModel(User user)
        {
            if (user != null)
                UserId = user.Id;
        }

        public AlbumCreateEditViewModel(Group group)
        {
            if (group != null)
                GroupId = group.Id;
        }

        public AlbumCreateEditViewModel(Album album)
        {
            if (album != null)
            {
                AlbumId = album.Id;
                Title = album.Title;
                Description = album.Description;
                IsOpen = album.IsOpen;

                GroupId = album.GroupId;
                UserId = album.UserId;
            }
        }
    }
}