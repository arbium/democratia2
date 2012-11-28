using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumViewModel
    {
        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool IsOpen { get; set; }
        public bool IsEmpty { get; set; }

        public readonly IList<AlbumItemViewModel> Items = new List<AlbumItemViewModel>();
        public readonly AlbumAddItemViewModel AddItem = new AlbumAddItemViewModel();
        
        public AlbumViewModel()
        {
        }

        public AlbumViewModel(Album album)
        {
            if (album == null)
                return;

            Id = album.Id;
            GroupId = album.GroupId;
            UserId = album.UserId;
            Title = album.Title;
            Description = album.Description;
            ChangeDate = album.ChangeDate;
            IsOpen = album.IsOpen;
            IsEmpty = album.Items.Count == 0;

            Items = album.Items.Select(x => new AlbumItemViewModel(x)).ToList();
            AddItem = new AlbumAddItemViewModel(album);
        }
    }
}