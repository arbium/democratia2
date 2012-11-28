namespace Federation.Core
{
    public class BugCollectorBadge : BaseBadge
    {
        public BugCollectorBadge()
        {
            Image = "debug.png";
            Title = "Ловец багов";
            Description = "Выдается за существенную помощь в обнаружении и исправлении багов и ошибок на Демократии2.";
            Type = BadgeType.BugCollector;
            IsAcquired = true;
        }
    }
}
