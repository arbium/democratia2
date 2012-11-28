using System;

namespace Federation.Core
{
    public partial class Tag
    {
        public Tag()
        {
            Id = Guid.NewGuid();
        }

        public Tag(string title) : this()
        {
            Title = title;
            LowerTitle = title.ToLower();
        }

        public void ChangeTitle(string newTitle)
        {
            Title = newTitle;
            LowerTitle = newTitle.ToLower();
        }
    }
}
