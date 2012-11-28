using System;

namespace Federation.Core
{
    public static class MessageService
    {
        private static readonly IMessageService Current = new MessageServiceImpl();

        public static void Send(MessageStruct messageInfo)
        {
            Current.Send(messageInfo);
        }

        public static Message Send(Guid? authorId, Guid recipientId, string text, MessageType type, DateTime? date = null)
        {
            if (!date.HasValue)
                date = DateTime.Now;

            var messageInfo = new MessageStruct
            {
                AuthorId = authorId,
                RecipientId = recipientId,
                Text = text,
                Type = (byte)type,
                Date = date.Value
            };

            return Current.Send(messageInfo);
        }

        public static void SendToGroup(Group group, MessageStruct messageInfo, GroupMessageRecipientType recipientType = GroupMessageRecipientType.Members)
        {
            Current.SendToGroup(group, messageInfo, recipientType);
        }

        public static void SendToGroup(Group group, string text, MessageType type, GroupMessageRecipientType recipientType = GroupMessageRecipientType.Members, Guid? authorId = null, DateTime? date = null)
        {
            if (!date.HasValue)
                date = DateTime.Now;

            var messageInfo = new MessageStruct
            {
                AuthorId = authorId,
                Text = text,
                Type = (byte)type,
                Date = date.Value
            };

            Current.SendToGroup(group, messageInfo, recipientType);
        }

        public static void DeleteMessage(Guid userId, Guid msgId)
        {
            Current.DeleteMessage(userId, msgId);
        }

        public static void MarkAsRead(Guid userId, Guid? contactId)
        {
            Current.MarkAsRead(userId, contactId);
        }

        public static void ClearOldServiceMessages()
        {
            Current.ClearOldServiceMessages();   
        }        
    }
}