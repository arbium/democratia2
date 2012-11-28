using System;
using System.Linq;

namespace Federation.Core
{
    [Serializable]
    public class CheckNotFinishedPollsTask : ScheduleTask
    {
        public CheckNotFinishedPollsTask()
        {
            _name = "Поиск незавершенных просроченных голосований";
            _isUnque = true;
        }

        public override void Execute()
        {
            //TODO: проверять остальные голосования
            var polls = DataService.PerThread.ContentSet.OfType<Poll>().Where(x => !x.IsFinished && x.State == (byte)ContentState.Approved && x.PublishDate.HasValue).ToList() ;
            foreach (var poll in polls)
            {
                if (poll.PublishDate != null && poll.Duration.HasValue && poll.PublishDate.Value.AddDays(poll.Duration.Value) <= DateTime.Now)
                    ScheduleService.AddJob(new FinishPollTask(poll.Id), DateTime.Now.AddMinutes(1), false, null, false);
            }
        }       
    }
}
