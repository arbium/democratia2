using System;
using System.Linq;

namespace Federation.Core
{
    public class GroupServiceImpl : IGroupService
    {
        public bool IsUserApprovedInGroup(Guid userId, Group group)
        {
            var gm = UserInGroup(userId, group);
            if (gm == null)
                return false;

            var state = (GroupMemberState) gm.State;
            if (state == GroupMemberState.Moderator || state == GroupMemberState.Approved)
                return true;

            return false;
        }

        public int CountMembers(string groupId)
        {
            var group = GetGroupByLabelOrId(groupId);
            return CountMembers(group);
        }

        public int CountMembers(Group group)
        {
            return group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator);
        }

        public bool CanElectionsBeStarted(string groupId, bool validate)
        {
            var group = GetGroupByLabelOrId(groupId);
            return CanElectionsBeStarted(group, validate);
        }

        public bool CanElectionsBeStarted(Group group, bool validate)
        {
            try
            {
                if (group.State == (byte)GroupState.Blank)
                    throw new BusinessLogicException("Нельзя инициировать выборы в еще неоформленных группах");

                if (GroupService.CountMembers(group) < ConstHelper.MinGMsCountForElection)
                    throw new BusinessLogicException("Для возможности инициации выборов группа должна иметь не менее " + ConstHelper.MinGMsCountForElection + " членов.");

                if (group.Content.OfType<Election>().Count(x => !x.IsFinished) != 0)
                    throw new BusinessLogicException("В группе еще есть незакончившиеся выборы");

                var lastElection = group.Content.OfType<Election>().OrderByDescending(x => x.PublishDate).FirstOrDefault();
                if (lastElection != null && lastElection.PublishDate.HasValue)
                    if(lastElection.PublishDate.Value.AddMonths(group.ElectionFrequency) < DateTime.Now && lastElection.Stage != (byte)ElectionStage.Failed)
                    throw new BusinessLogicException("Еще не пришло время. Установленная частота выборов в группе: каждые " +
                        DeclinationService.OfNumber(group.ElectionFrequency, "месяц", "месяца", "месяцев"));
            }
            catch (Exception)
            {
                if (validate)
                    throw;

                return false;
            }

            return true;
        }

        public Group GetGroupByLabelOrId(string id)
        {
            Group group;
            Guid groupGuid;

            if (string.IsNullOrWhiteSpace(id))
                throw new BusinessLogicException("Не указан идентификатор группы");

            if (Guid.TryParse(id, out groupGuid))
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupGuid);
            else
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == id);

            if (group == null)
                throw new BusinessLogicException("Группа не найдена");

            return group;
        }

        public Group GetGroupByLabelOrId(Guid id)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == id);
            if (group == null)
                throw new BusinessLogicException("Группа не найдена");

            return group;
        }

        private void UpdateGroupState(Guid groupId)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы");

            if (group.Type != (byte)GroupType.Root)
            {
                if (group.State == (byte)GroupState.Blank && group.ModeratorsCount < ConstHelper.MinModeratorsInGroup && group.CreationDate < DateTime.Now.AddDays(-7))
                {
                    //TODO: Удалять группу
                }

                if (group.State != (byte)GroupState.Archive)
                {
                    //Если группа фаундинг и кол-во модеров упало ниже минимального, то группа станет архивной
                    if (group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) < ConstHelper.MinModeratorsInGroup && group.State == (byte)GroupState.Founding)
                        group.State = (byte)GroupState.Archive;

                    //Если группа бланк и кол-во модеров стало больше равно установленному в настройках, то группа станет фаундинг
                    if (group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) >= group.ModeratorsCount && group.State == (byte)GroupState.Blank)
                    {
                        if (!group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration))
                        {
                            foreach (var member in group.GroupMembers)
                            {
                                if (member.State == (byte)GroupMemberState.NotApproved)
                                    ChangeMemberStatus(member.UserId, member.GroupId, GroupMemberState.Approved);
                            }
                        }
                        group.State = (byte)GroupState.Founding;
                    }

                    //TODO: условия превращения в Formed группу и обратно (полный состав модераторов - определенное количество пользователей)
                }
                else
                {
                    if (group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) >= group.ModeratorsCount)
                        group.State = (byte)GroupState.Founding;
                }

                DataService.PerThread.SaveChanges();
                CachService.DropViewModelByModel(group.Id);
            }
        }

        public void AddFriendlyGroup(string groupId, string friendlyGroupId, Guid? userId = null)
        {
            if (groupId == friendlyGroupId)
                throw new BusinessLogicException("Группа с собой не дружит");

            var group = GetGroupByLabelOrId(groupId);
            
            if (userId.HasValue)
            {
                var gm = UserInGroup(userId.Value, group, true);
                if (gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Вы не модератор");
            }

            var friendlyGroup = GetGroupByLabelOrId(friendlyGroupId);

            if (group.FriendlyGroups.Contains(friendlyGroup))
                return;

            group.FriendlyGroups.Add(friendlyGroup);

            DataService.PerThread.SaveChanges();

            var msgText = string.Format("Группа <a href='{0}'>{1}</a> добавила вашу группу <a href='{2}'>{3}</a> в друзья. Вы можете <a href='{4}'>сделать тоже самое в ответ</a>.",
                    UrlHelper.GetUrl<Group>(groupId), group.Name, UrlHelper.GetUrl<Group>(friendlyGroupId, false), friendlyGroup.Name,
                    UrlHelper.GetActionUrl("addfriendlygroup", "group", friendlyGroupId, "fg=" + groupId));

            MessageService.SendToGroup(group, msgText, MessageType.GroupModeratorNotice, GroupMessageRecipientType.Moderators);
        }

        public void RemoveFriendlyGroup(string groupId, string friendlyGroupId, Guid? userId = null)
        {
            var group = GetGroupByLabelOrId(groupId);

            if (userId.HasValue)
            {
                var gm = UserInGroup(userId.Value, group, true);
                if (gm.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Вы не модератор");
            }

            var friendlyGroup = GetGroupByLabelOrId(friendlyGroupId);

            if (!group.FriendlyGroups.Contains(friendlyGroup))
                return;

            group.FriendlyGroups.Remove(friendlyGroup);

            DataService.PerThread.SaveChanges();

            var msgText = string.Format("Группа <a href='{0}'>{1}</a> удалила вашу группу <a href='{2}'>{3}</a> из друзей. Вы можете <a href='{4}'>сделать тоже самое в ответ</a>.",
                    UrlHelper.GetUrl<Group>(groupId), group.Name, UrlHelper.GetUrl<Group>(friendlyGroupId, false), friendlyGroup.Name,
                    UrlHelper.GetActionUrl("removefriendlygroup", "group", friendlyGroupId, "fg=" + groupId));

            MessageService.SendToGroup(group, msgText, MessageType.GroupModeratorNotice, GroupMessageRecipientType.Moderators);
        }

        public Group SetPrivacy(GroupPrivacyStruct privacyData)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == privacyData.GroupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            var groupPrivacy = (GroupPrivacy)group.Privacy;

            if (privacyData.IsContentModeration)
            {
                if (!groupPrivacy.HasFlag(GroupPrivacy.ContentModeration))
                    groupPrivacy = (short)groupPrivacy + GroupPrivacy.ContentModeration;
            }
            else
            {
                if (groupPrivacy.HasFlag(GroupPrivacy.ContentModeration))
                    groupPrivacy = (GroupPrivacy)((short)groupPrivacy - (short)GroupPrivacy.ContentModeration);
            }

            if (privacyData.IsInvisible)
            {
                if (!groupPrivacy.HasFlag(GroupPrivacy.Invisible))
                    groupPrivacy = (short)groupPrivacy + GroupPrivacy.Invisible;
            }
            else
            {
                if (groupPrivacy.HasFlag(GroupPrivacy.Invisible))
                    groupPrivacy = (GroupPrivacy)((short)groupPrivacy - (short)GroupPrivacy.Invisible);
            }

            if (privacyData.IsMemberModeration)
            {
                if (!groupPrivacy.HasFlag(GroupPrivacy.MemberModeration))
                    groupPrivacy = (short)groupPrivacy + GroupPrivacy.MemberModeration;
            }
            else
            {
                if (groupPrivacy.HasFlag(GroupPrivacy.MemberModeration))
                    groupPrivacy = (GroupPrivacy)((short)groupPrivacy - (short)GroupPrivacy.MemberModeration);
            }

            if (privacyData.IsPrivateDiscussion)
            {
                if (!groupPrivacy.HasFlag(GroupPrivacy.PrivateDiscussion))
                    groupPrivacy = (short)groupPrivacy + GroupPrivacy.PrivateDiscussion;
            }
            else
            {
                if (groupPrivacy.HasFlag(GroupPrivacy.PrivateDiscussion))
                    groupPrivacy = (GroupPrivacy)((short)groupPrivacy - (short)GroupPrivacy.PrivateDiscussion);
            }

            group.Privacy = (short)groupPrivacy;

            DataService.PerThread.SaveChanges();

            return group;
        }

        public Group CreateGroup(AddGroupStruct groupData)
        {
            //var parentGroup = DataService.PerThread.GroupSet.Where(g => g.Id == groupData.ParentGroupId).SingleOrDefault();

            var parentGroup = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == ConstHelper.RootGroupId); //TODO: пока нет вложенности, будет так
            if (parentGroup == null)
                throw new BusinessLogicException("В качестве родительской указана не существующая группа");

            if (ExistAnotherGroupWithLabel(groupData.Label))
                throw new BusinessLogicException("Уже существует группа с указанным label (ЧПУ)");

            if (groupData.ElectionFrequency > 12)
                throw new BusinessLogicException("Выборы в группе не могут происходить реже чем раз в 12 месяцев");
            if (groupData.ElectionFrequency < 1)
                throw new BusinessLogicException("Выборы в группе не могут происходить реже чем раз в месяц");
            if (groupData.ModeratorsCount < 3)
                throw new BusinessLogicException("В группе должно быть не менее 3х модераторов");

            var group = new Group
            {
                CreationDate = DateTime.Now,
                Name = groupData.Name,
                Label = TextHelper.CleanGroupLabel(groupData.Label),
                Logo = groupData.Logo,
                Summary = groupData.Summary,
                ModeratorsCount = groupData.ModeratorsCount,
                ElectionFrequency = groupData.ElectionFrequency,
                ParentGroup = parentGroup,
                PollQuorum = groupData.PollQuorum,
                ElectionQuorum = groupData.ElectionQuorum,
                State = (byte)GroupState.Blank
            };

            if (groupData.Categories != null)
                foreach (var cid in groupData.Categories)
                {
                    var category = DataService.PerThread.GroupCategorySet.SingleOrDefault(x => x.Id == cid);
                    if (category != null)
                        group.Categories.Add(category);
                }

            group.GroupRating = new GroupRating();

            DataService.PerThread.GroupSet.AddObject(group);
            DataService.PerThread.SaveChanges();

            return group;
        }

        public Group UpdateGroup(EditGroupStruct newGroupData, bool saveChanges = true)
        {
            //var parentGroup = DataService.PerThread.GroupSet.Where(g => g.Id == groupData.ParentGroupId).SingleOrDefault();

            var parentGroup = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == ConstHelper.RootGroupId); //TODO: пока нет вложенности, будет так
            if (parentGroup == null)
                throw new BusinessLogicException("В качестве родительской указана не существующая группа");

            if (ExistAnotherGroupWithLabel(newGroupData.Label, newGroupData.GroupId))
                throw new BusinessLogicException("Существует другая группа с указанным label (ЧПУ)");

            var group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == newGroupData.GroupId);
            if (group == null)
                throw new BusinessLogicException("Такой группы не существует");

            if (newGroupData.ElectionFrequency > 12)
                throw new BusinessLogicException("Выборы в группе не могут происходить реже чем раз в 12 месяцев");
            if (newGroupData.ElectionFrequency < 1)
                throw new BusinessLogicException("Выборы в группе не могут происходить реже чем раз в месяц");

            if (newGroupData.ModeratorsCount < 3)
                throw new BusinessLogicException("В группе должно быть не менее 3х модераторов");

            group.Name = newGroupData.Name;
            group.Label = newGroupData.Label;
            group.Summary = newGroupData.Summary;
            
            group.Categories.Clear();

            foreach (var cid in newGroupData.Categories)
            {
                var category = DataService.PerThread.GroupCategorySet.SingleOrDefault(x => x.Id == cid);
                if (category != null)
                    group.Categories.Add(category);
            }

            //IList<Tag> tagsToDelete = new List<Tag>();
            //IList<ExpertVote> evToDelete = new List<ExpertVote>();

            //foreach (var tag in group.Tags)
            //    if (!newTagsList.Contains(tag))
            //    {
            //        tagsToDelete.Add(tag);
            //        foreach (var gm in group.GroupMembers)
            //            if (gm.Expert != null)
            //                gm.Expert.Tags.Remove(tag);
            //        foreach (var ev in DataService.PerThread.ExpertVoteSet.Where(x => x.GroupMember.GroupId == group.Id && x.TagId == tag.Id))
            //            evToDelete.Add(ev);
            //    }
            //foreach (var ev in evToDelete)
            //    DataService.PerThread.ExpertVoteSet.DeleteObject(ev);
            //foreach (var tag in tagsToDelete)
            //    group.Tags.Remove(tag);

            //foreach (var tag in newTagsList)
            //    if (!group.Tags.Contains(tag))
            //        group.Tags.Add(tag);

            group.ModeratorsCount = newGroupData.ModeratorsCount;
            group.ElectionFrequency = newGroupData.ElectionFrequency;
            group.ParentGroup = parentGroup;
            group.PollQuorum = newGroupData.PollQuorum;
            group.ElectionQuorum = newGroupData.ElectionQuorum;

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            CachService.DropViewModelByModel(group.Id);
            UserContextService.GroupMembersAbandon(group.Id);

            return group;
        }

        public bool ExistAnotherGroupWithLabel(string label, Guid? id = null)
        {
            var result = false;

            var group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == label);

            if (group != null)
            {
                if (id.HasValue && group.Id != id || !id.HasValue)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Добавление указанного пользователя в группу
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="groupId">идентификатор группы</param>
        /// <returns>экземпляр участника группы</returns>
        public GroupMember AddMember(Guid userId, Guid groupId, bool saveChanges = true)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Не найдена группа с указанным идентификатором");

            var gm = DataService.PerThread.GroupMemberSet.SingleOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            if (gm == null)
            {
                gm = new GroupMember
                {
                    EntryDate = DateTime.Now,
                    GroupId = groupId,
                    State = (byte)GroupMemberState.NotApproved,
                    UserId = userId
                };

                if (!group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration) && group.State != (byte)GroupState.Blank || group.State == (byte)GroupState.Archive)
                    gm.State = (byte)GroupMemberState.Approved;

                DataService.PerThread.GroupMemberSet.AddObject(gm);
                DataService.PerThread.SaveChanges();

                if (group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) == 0)
                    GroupService.ChangeMemberStatus(userId, group.Id, GroupMemberState.Moderator);

                SubscriptionService.SubscribeToGroup(group.Url, gm.UserId);
            }
            else if (gm.State != (byte)GroupMemberState.Approved && gm.State != (byte)GroupMemberState.Moderator)
            {
                if (gm.State == (byte)GroupMemberState.Banned && DateTime.Now < gm.ExitDate.Value.AddMonths(2))
                    throw new BusinessLogicException("Вы не можете вступать в эту группу еще " + (gm.ExitDate.Value.AddMonths(2) - DateTime.Now).Days + ", так как были заблокированы");

                if (group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator) == 0)
                {
                    GroupService.ChangeMemberStatus(userId, group.Id, GroupMemberState.Moderator);
                }
                else if (!group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration) && group.State != (byte)GroupState.Blank)
                {
                    gm.State = (byte)GroupMemberState.Approved;
                }
                else
                    gm.State = (byte)GroupMemberState.NotApproved;
            }

            UpdateGroupState(group.Id);

            return gm;
        }

        public void ChangeMemberStatus(Guid userId, Guid groupId, GroupMemberState state)
        {
            var gm = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.GroupId == groupId & x.UserId == userId);

            if (gm == null)
                throw new BusinessLogicException("Указанный участник группы не найден");

            if (gm.State != (byte)GroupMemberState.Moderator && state == GroupMemberState.NotMember)
            {
                gm.State = (byte)GroupMemberState.NotMember;
            }

            if (gm.State != (byte)GroupMemberState.Approved && state == GroupMemberState.Approved)
            {
                gm.State = (byte)GroupMemberState.Approved;
                VotingService.AnalizeGroupMemberBulletins(gm.Id);
            }

            if (gm.State != (byte)GroupMemberState.Moderator && state == GroupMemberState.Moderator)
            {
                if (gm.Group.ModeratorsCount > gm.Group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Moderator))
                    gm.State = (byte)GroupMemberState.Moderator;
                else
                {
                    gm.State = (byte)GroupMemberState.Approved;
                    VotingService.AnalizeGroupMemberBulletins(gm.Id);
                }
            }

            DataService.PerThread.SaveChanges();

            UpdateGroupState(gm.GroupId);
        }

        public GroupMember UserInGroup(Guid userId, Guid groupId, bool check = false)
        {
            GroupMember gm;

            if (check)
            {
                gm = DataService.PerThread.GroupMemberSet.SingleOrDefault(m => m.UserId == userId && m.GroupId == groupId);

                if (gm == null)
                    throw new BusinessLogicException("Вы не состоите в данной группе");
                if (gm.State == (byte)GroupMemberState.NotMember)
                    throw new BusinessLogicException("Вы не состоите в данной группе");
                if (gm.State == (byte)GroupMemberState.Banned)
                    throw new BusinessLogicException("Вы были забанены в этой группе");
                if (gm.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Ваше участие в этой группе еще не одобрено");
            }
            else
            {
                gm = DataService.PerThread.GroupMemberSet.SingleOrDefault(
                    m => m.UserId == userId && m.GroupId == groupId && m.State != (byte)GroupMemberState.Banned && m.State != (byte)GroupMemberState.NotMember);
            }

            return gm;
        }

        public GroupMember UserInGroup(Guid userId, Group group, bool check = false)
        {
            GroupMember gm;

            if (check)
            {
                gm = group.GroupMembers.SingleOrDefault(m => m.UserId == userId);

                if (gm == null)
                    throw new BusinessLogicException("Вы не состоите в данной группе");
                if (gm.State == (byte)GroupMemberState.NotMember)
                    throw new BusinessLogicException("Вы не состоите в данной группе");
                if (gm.State == (byte)GroupMemberState.Banned)
                    throw new BusinessLogicException("Вы были забанены в этой группе");
                if (gm.State == (byte)GroupMemberState.NotApproved)
                    throw new BusinessLogicException("Ваше участие в этой группе еще не одобрено");
            }
            else
            {
                gm =
                    DataService.PerThread.GroupMemberSet.SingleOrDefault(
                        m =>
                        m.GroupId == group.Id && m.UserId == userId && m.State != (byte) GroupMemberState.Banned &&
                        m.State != (byte) GroupMemberState.NotMember);

            }

            return gm;
        }

        public void RemoveMember(Guid userId, Guid groupId, bool saveChanges = true)
        {
            var gm = UserInGroup(userId, groupId);
            if (gm == null)
                throw new BusinessLogicException("Группа не найдена, либо пользователь не является членом этой группы");

            gm.State = (byte)GroupMemberState.NotMember;
            gm.ExitDate = DateTime.Now;
            gm.Group.Subscribers.Remove(gm.User);

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(e => e.GroupMember.GroupId == groupId && e.GroupMember.UserId == userId);

            if (expert != null)
                DelegationService.RemoveExpert(groupId.ToString(), userId);

            var evToDelete = gm.ExpertVotes.ToList();
            gm.ExpertVotes.Clear();

            foreach (var expertVote in evToDelete)
                DataService.PerThread.ExpertVoteSet.DeleteObject(expertVote);

            if (saveChanges)
                DataService.PerThread.SaveChanges();

            UpdateGroupState(groupId);
        }

        public void CloseGroup()
        {
        }

        public void ClearOldBlankGroups()
        {
        }
    }
}
