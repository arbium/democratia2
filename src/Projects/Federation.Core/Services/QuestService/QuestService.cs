using System.Linq;

namespace Federation.Core
{
    public static class QuestService
    {
        public static void Recalculate(User user)
        {
            var questProgress = (QuestProgress)user.QuestProgress;

            if (!questProgress.HasFlag(QuestProgress.Completed))
            {                
                if (!questProgress.HasFlag(QuestProgress.Avatar))
                    if (user.Avatar != null)
                        questProgress |= QuestProgress.Avatar;
                if (!questProgress.HasFlag(QuestProgress.Commenting))
                    if (user.Comments.Count != 0)
                        questProgress |= QuestProgress.Commenting;
                if (!questProgress.HasFlag(QuestProgress.Delegating))
                    if (user.GroupMembers.Count(x => x.ExpertVotes.Count != 0) != 0 || user.GroupMembers.Count(x => x.Expert != null) != 0)
                        questProgress |= QuestProgress.Delegating;
                if (!questProgress.HasFlag(QuestProgress.GroupsJoin))
                    if (user.GroupMembers.Count > 3)
                        questProgress |= QuestProgress.GroupsJoin;
                if (!questProgress.HasFlag(QuestProgress.Info))
                    if (!string.IsNullOrWhiteSpace(user.Info))
                        questProgress |= QuestProgress.Info;
                if (!questProgress.HasFlag(QuestProgress.Voting))
                {
                    if (user.SurveyBulletins.Count(x => x.Result != null) != 0 || user.SignedPetitions.Count != 0)
                        questProgress |= QuestProgress.Voting;
                    else if (user.GroupMembers.Count(gm => gm.Bulletins.OfType<ElectionBulletin>().Count(b => b.Result.Count != 0) != 0) != 0
                        || user.GroupMembers.Count(gm => gm.Bulletins.OfType<PollBulletin>().Count(b => b.Result != (byte)VoteOption.NotVoted) != 0) != 0)
                        questProgress |= QuestProgress.Voting;
                }

                if (questProgress.Percents() == 100)
                {
                    questProgress |= QuestProgress.Completed;
                    BadgeService.AwardUser<QuestBadge>(user.Id);
                }

                user.QuestProgress = (short)questProgress;
                DataService.PerThread.SaveChanges();
            }
        }
    }
}