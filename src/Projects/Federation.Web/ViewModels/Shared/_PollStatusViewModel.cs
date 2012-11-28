using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _PollStatusViewModel
    {
        public IList<_PollStatus_VotingViewModel> Votings { get; set; }

        public _PollStatusViewModel()
        {
            Votings = new List<_PollStatus_VotingViewModel>();
        }

        public _PollStatusViewModel(ICollection<Poll> polls, Guid? userId)
        {
            Votings = new List<_PollStatus_VotingViewModel>();
            if (polls != null)
            {
                foreach (var record in polls)
                    Votings.Add(new _PollStatus_VotingViewModel(record, userId));
            }
        }
    }

    public class _PollStatus_VotingViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }

        public string CommentsCount { get; set; }

        public Guid Id { get; set; }
        public string Title { get; set; }

        public bool IsFinished { get; set; }
        public string VoteResult { get; set; }
        public VoteOption VoteResultType { get; set; }

        public int Participants { get; set; }
        public int VotedYes { get; set; }
        public int VotedNo { get; set; }
        public int VotedForefit { get; set; }

        public int Voted { get; set; }
        public int NotVoted { get; set; }

        public DateTime FinishDate { get; set; }
        public DateTime StartDate { get; set; }
        public int Duration { get; set; }
        public string TimeLeft { get; set; }

        public bool IsUserParticipant { get; set; }
        public string UserVoteResult { get; set; }

        public _PollStatus_VotingViewModel()
        {
        }

        public _PollStatus_VotingViewModel(Poll record, Guid? userId)
        {
            if (record != null)
            {
                if (record.GroupId.HasValue)
                    GroupId = record.GroupId.Value;
                GroupName = record.Group.Name;
                GroupLogo = ImageService.GetImageUrl<Group>(record.Group.Logo); 
                GroupUrl = record.Group.Url;

                CommentsCount = DeclinationService.OfNumber(record.Comments.Count, "комментарий", "комментария", "комментариев");

                Id = record.Id;
                Title = record.Title;

                IsFinished = record.IsFinished;
                VoteResultType = (VoteOption)record.Result;

                switch (VoteResultType)
                {
                    case VoteOption.Yes: VoteResult = "<span class='vote yes'>Решение принято положительно</span>"; break;
                    case VoteOption.No: VoteResult = "<span class='vote no'>Рещение принято отрицательно</span>"; break;
                    case VoteOption.Refrained: VoteResult = "<span class='vote forefit'>Решение не приянто</span>"; break;
                    case VoteOption.NotVoted: VoteResult = "<span class='vote not'>Голосование не состоялось</span>"; break;
                }

                Participants = record.Bulletins.Count;
                foreach (var vote in record.Bulletins)
                {
                    if (vote.Result == (byte)VoteOption.Yes)
                    {
                        VotedYes += 1;
                        Voted += 1;
                    }
                    else if (vote.Result == (byte)VoteOption.No)
                    {
                        VotedNo += 1;
                        Voted += 1; 
                    }
                    else if (vote.Result == (byte)VoteOption.Refrained)
                    {
                        VotedForefit += 1;
                        Voted += 1;
                    }
                    else if (vote.Result == (byte)VoteOption.NotVoted)
                    {
                        NotVoted += 1;
                    }
                }

                if (record.PublishDate.HasValue)
                {
                    StartDate = record.PublishDate.Value;
                    if (record.Duration.HasValue)
                        FinishDate = record.PublishDate.Value.AddDays(record.Duration.Value);
                }
                if (record.Duration.HasValue)
                    Duration = record.Duration.Value;

                var left = FinishDate - DateTime.Now;
                if (left.TotalMilliseconds >= 0)
                    TimeLeft = string.Format("{0} {1:00}:{2:00}", DeclinationService.OfNumber((int) left.TotalDays, "день", "дня", "дней"), left.Hours, left.Minutes);

                IsUserParticipant = false;

                if (userId.HasValue && userId != Guid.Empty && record.GroupId.HasValue)
                {
                    var member = GroupService.UserInGroup(userId.Value, record.GroupId.Value);
                    if (member != null)
                    {
                        IsUserParticipant = true;
                        var bulletin = record.Bulletins.SingleOrDefault(x => x.OwnerId == member.Id);
                        if (bulletin != null)
                        {
                            switch ((VoteOption)bulletin.Result)
                            {
                                case VoteOption.Yes: UserVoteResult = "Вы проголосовали <span class='vote yes'>За</span>"; break;
                                case VoteOption.No: UserVoteResult = "Вы проголосовали <span class='vote no'>Против</span>"; break;
                                case VoteOption.Refrained: UserVoteResult = "Вы <span class='vote forefit'>Воздержались</span>"; break;
                                case VoteOption.NotVoted: UserVoteResult = "Вы <span class='vote not'>не голосовали</span> по этому вопросу"; break;
                            }
                        }
                        else
                            UserVoteResult = "";
                    }
                }
            }            
        }
    }
}