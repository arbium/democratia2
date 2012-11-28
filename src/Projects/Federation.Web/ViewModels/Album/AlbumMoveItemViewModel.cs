using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumMoveItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public AlbumItemType Type { get; set; }
        public Guid AlbumId { get; set; }
        public string AlbumTitle { get; set; }        

        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }

        public readonly IList<AlbumViewModel> Albums = new List<AlbumViewModel>();

        public AlbumMoveItemViewModel()
        {
        }

        public AlbumMoveItemViewModel(AlbumItem albumItem)
        {
            if (albumItem != null)
            {
                Id = albumItem.Id;
                Title = albumItem.Title;
                Type = (AlbumItemType)albumItem.Type;
                AlbumId = albumItem.AlbumId;
                AlbumTitle = albumItem.Album.Title;

                UserId = albumItem.Album.UserId;
                GroupId = albumItem.Album.GroupId;

                if (albumItem.Album.GroupId.HasValue)
                    Albums = albumItem.Album.Group.Albums.Select(x => new AlbumViewModel(x)).ToList();
                else if (albumItem.Album.UserId.HasValue)
                    Albums = albumItem.Album.User.Albums.Select(x => new AlbumViewModel(x)).ToList();
            }
        }
    }
}