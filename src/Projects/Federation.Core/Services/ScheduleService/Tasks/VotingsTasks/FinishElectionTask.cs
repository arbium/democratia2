using System;

namespace Federation.Core
{
    [Serializable]
    public class FinishElectionTask : ScheduleTask
    {
        private Guid _electionId = Guid.Empty;

        public FinishElectionTask(Guid ellectionId)
        {
            _electionId = ellectionId;
            _name = "Завершение выборов " + _electionId;
        }

        public override void Execute()
        {
            VotingService.FinishElection(_electionId);
        }       
    }
}
