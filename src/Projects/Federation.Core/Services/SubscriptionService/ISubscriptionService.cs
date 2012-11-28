using System;

namespace Federation.Core
{
    public interface ISubscriptionService
    {
        void SubscribeToGroup(string id, Guid userId, bool saveChanges);
        void SubscribeToGroup(Group group, User subscriber, bool saveChanges);
        void SubscribeToUser(Guid id, Guid userId, bool saveChanges);
        void UnsubscribeFromGroup(string id, Guid userId, bool saveChanges);
        void UnsubscribeFromUser(Guid id, Guid userId, bool saveChanges);
        bool IsSubscriberOfGroup(string id, Guid userId);
        bool IsSubscriberOfUser(Guid id, Guid userId);
    }
}