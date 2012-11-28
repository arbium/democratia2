using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_ElectionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public ElectionStage Stage { get; set; }        
        public int Quorum { get; set; }
        public int Turnout { get; set; }
        public int GroupModersCount { get; set; }
        public bool IsCandidate { get; set; }
        public Guid? CandidateId { get; set; }

        public IList<Group_Election_CandidateViewModel> Candidates { get; set; }
        public IList<Group_Election_CandidateViewModel> Winners { get; set; }

        public Group_Election_AboutViewModel About { get; set; }
        public Group_ElectionCandidatesViewModel ElectionCandidates { get; set; }
        public Group_ElectionVotingViewModel ElectionVoting { get; set; }

        public Group_ElectionViewModel()
        {
            Candidates = new List<Group_Election_CandidateViewModel>();
            Winners = new List<Group_Election_CandidateViewModel>();
            About = new Group_Election_AboutViewModel();
            ElectionCandidates = new Group_ElectionCandidatesViewModel();
            ElectionVoting = new Group_ElectionVotingViewModel();
        }

        public Group_ElectionViewModel(Election election, Guid? userId)
        {            
            if (election != null)
            {
                Id = election.Id;
                Title = election.Title;
                Stage = (ElectionStage)election.Stage;
                Quorum = election.Quorum;
                Turnout = election.Turnout;
                GroupModersCount = election.Group.ModeratorsCount;
                About = new Group_Election_AboutViewModel(election);

                GroupMember gm = null;
                if (userId.HasValue && election.GroupId.HasValue)
                    gm = GroupService.UserInGroup(userId.Value, election.GroupId.Value);

                IEnumerable<Candidate> candidates = election.Candidates.Where(x => x.Petition != null).OrderByDescending(x => x.Petition.Signers.Count);
                candidates = candidates.Union(election.Candidates.Where(x => x.Petition == null));
                if (election.Stage != (byte)ElectionStage.Agitation)
                    candidates = candidates.Where(x => x.Status == (byte)CandidateStatus.Confirmed);
                Candidates = candidates.Select(x => new Group_Election_CandidateViewModel(x, gm)).ToList();
                Winners = election.Candidates
                    .Where(x => x.Status == (byte)CandidateStatus.Winner)
                    .OrderByDescending(x => x.Electorate.Count)
                    .Select(x => new Group_Election_CandidateViewModel(x, gm))
                    .ToList();                

                switch (Stage)
                {
                    case ElectionStage.Agitation: ElectionCandidates = new Group_ElectionCandidatesViewModel(election, userId); break;
                    case ElectionStage.Voting: ElectionVoting = new Group_ElectionVotingViewModel(election, userId); break;
                }
            }
            else
            {
                Candidates = new List<Group_Election_CandidateViewModel>();
                Winners = new List<Group_Election_CandidateViewModel>();
                About = new Group_Election_AboutViewModel();
                ElectionCandidates = new Group_ElectionCandidatesViewModel();
                ElectionVoting = new Group_ElectionVotingViewModel();
            }

            if (UserContext.Current != null)
            {
                var candidate = Candidates.FirstOrDefault(x => x.UserId == UserContext.Current.Id);
                if (candidate != null)
                {
                    IsCandidate = true;
                    CandidateId = candidate.Id;
                }
                else
                    CandidateId = null;
            }
        }
    }

    public class Group_Election_AboutViewModel
    {
        public string StageClass { get; set; }
        public string GroupModersCountString { get; set; }
        public string ElectionPeriodString { get; set; }

        public Group_Election_AboutViewModel()
        {
        }

        public Group_Election_AboutViewModel(Election election)
        {
            if (election != null)
            {
                switch ((ElectionStage)election.Stage)
                {
                    case ElectionStage.Agitation:
                        StageClass = "agitation-stage";
                        break;
                    case ElectionStage.Voting:
                        StageClass = "voting-stage";
                        break;
                    case ElectionStage.Completed:
                        StageClass = "completed-stage";
                        break;
                    case ElectionStage.Failed:
                        StageClass = "failed-stage";
                        break;
                }

                GroupModersCountString = DeclinationService.OfNumber(election.Group.ModeratorsCount, "модератора", "модераторов", "модераторов");
                ElectionPeriodString = DeclinationService.OfNumber(election.Group.ElectionFrequency, "месяц", "месяца", "месяцов");
            }
        }
    }

    public class Group_Election_CandidateViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public bool IsChecked { get; set; }
        public string FullName { get; set; }
        public string Votes { get; set; }
        public Guid? PetitionId { get; set; }

        public Group_Election_CandidateViewModel()
        {
        }

        public Group_Election_CandidateViewModel(Candidate candidate, GroupMember gm)
        {
            if (candidate != null)
            {
                Id = candidate.Id;
                UserId = candidate.GroupMember.UserId;
                Avatar = ImageService.GetImageUrl<User>(candidate.GroupMember.User.Avatar);
                FullName = candidate.GroupMember.User.FullName;
                Votes = DeclinationService.OfNumber(candidate.Electorate.Count, "голос", "голоса", "голосов");
                PetitionId = candidate.Petition == null ? (Guid?)null : candidate.Petition.Id;
                
                if (gm != null)
                    IsChecked = candidate.Electorate.Count(x => x.OwnerId == gm.Id) > 0;
            }
        }
    }
}