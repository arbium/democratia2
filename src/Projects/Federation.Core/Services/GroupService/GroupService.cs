using System;

namespace Federation.Core
{
    public static class GroupService
    {
        private static readonly IGroupService Current = new GroupServiceImpl();

        public static bool IsUserApprovedInGroup(Guid userId, Group group)
        {
            return Current.IsUserApprovedInGroup(userId, group);
        }

        public static int CountMembers(Group group)
        {
            return Current.CountMembers(group);
        }

        public static int CountMembers(string groupId)
        {
            return Current.CountMembers(groupId);
        }

        public static Group GetGroupByLabelOrId(string id)
        {
            return Current.GetGroupByLabelOrId(id);
        }

        public static bool CanElectionsBeStarted(string groupId, bool validate = false)
        {
            return Current.CanElectionsBeStarted(groupId, validate);
        }

        public static bool CanElectionsBeStarted(Group group, bool validate = false)
        {
            return Current.CanElectionsBeStarted(group, validate);
        }

        public static Group CreateGroup(AddGroupStruct groupData)
        {
            return Current.CreateGroup(groupData);
        }

        public static Group UpdateGroup(EditGroupStruct newGroupData, bool saveChanges = true)
        {
            return Current.UpdateGroup(newGroupData, saveChanges);
        }

        public static void AddFriendlyGroup(string group, string friendlyGroup, Guid? userId = null)
        {
            Current.AddFriendlyGroup(group, friendlyGroup, userId);
        }

        public static void RemoveFriendlyGroup(string group, string friendlyGroup, Guid? userId = null)
        {
            Current.RemoveFriendlyGroup(group, friendlyGroup, userId);
        }

        public static Group SetPrivacy(GroupPrivacyStruct privacyData)
        {
            return Current.SetPrivacy(privacyData);
        }

        public static bool ExistAnotherGroupWithLabel(string label, Guid? id = null)
        {
            return Current.ExistAnotherGroupWithLabel(label, id);
        }

        public static GroupMember AddMember(Guid userId, Guid groupId, bool saveChanges = true)
        {
            return Current.AddMember(userId, groupId, saveChanges);
        }

        public static Group GetGroupByLabelOrId(Guid id)
        {
            return Current.GetGroupByLabelOrId(id);
        }

        public static void ChangeMemberStatus(Guid userId, Guid groupId, GroupMemberState role)
        {
            Current.ChangeMemberStatus(userId, groupId, role);
        }

        public static GroupMember UserInGroup(Guid userId, Guid groupId, bool check = false)
        {
            return Current.UserInGroup(userId, groupId, check);
        }

        public static GroupMember UserInGroup(Guid userId, Group group, bool check = false)
        {
            return Current.UserInGroup(userId, group, check);
        }

        public static void RemoveMember(Guid userId, Guid groupId, bool saveChanges = true)
        {
            Current.RemoveMember(userId, groupId, saveChanges);
        }

        public static void CloseGroup()
        {
            Current.CloseGroup();
        }

        public static void ClearOldBlankGroups()
        {
            Current.ClearOldBlankGroups();
        }
    }
}