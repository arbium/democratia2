using System;

namespace Federation.Core
{
    public class GroupPrivacyStruct
    {
        public Guid GroupId { get; set; }
        public bool IsMemberModeration { get; set; }
        public bool IsContentModeration { get; set; }
        public bool IsPrivateDiscussion { get; set; }
        public bool IsInvisible { get; set; }
    }
}