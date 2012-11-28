using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_ElectionCandidatesViewModel
    {
        public Guid Id { get; set; }
        public DateTime AgitationStart { get; set; }
        public DateTime AgitationEnds { get; set; }
        public string AgitationTimeRemaining { get; set; }
        public int RelativeTimeRemaining { get; set; }
        public Guid GroupId { get; set; }

        public string ElectionFrequency { get; set; }
        public string ModeratorsCount { get; set; }
        public string SignsMinimumLimit { get; set; }

        public Guid ElectionId { get; set; }
        public DateTime ElectionEnd { get; set; }

        public bool IsCandidate { get; set; }

        public List<Group_ElectionCandidates_CandidateViewModel> Candidates = new List<Group_ElectionCandidates_CandidateViewModel>();

        public Group_ElectionCandidatesViewModel()
        {
        }

        public Group_ElectionCandidatesViewModel(Election election, Guid? userId)
        {
            if (election != null)
            {

                Id = election.Id;

                if (election.Group!=null)
                GroupId = election.Group.Id;

                if(!election.PublishDate.HasValue)
                    throw new BusinessLogicException("Данные выборы еще не опубликованы");

                if (userId.HasValue && election.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(userId.Value, election.GroupId.Value);
                    if (gm.Candidate != null && gm.Candidate.ElectionId == election.Id)
                        IsCandidate = true;
                }

                AgitationStart = election.PublishDate.Value;
                AgitationEnds = AgitationStart.AddDays(election.AgitationDuration);
                var ts = AgitationEnds - DateTime.Now;
                AgitationTimeRemaining = string.Format("{0} дней {1:00}:{2:00}:{3:00}", (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds);
                RelativeTimeRemaining = (election.AgitationDuration - (int)ts.TotalDays) * 100 / election.AgitationDuration;
                ElectionEnd = election.EndDate.Value;
                ElectionId = election.Id;
                ElectionFrequency = DeclinationService.OfNumber(election.Group.ElectionFrequency,"месяц","месяца","месяцев");
                ModeratorsCount = DeclinationService.OfNumber(election.Group.ModeratorsCount, "человек", "человека", "человек");
                SignsMinimumLimit = DeclinationService.OfNumber(ConstHelper.CandidatePetitionNecessarySigners, "подпись", "подписи", "подписей");

                var membersCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == election.GroupId && (x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator));
                foreach (var candidate in election.Candidates)
                    Candidates.Add(new Group_ElectionCandidates_CandidateViewModel(candidate, membersCount));
            }
        }
    }

    public class Group_ElectionCandidates_CandidateViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }

        public Guid? PetitionId { get; set; }
        public string PetitonSigns { get; set; }
        public float PetitonRelativeSigns { get; set; }
        public string PetitonSummary { get; set; }

        public Group_ElectionCandidates_CandidateViewModel()
        {
        }

        public Group_ElectionCandidates_CandidateViewModel(Candidate candidate, int totalMembersCount)
        {
            if (candidate != null)
            {
                Id = candidate.Id;
                UserId = candidate.GroupMember.UserId;
                Avatar = ImageService.GetImageUrl<User>(candidate.GroupMember.User.Avatar);
                FullName = candidate.GroupMember.User.FullName;

                PetitionId = candidate.Petition == null ? (Guid?)null : candidate.Petition.Id;
                if (candidate.Petition != null)
                {
                    var signs = candidate.Petition.Signers.Count;
                    PetitonSigns = DeclinationService.OfNumber(signs, "подпись", "подписи", "подписей");
                    PetitonRelativeSigns = ((float)signs) / totalMembersCount;
                    PetitonSummary = TextHelper.CleanTags(candidate.Petition.Text);
                    if (PetitonSummary.Length > 140)
                        PetitonSummary = PetitonSummary.Substring(0, 140)+"...";
                }
            }
        }
    }
}