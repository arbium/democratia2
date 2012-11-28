using System;

namespace Federation.Core
{
    [Serializable]
    public class FinishPollTask : ScheduleTask
    {
        private Guid _pollId = Guid.Empty;

        public FinishPollTask(Guid pollId)
        {
            _pollId = pollId;
            _name = "Завершение голосования " + _pollId;
        }

        public override void Execute()
        {
            VotingService.SummarizePoll(_pollId);
        }       
    }
}
