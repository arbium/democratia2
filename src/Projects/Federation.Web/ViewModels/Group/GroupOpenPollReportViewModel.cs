using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupOpenPollReportViewModel
    {
        public Guid GroupId { get; set; }
        public IList<GroupOpenPollReport_BulletinViewModel> Bulletins { get; set; }

        public GroupOpenPollReportViewModel(Poll poll)
        {
            if (poll == null)
                return;

            if (poll.GroupId.HasValue)
                GroupId = poll.GroupId.Value;

            Bulletins = poll.Bulletins.Select(x => new GroupOpenPollReport_BulletinViewModel(x)).OrderByDescending(x => x.Result).ThenBy(x => x.Name).ToList();
        }
    }

    public class GroupOpenPollReport_BulletinViewModel
    {
        public string Name { get; set; }
        public VoteOption Result { get; set; }

        public GroupOpenPollReport_BulletinViewModel(PollBulletin bulletin)
        {
            if (bulletin == null)
                return;

            Name = bulletin.Owner.User.FullName;
            Result = (VoteOption)bulletin.Result;
        }
    }
}