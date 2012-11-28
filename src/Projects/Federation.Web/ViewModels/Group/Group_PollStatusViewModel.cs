using System;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_PollStatusViewModel
    {
        public Guid PollId { get; set; }
        public int ParticipantsCount { get; set; }
        public bool IsFinished { get; set; }
        public string VoteResult { get; set; }

        public string ReportsUrl { get; set; }

        public int PositiveVotes { get; set; }
        public int NegativeVotes { get; set; }
        public int ForefitVotes { get; set; }        
        public int TookPart { get; set; }
        public int NotVoted { get; set; }

        public int RelativePositiveVotes { get; set; }
        public int RelativeNegativeVotes { get; set; }
        public int RelativeForefitVotes { get; set; }
        public int RelativeNotVoted { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int SecondsLeft { get; set; }
        public string TimeRemaing { get; set; }

        public string PollBulletinId { get; set; }

        public bool HasOpenProtocol { get; set; }

        public Group_PollStatusViewModel()
        {            
        }

        public Group_PollStatusViewModel(Poll poll)
        {

            if(UserContext.Current != null)
            {
                var userId = UserContext.Current.Id;

                var gm =
                    DataService.PerThread.GroupMemberSet.FirstOrDefault(
                        x => x.UserId == userId && x.GroupId == poll.GroupId);

                if (gm != null)
                {
                    var bulletin = DataService.PerThread.BulletinSet.OfType<PollBulletin>().FirstOrDefault(
                        x => x.OwnerId == gm.Id && x.PollId == poll.Id);

                    if (bulletin != null)
                        PollBulletinId = bulletin.Id.ToString();
                }
            }

            ReportsUrl = "\\MediaContent\\Reports\\" + poll.Id + ".csv";

            if (poll != null)
            {
                PollId = poll.Id;
                ParticipantsCount = poll.Bulletins.Count;
                HasOpenProtocol = poll.HasOpenProtocol;

                if (poll.PublishDate.HasValue)
                {
                    StartDate = poll.PublishDate.Value;
                    FinishDate = poll.PublishDate.Value.AddDays(poll.Duration.Value);
                    IsFinished = poll.IsFinished;
                    if (IsFinished)
                    {
                        switch ((VoteOption)poll.Result)
                        {
                            case VoteOption.Yes: VoteResult = "<span class='vote yes'>Решение принято положительно</span>"; break;
                            case VoteOption.No: VoteResult = "<span class='vote no'>Решение принято отрицательно</span>"; break;
                            case VoteOption.Refrained: VoteResult = "<span class='vote forefit'>Решение не принято</span>"; break;
                            case VoteOption.NotVoted: VoteResult = "<span class='vote not'>В связи с низкой явкой, <span>голосование не состоялось</span></span>"; break;
                        }
                    }
                    else
                    {
                        var ts = FinishDate - DateTime.Now;
                        TimeRemaing = string.Format("{0} дней {1:00}:{2:00}:{3:00}", (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds);
                        SecondsLeft = (int)ts.TotalSeconds;
                    }
                }

                if (ParticipantsCount > 0)
                {
                    foreach (var result in poll.Bulletins)
                    {
                        if (result.Result != (byte)VoteOption.NotVoted)
                        {
                            if (result.Result == (byte)VoteOption.Yes)
                                PositiveVotes += result.Weight;
                            if (result.Result == (byte)VoteOption.No)
                                NegativeVotes += result.Weight;
                            if (result.Result == (byte)VoteOption.Refrained)
                                ForefitVotes += result.Weight;

                            TookPart += result.Weight;
                        }
                    }
                    NotVoted = ParticipantsCount - TookPart;

                    double temp = 0;
                    temp = PositiveVotes;
                    RelativePositiveVotes = (int)Math.Round(100 * temp / ParticipantsCount,0, MidpointRounding.ToEven);
                    temp = NegativeVotes;
                    RelativeNegativeVotes = (int)Math.Round(100 * temp / ParticipantsCount, 0, MidpointRounding.ToEven);
                    temp = ForefitVotes;
                    RelativeForefitVotes = (int)Math.Round(100 * temp / ParticipantsCount, 0, MidpointRounding.ToEven);
                    temp = NotVoted;
                    RelativeNotVoted = (int)Math.Round(100 * temp / ParticipantsCount, 0, MidpointRounding.ToEven);

                    poll.Bulletins.OrderByDescending(x => x.Weight).Take(5);
                }
            }
        }
    }
}