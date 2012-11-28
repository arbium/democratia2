using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _BadgesViewModel
    {
        public IList<BadgeViewModel> Badges { get; set; }

        public _BadgesViewModel()
        {
            Badges = new List<BadgeViewModel>();
        }

        public _BadgesViewModel(Group group)
        {
            if (group != null)
                Badges = group.Badges.Where(x => x.IsAcquired).Select(x => new BadgeViewModel(x)).ToList();
            else
                Badges = new List<BadgeViewModel>();
        }

        public _BadgesViewModel(User user)
        {
            if (user != null)
                Badges = user.Badges.Where(x => x.IsAcquired).Select(x => new BadgeViewModel(x)).ToList();
            else
                Badges = new List<BadgeViewModel>();
        }
    }
}