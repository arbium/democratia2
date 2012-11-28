using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_PollVotePanelViewModel
    {
        public Guid PollId { get; set; }
        public Guid BulletinId { get; set; }
        
        public bool IsFinished { get; set; }
        public bool AlreadyVoted { get; set; }
        public string VoteResult { get; set; }

        public bool IsExpert { get; set; }
        public int Represents { get; set; }
        public int RelativeRepresents { get; set; }
        public int TotalRecomends { get; set; }
        public double RecomendsVoteYes { get; set; }
        public double RecomendsVoteNo { get; set; }
        public double RecomendsVoteForefit { get; set; }

        public bool HaveExpert { get; set; }
        public List<Group_PollVotePanel_ExpertViewModel> Experts { get; set; }

        public Group_PollVotePanelViewModel()
        {
            Experts = new List<Group_PollVotePanel_ExpertViewModel>();
        }

        public Group_PollVotePanelViewModel(Guid pollId, Guid userId)
        {

            var bulletin = (from pollRecord in DataService.PerThread.ContentSet.OfType<Poll>()
                            where pollRecord.Id == pollId
                            join bulletinRecord in DataService.PerThread.BulletinSet.OfType<PollBulletin>() on pollRecord.Id equals bulletinRecord.PollId
                            where bulletinRecord.Owner.UserId == userId
                            select bulletinRecord).SingleOrDefault();
            
            Experts = new List<Group_PollVotePanel_ExpertViewModel>();

            PollId = pollId;

            if (bulletin != null)
            {
                BulletinId = bulletin.Id;
                IsFinished = bulletin.Poll.IsFinished;
                AlreadyVoted = bulletin.Result != (byte)VoteOption.NotVoted;
                switch ((VoteOption)bulletin.Result)
                {
                    case VoteOption.Yes: VoteResult = "<span class='vote yes'>За</span>"; break;
                    case VoteOption.No: VoteResult = "<span class='vote no'>Против</span>"; break;
                    case VoteOption.Refrained: VoteResult = "<span class='vote forefit'>Воздержался</span>"; break;
                    case VoteOption.NotVoted: VoteResult = "<span class='vote not'>Не голосовал</span>"; break;
                }

                if (bulletin.Weight > 1)
                {
                    IsExpert = true;
                    Represents = bulletin.Weight - 1;
                    RelativeRepresents = 100 * Represents / bulletin.Poll.Bulletins.Count;
                    foreach (var granterBulletin in bulletin.GrantorBulletins)
                    {
                        TotalRecomends += 1;
                        switch ((VoteOption)granterBulletin.Result)
                        {
                            case VoteOption.Yes: RecomendsVoteYes += 1; break;
                            case VoteOption.No: RecomendsVoteNo += 1; break;
                            case VoteOption.Refrained: RecomendsVoteForefit += 1; break;
                        }
                    }
                    RecomendsVoteYes = Math.Round(RecomendsVoteYes * 100 / bulletin.GrantorBulletins.Count, 2);
                    RecomendsVoteNo = Math.Round(RecomendsVoteNo * 100 / bulletin.GrantorBulletins.Count, 2);
                    RecomendsVoteForefit = Math.Round(RecomendsVoteForefit * 100 / bulletin.GrantorBulletins.Count, 2);
                }

                foreach (var expertbulletin in bulletin.ExpertBulletins)
                {
                    Experts.Add(new Group_PollVotePanel_ExpertViewModel(expertbulletin));
                }
                HaveExpert = Experts.Count > 0;
            }
        }
    }

    public class Group_PollVotePanel_ExpertViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }
        public string Avatar { get; set; }
        public string VoteResult { get; set; }

        public Group_PollVotePanel_ExpertViewModel()
        {
        }

        public Group_PollVotePanel_ExpertViewModel(PollBulletin expertBulletin)
        {
            if (expertBulletin != null)
            {
                var expert = expertBulletin.Owner.User;
                Id = expert.Id;
                Name = expert.FirstName;
                SurName = expert.SurName;
                Patronymic = expert.Patronymic;
                Avatar = ImageService.GetImageUrl<User>(expert.Avatar);

                switch ((VoteOption)expertBulletin.Result)
                {
                    case VoteOption.Yes: VoteResult = "<span class='vote yes'>За</span>"; break;
                    case VoteOption.No: VoteResult = "<span class='vote no'>Против</span>"; break;
                    case VoteOption.Refrained: VoteResult = "<span class='vote forefit'>Воздержался</span>"; break;
                    case VoteOption.NotVoted: VoteResult = "<span class='vote not'>Не голосовал</span>"; break;
                }
            }
        }
    }
}