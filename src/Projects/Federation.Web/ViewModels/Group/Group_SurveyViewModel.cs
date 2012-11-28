using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_SurveyViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsFinished { get; set; }
        public ContentState State { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TimeRemaining { get; set; }
        public bool IsUserVoted { get; set; }
        public string VotedUsers { get; set; }
        public string AbstainedUsers { get; set; }
        public int VotesCount { get; set; }
        public byte VariantsCount { get; set; }
        public string VariantsCountString { get; set; }
        public IList<GroupSurveyOptionViewModel> Options { get; set; }
        public Guid? RadioOptionId { get; set; }
        public bool HasOpenProtocol { get; set; }

        public Guid UserBulletinId { get; set; }
        public Guid? GroupId { get; set; }

        public Group_SurveyViewModel()
        {
        }

        public Group_SurveyViewModel(Survey survey, Guid? userId)
        {
            if (survey != null)
            {
                Id = survey.Id;
                Title = survey.Title;
                Text = survey.Text;
                IsPrivate = survey.IsPrivate;
                IsFinished = survey.IsFinished;
                State = (ContentState)survey.State;
                HasOpenProtocol = survey.HasOpenProtocol;

                GroupId = survey.GroupId;

                Options = survey.Options.Select(x => new GroupSurveyOptionViewModel(x)).OrderBy(x => x.Position).ToList();
                if (survey.PublishDate.HasValue)
                {
                    StartDate = survey.PublishDate.Value;
                    if (survey.Duration.HasValue)
                    {
                        EndDate = survey.PublishDate.Value.AddDays(survey.Duration.Value);
                        if (!survey.IsFinished)
                        {
                            var ts = EndDate.Value - DateTime.Now;
                            TimeRemaining = string.Format("{0} дней {1:00}:{2:00}:{3:00}", (int)ts.TotalDays, ts.Hours, ts.Minutes, ts.Seconds);
                        }
                    }
                }

                var bulletins = survey.Bulletins;
                var voted = bulletins.Count(x => x.Result.Count != 0);                
                var votedString = DeclinationService.OfNumber(voted, "человек", "человека", "человек");
                if (votedString[votedString.Length - 1] == 'а')
                    VotedUsers = "Проголосовали ";
                else
                    VotedUsers = "Проголосовало ";
                VotedUsers += votedString;
                var abstained = bulletins.Count - voted;
                var abstainedString = DeclinationService.OfNumber(abstained, "человек", "человека", "человек");
                if (abstainedString[abstainedString.Length - 1] == 'а')
                    AbstainedUsers = "Воздержались ";
                else
                    AbstainedUsers = "Воздержалось ";
                AbstainedUsers += abstainedString;

                VotesCount = 0;
                foreach (var bulletin in bulletins)
                    VotesCount += bulletin.Result.Count;
                VariantsCount = survey.VariantsCount;
                VariantsCountString = "Вы можете выбрать " + DeclinationService.OfNumber(VariantsCount, "вариант", "варианта", "вариантов");

                if (userId.HasValue)
                {
                    var blt = survey.Bulletins.SingleOrDefault(x => x.UserId == userId);
                    if (blt != null)
                    {
                        IsUserVoted = true;
                        UserBulletinId = blt.Id;
                        foreach (var o in blt.Result)
                            Options.Single(x => x.Id == o.Id).IsChecked = true;
                    }
                }
            }
        }
    }
}