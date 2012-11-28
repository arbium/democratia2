using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Federation.Core;
using Federation.Web.ViewModels;

namespace Federation.Web.Controllers
{
    public class GroupController : MainController
    {
        private ActionResult HideInvisibleGroup(Group group)
        {
            if (group.PrivacyEnum.HasFlag(GroupPrivacy.Invisible))
            {
                if (!Request.IsAuthenticated)
                    return RedirectToAction("index", "home");

                var gm = GroupService.UserInGroup(UserContext.Current.Id, group);

                if (gm == null || gm.State == (byte)GroupMemberState.NotApproved)
                    return RedirectToAction("index", "home");
            }

            return null;
        }

        public ActionResult Content(Guid id, bool? invert = null)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == id);
            if (content == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента");

            var redirect = HideInvisibleGroup(content.Group);
            if (redirect != null)
                return redirect;

            var userId = Request.IsAuthenticated ? UserContext.Current.Id : (Guid?)null;

            return View(new _ContentLayoutViewModel(content, userId, invert));
        }

        #region Акшены просмотра и управления группой

        public ActionResult Index(string id, Guid? topic, bool? byDate, bool? ajax)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var model = CachService.GetViewModel(new GroupIndexViewModel(group.Id, UserContext.Current != null ? UserContext.Current.Id : (Guid?)null, topic, byDate));

            if (ajax.HasValue && ajax.Value)
                return View("_feed", "_emptylayout", ((GroupIndexViewModel)model).Feed);

            return View(model);
        }

        public ActionResult Search(string id, string tag)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new GroupSearchViewModel(id, tag));
        }

        public ActionResult Archive(string id, int? year, int? month, int? day, bool? posts, bool? polls, bool? petitions, bool? elections)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            posts = !posts.HasValue || posts.Value;
            polls = !polls.HasValue || polls.Value;
            petitions = !petitions.HasValue || petitions.Value;
            elections = !elections.HasValue || elections.Value;

            var contentType = new ContentTypeFilter
            {
                Posts = posts.Value,
                Polls = polls.Value,
                Petitions = petitions.Value,
                Elections = elections.Value
            };

            return View(new GroupArchiveViewModel(group, year, month, day, contentType));
        }

        public ActionResult Details(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var userId = UserContext.Current != null ? UserContext.Current.Id : (Guid?)null;
            var model = new GroupDetailsViewModel(group, userId);

            return View(model);
        }

        public ActionResult AddFgForm(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var gm = GroupService.UserInGroup(UserContext.Current.Id, group, true);
            if (gm.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не модератор");

            return View(new GroupAddFgFormViewModel(group));
        }

        public ActionResult AddFriendlyGroup(string id, string fg)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var split = fg.Split('/');
            if (split.ElementAtOrDefault(1) == "group")
                fg = split.ElementAtOrDefault(2);
            else if (split.ElementAtOrDefault(3) == "group")
                fg = split.ElementAtOrDefault(4);

            GroupService.AddFriendlyGroup(id, fg, UserContext.Current.Id);

            return RedirectToAction("index", new { id });
        }

        public ActionResult RemoveFriendlyGroup(string id, string fg)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            GroupService.RemoveFriendlyGroup(id, fg, UserContext.Current.Id);

            return RedirectToAction("index", new { id });
        }

        public ActionResult Privacy(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, group.Id).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (group.Content.OfType<Election>().Count(x => !x.IsFinished) != 0)
                throw new BusinessLogicException("Редактирование группы закрыто на время выборов");

            return View(new GroupPrivacyViewModel(group));
        }

        [HttpPost]
        public ActionResult Privacy(GroupPrivacyViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupId);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, model.GroupId).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (group.Content.OfType<Election>().Count(x => !x.IsFinished) != 0)
                throw new BusinessLogicException("Редактирование группы закрыто на время выборов");

            if (ModelState.IsValid)
            {
                var groupPrivacyData = new GroupPrivacyStruct
                {
                    GroupId = model.GroupId,
                    IsContentModeration = model.IsContentModeration,                    
                    IsMemberModeration = model.IsInvisible || model.IsMemberModeration,
                    IsPrivateDiscussion = model.IsInvisible || model.IsPrivateDiscussion,
                    IsInvisible = model.IsInvisible
                };

                GroupService.SetPrivacy(groupPrivacyData);

                return RedirectToAction("details", new { id = group.Url });
            }

            return View(model);
        }


        public ActionResult Banner(string id)
        {
            const string logoPath = "~/MediaContent/GroupBanners/";

            var path = Path.Combine(Server.MapPath(logoPath), id);
            
            return File(path, "text/html");
        }

        public ActionResult Edit(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, group.Id).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (group.Content.OfType<Election>().Count(x => !x.IsFinished) != 0)
                throw new BusinessLogicException("Редактирование группы закрыто на время выборов");

            return View(new GroupEditViewModel(group));
        }

        [HttpPost]
        public ActionResult Edit(GroupEditViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupId);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, model.GroupId).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (group.Content.OfType<Election>().Count(x => !x.IsFinished) != 0)
                throw new BusinessLogicException("Редактирование группы закрыто на время выборов");

            if (ModelState.IsValid)
            {
                var groupData = new EditGroupStruct
                {
                    GroupId = model.GroupId,
                    ElectionFrequency = model.ElectionFrequency,
                    Label = model.Label,
                    ModeratorsCount = model.ModeratorsCount,
                    Name = model.Name,
                    ParentGroupId = model.ParentGroupId,
                    Summary = model.Summary,
                    PollQuorum = model.PollQuorum,
                    ElectionQuorum = model.ElectionQuorum,
                    Categories = new List<Guid>()
                    
                };

                if (model.Category.HasValue) // TODO: можно переделать на много категорий
                    groupData.Categories.Add(model.Category.Value);

                GroupService.UpdateGroup(groupData);                

                return RedirectToAction("details", new { id = group.Url });
            }

            return View(model);
        }

        public ActionResult EditLogo(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var gm = GroupService.UserInGroup(UserContext.Current.Id, group.Id);
            if (gm == null)
                throw new BusinessLogicException("Вы не имеете права");
            if (gm.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            return View(new GroupEditLogoViewModel(group));
        }

        [HttpPost]
        public ActionResult EditLogo(GroupEditLogoViewModel model, HttpPostedFileBase logo)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, group.Id).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (ModelState.IsValid)
            {
                string fileName;
                const string logoPath = "~/MediaContent/GroupLogos/";

                if (UploadImageValidationService.ValidateImageAndGetNewName(logo, out fileName))
                {
                    var path = Path.Combine(Server.MapPath(logoPath), fileName);

                    ImageService.DeleteImage<Group>(model.LogoImageName);

                    logo.SaveAs(path);
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
                        content = br.ReadBytes(5242880); // Ограничение по размеру в 5Мб
                        br.Close();
                    }
                    wResp.Close();

                    if (UploadImageValidationService.ValidateImageTypeAndGetNewName(wResp, out fileName))
                    {
                        ImageService.DeleteImage<Group>(model.LogoImageName);

                        var path = Path.Combine(Server.MapPath(logoPath), fileName);
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
                    throw new BusinessLogicException("Укажите источник");

                group.Logo = fileName;

                DataService.PerThread.SaveChanges();

                return RedirectToAction("editlogo");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CropLogo(GroupEditLogoViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, group.Id).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не являетесь модератором данной группы");

            if (ModelState.IsValid)
            {
                var newImage = ImageService.CropExistedImage<Group>(model.LogoImageName, model.X1, model.Y1, model.X2, model.Y2);
                ImageService.DeleteImage<Group>(model.LogoImageName);

                group.Logo = newImage;
                DataService.PerThread.SaveChanges();

                return RedirectToAction("editlogo", new { id = group.Url });
            }

            return RedirectToAction("editlogo", new { id = group.Url });
        }

        public JsonResult ExistAnotherGroupWithLabel(string label, Guid? id)
        {
            return new JsonResult { Data = GroupService.ExistAnotherGroupWithLabel(label, id), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AddTag(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new GroupAddTagViewModel(group));
        }

        [HttpPost]
        public ActionResult AddTag(GroupAddTagViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

                var redirect = HideInvisibleGroup(group);
                if (redirect != null)
                    return redirect;

                if (UserContext.Current == null)
                    return RedirectToAction("topics", new { id = model.GroupUrl });

                var gm = GroupService.UserInGroup(UserContext.Current.Id, group, true);

                TagService.AddTagByUser(model.Title, model.Description, gm);

                return RedirectToAction("topics", new { id = model.GroupUrl });
            }

            return View(model);
        }

        public ActionResult EditTag(string id, Guid tagId)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new GroupEditTagViewModel(group, tagId));
        }

        [HttpPost]
        public ActionResult EditTag(GroupEditTagViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

                var redirect = HideInvisibleGroup(group);
                if (redirect != null)
                    return redirect;

                if (UserContext.Current == null)
                    return RedirectToAction("topics", new { id = model.GroupUrl });

                var gm = GroupService.UserInGroup(UserContext.Current.Id, group, true);

                var result = TagService.UpdateTagByUser(model.TagId, model.Title, model.Description, gm);

                if (result)
                    return RedirectToAction("topics", new { id = model.GroupUrl });

                throw new ValidationException("Тема с аналогичным заголовком уже существует: <a href='"+Url.Action("Search",new {Tag = model.Title})+"'>"+model.Title+"</a>");
            }

            return View(model);
        }

        public ActionResult ModerateTag(string id, Guid tagId, bool approve)
        {
            if (UserContext.Current == null)
                throw new AuthenticationException();

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == UserContext.Current.Id);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var uig = GroupService.UserInGroup(user.Id, group.Id);
            if (uig == null)
                throw new BusinessLogicException("Вы не состоите в данной группе");

            if (uig.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Вы не модератор");

            var tag = DataService.PerThread.TagSet.SingleOrDefault(t => t.Id == tagId);
            if (tag == null)
                throw new BusinessLogicException("Неправильный идентификатор тэга");

            if (approve)
                tag.TopicState = (byte)TopicState.GroupTopic;
            else
            {
                if (tag.TopicState == (byte)TopicState.GroupTopic)
                {
                    tag.Experts.Clear();
                    tag.ExpertVotes.Clear();
                }
                tag.TopicState = (byte)TopicState.NotTopic;
            }

            DataService.PerThread.SaveChanges();

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("details", new { id = group.Url });
        }

        public ActionResult Children(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new GroupChildrenViewModel(group));
        }

        #endregion

        #region Делегирование

        public ActionResult EditExpertInfo(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (group.Tags.Count == 0)
                throw new BusinessLogicException("В группе не обозначены темы (тэги)");

            GroupService.UserInGroup(UserContext.Current.Id, group.Id, true);

            var model = new GroupEditExpertInfoViewModel(group, UserContext.Current.Id);

            if (Request.UrlReferrer != null)
                model.ReturnUrl = Request.UrlReferrer.ToString();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditExpertInfo(GroupEditExpertInfoViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            DelegationService.EditExpertInfo(UserContext.Current.Id, model.GroupUrl, model.Info);

            return RedirectToAction("index", new { id = model.GroupUrl });
        }

        public ActionResult Topics(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            if (group == null)
                throw new BusinessLogicException("Такой страницы не существует. Указан неверный идентификатор группы.");

            return View(new GroupTopicsViewModel(group.Id));
        }

        public ActionResult TopicExperts(Guid id)
        {
            return View(new GroupTopicExpertsViewModel(id));
        }

        public ActionResult Tags(string id)
        { 
            var group = GroupService.GetGroupByLabelOrId(id);

            if (group == null)
                throw new BusinessLogicException("Такой страницы не существует. Указан неверный идентификатор группы.");

            return View(new GroupTagsViewModel(group.Id));
        }

        public ActionResult SetTagRecomendation(string id, Guid tagid, bool recomended)
        {
            var group = GroupService.GetGroupByLabelOrId(id);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы.");

            TagService.SetTagRecomendation(tagid, recomended);

            return Redirect(Url.Action("tags", new { id = group.Url }) + "#tagid_" + tagid);
        }

        public ActionResult Experts(string id, Guid? tagid)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            if (group == null)
                throw new BusinessLogicException("Такой страницы не существует. Указан неверный идентификатор группы.");

            return View(new GroupExpertsViewModel(group, tagid));
        }

        #endregion

        #region MemberWork

        public ActionResult Members(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (Request.IsAuthenticated)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, group);
                if (gm != null && gm.State == (byte) GroupMemberState.Moderator)
                    return View("Members_Moderator", new GroupMembers_ModeratorViewModel(group));
            }

            return View(new GroupMembersViewModel(group));
        }

        [HttpPost]
        public ActionResult Members(Guid groupId, string query)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы");

            if (Request.IsAuthenticated)
            {
                var gm = GroupService.UserInGroup(UserContext.Current.Id, group);
                if (gm != null && gm.State == (byte)GroupMemberState.Moderator)
                    return View("Members_Moderator", new GroupMembers_ModeratorViewModel(group, query));
            }

            return View(new GroupMembersViewModel(group, query));
        }

        public ActionResult UserRequests(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (GroupService.UserInGroup(UserContext.Current.Id, group.Id).State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("У вас нет доступа к странице, т.к. вы не являетесь администратором");

            return View(new GroupRequestsViewModel(group));
        }

        /// <summary>
        /// Метод используется в 3 кейсах: 
        /// - модератор подтверждает вступление пользователя в закрытую группу, 
        /// - создатель группы набирает модераторов, 
        /// - член группы занимает вакантное место модератор
        /// </summary>
        public ActionResult ApproveMember(Guid id, bool approve = true)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var member = DataService.PerThread.GroupMemberSet.SingleOrDefault(g => g.Id == id);
            if (member == null)
                throw new BusinessLogicException("Не найден член с указанным идентификатором");

            var group = member.Group;

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var currentGm = GroupService.UserInGroup(UserContext.Current.Id, member.GroupId);            

            if (currentGm.State == (byte)GroupMemberState.Moderator)
            {
                if (member.State == (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Указанный пользователь является модератором");

                if (member.State == (byte)GroupMemberState.Approved)
                {
                    if (!approve)
                    {
                        GroupService.ChangeMemberStatus(member.UserId, member.GroupId, GroupMemberState.NotMember);

                        var msgText = string.Format("Модератор <a href='{0}'>{1}</a> иключил вас из группы <a href='{2}'>{3}</a>.",
                            Url.Action("index", "user", new { id = currentGm.User.Id }), currentGm.User.FullName,
                            Url.Action("details", "group", new { id = member.Group.Url }), member.Group.Name);

                        MessageService.Send(null, member.UserId, msgText, MessageType.GroupMemberNotice);
                    }
                    else
                        throw new BusinessLogicException("Указанный пользователь уже одобрен");
                }
                else if (group.State == (byte)GroupState.Blank || group.State == (byte)GroupState.Archive || 
                    (member.Group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) < member.Group.ModeratorsCount))
                {
                    GroupService.ChangeMemberStatus(member.UserId, member.GroupId, approve ? GroupMemberState.Moderator : GroupMemberState.NotMember);

                    string msgText;
                    if (approve)
                        msgText = string.Format( "Модератор группы <a href='{0}'>{1}</a> рассмотрел и одобрил вашу заявку. Теперь вы модератор группы <a href='{2}'>{3}</a>.",
                            Url.Action("index", "user", new { id = currentGm.User.Id }), currentGm.User.FullName,
                            Url.Action("details", "group", new { id = member.Group.Url }), member.Group.Name);
                    else
                        msgText = string.Format(
                            "Модератор группы <a href='{0}'>{1}</a> рассмотрел и отклонил вашу заявку.",
                            Url.Action("index", "user", new {id = currentGm.User.Id}), currentGm.User.FullName);

                    MessageService.Send(null, member.UserId, msgText, MessageType.GroupMemberNotice);

                    UserContext.Abandon(member.UserId);
                }
                else
                {
                    if (member.State == (byte)GroupMemberState.NotApproved)
                    {
                        GroupService.ChangeMemberStatus(member.UserId, member.GroupId, approve ? GroupMemberState.Approved : GroupMemberState.NotMember);

                        string msgText;
                        if (approve)
                            msgText = string.Format("Модератор группы <a href='{0}'>{1}</a> рассмотрел и одобрил вашу заявку. Теперь вы участник группы <a href='{2}'>{3}</a>.",
                                Url.Action("index", "user", new { id = currentGm.User.Id }), currentGm.User.FullName,
                                Url.Action("details", "group", new { id = member.Group.Url }), member.Group.Name);
                        else
                            msgText = string.Format("Модератор группы <a href='{0}'>{1}</a> рассмотрел и отклонил вашу заявку.",
                                Url.Action("index", "user", new { id = currentGm.User.Id }), currentGm.User.FullName);

                        MessageService.Send(null, member.UserId, msgText, MessageType.GroupMemberNotice);

                        UserContext.Abandon(member.UserId);
                    }
                    else
                        throw new BusinessLogicException("Этот пользователь забанен или не состоит в группе");
                }
            }
            else
                throw new BusinessLogicException("Вы не можете выполнить данное действие, т.к. вы не являетесь модератором группы");

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.PathAndQuery);

            return RedirectToAction("index", new { id = group.Url });
        }

        public ActionResult Leave(string id)
        {
            if (!Request.IsAuthenticated)
                throw new BusinessLogicException("Чтобы покинуть группу, нужно быть зарегистрированным и аутентифицированным");

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var userId = UserContext.Current.Id;

            var gm = GroupService.UserInGroup(userId, group.Id);
            if (gm == null)
                throw new BusinessLogicException("Вы не являетесь участником данной группы");

            //if (DateTime.Now - gm.EntryDate < ConstHelper.MinGroupMemberDuration)
            //    throw new BusinessLogicException("Нельзя покинуть группу ранее, чем через " + ConstHelper.MinGroupMemberDuration.TotalDays + " дней после вступления");

            if (gm.State == (byte)GroupMemberState.Moderator)
                GroupService.ChangeMemberStatus(gm.UserId, group.Id, GroupMemberState.Approved);
            else
            {
                if (group.Id == ConstHelper.RootGroupId)
                    throw new BusinessLogicException("Нельзя выйти из группы Федерация");

                GroupService.RemoveMember(userId, group.Id);
            }

            UserContext.Abandon();

            return RedirectToAction("index", new { id = DataService.PerThread.GroupSet.Single(g => g.Id == group.Id).Url });
        }

        public ActionResult Join(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            GroupService.AddMember(UserContext.Current.Id, group.Id);
            UserContext.Abandon();

            if (group.PrivacyEnum.HasFlag(GroupPrivacy.Invisible))
                return View("Result", (object)"Заявка подана, ожидайте подтверждения!");

            return RedirectToAction("index", new { id = group.Url });
        }

        #endregion

        #region Добавление и редактирование контента

        public ActionResult Post(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult AddPost(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var gm = GroupService.UserInGroup(UserContext.Current.Id, group.Id, true);

            if (group.Type == (byte)GroupType.Root && gm.State != (byte)GroupMemberState.Moderator) // TODO: на данный момент закрываем Федерацию для простых смертных
                throw new BusinessLogicException("На данный момент закрываем Федерацию для простых смертных");

            return View(new GroupAddPostViewModel(group));
        }

        [HttpPost]
        public ActionResult AddPost(GroupAddPostViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (ModelState.IsValid)
            {
                var userId = UserContext.Current.Id;
                var gm = GroupService.UserInGroup(userId, model.GroupId, true);

                if (group.Type == (byte)GroupType.Root && gm.State != (byte)GroupMemberState.Moderator) // TODO: на данный момент закрываем Федерацию для простых смертных
                    throw new BusinessLogicException("На данный момент закрываем Федерацию для простых смертных");

                ContentService.CreatePost(model.GroupId, userId, model.Title, model.Text, model.TagTitles, model.IsDraft, model.IsExternal);

                if (model.IsDraft)
                    return RedirectToAction("drafts", "user");

                return RedirectToAction("index", new { id = model.GroupUrl });
            }

            return View(model);
        }

        public ActionResult EditPost(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var post = DataService.PerThread.ContentSet.OfType<Post>().SingleOrDefault(p => p.Id == id);
            if (post == null)
                throw new BusinessLogicException("Указан неверный идентификатор поста");

            var group = post.Group;
            if (group == null)
                throw new BusinessLogicException("Данный пост не принадлежит ни к какой группе");

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var userId = UserContext.Current.Id;

            GroupService.UserInGroup(userId, group, true);

            if (post.AuthorId.HasValue && post.AuthorId.Value != userId)
                throw new BusinessLogicException("Вы не можете редактировать данный пост, т.к. не являетесь его автором");

            return View("editPost", new GroupEditPostViewModel(post));
        }

        [HttpPost]
        public ActionResult EditPost(GroupEditPostViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (ModelState.IsValid)
            {
                ContentService.UpdatePost(model.PostId, UserContext.Current.Id, model.Title, model.Text, model.TagTitles, model.IsDraft);

                return RedirectToAction("post", new { id = model.PostId });
            }

            return View(model);
        }

        public ActionResult ReplyComment(Guid id)
        {
            var parentComment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (parentComment == null)
                throw new BusinessLogicException("Не найден комментарий с указанным идентификатором");

            if (parentComment.Content.GroupId.HasValue)
            {
                var group = GroupService.GetGroupByLabelOrId(parentComment.Content.GroupId.Value);

                var redirect = HideInvisibleGroup(group);
                if (redirect != null)
                    return redirect;
            }

            return View(new ReplyCommentViewModel(parentComment));
        }

        public ActionResult EditComment(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var comment = DataService.PerThread.CommentSet.SingleOrDefault(x => x.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Указан неверный идентификатор комментария");

            var redirect = HideInvisibleGroup(comment.Content.Group);
            if (redirect != null)
                return redirect;

            if (comment.UserId != UserContext.Current.Id)
                throw new BusinessLogicException("Нельзя редактировать чужие комментарии");
            if (DateTime.Now - comment.DateTime > ConstHelper.CommentEditTimeSpan)
                throw new BusinessLogicException("Время на редактирование вышло");

            return View(new EditCommentViewModel(comment));
        }

        #endregion

        public ActionResult Albums(string id)
        {
            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new AlbumsViewModel(group));
        }

        #region Петиции

        public ActionResult Petition(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult PetitionSigners(Guid id)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Указан неверный идентификатор петиции");

            var group = petition.Group;
            if (group == null)
                throw new BusinessLogicException("Данная петиция не привязана ни к какой группе");

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            return View(new GroupPetitionSignersViewModel(petition));
        }

        public ActionResult CreatePetition(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            GroupService.UserInGroup(UserContext.Current.Id, group.Id, true);

            return View(new GroupCreatePetitionViewModel(group));
        }

        public ActionResult EditPetition(Guid id)
        {
            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            var redirect = HideInvisibleGroup(petition.Group);
            if (redirect != null)
                return redirect;

            if (petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Редактировать можно только черновые петиции");
            if (!petition.AuthorId.HasValue)
                throw new BusinessLogicException("У петиции нет автора");
            if (petition.AuthorId.Value != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            return View(new GroupEditPetitionViewModel(petition));
        }

        public ActionResult EditPetitionCoauthors(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var petition = DataService.PerThread.ContentSet.OfType<Petition>().SingleOrDefault(p => p.Id == id);
            if (petition == null)
                throw new BusinessLogicException("Не найдена петиция с указанным идентификатором");

            var redirect = HideInvisibleGroup(petition.Group);
            if (redirect != null)
                return redirect;

            if (petition.State != (byte)ContentState.Draft)
                throw new BusinessLogicException("Редактировать можно только черновые петиции");
            if (!petition.AuthorId.HasValue)
                throw new BusinessLogicException("У петиции нет автора");
            if (petition.AuthorId.Value != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не являетесь автором данной петиции");

            return View(new GroupEditPetitionCoauthorsViewModel(petition));
        }

        #endregion

        #region Голосования

        public ActionResult Poll(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult CreatePoll(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            GroupService.UserInGroup(UserContext.Current.Id, group.Id, true);

            if (group.State == (byte)GroupState.Blank)
                throw new BusinessLogicException("Нельзя создавать голосования в еще не оформленных группах");

            return View(new GroupCreatePollViewModel(group));
        }

        [HttpPost]
        public ActionResult CreatePoll(GroupCreatePollViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(model.GroupUrl);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            if (!ModelState.IsValid)
                return View(model);

            var data = new PollContainer
            {
                Title = model.Title,
                Text = model.Text,
                IsDraft = model.IsDraft,
                Duration = model.Duration,
                HasOpenProtocol = model.HasOpenProtocol
            };

            if (model.Topics != null)
                foreach (var t in model.Topics.Where(x => x.IsChecked))
                {
                    var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == t.Id);
                    if (tag == null)
                        throw new Exception("Неправильный идентификатор тэга");

                    data.Tags.Add(tag);
                }

            foreach (var tag in TagsHelper.ConvertStringToTagList(model.TagTitles, model.GroupId))
                data.Tags.Add(tag);

            var poll = VotingService.CreatePoll(model.GroupUrl, UserContext.Current.Id, data);

            return RedirectToAction("poll", new { id = poll.Id });
        }

        public ActionResult EditPoll(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(p => p.Id == id);
            if (poll == null)
                throw new BusinessLogicException("Указан неверный идентификатор голосования");

            var group = poll.Group;

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            GroupService.UserInGroup(UserContext.Current.Id, group, true);

            if (poll.AuthorId.HasValue && poll.AuthorId.Value != UserContext.Current.Id)
                throw new BusinessLogicException("Вы не можете редактировать данное голосование, т.к. не являетесь его автором");

            return View("editpoll", new GroupEditPollViewModel(poll));
        }

        [HttpPost]
        public ActionResult EditPoll(GroupEditPollViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var pollContainer = new PollContainer
                {
                    Duration = model.Duration,
                    IsDraft = model.IsDraft,
                    Text = model.Text,
                    Title = model.Title
                };

            if (model.Topics != null)
                foreach (var tag in model.Topics.Where(x => x.IsChecked))
                {
                    var tmp = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tag.Id);
                    if (tmp == null)
                        throw new BusinessLogicException("Указан неверный идентификатор тэга");

                    pollContainer.Tags.Add(tmp);
                }

            foreach (var tag in TagsHelper.ConvertStringToTagList(model.TagTitles, model.GroupId))
                pollContainer.Tags.Add(tag);

            VotingService.UpdatePoll(model.PollId, pollContainer);

            return RedirectToAction("poll", new {id = model.PollId});
        }

        public ActionResult StartPoll(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var poll = VotingService.StartPoll(id, UserContext.Current.Id);

            return RedirectToAction("poll", new { id = poll.Id });
        }

        public ActionResult VoteFor(Guid id, byte voteOption, string comment)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            VotingService.VotePoll(id, UserContext.Current.Id, (VoteOption)voteOption, comment);

            DataService.PerThread.SaveChanges();

            return RedirectToAction("poll", new { id });
        }

        public ActionResult RequestBulletin(Guid votingId, Guid userId)
        {
            VotingService.AddBulletinRequest(votingId, userId);

            return RedirectToAction("poll", new { id = votingId });
        }

        public ActionResult PollReport(Guid id)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == id);
            if (poll == null)
                throw new BusinessLogicException("Неверно указан идентификатор голосования");

            if (poll.HasOpenProtocol)
                return RedirectToAction("openpollreport", new { id = id });

            return View(new GroupPollReportViewModel(poll));
        }

        public ActionResult OpenPollReport(Guid id)
        {
            var poll = DataService.PerThread.ContentSet.OfType<Poll>().SingleOrDefault(x => x.Id == id);
            if (poll == null)
                throw new BusinessLogicException("Неверно указан идентификатор голосования");

            if (!poll.HasOpenProtocol)
                return RedirectToAction("pollreport", new { id = id });

            return View(new GroupOpenPollReportViewModel(poll));
        }

        public ActionResult SurveyResults(Guid id)
        {
            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == id);
            if (survey == null)
                throw new BusinessLogicException("Неверно указан идентификатор опроса");

            return View(new GroupSurveyResultsViewModel(survey));
        }

        #endregion

        #region Выборы

        public ActionResult Election(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult CreateElection(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            if (group.Type == (byte)GroupType.Root) // TODO: на данный момент не допускаем выборов в Федерации
                throw new BusinessLogicException("На данный момент не допускаем выборов в Федерации");

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            var gm = GroupService.UserInGroup(UserContext.Current.Id, group.Id);
            if (gm == null)
                throw new BusinessLogicException("Вы не состоите в этой группе");

            Guid? authorId = UserContext.Current.Id;

            if (GroupService.CanElectionsBeStarted(group) && gm.State == (byte)GroupMemberState.Approved)
                authorId = null;

            var election = VotingService.CreateElection(id, authorId);

            return RedirectToAction("election", new { election.Id });
        }

        public ActionResult BecomeCandidate(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var candidate = VotingService.BecomeCandidate(id, UserContext.Current.Id);

            return RedirectToAction("editcandidatepetition", new { id = candidate.Id });
        }

        public ActionResult EditCandidatePetition(Guid id)
        {
            var candidate = DataService.PerThread.CandidateSet.SingleOrDefault(x => x.Id == id);
            if (candidate == null)
                throw new BusinessLogicException("Не найден кандидат с указанным идентификатором");

            var redirect = HideInvisibleGroup(candidate.GroupMember.Group);
            if (redirect != null)
                return redirect;

            return View(new GroupEditCandidatePetitionViewModel(candidate));
        }

        [HttpPost]
        public ActionResult EditCandidatePetition(GroupEditCandidatePetitionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var candidate = DataService.PerThread.CandidateSet.SingleOrDefault(x => x.Id == model.CandidateId);
            if (candidate == null)
                throw new BusinessLogicException("Не найден кандидат с указанным идентификатором");

            var group = candidate.GroupMember.Group;

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            switch ((CandidateStatus)candidate.Status)
            {
                case CandidateStatus.Confirmed:
                    if (!string.IsNullOrWhiteSpace(model.Text))
                    {
                        VotingService.CreateCandidatePetition(candidate, model.Text);
                        DataService.PerThread.SaveChanges();
                    }
                    break;

                case CandidateStatus.Declared:
                    candidate.Petition.Text = model.Text;
                    DataService.PerThread.SaveChanges();
                    break;
            }

            return RedirectToAction("index", new { id = group.Url });
        }

        [HttpPost]
        public ActionResult ElectionVote(Group_ElectionViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            IList<Guid> result = model.Candidates.Where(x => x.IsChecked).Select(candidate => candidate.Id).ToList();

            var election = VotingService.ElectionVote(model.Id, UserContext.Current.Id, result);

            return RedirectToAction("election", new { id = election.Id });
        }

        #endregion

        #region Опросы

        public ActionResult Survey(Guid id)
        {
            return RedirectToAction("content", new { id });
        }

        public ActionResult CreateSurvey(string id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var group = GroupService.GetGroupByLabelOrId(id);

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            GroupService.UserInGroup(UserContext.Current.Id, group, true);

            return View(new GroupCreateEditSurveyViewModel(group));
        }

        [HttpPost]
        public ActionResult CreateSurvey(GroupCreateEditSurveyViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            if (!ModelState.IsValid)
                return View(model);

            var data = new SurveyData
                {
                    Duration = model.Duration,
                    GroupId = model.GroupId,
                    IsPrivate = model.IsPrivate,
                    IsDraft = model.IsDraft,
                    Tags = model.TagTitles,
                    Text = model.Text,
                    Title = model.Title,
                    VariantsCount = model.VariantsCount,
                    HasOpenProtocol = model.HasOpenProtocol
                };

            var options = model.Options
                .Select(o => new SurveyOptionData
                    {
                        Description = o.Description,
                        Title = o.Title
                    })
                .ToList();

            data.Options = options;

            var survey = VotingService.CreateSurvey(data, UserContext.Current.Id);

            return RedirectToAction("survey", new { id = survey.Id });
        }

        public ActionResult EditSurvey(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == id);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            var userId = UserContext.Current.Id;

            if (!survey.AuthorId.HasValue || survey.AuthorId.Value != userId)
                throw new BusinessLogicException("Вы не являетесь автором этого опроса");

            var group = survey.Group;

            var redirect = HideInvisibleGroup(group);
            if (redirect != null)
                return redirect;

            GroupService.UserInGroup(userId, group, true);

            return View(new GroupCreateEditSurveyViewModel(survey));
        }

        [HttpPost]
        public ActionResult EditSurvey(GroupCreateEditSurveyViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var survey = DataService.PerThread.ContentSet.OfType<Survey>().SingleOrDefault(x => x.Id == model.SurveyId);
            if (survey == null)
                throw new BusinessLogicException("Указан неверный идентификатор опроса");

            if (!ModelState.IsValid)
                return View(model);

            var data = new SurveyData
            {
                Id = model.SurveyId,
                Duration = model.Duration,
                IsPrivate = model.IsPrivate,
                Tags = model.TagTitles,
                Text = model.Text,
                Title = model.Title,
                VariantsCount = model.VariantsCount
            };

            var options = model.Options
                .Select(o => new SurveyOptionData
                {
                    Description = o.Description,
                    Title = o.Title
                })
                .ToList();

            data.Options = options;

            VotingService.UpdateSurvey(survey, data, UserContext.Current.Id);

            return RedirectToAction("survey", new { id = survey.Id });
        }

        [HttpPost]
        public ActionResult SurveyVote(Group_SurveyViewModel model)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            Survey survey;

            if (model.RadioOptionId.HasValue)
                survey = VotingService.SurveyVote(model.RadioOptionId.Value, UserContext.Current.Id);
            else
            {
                if (model.Options.Count(x => x.IsChecked) == 0)
                    throw new BusinessLogicException("Выберите вариант(ы)");

                var result = model.Options.Where(x => x.IsChecked).Select(option => option.Id).ToList();

                survey = VotingService.SurveyVote(model.Id, result, UserContext.Current.Id);
            }

            return RedirectToAction("survey", new { id = survey.Id });
        }

        public ActionResult SurveyNotVote(Guid id)
        {
            if (!Request.IsAuthenticated)
                throw new AuthenticationException();

            var survey = VotingService.SurveyNotVote(id, UserContext.Current.Id);

            return RedirectToAction("survey", new { id = survey.Id });
        }

        #endregion

        public ActionResult Rss(string id)
        {
            Group group;
            Guid groupGuid;

            if (string.IsNullOrWhiteSpace(id))
                return new EmptyResult();

            if (Guid.TryParse(id, out groupGuid))
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupGuid);
            else
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == id);

            if (group == null || group.PrivacyEnum.HasFlag(GroupPrivacy.Invisible))
                return new EmptyResult();

            var recentContent = group.Content
                .Where(x => x.State == (byte)ContentState.Approved && x.PublishDate.HasValue).OrderByDescending(x => x.PublishDate).Take(100);

            var model = new _RssFeedViewModel
            {
                Title = "Демократия2. " + group.Name + ". RSS",
                Description = "RSS-лента " + group.Name + " на Демократии2",
                Link = "http://democratia2.ru/Group/rss/" + group.Url
            };

            foreach (var content in recentContent)
            {
                var info = new StringBuilder();
                info.Append("<p>");
                info.Append("Автор: <a href='" + Url.Action("blog", "user", new { content.Author.Id }, "http") + "'>" + content.Author.FullName + "</a>");
                info.Append("</p>");

                var description = info + content.Text;
                var item = new _RssFeedItemViewModel
                    {
                        Title = content.Title,
                        Link = content.GetUrl(false),
                        Description = description
                    };

                if (content.PublishDate.HasValue)
                    item.PublishDate = content.PublishDate.Value.ToString("R");

                model.Items.Add(item);
            }

            return View("_Rss", "_EmptyLayout", model);
        }
    }
}
