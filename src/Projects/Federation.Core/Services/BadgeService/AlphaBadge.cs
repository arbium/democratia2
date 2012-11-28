namespace Federation.Core
{
    public class AlphaBadge : BaseBadge
    {
        public AlphaBadge()
        {
            Image = "alpha.png";
            Title = "Первый поселенец";
            Description = "Присвоен всем, кто зарегистрировался в Демократии2 ещё на стадии альфа-версии. И прошел активацию учетной записи.";
            Type = BadgeType.Alpha;
            IsAcquired = true;
        }
    }
}