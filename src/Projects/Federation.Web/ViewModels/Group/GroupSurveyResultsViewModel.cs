using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupSurveyResultsViewModel
    {
        public Guid GroupId { get; set; }
        public IList<GroupSurveyResults_BulletinViewModel> Bulletins { get; set; }

        public GroupSurveyResultsViewModel(Survey survey)
        {
            if (survey == null)
                return;

            if (survey.GroupId.HasValue)
                GroupId = survey.GroupId.Value;

            Bulletins = survey.Bulletins.Select(x => new GroupSurveyResults_BulletinViewModel(x, survey.HasOpenProtocol)).OrderBy(x => x.Name).ToList();
        }
    }

    public class GroupSurveyResults_BulletinViewModel
    {
        public string Name { get; set; }
        public List<string> Results { get; set; }

        public GroupSurveyResults_BulletinViewModel(SurveyBulletin bulletin, bool hasOpenProtocol)
        {
            if (bulletin == null)
                return;

            Name = hasOpenProtocol ? bulletin.User.FullName : bulletin.Id.ToString();
            Results = bulletin.Result.Select(x => x.Title).ToList();
        }
    }
}