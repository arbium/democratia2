using System;
using System.Linq;

namespace Federation.Core
{
    public partial class Election : Voting
    {
        public override string Text // TODO: сделать шедулер
        {
            get
            {
                //if (!IsFinished)
                //{
                //    if (Stage == (byte)ElectionStage.Voting)
                //    {
                //        if (DateTime.Now >= EndDate)
                //            VotingService.FinishElection(Id);
                //    }
                //    else if (Stage == (byte)ElectionStage.Agitation)
                //    {
                //        if (DateTime.Now >= VotingDate.AddMinutes(-1))
                //            VotingService.StartElection(Id);
                //    }
                //}

                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// Начало голосования
        /// </summary>
        public DateTime VotingDate
        {
            get { return PublishDate.Value.AddDays(AgitationDuration); }
        }

        /// <summary>
        /// Окончание голосования
        /// </summary>
        public DateTime? EndDate
        {
            get
            {
                if (Duration.HasValue)
                    return VotingDate.AddDays(Duration.Value);

                return null;
            }
        }

        /// <summary>
        /// Явка
        /// </summary>
        public int Turnout
        {
            get { return ElectionBulletins.Count(x => x.Result.Count != 0); }
        }
    }
}