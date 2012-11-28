using System;

namespace Federation.Core
{
    public static class SubscriptionService
    {
        private static readonly ISubscriptionService _subscribeService = new SubscriptionServiceImpl();

        public static void SubscribeToGroup(string id, Guid userId, bool saveChanges = true)
        {
            _subscribeService.SubscribeToGroup(id, userId, saveChanges);
        }

        public static void SubscribeToGroup(Group group, User subscriber, bool saveChanges = true)
        {
            _subscribeService.SubscribeToGroup(group, subscriber, saveChanges);
        }

        public static void SubscribeToUser(Guid id, Guid userId, bool saveChanges = true)
        {
            _subscribeService.SubscribeToUser(id, userId, saveChanges);
        }

        public static void UnsubscribeFromGroup(string id, Guid userId, bool saveChanges = true)
        {
            _subscribeService.UnsubscribeFromGroup(id, userId, saveChanges);
        }

        public static void UnsubscribeFromUser(Guid id, Guid userId, bool saveChanges = true)
        {
            _subscribeService.UnsubscribeFromUser(id, userId, saveChanges);
        }

        public static bool IsSubscriberOfGroup(string id, Guid userId)
        {
            return _subscribeService.IsSubscriberOfGroup(id, userId);
        }

        public static bool IsSubscriberOfUser(Guid id, Guid userId)
        {
            return _subscribeService.IsSubscriberOfUser(id, userId);
        }        
    }
}