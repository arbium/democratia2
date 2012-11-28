using System;
using Federation.Core.Helpers;

namespace Federation.Core
{
    [Serializable]
    public class AdminKeysClearTask : ScheduleTask
    {
        public AdminKeysClearTask()
        {
            _name = "Очистка админских ключей";
            _isUnque = true;
        }

        public override void Execute()
        {
            AdminKeys.ClearUnused();
        }
    }
}
