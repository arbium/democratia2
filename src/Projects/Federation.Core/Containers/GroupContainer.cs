using System;

namespace Federation.Core
{
    public class GroupContainer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public GroupState State { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }

        public bool IsApprovedMember { get; set; }
        public bool IsModerator { get; set; }

        public GroupContainer()
        {
        }

        public GroupContainer(GroupMember groupMember)
        {
            if (groupMember != null)
            {
                Id = groupMember.Group.Id;
                Name = groupMember.Group.Name;
                State = (GroupState)groupMember.Group.State;
                Url = groupMember.Group.Url;
                Logo = groupMember.Group.Logo;
                IsApprovedMember = groupMember.State == (byte)GroupMemberState.Approved || groupMember.State == (byte)GroupMemberState.Moderator;
                IsModerator = groupMember.State == (byte)GroupMemberState.Moderator;
            }
        }
    }
}
