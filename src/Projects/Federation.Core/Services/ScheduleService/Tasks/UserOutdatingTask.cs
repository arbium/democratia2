using System;

namespace Federation.Core
{
    [Serializable]
    public class UserOutdatingTask : ScheduleTask
    {
        public UserOutdatingTask()
        {
            _name = "Проверка неактивных пользователей";
            _isUnque = true;
        }

        public override void Execute()
        {
            UserService.UserOutdating();
        }
    }
}