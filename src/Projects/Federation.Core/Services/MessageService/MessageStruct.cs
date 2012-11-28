using System;

namespace Federation.Core
{
    public class MessageStruct
    {
        public Guid? AuthorId { get; set; }
        public Guid RecipientId { get; set; }
        public string Text { get; set; }
        public byte Type { get; set; }
        public DateTime Date { get; set; }
    }
}