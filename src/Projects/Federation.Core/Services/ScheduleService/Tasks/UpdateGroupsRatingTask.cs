using System;
using System.Linq;
using System.Collections.Generic;

namespace Federation.Core
{
    [Serializable]
    public class UpdateGroupsRatingTask : ScheduleTask
    {
        public UpdateGroupsRatingTask()
        {
            _name = "Обновление рейтинга у групп";
            _isUnque = true;
        }

        public override void Execute()
        {
            var lastmonthDate = DateTime.Now.AddMonths(-1);
            var allGroups = DataService.PerThread.GroupSet.OrderBy(x=>x.GroupRating.OverallRating).ToList();
            List<GrouRatingModel> groupsRating = new List<GrouRatingModel>();

            foreach (var group in allGroups)
            {
                if (group.State == (byte)GroupState.Archive || group.State == (byte)GroupState.Blank)
                    SetNullRating(group);
                else
                {
                    var membersScore = group.GroupMembers.Count(x => x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator);
                    var expertsScore = group.GroupMembers.Where(x => x.Expert != null && x.Expert.Tags.Count() > 0).Sum(x => 1 + 2 * x.Expert.ExpertVotes.Count());
                    var contentScore = group.Content.Where(x => x.CreationDate >= lastmonthDate && x.State == (byte)ContentState.Approved).Sum(x => x.Comments.Count() + 20 * x.Likes.Sum(l => l.Value ? 1 : -1));

                    var ratingModel = new GrouRatingModel
                    {
                        GroupId = group.Id,
                        MembersScore = membersScore,
                        ExpertsScore = expertsScore,
                        ContentScore = contentScore
                    };
                    groupsRating.Add(ratingModel);
                }
            }

            var orderedRating = groupsRating.OrderByDescending(x=>x.ExpertsScore+x.MembersScore+x.ContentScore).ToArray();
            for (int i = 0; i < groupsRating.Count(); i++)
            {
                var score = orderedRating[i];
                var group = allGroups.Single(x => x.Id == score.GroupId);
                group.GroupRating.OldOverallRating = group.GroupRating.OverallRating;
                group.GroupRating.OverallRating = i+1;
            }

            var membersRating = groupsRating.OrderByDescending(x => x.MembersScore).ToArray();
            for (int i = 0; i < groupsRating.Count(); i++)
            {
                var score = membersRating[i];
                var group = allGroups.Single(x => x.Id == score.GroupId);
                group.GroupRating.MembersRating = i + 1;
            }

            var contentRating = groupsRating.OrderByDescending(x => x.ContentScore).ToArray();
            for (int i = 0; i < groupsRating.Count(); i++)
            {
                var score = contentRating[i];
                var group = allGroups.Single(x => x.Id == score.GroupId);
                group.GroupRating.ContentRating = i + 1;
            }

            var expertsRating = groupsRating.OrderByDescending(x => x.ExpertsScore).ToArray();
            for (int i = 0; i < groupsRating.Count(); i++)
            {
                var score = expertsRating[i];
                var group = allGroups.Single(x => x.Id == score.GroupId);
                group.GroupRating.ExpertsRating = i + 1;
            }
            DataService.PerThread.SaveChanges();
        }

        private void SetNullRating(Group group)
        {
            group.GroupRating.OldOverallRating = group.GroupRating.OverallRating;
            group.GroupRating.OverallRating = 0;
            group.GroupRating.ExpertsRating = 0;
            group.GroupRating.MembersRating = 0;
            group.GroupRating.ContentRating = 0;
        }
    }

    public class GrouRatingModel
    {
        public Guid GroupId { get; set; }
        public int MembersScore { get; set; }
        public int ContentScore { get; set; }
        public int ExpertsScore { get; set; }
    }
}