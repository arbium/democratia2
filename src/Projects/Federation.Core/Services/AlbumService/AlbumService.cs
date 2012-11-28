using System;
using System.IO;
using System.Linq;
using System.Web;
using Sgml;

namespace Federation.Core
{
    public static class AlbumService
    {
        public static Album CreateAlbum(Guid? groupId, Guid? userId, string title, string description, bool isOpen)
        {
            var album = new Album
            {
                GroupId = groupId,
                UserId = userId,
                Title = title,
                Description = description,
                ChangeDate = DateTime.Now,
                IsOpen = isOpen
            };

            DataService.PerThread.AlbumSet.AddObject(album);
            DataService.PerThread.SaveChanges();

            return album;
        }

        public static Album EditAlbum(UserContainer currContext, Guid albumId, string title, string description, bool isOpen)
        {
            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == albumId);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            if (currContext != null)
            {
                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value, true);
                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя редактировать чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            album.Title = title;
            album.Description = description;
            album.IsOpen = isOpen;
            album.ChangeDate = DateTime.Now;

            DataService.PerThread.SaveChanges();

            return album;
        }

        public static void DeleteAlbum(UserContainer currContext, Album album)
        {            
            if (currContext != null)
            {
                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value, true);
                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут удалить альбом группы");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя удалить чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            DataService.PerThread.AlbumSet.DeleteObject(album);
            DataService.PerThread.SaveChanges();
        }

        public static AlbumItem AddImage(UserContainer currContext, Guid albumId, string title, string description, string fileName)
        {
            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == albumId);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор");            

            if (currContext != null)
            {
                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value);
                    if (gm == null)
                        throw new BusinessLogicException("Вы не состоите в группе");

                    if (album.IsOpen)
                    {
                        if (!(gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator))
                            throw new BusinessLogicException("Только члены группы могут добавлять изображения в альбом");
                    }
                    else if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут добавлять изображения в альбом");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя добавлять изображения в чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            var albumItem = new AlbumItem
            {
                AlbumId = albumId,
                Title = title,
                Description = description,
                Type = (byte)AlbumItemType.Image,
                Src = fileName,
                CreationDate = DateTime.Now
            };

            DataService.PerThread.AlbumItemSet.AddObject(albumItem);
            DataService.PerThread.LoadProperty<AlbumItem>(albumItem, x => x.Album);

            albumItem.Album.ChangeDate = DateTime.Now;

            DataService.PerThread.SaveChanges();

            return albumItem;
        }

        public static AlbumItem AddVideo(UserContainer currContext, Guid albumId, string title, string description, string url, string embedCode)
        {
            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == albumId);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessLogicException("Не указано название видео");

            if (currContext != null)
            {
                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value);
                    if (gm == null)
                        throw new BusinessLogicException("Вы не состоите в группе");

                    if (album.IsOpen)
                    {
                        if (!(gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator))
                            throw new BusinessLogicException("Только члены группы могут добавлять видео в альбом");
                    }
                    else if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут добавлять видео в альбом");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя добавлять видео в чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            string src;

            if (!string.IsNullOrWhiteSpace(embedCode))
            {
                using (var sgml = new SgmlReader())
                {
                    sgml.InputStream = new StringReader(embedCode);
                    sgml.Read();
                    src = sgml.GetAttribute("src");
                }
            }
            else if (!string.IsNullOrWhiteSpace(url))
            {
                var uri = new Uri(url);
                src = url; // TODO: мб лучше regexp для вычленения src

                switch (uri.Host.Replace("www.", string.Empty))
                {
                    case "youtube.com":
                        //src = uri.Scheme + "://" + uri.Host + "/embed/" + HttpUtility.ParseQueryString(uri.Query).GetValues("v").First(); // это для iframe
                        src = uri.Scheme + "://" + uri.Host + "/v/" + HttpUtility.ParseQueryString(uri.Query).GetValues("v").First();
                        break;

                    case "youtu.be":
                        //src = uri.Scheme + "://youtube.com/embed/" + uri.Segments[1]; // это для iframe
                        src = uri.Scheme + "://youtube.com/v/" + uri.Segments[1];
                        break;

                    /*case "vimeo.com":
                        src = uri.Scheme + "://" + "player." + uri.Host + "/video" + uri.PathAndQuery;
                        break;

                    case "dailymotion.com":
                        var query = uri.Fragment.Replace("#", string.Empty);
                        src = uri.Scheme + "://" + uri.Host + "/embed/video/" + HttpUtility.ParseQueryString(query).GetValues("videoId").First();
                        break;*/

                    case "e2-e4.tv":
                        src = uri.Scheme + "://" + uri.Host + uri.PathAndQuery + "/swf/player2.swf";
                        break;

                    case "e2e4.tv":
                        src = uri.Scheme + "://" + uri.Host + uri.PathAndQuery + "/swf/player2.swf";
                        break;
                }
            }
            else
                throw new BusinessLogicException("Источник видео не указан");            

            var albumItem = new AlbumItem
            {
                AlbumId = albumId,
                Title = title,
                Description = description,
                Type = (byte)AlbumItemType.Video,
                Src = src,
                CreationDate = DateTime.Now
            };

            DataService.PerThread.AlbumItemSet.AddObject(albumItem);
            DataService.PerThread.LoadProperty(albumItem, x => x.Album);            

            albumItem.Album.ChangeDate = DateTime.Now;
            
            DataService.PerThread.SaveChanges();

            return albumItem;
        }

        public static AlbumItem EditItem(UserContainer currContext, Guid itemId, string title, string description)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == itemId);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор");
            
            if (currContext != null)
            {
                var album = albumItem.Album;

                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value);
                    if (gm == null)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");

                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя редактировать чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            albumItem.Title = title;
            albumItem.Description = description;
            albumItem.Album.ChangeDate = DateTime.Now;

            DataService.PerThread.SaveChanges();

            return albumItem;
        }

        public static AlbumItem MoveItem(UserContainer currContext, Guid itemId, Guid albumId)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == itemId);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            if (currContext != null)
            {
                var album = albumItem.Album;

                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value);
                    if (gm == null)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");

                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя редактировать чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            albumItem.AlbumId = albumId;
            albumItem.Album.ChangeDate = DateTime.Now;

            DataService.PerThread.SaveChanges();

            return albumItem;
        }

        public static Album DeleteItem(UserContainer currContext, Guid itemId)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == itemId);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            var album = albumItem.Album;

            if (currContext != null)
            {
                if (album.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currContext.Id, album.GroupId.Value);
                    if (gm == null)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");

                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Только модераторы могут редактировать альбом группы");
                }
                else if (album.UserId.HasValue)
                {
                    if (album.UserId != currContext.Id)
                        throw new BusinessLogicException("Нельзя редактировать чужой альбом");
                }
                else
                    throw new BusinessLogicException("Альбом ни к чему не привязан");
            }

            if (albumItem.Type == (byte)AlbumItemType.Image)
                ImageService.DeleteImage<AlbumItem>(albumItem.Src);

            albumItem.Album.ChangeDate = DateTime.Now;
            albumItem.Likes.Clear();

            DataService.PerThread.AlbumItemSet.DeleteObject(albumItem);
            DataService.PerThread.SaveChanges();

            return album;
        }
    }
}