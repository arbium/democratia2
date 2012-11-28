using System.Collections.Generic;

namespace Federation.Core
{
    public class PollContainer 
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsDraft { get; set; }
        public IList<Tag> Tags { get; set; }
        public short Duration { get; set; }
        public bool HasOpenProtocol { get; set; }

        public PollContainer()
        {
            Tags = new List<Tag>();
        }
    }
}