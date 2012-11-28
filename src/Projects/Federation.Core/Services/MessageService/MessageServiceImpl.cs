using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;

namespace Federation.Core
{
    public class MessageServiceImpl : IMessageService
    {
        public Message Send(MessageStruct messageInfo)
        {
            if (DataService.PerThread.BaseUserSet.Count(u => u.Id == messageInfo.RecipientId) == 0)
                throw new BusinessLogicException("Получатель не найден!");

            if (messageInfo.AuthorId.HasValue)
            {
                if (DataService.PerThread.BaseUserSet.SingleOrDefault(u => u.Id == messageInfo.AuthorId) == null)
                    throw new BusinessLogicException("Отправитель не найден!");

                if (messageInfo.AuthorId == messageInfo.RecipientId)
                    throw new BusinessLogicException("Нельзя посылать самому себе!");
            }

            var message = new Message
            {
                AuthorId = messageInfo.AuthorId,
                Date = messageInfo.Date,                
                RecipientId = messageInfo.RecipientId,
                Text = messageInfo.Text,
                Type = messageInfo.Type,
                IsDeletedByAuthor = false,
                IsDeletedByRecipient = false
            };

            DataService.PerThread.MessageSet.AddObject(message);
            DataService.PerThread.SaveChanges();

            UserContextService.Abandon(message.RecipientId);

            return message;
        }

        public void SendToGroup(Group group, MessageStruct messageInfo, GroupMessageRecipientType recipientType)
        {
            var recipientIdList = new List<Guid>();

            if (recipientType.HasFlag(GroupMessageRecipientType.Members))
                recipientIdList.AddRange(DataService.PerThread.GroupMemberSet.Where(
                    x => x.GroupId == group.Id && x.State == (byte)GroupMemberState.Approved).Select(uig => uig.UserId)
                                             .ToList());

            if (recipientType.HasFlag(GroupMessageRecipientType.Moderators))
                recipientIdList.AddRange(DataService.PerThread.GroupMemberSet.Where(
                    x => x.GroupId == group.Id && x.State == (byte)GroupMemberState.Moderator).Select(uig => uig.UserId)
                                             .ToList());

            if (recipientType.HasFlag(GroupMessageRecipientType.NotApprovedUsers))
                recipientIdList.AddRange(DataService.PerThread.GroupMemberSet.Where(
                    x => x.GroupId == group.Id && x.State == (byte)GroupMemberState.NotApproved).Select(uig => uig.UserId)
                                             .ToList());

            recipientIdList = recipientIdList.Distinct().ToList();

            if (recipientIdList.Count > 0)
            {
                foreach (var userId in recipientIdList)
                {
                    var message = new MessageStruct
                    {
                        AuthorId = messageInfo.AuthorId,
                        RecipientId = userId,
                        Text = messageInfo.Text,
                        Type = messageInfo.Type,
                        Date = messageInfo.Date
                    };

                    Send(message);
                }

                DataService.PerThread.SaveChanges();
            }

            UserContextService.GroupMembersAbandon(group.Id);
        }

        public void DeleteMessage(Guid userId, Guid msgId)
        {
            var message = DataService.PerThread.MessageSet.SingleOrDefault(x => x.Id == msgId);
            if (message == null)
                throw new BusinessLogicException("Указан неверный идентификатор сообщения");

            if (message.AuthorId != userId && message.RecipientId != userId)
                throw new BusinessLogicException("Пользователь не имеет права удалять данное сообщение");

            if (message.AuthorId == userId)
                message.IsDeletedByAuthor = true;
            else if (message.RecipientId == userId)
            {
                message.IsRead = true;
                message.IsDeletedByRecipient = true;
            }

            DataService.PerThread.SaveChanges();

            if (message.RecipientId == userId)
                UserContextService.Abandon(userId);
        }

        public void MarkAsRead(Guid userId, Guid? contactId)
        {
            var messages = DataService.PerThread.MessageSet.Where(x => x.RecipientId == userId);

            if (contactId.HasValue)
                messages = messages.Where(x => x.AuthorId == contactId.Value);
            else
                messages = messages.Where(x => !x.AuthorId.HasValue);

            foreach (var msg in messages)
                msg.IsRead = true;

            DataService.PerThread.SaveChanges();

            UserContextService.Abandon(userId);
        }

        public void ClearOldServiceMessages()
        {
            var targets = DataService.PerThread.MessageSet
                .Where(x => x.Type != (byte)MessageType.PrivateMessage && EntityFunctions.DiffMonths(x.Date, DateTime.Today) > 1)
                .ToList();

            foreach (var msg in targets)
                DataService.PerThread.MessageSet.DeleteObject(msg);

            DataService.PerThread.SaveChanges();
        }        
    }
}