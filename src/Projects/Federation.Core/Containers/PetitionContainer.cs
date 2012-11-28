using System;

namespace Federation.Core
{
    public class PetitionContainer
    {
        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPrivate { get; set; }
        public string Tags { get; set; }
    }
}