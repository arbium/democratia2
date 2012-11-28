using System;

namespace Federation.Core
{
    [Serializable]
    public class ConfirmationClearTask : ScheduleTask
    {
        public ConfirmationClearTask()
        {
            _name = "Очистка ключей подтверждения действий";
            _isUnque = true;
        }

        public override void Execute()
        {
            ConfirmationService.Clear();
        }       
    }
}
