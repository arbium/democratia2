using System;

namespace Federation.Core
{
    public class PollPublishDataContainer 
    {
        private DateTime _beganAt = DateTime.Now;
        private bool _isFailed = false;
        private string _failMessage = String.Empty;

        public DateTime BeganAt { get { return _beganAt; } }
        public bool IsFailed { get { return _isFailed; } }
        public string FailMessage { get { return _failMessage; } }

        internal void Fail(string message)
        {
            _isFailed = true;
            _failMessage = message;
        }
    }
}
