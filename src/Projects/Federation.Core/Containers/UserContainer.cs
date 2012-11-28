using System;
using System.Collections.Generic;
using System.Linq;

namespace Federation.Core
{
    public class UserContainer
    {
        public Guid Id { get; set; }
        public bool IsVerified { get; set; }
        public DateTime LastAction { get; set; }
        public bool IsOutdated { get; set; }
        public bool IsPhoneVerified { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public TimeSpan UTCOffset { get; set; }
        public int UnreadMessages { get; set; }

        public IList<GroupContainer> Groups { get; set; }

        public List<Guid> SubscribedUsers { get; set; }
        public List<Guid> SubscribedGroups { get; set; }
        public List<Guid> BlackList { get; set; }

        public byte SentSmsCount { get; set; }

        public UserContainer()
        {
        }

        public UserContainer(User user)
        {
            if (user != null)
            {
                user.LastActivity = DateTime.Now;
                user.IsOutdated = false;

                Id = user.Id;
                IsVerified = user.IsVerified;
                LastAction = user.LastActivity;
                IsOutdated = user.IsOutdated;
                IsPhoneVerified = user.IsPhoneVerified;

                FirstName = user.FirstName;
                MiddleName = user.Patronymic;
                LastName = user.SurName;
                DisplayName = user.FullName;
                Avatar = user.Avatar;
                UTCOffset = new TimeSpan(0, user.UTCOffset, 0);

                UnreadMessages = user.InboxMessages.Count(x => !x.IsRead);

                SubscribedUsers = new List<Guid>();
                SubscribedGroups = new List<Guid>();

                foreach (var item in user.SubscriptionUsers)
                    SubscribedUsers.Add(item.Id);
                foreach (var item in user.SubscriptionGroups)
                    SubscribedGroups.Add(item.Id);

                BlackList = user.BlackList.Select(x => x.Id).ToList();

                Groups = new List<GroupContainer>();

                foreach (var usergroupe in user.GroupMembers
                    .Where(gm => gm.State != (byte)GroupMemberState.Banned && gm.State != (byte)GroupMemberState.NotMember)
                    .OrderByDescending(g => g.Group.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator)))
                    Groups.Add(new GroupContainer(usergroupe));
                
                DataService.PerThread.SaveChanges();
            }
        }

        public bool IsUserInGroup(Guid groupId)
        {
            var group = GroupService.UserInGroup(Id, groupId);

            if (group != null)
                return true;

            return false;
        }

        public bool IsUserApprovedInGroup(Guid groupId)
        {
            var group = Groups.SingleOrDefault(x => x.Id == groupId);

            if (group != null)
                return group.IsApprovedMember;

            return false;
        }

        public bool IsUserModeratorInGroup(Guid groupId)
        {
            var group = Groups.SingleOrDefault(x => x.Id == groupId);

            if (group != null)
                return group.IsModerator;

            return false;
        }
    }
}
