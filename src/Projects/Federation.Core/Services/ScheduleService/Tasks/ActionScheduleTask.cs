using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    [Serializable]
    public class ActionScheduleTask : ScheduleTask
    {
        private Action _innerAction;

        public ActionScheduleTask(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _name = "Упакованный акшен " + _innerAction.Method.Name;
            _innerAction = action;
        }

        public override void Execute()
        {
            _innerAction.Invoke();
        }       
    }
}
