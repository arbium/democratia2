using System;

namespace Federation.Core
{
    [Serializable]
    public class PasswordRecoveriesClearTask : ScheduleTask
    {
        public PasswordRecoveriesClearTask()
        {
            _name = "Очистка от старых восстановлений паролей";
            _isUnque = true;
        }

        public override void Execute()
        {
            AccountService.RemoveOldPasswordRecoveries();
        }       
    }
}
