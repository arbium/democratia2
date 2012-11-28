using System;
using System.Linq;
using System.Collections.Concurrent;

namespace Federation.Core
{
    public static class UserContextService
    {
        private static readonly ConcurrentDictionary<string, UserContainer> Context = new ConcurrentDictionary<string, UserContainer>();
        private static void Remove(string key)
        {
            var user = GetUserContainer(key);

            var userFromDb = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == user.Id);
            if (userFromDb == null)
                throw new BusinessLogicException("Пользователь не найден!");

            if (user.LastAction.Date != userFromDb.LastActivity.Date)
                userFromDb.LastActivity = user.LastAction;

            Context.TryRemove(key, out user);
        }

        public static UserContainer GetUserContainer(string key)
        {
            UserContainer user;
            Context.TryGetValue(key, out user);
            
            if(user != null)
                user.LastAction = DateTime.Now;

            return user;
        }

        public static UserContainer CreateNewUserContainer(string key)
        {
            UserContainer result = null;
            Guid userId;

            if (Guid.TryParse(key, out userId))
            {
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
                if (user != null)
                    result = new UserContainer(user);
            }

            return result;
        }

        public static bool RegisterNewUserContainer(string key, UserContainer user)
        {
            if (user != null)
            {
                if (!Context.ContainsKey(key))
                    Context.TryAdd(key, user);
                return true;
            }

            return false;
        }

        public static void Abandon(string key)
        {
            if (Context.ContainsKey(key))
                Remove(key);
        }

        public static void Abandon(Guid userId)
        {
            var userContexts = Context.Where(x => x.Value.Id == userId).ToList();
            foreach (var context in userContexts)
                Abandon(context.Key);
        }

        public static void GroupMembersAbandon(Guid groupId)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId);
            if (group == null)
                return;

            foreach (var gm in group.GroupMembers.Where(x => x.State != (byte)GroupMemberState.Banned && x.State != (byte)GroupMemberState.NotMember))
                Abandon(gm.UserId);
        }

        public static void ClearUnused()
        {
            UserContainer user;

            var clearNull = Context.Where(u => u.Value == null).ToList();
            foreach (var item in clearNull)
                Context.TryRemove(item.Key, out user);

            var clearOutOfDate = Context.Where(u => u.Value.LastAction.Date < DateTime.Now.Date.AddMinutes(-15)).ToList();
            foreach (var item in clearOutOfDate)
                Context.TryRemove(item.Key, out user);     
        }
    }
}