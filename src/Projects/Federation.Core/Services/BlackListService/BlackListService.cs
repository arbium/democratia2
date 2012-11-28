namespace Federation.Core
{
    public static class BlackListService
    {
        public static void AddUser(User owner, User user)
        {
            if (owner.BlackList.Contains(user))
                throw new BusinessLogicException("Указанный пользователь уже есть в вашем черном списке");

            owner.BlackList.Add(user);

            if (owner.SubscriptionUsers.Contains(user))
                owner.SubscriptionUsers.Remove(user);

            DataService.PerThread.SaveChanges();
        }

        public static void RemoveUser(User owner, User user)
        {
            if (!owner.BlackList.Contains(user))
                throw new BusinessLogicException("Указанный пользователь не состоял в вашем черном списке");

            owner.BlackList.Remove(user);

            DataService.PerThread.SaveChanges();
        }
    }
}