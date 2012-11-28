namespace Federation.Core
{
    public class TicketVerifiedBadge : BaseBadge
    {
        public TicketVerifiedBadge()
        {
            Image = "ticket_verified.png";
            Title = "Верифицированный через ЦВК";
            Description = "Присваивается тем, кто участвовал в выборах в КСО.";
            Type = BadgeType.TicketVerified;
            IsAcquired = true;
        }
    }
}