using System;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserVotingsViewModel
    {
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public _ContentFeedViewModel VotingsFeed { get; set; }
        public _PollStatusViewModel PreviosPolls { get; set; }

        public UserVotingsViewModel()
        {
            VotingsFeed = new _ContentFeedViewModel();
            PreviosPolls = new _PollStatusViewModel();
        }

        public UserVotingsViewModel(User user)
        {
            VotingsFeed = new _ContentFeedViewModel();
            PreviosPolls = new _PollStatusViewModel();
            if (user != null)
            {
                UserName = user.FullName;
                UserId = user.Id;
                var newVotings = (from _group in user.GroupMembers.Select(x => x.Group)
                                    join content in DataService.PerThread.ContentSet.OfType<Poll>() on _group.Id equals content.GroupId
                                    where content.State == (byte)ContentState.Approved && !content.IsFinished
                                    select content).OfType<Content>();

                var finishedVotings = (from _group in user.GroupMembers.Select(x => x.Group)
                                    join content in DataService.PerThread.ContentSet.OfType<Poll>() on _group.Id equals content.GroupId
                                    where content.State == (byte)ContentState.Approved && content.IsFinished
                                    select content).Take(10).ToList();

                VotingsFeed = new _ContentFeedViewModel(newVotings, UserId);
                PreviosPolls = new _PollStatusViewModel(finishedVotings, user.Id);
            }
        }
    }
}