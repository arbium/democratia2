using System;

namespace Federation.Core
{
    [Serializable]
    public class WasteGroupsClearTask : ScheduleTask
    {
        public WasteGroupsClearTask()
        {
            _name = "Очистка старых бланковых групп";
            _isUnque = true;
        }

        public override void Execute()
        {
            GroupService.ClearOldBlankGroups();
        }       
    }
}
