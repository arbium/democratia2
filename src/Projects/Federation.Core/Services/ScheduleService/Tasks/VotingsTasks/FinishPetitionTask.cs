using System;

namespace Federation.Core
{
    [Serializable]
    public class FinishPetitionTask : ScheduleTask
    {
        private Guid _petitionId = Guid.Empty;

        public FinishPetitionTask(Guid petitionId)
        {
            _petitionId = petitionId;
            _name = "Завершение петиции " + _petitionId;
        }

        public override void Execute()
        {
            VotingService.FinishPetition(_petitionId);
        }       
    }
}
