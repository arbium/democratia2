using System;

namespace Federation.Core
{
    [Serializable]
    public class ServiceMsgsClearTask : ScheduleTask
    {
        public ServiceMsgsClearTask()
        {
            _name = "Очистка служебных сообщений";
            _isUnque = true;
        }

        public override void Execute()
        {
            MessageService.ClearOldServiceMessages();
        }       
    }
}
