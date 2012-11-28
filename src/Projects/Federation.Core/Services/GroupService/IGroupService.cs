using System;

namespace Federation.Core
{
    public interface IGroupService
    {
        bool IsUserApprovedInGroup(Guid userId, Group group);

        int CountMembers(Group group);
        int CountMembers(string groupId);

        bool CanElectionsBeStarted(string groupId, bool validate);
        bool CanElectionsBeStarted(Group group, bool validate);

        Group GetGroupByLabelOrId(string id);
		Group GetGroupByLabelOrId(Guid id);
		
        //Создание новой группы
        Group CreateGroup(AddGroupStruct groupData);

        //Обновление информации о группе
        Group UpdateGroup(EditGroupStruct newGroupData, bool saveChanges);

        void AddFriendlyGroup(string group, string friendlyGroup, Guid? userId);
        void RemoveFriendlyGroup(string group, string friendlyGroup, Guid? userId);

        Group SetPrivacy(GroupPrivacyStruct privacyData);

        //Существование другой группы с таким label
        bool ExistAnotherGroupWithLabel(string label, Guid? id);

        //Добавление нового члена
        GroupMember AddMember(Guid userId, Guid groupId, bool saveChanges);

        void ChangeMemberStatus(Guid userId, Guid groupId, GroupMemberState role);

        //Пользователь как член группы
        GroupMember UserInGroup(Guid userId, Guid groupId, bool check);
        GroupMember UserInGroup(Guid userId, Group group, bool check);

        void RemoveMember(Guid userId, Guid groupId, bool saveChanges);

        //Закрытие группы
        void CloseGroup();

        void ClearOldBlankGroups();
    }
}