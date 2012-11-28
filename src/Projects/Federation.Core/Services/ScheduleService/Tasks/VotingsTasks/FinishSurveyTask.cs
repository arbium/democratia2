using System;

namespace Federation.Core
{
    [Serializable]
    public class FinishSurveyTask : ScheduleTask
    {
        private Guid _surveyId = Guid.Empty;

        public FinishSurveyTask(Guid surveyId)
        {
            _surveyId = surveyId;
            _name = "Завершение опроса " + _surveyId;
        }

        public override void Execute()
        {
            VotingService.FinishSurvey(_surveyId);
        }       
    }
}
