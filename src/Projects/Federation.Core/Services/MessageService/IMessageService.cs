using System;

namespace Federation.Core
{
    public interface IMessageService
    {
        Message Send(MessageStruct mesageInfo);
        void SendToGroup(Group group, MessageStruct messageInfo, GroupMessageRecipientType recipientType);
        void DeleteMessage(Guid userId, Guid msgId);
        void MarkAsRead(Guid userId, Guid? contactId);
        void ClearOldServiceMessages();
    }
}