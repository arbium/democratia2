using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Federation.Core
{
    [Serializable]
    public abstract class ScheduleTask
    {
        protected string _name = "";
        protected bool _isUnque = false;

        public string Name 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_name))
                    return "";
                //    throw new NotImplementedException("Класс не инициализирован");

                return _name; 
            } 
        }
        public bool IsUnique { get { return _isUnque; } }
        public abstract void Execute();
    }
}
