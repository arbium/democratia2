using System;

namespace Federation.Core
{
    public partial class Poll : Voting
    {
        public Poll()
        {
            Result = 0;
        }

        private delegate void SummarizePollCallback(Guid pollId);

        //public override string Title
        //{
        //    get
        //    {
        //        if (PublishDate.HasValue && !IsFinished)
        //        {
        //            if (State == (byte)ContentState.Approved && PublishDate.Value.AddDays(Duration) <= DateTime.Now)
        //            {
        //                SummarizePollCallback pollCallback = VotingService.SummarizePoll;
        //                pollCallback.BeginInvoke(Id);
        //            }
        //        }
        //        return base.Title;
        //    }
        //    set
        //    {
        //        base.Title = value;
        //    }
        //}
    }
}