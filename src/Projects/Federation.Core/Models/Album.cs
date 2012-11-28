using System;

namespace Federation.Core
{
    public partial class Album
    {
        public Album()
        {
            Id = Guid.NewGuid();
        }

        public string Controller
        {
            get
            {
                if (GroupId.HasValue)
                    return "group";

                return "user";
            }
        }
    }
}