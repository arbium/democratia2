using System;

namespace Federation.Core
{
    [Serializable]
    public class UnbindElectionTask : ScheduleTask
    {
        private Guid _ellectionId = Guid.Empty;

        public UnbindElectionTask(Guid ellectionId)
        {
            _name = "Открепление выборов со стены + _ellectionId";
            _ellectionId = ellectionId;
        }

        public override void Execute()
        {
            ContentService.Detach(_ellectionId, null, AttachDetachTarget.Group);
        }       
    }
}
