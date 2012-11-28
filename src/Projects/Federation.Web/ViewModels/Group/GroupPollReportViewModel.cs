using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupPollReportViewModel
    {
        public Guid GroupId { get; set; }
        public IList<GroupPollReport_BulletinViewModel> Bulletins { get; set; }

        public GroupPollReportViewModel(Poll poll)
        {
            if (poll == null)
                return;

            if (!poll.IsFinished)
                throw new BusinessLogicException("Голосование еще не завершено!");

            if (poll.GroupId.HasValue)
                GroupId = poll.GroupId.Value;

            Bulletins = poll.Bulletins.Select(x => new GroupPollReport_BulletinViewModel(x)).OrderByDescending(x => x.Weight).ThenByDescending(x => x.Result).ThenBy(x => x.Id).ToList();
        }
    }

    public class GroupPollReport_BulletinViewModel
    {
        public Guid Id { get; set; }
        public int Weight { get; set; }
        public VoteOption Result { get; set; }

        public GroupPollReport_BulletinViewModel(PollBulletin bulletin)
        {
            if (bulletin == null)
                return;

            Id = bulletin.Id;
            Weight = bulletin.Weight;
            Result = (VoteOption)bulletin.Result;

        }
    }
}