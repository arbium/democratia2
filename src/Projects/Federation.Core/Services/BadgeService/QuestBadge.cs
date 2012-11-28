namespace Federation.Core
{
    public class QuestBadge : BaseBadge
    {
        public QuestBadge()
        {
            Image = "graduate.png";
            Title = "Освоившийся";
            Description = "Присваивается всем, кто прошел ознакомительный квест.";
            Type = BadgeType.Quest;
            IsAcquired = true;
        }
    }
}