using System;
using System.Linq;

namespace Federation.Core
{
    public class SubscriptionServiceImpl : ISubscriptionService
    {
        public void SubscribeToGroup(string groupUrl, Guid userId, bool saveChanges)
        {
            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Пользователь с указанный идентификатором не найден");

            Group group;
            Guid groupGuid;

            if (string.IsNullOrWhiteSpace(groupUrl))
                throw new BusinessLogicException("Не указан идентификатор группы");

            if (Guid.TryParse(groupUrl, out groupGuid))
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupGuid);
            else
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == groupUrl);

            if (group == null)
                throw new BusinessLogicException("Указанная группа не найдена");

            SubscribeToGroup(group, subscriber, saveChanges);
        }

        public void SubscribeToGroup(Group group, User subscriber, bool saveChanges)
        {
            if (!subscriber.SubscriptionGroups.Contains(group))
            {
                subscriber.SubscriptionGroups.Add(group);

                if (saveChanges)
                    DataService.PerThread.SaveChanges();
            }
        }

        public void SubscribeToUser(Guid id, Guid userId, bool saveChanges)
        {
            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Перезайдите");

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                throw new BusinessLogicException("Указан неверный идентификатор пользователя");
            
            if (!subscriber.SubscriptionUsers.Contains(user))
                subscriber.SubscriptionUsers.Add(user);

            if (subscriber.BlackList.Contains(user))
                subscriber.BlackList.Remove(user);

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public void UnsubscribeFromGroup(string id, Guid userId, bool saveChanges)
        {
            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Перезайдите");

            Group group;
            Guid groupGuid;

            if (string.IsNullOrWhiteSpace(id))
                throw new BusinessLogicException("Не указан идентификатор группы");

            if (Guid.TryParse(id, out groupGuid))
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupGuid);
            else
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == id);

            if (group == null)
                throw new BusinessLogicException("Группа не найдена");

            var uig = GroupService.UserInGroup(subscriber.Id, group.Id);
            if (uig != null)
                throw new BusinessLogicException("Нельзя отписаться от группы, в которой вы состоите");

            if (subscriber.SubscriptionGroups.Contains(group))
                subscriber.SubscriptionGroups.Remove(group);

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public void UnsubscribeFromUser(Guid id, Guid userId, bool saveChanges)
        {
            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Перезайдите");

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                throw new BusinessLogicException("Указан неверный идентификатор пользователя");
            
            if (subscriber.SubscriptionUsers.Contains(user))
                subscriber.SubscriptionUsers.Remove(user);

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public bool IsSubscriberOfGroup(string id, Guid userId)
        {
            Group group;
            Guid groupGuid;

            if (string.IsNullOrWhiteSpace(id))
                throw new BusinessLogicException("Не указан идентификатор группы");

            if (Guid.TryParse(id, out groupGuid))
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Id == groupGuid);
            else
                group = DataService.PerThread.GroupSet.SingleOrDefault(g => g.Label == id);

            if (group == null)
                throw new BusinessLogicException("Группа не найдена");

            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Перезайдите");

            return group.Subscribers.Contains(subscriber);
        }

        public bool IsSubscriberOfUser(Guid id, Guid userId)
        {
            var subscriber = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (subscriber == null)
                throw new BusinessLogicException("Перезайдите");

            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == id);
            if (user == null)
                throw new BusinessLogicException("Указан неверный идентификатор пользователя");

            return user.Subscribers.Contains(subscriber);
        }
    }
}
