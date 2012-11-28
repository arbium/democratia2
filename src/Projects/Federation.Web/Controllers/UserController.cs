using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Federation.Core;
using Federation.Core.Services;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class UserController : MainController
    {
        public ActionResult Quest()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            QuestService.Recalculate(user);

            return View(new UserQuestViewModel(user));
        }

        public ActionResult RejectQuest()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            user.IsQuestRejected = true;
            DataService.PerThread.SaveChanges();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());

            return RedirectToAction("index");
        }

        public ActionResult Content(Guid id, bool? invert = null)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == id);
            if (content == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента");

            var userId = Request.IsAuthenticated ? UserContext.Current.Id : (Guid?)null;

            return View(new _ContentLayoutViewModel(content, userId, invert));
        }

        public ActionResult Albums(Guid? id)
        {
            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                id = UserContext.Current.Id;
            }

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new AlbumsViewModel(user));
        }

        public ActionResult EditSubscription()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserEditSubscriptionViewModel(user));
        }

        public ActionResult Archive(Guid? id, int? year, int? month, int? day, bool? posts, bool? petitions, bool? user, bool? group)
        {
            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                id = UserContext.Current.Id;
            }

            var dude = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == id.Value);
            if (dude == null)
                new BusinessLogicException("Чувак не найден");

            posts = !posts.HasValue || posts.Value;
            petitions = !petitions.HasValue || petitions.Value;

            var contentType = new ContentTypeFilter { Posts = posts.Value, Petitions = petitions.Value };

            user = !user.HasValue || user.Value;
            group = !group.HasValue || group.Value;

            var personality = new Personality { User = user.Value, Group = group.Value };

            return View(new UserArchiveViewModel(dude, year, month, day, contentType, personality));
        }

        #region Редактирование профиля

        [RequireHttps]
        public ActionResult Edit()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var userId = Guid.Parse(User.Identity.Name);
            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == userId);

            return View(new UserEditViewModel(user));
        }

        [RequireHttps]
        public ActionResult EditAvatar()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            return View(new UserEditAvatarViewModel(user));
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult EditAvatar(UserEditAvatarViewModel model, HttpPostedFileBase avatar)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            string fileName;
            const string avatarPath = "~/MediaContent/Avatars/";

            if (UploadImageValidationService.ValidateImageAndGetNewName(avatar, out fileName))
            {
                var path = Path.Combine(Server.MapPath(avatarPath), fileName);

                ImageService.DeleteImage<User>(model.AvatarImageName);
                    
                avatar.SaveAs(path);
            }
            else if (!string.IsNullOrWhiteSpace(model.Url))
            {
                byte[] content;
                var hwReq = (HttpWebRequest)WebRequest.Create(model.Url);
                var wResp = hwReq.GetResponse();
                var stream = wResp.GetResponseStream();

                if (stream == null)
                    throw new BusinessLogicException("Произошла ошибка загрузки");

                using (var br = new BinaryReader(stream))
                {
                    content = br.ReadBytes(10485760); // Ограничение по размеру в 10Мб
                    br.Close();
                }
                wResp.Close();

                if (UploadImageValidationService.ValidateImageTypeAndGetNewName(wResp, out fileName))
                {
                    ImageService.DeleteImage<Group>(model.AvatarImageName);

                    var path = Path.Combine(Server.MapPath(avatarPath), fileName);
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
            }
            else
                throw new ValidationException("Укажите источник");

            user.Avatar = fileName;

            DataService.PerThread.SaveChanges();
            UserContext.Abandon();

            return RedirectToAction("editavatar");                        
        }

        [HttpPost]
        public ActionResult CropAvatar(UserEditAvatarViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (ModelState.IsValid)
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

                var newImage = ImageService.CropExistedImage<User>(model.AvatarImageName, model.X1, model.Y1, model.X2, model.Y2);
                ImageService.DeleteImage<User>(model.AvatarImageName);

                user.Avatar = newImage;
                DataService.PerThread.SaveChanges();
                UserContext.Abandon();

                return RedirectToAction("editavatar");
            }

            return View("EditAvatar");
        }

        [RequireHttps]
        public ActionResult EditPersonal()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var userId = Guid.Parse(User.Identity.Name);
            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == userId);

            return View(new UserEditPersonalViewModel(user));
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult EditPersonal(UserEditPersonalViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            user.Patronymic = model.Patronymic;
            user.Sex = model.Sex;
            user.UTCOffset = (short)TimeSpan.Parse(model.UTCOffset).TotalMinutes;

            if (!string.IsNullOrWhiteSpace(model.BirthDate))
                user.BirthDate = DateTime.Parse(model.BirthDate);

            DataService.PerThread.SaveChanges();
            UserContext.Abandon();

            return RedirectToAction("profile");
        }

        [RequireHttps]
        public ActionResult EditContacts()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            return View(new UserEditContactsViewModel(user));
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult EditContacts(UserEditContactsViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            user.LiveJournalSindication = model.LJSindication;
            user.LiveJournalSindicateAsDraft = model.LJPublishState;

            if (string.IsNullOrWhiteSpace(user.Facebook))
                user.Facebook = model.Facebook;
            if (string.IsNullOrWhiteSpace(user.LiveJournal))
                user.LiveJournal = model.LiveJournal;

            DataService.PerThread.SaveChanges();

            return RedirectToAction("profile");
        }

        [RequireHttps]
        public ActionResult EditBio()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            return View(new UserEditBioViewModel { Info = user.Info });
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult EditBio(UserEditBioViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);

            user.Info = model.Info;

            DataService.PerThread.SaveChanges();

            return RedirectToAction("profile");
        }

        #endregion

        #region Разного рода голосования

        public ActionResult Votings(Guid? id)
        {
            User user;

            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);

                if (user == null)
                    throw new BusinessLogicException("Перезайдите");
            }
            else
            {
                user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);

                if (user == null)
                    throw new BusinessLogicException("Неверный идентификатор пользователя");
            }

            return View(new UserVotingsViewModel(user));
        }

        public ActionResult Petition(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult PetitionSigners(Guid id)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Указан неверный идентификатор петиции");

            if (!petition.AuthorId.HasValue)
                throw new BusinessLogicException("У петиции нет автора");

            return View(new UserPetitionSignersViewModel(petition));
        }

        public ActionResult PetitionNotices()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            return View(new UserPetitionNoticesViewModel(UserContext.Current.Id));
        }

        public ActionResult CreatePetition()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            return View(new _CreatePetitionViewModel());
        }

        public ActionResult EditPetition(Guid id)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            if (petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Редактировать можно только черновые петиции");
            if (!petition.AuthorId.HasValue)
                throw new BusinessLogicException("У петиции нет автора");
            if (petition.AuthorId.Value != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            return View(new _EditPetitionViewModel(petition));
        }

        public ActionResult EditPetitionCoauthors(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            if (!petition.AuthorId.HasValue)
                throw new BusinessLogicException("У петиции нет автора");
            if (petition.AuthorId.Value != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            return View(new _EditPetitionCoauthorsViewModel(petition));
        }

        public ActionResult Experts()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserExpertsViewModel(user));
        }

        public ActionResult Voters(Guid? id)
        {
            User user;
            bool isForeign;

            if (id == null)
            {
                user =
                    DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
                isForeign = false;
                if (user == null)
                    throw new BusinessLogicException("Перезайдите");
            }
            else
            {
                var userId = id.Value;
                user =
                    DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
                isForeign = true;
                if (user == null)
                    throw new BusinessLogicException("Пользователь не найден");
            }

            return View(new UserVotersViewModel(user, isForeign));
        }

        public ActionResult Delegation(Guid? id, string GroupUrl)
        {
            if (!id.HasValue && UserContext.Current == null)
                throw new AuthenticationException();

            if ((UserContext.Current != null && id.HasValue && UserContext.Current.Id == id) || (UserContext.Current != null && !id.HasValue))
            {
                id = UserContext.Current.Id;
                return View("SelfDelegation", new UserSelfDelegationViewModel(id.Value, GroupUrl));
            }

            return View(new UserDelegationViewModel(id.Value, GroupUrl));
        }

        public ActionResult BecomeExpert(Guid id, string url)
        {
            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
                url = HttpUtility.UrlDecode(url);
            else
                url = Url.Action("index", "user", new { id = id });

            var confirmationViewModel = new ConfirmationViewModel()
            {
                Message = "Вы действительно хотите стать экспертом по выбранной теме?",
                YesUrlAction = "becomeexpertconfirmation",
                YesUrlController = "user",
                NoUrl = url
            };

            confirmationViewModel.RouteValues.Add("id", id);
            confirmationViewModel.RouteValues.Add("url", url);

            return View("Confirmation", confirmationViewModel);
        }

        [HttpPost]
        public ActionResult BecomeExpertConfirmation(Guid id, string url)//id - tagId
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == id);

            DelegationService.BecomeExpert(UserContext.Current.Id, id);

            return Redirect(url);
        }

        public ActionResult CeaseToBeExpert(Guid id, string url)
        {
            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
                url = HttpUtility.UrlDecode(url);
            else
                url = Url.Action("index", "user", new { id = id });

            var confirmationViewModel = new ConfirmationViewModel()
            {
                Message = "Вы действительно хотите перестать быть экспертом по выбранной теме (при этом вы потеряете голоса всех своих сторонников)?",
                YesUrlAction = "ceasetobeexpertconfirmation",
                YesUrlController = "user",
                NoUrl = url
            };

            confirmationViewModel.RouteValues.Add("id", id);
            confirmationViewModel.RouteValues.Add("url", url);

            return View("Confirmation", confirmationViewModel);
        }

        [HttpPost]
        public ActionResult CeaseToBeExpertConfirmation(Guid id, string url)//id - tagId
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == id);

            DelegationService.CeaseToBeExpert(UserContext.Current.Id, id);

            return Redirect(url);
        }

        public ActionResult RemoveExpert(string id, string url)
        {
            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
                url = HttpUtility.UrlDecode(url);
            else
                url = Url.Action("index", "user", new { id = id });

            var confirmationViewModel = new ConfirmationViewModel()
            {
                Message = "Вы действительно хотите перестать быть экспертом по выбранной теме (при этом вы потеряете голоса всех своих сторонников)?",
                YesUrlAction = "removeexpertconfirmation",
                YesUrlController = "user",
                NoUrl = url
            };

            confirmationViewModel.RouteValues.Add("id", id);
            confirmationViewModel.RouteValues.Add("url", url);

            return View("Confirmation", confirmationViewModel);
        }

        [HttpPost]
        public ActionResult RemoveExpertConfirmation(string id, string url)//id - groupIdorLabel
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            DelegationService.RemoveExpert(id, UserContext.Current.Id);

            return Redirect(url);
        }

        public ActionResult Delegate(Guid id, Guid tagId, string url)
        {
            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
                url = HttpUtility.UrlDecode(url);
            else
                url = Url.Action("delegation", "user", new {id = id});

            var confirmationViewModel = new ConfirmationViewModel()
                                            {
                                                Message = "Вы действительно хотите делегровать свой голос выбранному эксперту?",
                                                YesUrlAction = "delegateconfirmation",
                                                YesUrlController = "user", 
                                                NoUrl = url
                                            };

            confirmationViewModel.RouteValues.Add("id", id);
            confirmationViewModel.RouteValues.Add("tagId", tagId);
            confirmationViewModel.RouteValues.Add("url", url);

            return View("Confirmation",confirmationViewModel);
        }

        [HttpPost]
        public ActionResult DelegateConfirmation(Guid id, Guid tagId, string url)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();
           
            var voterId = UserContext.Current.Id;

            DelegationService.Delegate(voterId, id, tagId);

            return Redirect(url);
        }

        public ActionResult Undelegate(Guid id, string url)
        {
            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
                url = HttpUtility.UrlDecode(url);
            else
                url = Url.Action("delegation", "user", new { id = id });

            var confirmationViewModel = new ConfirmationViewModel()
            {
                Message = "Вы действительно не хотите больше делегровать свой голос выбранному эксперту?",
                YesUrlAction = "undelegateconfirmation",
                YesUrlController = "user",
                NoUrl = url
            };

            confirmationViewModel.RouteValues.Add("id", id);
            confirmationViewModel.RouteValues.Add("url", url);

            return View("Confirmation", confirmationViewModel);
        }

        [HttpPost]
        public ActionResult UndelegateConfirmation(Guid id, string url)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var voterId = UserContext.Current.Id;

            DelegationService.Undelegate(voterId, id);

            if (Request.UrlReferrer != null && String.IsNullOrWhiteSpace(url))
                url = Request.UrlReferrer.PathAndQuery;

            if (!String.IsNullOrWhiteSpace(url))
            {
                url = HttpUtility.UrlDecode(url);
                return Redirect(url);
            }

            return RedirectToAction("experts", "user");
        }

        //public ActionResult DeleteDelegating(Guid id)
        //{
        //    if (!Request.IsAuthenticated)
        //        throw new AuthenticationException();

        //    var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
        //    if (user == null)
        //        throw new BusinessLogicException("Перезайдите");

        //    IList<ExpertVote> todelete = new List<ExpertVote>();

        //    foreach (var gm in user.GroupMembers)
        //        foreach (var ev in gm.ExpertVotes)
        //            if (ev.ExpertId == id)
        //                todelete.Add(ev);

        //    foreach (var ev in todelete)
        //        DataService.PerThread.ExpertVoteSet.DeleteObject(ev);

        //    DataService.PerThread.SaveChanges();

        //    return RedirectToAction("Experts");
        //}

        #endregion

        public ActionResult BlackList()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserBlackListViewModel(user));
        }

        public ActionResult UserInBlackList(Guid id, bool block)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            if (id == user.Id)
                throw new BusinessLogicException("Нельзя самого себя вносить в черный список");

            var blockedUser = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == id);
            if (blockedUser == null)
                throw new BusinessLogicException("Указан неверный идентификатор пользователя");

            if (block)
                BlackListService.AddUser(user, blockedUser);
            else
                BlackListService.RemoveUser(user, blockedUser);

            UserContext.Abandon();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());

            return RedirectToAction("index", new { id });
        }

        public ActionResult ReplyComment(Guid id)
        {
            var parentComment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (parentComment == null)
                throw new BusinessLogicException("Не найден комментарий с указанным идентификатором");

            if (parentComment.IsHidden)
                throw new BusinessLogicException("Данный комментарий скрыт, его нельзя комментировать");
            if (parentComment.Content.IsDiscussionClosed)
                throw new BusinessLogicException("Обсуждение закрыто!");

            return View(new ReplyCommentViewModel(parentComment));
        }

        public ActionResult EditComment(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var comment = DataService.PerThread.CommentSet.SingleOrDefault(x => x.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Указан неверный идентификатор комментария");

            if (comment.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя редактировать чужие комментарии");
            if (DateTime.Now - comment.DateTime > ConstHelper.CommentEditTimeSpan)
                throw new BusinessLogicException("Время на редактирование вышло");

            return View(new EditCommentViewModel(comment));
        }

        public ActionResult Index(string id, bool? ajax)
        {
            if (!string.IsNullOrWhiteSpace(id))
                return RedirectToAction("blog", "user", new { id });

            if (!Request.IsAuthenticated)
            {
                AuthenticationService.SignOut();

                return RedirectToAction("signin", "account", null);
            }

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            var model = new UserIndexViewModel(user);

            if (ajax.HasValue && ajax.Value)
                return View("_contentfeed", "_emptylayout", model.Subscription.LastContents);

            return View(model);
        }

        public ActionResult Profile(Guid? id)
        {
            Guid userId;

            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                userId = UserContext.Current.Id;
            }
            else
                userId = id.Value;

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Не найден пользователь с указанным идентификатором");

            return View(new UserProfileViewModel(user));
        }

        public ActionResult Comments(Guid? id, bool? invert = null)
        {
            Guid userId;

            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                userId = UserContext.Current.Id;
            }
            else
                userId = id.Value;

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Не найден пользователь с указанным идентификатором");

            return View(new UserCommentsViewModel(user, invert));
        }

        public ActionResult Blog(Guid? id, bool? ajax)
        {
            Guid userId;
            var currentUid = Request.IsAuthenticated ? UserContext.Current.Id : (Guid?)null;

            if (!id.HasValue)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                userId = UserContext.Current.Id;
            }
            else
                userId = id.Value;

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Не найден пользователь с указанным идентификатором");

            var model = new UserBlogViewModel(user, currentUid);

            if (ajax.HasValue && ajax.Value)
                return View("_feed", "_emptylayout", model.Feed);

            return View(model);
        }
        
        public ActionResult Drafts()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserDraftsViewModel(user));
        }

        public ActionResult Groups(Guid? id)
        {            
            User user;

            if (id == null || id == Guid.Empty)
            {
                if (!Request.IsAuthenticated)
                    throw new AuthenticationException();

                user = DataService.PerThread.BaseUserSet.OfType<User>().Single(u => u.Id == UserContext.Current.Id);
            }
            else
            {
                user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
                if (user == null)
                    throw new BusinessLogicException("Указанный пользователь не найден");
            }

            return View(new UserGroupsViewModel(user));
        }

        public ActionResult Post(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult AddPost()
        {
            if (!Request.IsAuthenticated)
                throw new BusinessLogicException("Нельзя постить в блог будучи неаутентифицированным");

            var model = new UserAddPostViewModel
            {
                AuthorName = UserContext.Current.DisplayName,
                AuthorId = UserContext.Current.Id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPost(UserAddPostViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new BusinessLogicException("Нельзя постить в блог будучи неаутентифицированным");

            if (!ModelState.IsValid)
                return View(model);

            if (UserContext.Current.Id != model.AuthorId)
                throw new BusinessLogicException("Вы не можете постить в данный блог, т.к. не являетесь его владельцем");

            ContentService.CreatePost(null, model.AuthorId, model.Title, model.Text, model.TagTitles, model.IsDraft, true);

            return RedirectToAction(model.IsDraft ? "drafts" : "index");
        }

        public ActionResult EditPost(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var post = DataService.PerThread.ContentSet.OfType<Post>().SingleOrDefault(p => p.Id == id);
            if (post == null)
                throw new BusinessLogicException("Не найден пост с указанным идентификатором");

            return View("editpost", new UserEditPostViewModel(post));
        }

        [HttpPost]
        public ActionResult EditPost(UserEditPostViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            ContentService.UpdatePost(model.PostId, UserContext.Current.Id, model.Title, model.Text, model.TagTitles, model.IsDraft);

            return RedirectToAction("post", new {id = model.PostId});
        }

        public ActionResult CreateGroup()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            return View(new UserCreateGroupViewModel());
        }

        [HttpPost]
        public ActionResult CreateGroup(UserCreateGroupViewModel model, HttpPostedFileBase logo)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);
              
            string fileName;

            if (UploadImageValidationService.ValidateImageAndGetNewName(logo, out fileName))
            {
                var path = Path.Combine(Server.MapPath("~/MediaContent/GroupLogos/"), fileName);
                logo.SaveAs(path);
            }
            else if (!string.IsNullOrWhiteSpace(model.LogoUrl))
            {
                byte[] content;
                var hwReq = (HttpWebRequest)WebRequest.Create(model.LogoUrl);
                var wResp = hwReq.GetResponse();
                var stream = wResp.GetResponseStream();

                using (var br = new BinaryReader(stream))
                {
                    content = br.ReadBytes(5242880); // Ограничение по размеру в 5Мб
                    br.Close();
                }
                wResp.Close();

                var type = wResp.ContentType.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (type[0] == "image")
                {
                    fileName = Guid.NewGuid() + "." + type[1];

                    var path = Path.Combine(Server.MapPath("~/MediaContent/GroupLogos/"), fileName);
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
            }

            var groupData = new AddGroupStruct
            {
                ElectionFrequency = model.ElectionFrequency,
                Label = model.Label,
                Logo = fileName,
                ModeratorsCount = model.ModeratorsCount,
                Name = model.Name,
                ParentGroupId = model.ParentGroupId,
                Summary = model.Summary,
                Categories = new List<Guid>()
            };

            if (model.Category.HasValue) // TODO: можно переделать на много категорий
                groupData.Categories.Add(model.Category.Value);

            var group = GroupService.CreateGroup(groupData);
            GroupService.AddMember(UserContext.Current.Id, group.Id);

            UserContext.Abandon();

            return RedirectToAction("privacy", "group", new { id = group.Url });            
        }

        #region Приглашения

        public ActionResult InviteUser()
        {
            throw new BusinessLogicException("Данный вид регистрации отключен. Для более точной информации свяжитесь с технической поддержкой support@democratia2.ru");

            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            return View(new UserInviteUserViewModel { ReferalId = UserContext.Current.Id, GivenInvitesCount = DataService.PerThread.InviteSet.Count(i => i.Referal.Id == UserContext.Current.Id) });
        }

        [HttpPost]
        public ActionResult InviteUser(UserInviteUserViewModel model)
        {
            throw new BusinessLogicException("Данный вид регистрации отключен. Для более точной информации свяжитесь с технической поддержкой support@democratia2.ru");

            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var referal = DataService.PerThread.BaseUserSet.SingleOrDefault(u => u.Id == model.ReferalId);
            if (referal == null)
                throw new BusinessLogicException("Пользователь не найден!");

            var givenInvitesCount = DataService.PerThread.InviteSet.Count(i => i.Referal.Id == referal.Id);
            if (givenInvitesCount >= ConstHelper.MaxInvitesCount)
                throw new ValidationException("Вы исчерпали лимит приглашений!");

            var allreadyInvitedCount =
                DataService.PerThread.InviteSet.Count(
                    i => i.Email == model.Email && i.State != (byte) InviteState.Blocked);
            if (allreadyInvitedCount != 0)
                throw new ValidationException("Пользователь с таким адресом электропочты уже приглашен!");

            var existedUser =
                DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault();
            if (existedUser != null)
                throw new ValidationException("Пользователь с таким адресом электропочты уже зарегистрирован!");

            var inviteData = new InviteDataStruct
                {
                    Name = model.Name,
                    Surname = model.SurName,
                    Patronymic = model.Patronymic,
                    Email = model.Email,
                    UserInfo = model.Info
                };

            var invite = InviteService.CreateInvite(inviteData);
            invite.Referal = referal;

            DataService.PerThread.SaveChanges();
            InviteService.SendInvite(invite.Id);

            //TODO - сделать систему вывода сообщений
            return RedirectToAction("inviteusercomplete");
        }

        public ActionResult InviteUserComplete()
        {
            return View();
        }

        public ActionResult Invites()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserInvitesViewModel(user));
        }

        public ActionResult SendInvite(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var invite = DataService.PerThread.InviteSet.SingleOrDefault(x => x.Id == id);
            if (invite == null)
                throw new BusinessLogicException("Неправильный идентификатор приглашения");

            if (invite.Referal == null)
                throw new BusinessLogicException("Вы не можете перепосылать автосгенерированные приглашения");
            if (invite.Referal.Id != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не являетесь автором этого приглашения");
            if (!(invite.State == (byte)InviteState.NotSent || invite.State == (byte)InviteState.Sent))
                throw new BusinessLogicException("Вы не можете перепослать данное приглашение");

            if (DateTime.Now - invite.CreationDate <= ConstHelper.MinInviteResendInterval)
                throw new BusinessLogicException("Превышен лимит посылания для данного приглашения, повторите позже");

            InviteService.SendInvite(id);

            return RedirectToAction("invites");
        }

        #endregion

        public ActionResult Rss(Guid id)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                return new EmptyResult();

            var recentContent = user.Contents
                .Where(x => x.State == (byte)ContentState.Approved && x.PublishDate.HasValue && (x.Group == null || ((x.Group.Privacy & (byte)GroupPrivacy.Invisible) != (byte)GroupPrivacy.Invisible)))
                .OrderByDescending(x => x.PublishDate.Value).Take(100);

            var model = new _RssFeedViewModel
            {
                Title = "Демократия2. " + user.FullName + ". Блог",
                Description = "RSS-лента " + user.FullName + " на Демократии2",
                Link = "http://democratia2.ru/User/Index/" + user.Id
            };

            foreach (var content in recentContent)
            {
                var item = new _RssFeedItemViewModel
                {
                    Title = content.Title,
                    Link = content.GetUrl(false),
                    Description = content.Text
                };

                if (content.PublishDate.HasValue)
                    item.PublishDate = content.PublishDate.Value.ToString("R");

                model.Items.Add(item);
            }

            return View("_Rss", "_EmptyLayout", model);
        }

        public ActionResult RssSubscribtion(Guid id)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                return new EmptyResult();

            var groupContent = (from _group in user.SubscriptionGroups
                                join content in DataService.PerThread.ContentSet on _group.Id equals content.GroupId
                                where content.AuthorId != id && content.State == (byte)ContentState.Approved && content.PublishDate.HasValue && (content.Group == null || (content.Group.Privacy & (byte)GroupPrivacy.Invisible) != (byte)GroupPrivacy.Invisible)
                                orderby content.PublishDate descending
                                select content).Take(100);

            var userContent = (from _user in user.SubscriptionUsers
                               join content in DataService.PerThread.ContentSet on _user.Id equals content.AuthorId
                               where content.AuthorId != id && content.State == (byte)ContentState.Approved && content.PublishDate.HasValue && (content.Group == null || (content.Group.Privacy & (byte)GroupPrivacy.Invisible) != (byte)GroupPrivacy.Invisible)
                               orderby content.PublishDate descending
                               select content).Take(100);

            var feed = groupContent.Union(userContent).Distinct().Where(x => x.AuthorId != user.Id).OrderByDescending(c => c.PublishDate.Value);

            var model = new _RssFeedViewModel
            {
                Title = "Демократия2. " + user.FullName + ". Подписка",
                Description = "RSS-лента подписки " + user.FullName + " на Демократии2",
                Link = "http://democratia2.ru/User/Index/" + user.Id
            };

            foreach (var content in feed)
            {
                var info = new StringBuilder();
                info.Append("<p>");
                info.Append("Автор: <a href='" + Url.Action("blog", "user", new { content.Author.Id }, "http") + "'>" + content.Author.FullName + "</a>");
                if (content.Group != null)
                    info.Append("<br/>Группа: <a href='" + Url.Action("index", "group", new { id = content.GroupId }, "http") + "'>" + content.Group.Name + "</a>");
                info.Append("</p>");

                var item = new _RssFeedItemViewModel
                {
                    Title = content.Title,
                    Link = content.GetUrl(false),
                    Description = info + content.Text                    
                };

                if (content.PublishDate.HasValue)
                    item.PublishDate = content.PublishDate.Value.ToString("R");

                model.Items.Add(item);
            }

            return View("_Rss", "_EmptyLayout", model);
        }

        #region Личка

        public ActionResult Dialogs()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();
            
            return View(new UserDialogsViewModel());
        }

        public ActionResult Dialog(Guid? id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            UserDialogViewModel model;

            if (id.HasValue)
            {
                if (id.Value == UserContext.Current.Id)
                    throw new BusinessLogicException("Нельзя посылать сообщения самому себе");

                var contact = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == id.Value);
                if (contact == null)
                    throw new BusinessLogicException("Указан неверный идентификатор пользователя");

                if (contact.BlackList.Contains(user))
                    throw new BusinessLogicException("Вы находитесь в черном списке у данного пользователя, поэтому не можете посылать ему сообщения");

                model = new UserDialogViewModel(contact);
            }
            else
                model = new UserDialogViewModel(null);

            MessageService.MarkAsRead(user.Id, id);

            return View(model);
        }

        [HttpPost]
        public object Dialog(UserDialogViewModel model, bool? ajax)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            if (!model.ContactId.HasValue)
                throw new BusinessLogicException("Не указан идентификатор контакта");

            var message = MessageService.Send(UserContext.Current.Id, model.ContactId.Value, model.Text, MessageType.PrivateMessage);

            if (ajax.HasValue && ajax.Value)
                return Json(new UserDialog_MessageViewModel(message));

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());

            return RedirectToAction("dialog", new { id = model.ContactId });
        }
        
        public JsonResult GetNewMessages(string after, int utcOffset, Guid contactId)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            DateTime afterTime;
            if (DateTime.TryParse(after.Replace("UTC", "GMT"), out afterTime))
            {
                var afterServerTime = afterTime - new TimeSpan(0, utcOffset, 0) + TimeZoneInfo.Local.BaseUtcOffset;

                var messages = DataService.PerThread.MessageSet
                    .Where(x => x.AuthorId == contactId && x.RecipientId == UserContext.Current.Id && x.Date >= afterServerTime)
                    .OrderBy(x => x.Date)
                    .ToList();

                var result = new JsonResult
                    {
                        Data = messages.Select(x => new UserDialog_MessageViewModel(x)).ToList(), 
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                MessageService.MarkAsRead(UserContext.Current.Id, contactId);

                return result;
            }

            return null;
        }

        public ActionResult TrashMessages()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            return View(new UserTrashMessagesViewModel(user));
        }

        public ActionResult Message(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var message = DataService.PerThread.MessageSet.SingleOrDefault(x => x.Id == id);
            if (message == null)
                throw new BusinessLogicException("Указан неверный идентификатор сообщения");

            return View(new UserMessageViewModel(message));
        }

        public ActionResult MarkAsRead(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var msg = DataService.PerThread.MessageSet.SingleOrDefault(x => x.Id == id && !x.IsRead && x.RecipientId == UserContext.Current.Id);

            if (msg != null)
            {
                msg.IsRead = true;
                
                DataService.PerThread.SaveChanges();
                UserContext.Abandon();
            }

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());

            return RedirectToAction("index");
        }

        public void DeleteMessage(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            MessageService.DeleteMessage(UserContext.Current.Id, id);
        }      

        #endregion

        public ActionResult Subscription()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            return View(new UserSubscriptionViewModel(user));
        }

        [HttpPost]
        public ActionResult Subscription(UserSubscriptionViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            if (user.SubscriptionSettings.IsSubscribed != model.IsSubscribed)
            {
                user.SubscriptionSettings.IsSubscribed = model.IsSubscribed;

                DataService.PerThread.SaveChanges();
            }

            return RedirectToAction("index");            
        }

        [RequireHttps]
        public ActionResult PayPalValidation()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            if (user.IsPayPalVerified)
                return RedirectToAction("Profile");

            var userPayPalValidationViewModel = new UserPayPalValidationViewModel()
                                                    {
                                                        Email = user.Email,
                                                        FirstName = TransliterationService.Translite(user.FirstName),
                                                        LastName = TransliterationService.Translite(user.SurName)
                                                    };

            return View(userPayPalValidationViewModel);
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult PayPalValidation(UserPayPalValidationViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            if(user.IsPayPalVerified)
                return RedirectToAction("Profile");

            bool valid = BankCardValidationService.Validate(user.Email, model.FirstName, model.LastName, user);
            if (valid)
            {
                user.IsPayPalVerified = true;
                DataService.PerThread.SaveChanges();
                return RedirectToAction("Profile");
            }

            throw new ValidationException("PayPal не подтвердил указанные вами данные");
        }


        public ActionResult SendEmailVerification()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден");

            if (user.IsEmailVerified)
                throw new BusinessLogicException("Вы уже подтвердили Вашу почту");

            AccountService.SendEmailVerification(UserContext.Current.Id);

            return RedirectToAction("subscription");
        }

        [RequireHttps]
        public ActionResult ProfileChangeRequest()
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден!");

            return View(new UserProfileChangeRequestViewModel(user));
        }

        [HttpPost]
        [RequireHttps]
        public ActionResult ProfileChangeRequest(UserProfileChangeRequestViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            UserService.ProfileChangeRequest(UserContext.Current.Id, model.Firstname, model.Surname, model.Patronymic, 
                model.Facebook, model.LiveJournal, model.PhoneNumber, model.Comment);

            return RedirectToAction("index");
        }
    }
}
