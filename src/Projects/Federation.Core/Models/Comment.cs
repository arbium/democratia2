using System;
using System.Linq;

namespace Federation.Core
{
    public partial class Comment
    {
        public int Rating
        {
            get { return Likes.Count(x => x.Value) - Likes.Count(x => !x.Value); }
        }

        public Comment()
        {
            Id = Guid.NewGuid();
            IsHidden = false;
            DateTime = DateTime.Now;
        }
    }
}