using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_ElectionVotingViewModel
    {
        public Guid Id { get; set; }

        public DateTime AgitationStart { get; set; }
        public DateTime AgitationEnds { get; set; }
        public string TimeRemaining { get; set; }
        public int RelativeTimeRemaining { get; set; }

        public string ElectionFrequency { get; set; }
        public string ModeratorsCount { get; set; }
        public string SignsMinimumLimit { get; set; }

        public DateTime ElectionsEnds { get; set; }

        public bool CurrentUserVoted { get; set; }
        public bool CurrentUserTakingPart { get; set; }

        public List<Group_ElectionVoting_CandidateViewModel> Candidates = new List<Group_ElectionVoting_CandidateViewModel>();

        public Group_ElectionVotingViewModel()
        {
            CurrentUserTakingPart = false;
            CurrentUserVoted = false;
        }

        public Group_ElectionVotingViewModel(Election election, Guid? userId)
        {
            CurrentUserTakingPart = false;
            CurrentUserVoted = false;

            if (election != null)
            {
                if(!election.PublishDate.HasValue)
                    throw new BusinessLogicException("Данные выборы еще не опубликованы");

                Id = election.Id;

                AgitationStart = election.PublishDate.Value;
                AgitationEnds = AgitationStart.AddDays(election.AgitationDuration);
                if (election.EndDate.HasValue)
                    ElectionsEnds = election.EndDate.Value;
                var ts = ElectionsEnds - DateTime.Now;
                TimeRemaining = string.Format("{0} дней {1:00}:{2:00}:{3:00}", (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds);
                RelativeTimeRemaining = (election.AgitationDuration - (int)ts.TotalDays) * 100 / election.AgitationDuration;

                ElectionFrequency = DeclinationService.OfNumber(election.Group.ElectionFrequency,"месяц","месяца","месяцев");
                ModeratorsCount = DeclinationService.OfNumber(election.Group.ModeratorsCount, "человек", "человека", "человек");
                SignsMinimumLimit = DeclinationService.OfNumber(ConstHelper.CandidatePetitionNecessarySigners, "подпись", "подписи", "подписей");

                var membersCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == election.GroupId && (x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator));

                var bulletin = DataService.PerThread.BulletinSet.OfType<ElectionBulletin>().SingleOrDefault(x => x.ElectionId == election.Id && x.Owner.UserId == userId);
                if (bulletin != null)
                {
                    CurrentUserTakingPart = true;
                    CurrentUserVoted = bulletin.Result.Count > 0;
                }

                foreach (var candidate in election.Candidates)
                    if (candidate.Status == (short) CandidateStatus.Confirmed)
                    {
                        var voted = bulletin != null &&
                                    bulletin.Result.SingleOrDefault(x => x.Id == candidate.Id) != null;

                        Candidates.Add(new Group_ElectionVoting_CandidateViewModel(candidate, membersCount, voted));
                    }
            }
        }
    }

    public class Group_ElectionVoting_CandidateViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }

        public Guid? PetitionId { get; set; }
        public string PetitonSigns { get; set; }
        public float PetitonRelativeSigns { get; set; }
        public string PetitonSummary { get; set; }

        public bool IsChecked { get; set; }

        public Group_ElectionVoting_CandidateViewModel()
        {
        }

        public Group_ElectionVoting_CandidateViewModel(Candidate candidate, int totalMembersCount, bool voted)
        {
            if (candidate != null)
            {
                Id = candidate.Id;
                UserId = candidate.GroupMember.UserId;
                Avatar = ImageService.GetImageUrl<User>(candidate.GroupMember.User.Avatar);
                FullName = candidate.GroupMember.User.FullName;
                IsChecked = voted;

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