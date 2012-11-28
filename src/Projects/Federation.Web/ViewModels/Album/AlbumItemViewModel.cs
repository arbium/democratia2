using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumItemViewModel
    {
        public Guid Id { get; set; }
        public Guid AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string AlbumController { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlbumItemType Type { get; set; }
        public string Src { get; set; }
        public DateTime CreationDate { get; set; }

        public Guid? AlbumUserId { get; set; }
        public Guid? AlbumGroupId { get; set; }
        
        public AlbumItemViewModel()
        {
        }

        public AlbumItemViewModel(AlbumItem albumItem)
        {
            if (albumItem != null)
            {
                Id = albumItem.Id;
                AlbumId = albumItem.AlbumId;
                AlbumTitle = albumItem.Album.Title;
                AlbumController = albumItem.Album.Controller;
                Title = albumItem.Title;
                Description = albumItem.Description;
                CreationDate = albumItem.CreationDate;
                Type = (AlbumItemType)albumItem.Type;
                switch ((AlbumItemType)albumItem.Type)
                {
                    case AlbumItemType.Image:
                        Src = ImageService.GetImageUrl<AlbumItem>(albumItem.Src);
                        break;
                        
                    case AlbumItemType.Video:
                        Src = albumItem.Src;
                        break;
                }                                

                AlbumUserId = albumItem.Album.UserId;
                AlbumGroupId = albumItem.Album.GroupId;
            }
        }
    }
}