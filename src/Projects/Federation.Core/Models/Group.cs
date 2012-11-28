using System;

namespace Federation.Core
{
    public partial class Group
    {
        public string Url
        {
            get
            {
                return String.IsNullOrWhiteSpace(Label) ? Id.ToString() : Label;
            }
        }

        public GroupPrivacy PrivacyEnum
        {
            get { return (GroupPrivacy) Privacy; }
        }

        public int MembersCount
        {
            get { return GroupService.CountMembers(this); }
        }

        public Group()
        {
            Id = Guid.NewGuid();
        }
    }
}
