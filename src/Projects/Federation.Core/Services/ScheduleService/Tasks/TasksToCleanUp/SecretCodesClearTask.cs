using System;

namespace Federation.Core
{
    [Serializable]
    public class SecretCodesClearTask : ScheduleTask
    {
        public SecretCodesClearTask()
        {
            _name = "Очистка от старых кодов регистрации";
            _isUnque = true;
        }

        public override void Execute()
        {
            AccountService.RemoveOldSecretCodes();
        }       
    }
}
