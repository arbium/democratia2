using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AlbumsViewModel
    {
        public Guid? UserId { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }

        public IList<AlbumViewModel> Albums { get; set; }
        public IList<AlbumViewModel> GroupAlbums { get; set; }
        public IList<AlbumViewModel> UserAlbums { get; set; }
        
        public AlbumCreateEditViewModel CreateGroupAlbum { get; set; }
        public AlbumCreateEditViewModel CreateUserAlbum { get; set; }
        
        public AlbumsViewModel()
        {
            Albums = new List<AlbumViewModel>();
            GroupAlbums = new List<AlbumViewModel>();
            UserAlbums = new List<AlbumViewModel>();

            CreateGroupAlbum = new AlbumCreateEditViewModel();
            CreateUserAlbum = new AlbumCreateEditViewModel();
        }

        public AlbumsViewModel(User user)
        {
            GroupAlbums = new List<AlbumViewModel>();
            UserAlbums = new List<AlbumViewModel>();

            CreateGroupAlbum = new AlbumCreateEditViewModel();
            CreateUserAlbum = new AlbumCreateEditViewModel();

            if (user != null)
            {
                UserId = user.Id;
                UserAlbums = user.Albums.Select(x => new AlbumViewModel(x)).ToList();

                CreateUserAlbum = new AlbumCreateEditViewModel(user);
            }

            Albums = UserAlbums;
        }

        public AlbumsViewModel(Group group)
        {
            GroupAlbums = new List<AlbumViewModel>();
            UserAlbums = new List<AlbumViewModel>();

            CreateGroupAlbum = new AlbumCreateEditViewModel();
            CreateUserAlbum = new AlbumCreateEditViewModel();

            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupAlbums = group.Albums.Select(x => new AlbumViewModel(x)).ToList();

                CreateGroupAlbum = new AlbumCreateEditViewModel(group);
            }

            Albums = GroupAlbums;
        }

        public AlbumsViewModel(User user, Group group)
        {
            GroupAlbums = new List<AlbumViewModel>();
            UserAlbums = new List<AlbumViewModel>();

            CreateGroupAlbum = new AlbumCreateEditViewModel();
            CreateUserAlbum = new AlbumCreateEditViewModel();

            if (group != null)
            {
                GroupId = group.Id;
                GroupAlbums = group.Albums.Select(x => new AlbumViewModel(x)).ToList();

                CreateGroupAlbum = new AlbumCreateEditViewModel(group);
            }

            if (user != null)
            {
                UserId = user.Id;
                UserAlbums = user.Albums.Select(x => new AlbumViewModel(x)).ToList();

                CreateUserAlbum = new AlbumCreateEditViewModel(user);
            }

            Albums = GroupAlbums.Union(UserAlbums).ToList();
        }
    }
}