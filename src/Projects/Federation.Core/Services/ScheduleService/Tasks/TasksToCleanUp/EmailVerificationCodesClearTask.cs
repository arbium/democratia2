using System;

namespace Federation.Core
{
    [Serializable]
    public class EmailVerificationCodesClearTask : ScheduleTask
    {
        public EmailVerificationCodesClearTask()
        {
            _name = "Очистка от старых кодов подтверждения почты";
            _isUnque = true;
        }

        public override void Execute()
        {
            AccountService.RemoveOldEmailVerifications();
        }       
    }
}
