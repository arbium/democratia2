using System;

namespace Federation.Core
{
    [Serializable]
    public class UpdateTagsWeightTask : ScheduleTask
    {
        public UpdateTagsWeightTask()
        {
            _name = "Обновление веса у тэгов добавлен";
            _isUnque = true;
        }

        public override void Execute()
        {
            TagService.UpdateTagsWeight();
        }
    }
}