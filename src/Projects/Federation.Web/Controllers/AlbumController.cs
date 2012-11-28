using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class AlbumController : MainController
    {        
        public ActionResult Create(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            AlbumCreateEditViewModel model;

            if (string.IsNullOrEmpty(id))
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
                if (user == null)
                    throw new BusinessLogicException("Перезайдите");

                model = new AlbumCreateEditViewModel(user);
            }
            else
            {
                var group = GroupService.GetGroupByLabelOrId(id);

                var gm = GroupService.UserInGroup(UserContext.Current.Id, group, true);

                if (gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Только модераторы могут создавать альбомы в группах");

                model = new AlbumCreateEditViewModel(group);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AlbumCreateEditViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);
            
            var album = AlbumService.CreateAlbum(model.GroupId, model.UserId, model.Title, model.Description, model.IsOpen);

            if (Request.UrlReferrer != null && Request.UrlReferrer.Segments[2] == "albumsforeditor")
                return RedirectToAction("foreditor", "album", new { id = album.Id });

            return RedirectToAction("index", new { id = album.Id });
        }

        public ActionResult Edit(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            if (album.GroupId.HasValue)
            {
                var group = album.Group;

                var gm = GroupService.UserInGroup(UserContext.Current.Id, group);
                if (gm == null)
                    throw new BusinessLogicException("Вы не являетесь участником группы");

                if (gm.State != (byte) GroupMemberState.Moderator)
                    throw new BusinessLogicException("Только модераторы могут создавать альбомы в группах");
            }
            else if (album.UserId.HasValue)
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
                if (user == null)
                    throw new BusinessLogicException("Перезайдите");

                if (album.User != user)
                    throw new BusinessLogicException("Нельзя редактировать чужой альбом");
            }
            else
                throw new BusinessLogicException("Данный альбом испорчен");

            return View(new AlbumCreateEditViewModel(album));
        }

        [HttpPost]
        public ActionResult Edit(AlbumCreateEditViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var album = AlbumService.EditAlbum(UserContext.Current, model.AlbumId, model.Title, model.Description, model.IsOpen);

            return RedirectToAction("index", new { id = album.Id });
        }

        public ActionResult Delete(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            var routeId = album.GroupId ?? album.UserId;
            var controller = album.Controller;

            AlbumService.DeleteAlbum(UserContext.Current, album);

            return RedirectToAction("albums", controller, new { id = routeId });
        }

        public ActionResult Index(Guid id)
        {
            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            return View(new AlbumViewModel(album));
        }

        public ActionResult AddImage(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            if (album.GroupId.HasValue)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, album.Group, true);

                if (!album.IsOpen && gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("В этот альбом добавлять изображения может только модератор");
            }
            else if (album.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя добавлять изображения в чужой альбом");

            return View(new AlbumAddItemViewModel(album));
        }

        [HttpPost]
        public ActionResult AddImage(AlbumAddItemViewModel model, HttpPostedFileBase image)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            string fileName;
            AlbumItem albumItem;

            if (image != null)
            {
                if (UploadImageValidationService.ValidateImageAndGetNewName(image, out fileName))
                {
                    albumItem = AlbumService.AddImage(UserContext.Current, model.AlbumId, model.Title, model.Description, fileName);

                    var path = Path.Combine(Server.MapPath("~/MediaContent/Albums/Images/"), fileName);
                    image.SaveAs(path);
                }
                else
                    throw new BusinessLogicException("Неверный формат файла");
            }
            else if (!string.IsNullOrWhiteSpace(model.Url))
            {
                byte[] content;
                var hwReq = (HttpWebRequest)WebRequest.Create(model.Url);
                var wResp = hwReq.GetResponse();
                var stream = wResp.GetResponseStream();

                if (stream == null)
                    throw new BusinessLogicException("Произошла ошибка при открытии ссылки");

                using (var br = new BinaryReader(stream))
                {
                    content = br.ReadBytes(5242880); // Ограничение по размеру в 5Мб
                    br.Close();
                }
                wResp.Close();

                if (UploadImageValidationService.ValidateImageTypeAndGetNewName(wResp, out fileName))
                {
                    albumItem = AlbumService.AddImage(UserContext.Current, model.AlbumId, model.Title, model.Description, fileName);

                    var path = Path.Combine(Server.MapPath("~/MediaContent/Albums/Images/"), fileName);
                    var fs = new FileStream(path, FileMode.Create);
                    var w = new BinaryWriter(fs);

                    try
                    {
                        w.Write(content);
                    }
                    finally
                    {
                        fs.Close();
                        w.Close();
                    }
                }
                else
                    throw new ValidationException("Указана некорректная ссылка");
            }
            else
                throw new ValidationException("Изображение не выбрано");

            if (Request.UrlReferrer != null && Request.UrlReferrer.Segments[2] == "foreditor/")
                return RedirectToAction("itemforeditor", "album", new { id = albumItem.Id, groupId = model.GroupId });

            return RedirectToAction("item", new { id = albumItem.Id });
        }

        public ActionResult AddVideo(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор альбома");

            if (album.GroupId.HasValue)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, album.Group, true);

                if (!album.IsOpen && gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("В этот альбом добавлять видео может только модератор");
            }
            else if (album.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя добавлять видео в чужой альбом");

            return View(new AlbumAddItemViewModel(album));
        }
        
        [HttpPost]
        public ActionResult AddVideo(AlbumAddItemViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var albumItem = AlbumService.AddVideo(UserContext.Current, model.AlbumId, model.Title, model.Description, model.Url, model.EmbedCode);

            if (Request.UrlReferrer != null && Request.UrlReferrer.Segments[2] == "foreditor/")
                return RedirectToAction("itemforeditor", "album", new { id = albumItem.Id, groupId = model.GroupId });

            return RedirectToAction("item", new { id = albumItem.Id });
        }

        public ActionResult Item(Guid id)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента альбома");

            return View(new AlbumItemViewModel(albumItem));
        }

        public ActionResult EditItem(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента альбома");

            if (albumItem.Album.GroupId.HasValue)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, albumItem.Album.Group);
                if (gm == null)
                    throw new BusinessLogicException("Вы не состоите в группе");

                if (gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Только модераторы могут изменять альбомы группы");
            }
            else if (albumItem.Album.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя редактировать чужие альбомы");

            return View(new AlbumEditItemViewModel(albumItem));
        }

        [HttpPost]
        public ActionResult EditItem(AlbumEditItemViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var albumItem = AlbumService.EditItem(UserContext.Current, model.Id, model.Title, model.Description);

            return RedirectToAction("item", new { id = albumItem.Id });
        }

        public ActionResult MoveItem(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента альбома");

            if (albumItem.Album.GroupId.HasValue)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, albumItem.Album.Group);
                if (gm == null)
                    throw new BusinessLogicException("Вы не состоите в группе");

                if (gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Только модераторы могут изменять альбомы группы");
            }
            else if (albumItem.Album.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя перемещать в чужом альбоме");

            return View(new AlbumMoveItemViewModel(albumItem));
        }

        public ActionResult MoveItemAction(Guid id, Guid albumId)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var albumItem = AlbumService.MoveItem(UserContext.Current, id, albumId);

            return RedirectToAction("item", new { id = albumItem.Id });
        }

        public ActionResult DeleteItem(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var album = AlbumService.DeleteItem(UserContext.Current, id);

            return RedirectToAction("index", new { id = album.Id });
        }

        public ActionResult NextItem(Guid id)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Введен неверный идентификатор");

            var itemsList = albumItem.Album.Items.ToList();
            var index = itemsList.IndexOf(albumItem);
            var next = itemsList.ElementAtOrDefault(index + 1);
            var itemId = next == null ? albumItem.Id : next.Id;

            return RedirectToAction("item", new { id = itemId });
        }

        public ActionResult PreviousItem(Guid id)
        {
            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Введен неверный идентификатор");

            var itemsList = albumItem.Album.Items.ToList();
            var index = itemsList.IndexOf(albumItem);
            var previous = itemsList.ElementAtOrDefault(index - 1);
            var itemId = previous == null ? albumItem.Id : previous.Id;

            return RedirectToAction("item", new { id = itemId });
        }

        #region forWYSIWYG

        public ActionResult AlbumsForEditor(Guid? groupId)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            AlbumsViewModel model;
            if (groupId.HasValue)
            {
                var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId.Value);
                if (group == null)
                    throw new BusinessLogicException("Указан неверный идентификатор");

                model = new AlbumsViewModel(user, group);
            }
            else
                model = new AlbumsViewModel(user);

            return View("albumsforeditor", "_ModalLayout", model);
        }

        public ActionResult ForEditor(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            var album = DataService.PerThread.AlbumSet.SingleOrDefault(x => x.Id == id);
            if (album == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            return View("foreditor", "_ModalLayout", new AlbumViewModel(album));
        }

        public ActionResult ItemForEditor(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == id);
            if (albumItem == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            return View("itemforeditor", "_ModalLayout", new AlbumItemViewModel(albumItem));
        }

        #endregion
    }
}