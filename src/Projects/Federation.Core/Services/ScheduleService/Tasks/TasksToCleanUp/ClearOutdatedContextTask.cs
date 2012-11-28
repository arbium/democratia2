using System;

namespace Federation.Core
{
    [Serializable]
    public class ClearOutdatedContext : ScheduleTask
    {
        public ClearOutdatedContext()
        {
            _name = "Очистка неиспользуемого контекста";
            _isUnque = true;
        }

        public override void Execute()
        {
            UserContextService.ClearUnused();
        }
    }
}