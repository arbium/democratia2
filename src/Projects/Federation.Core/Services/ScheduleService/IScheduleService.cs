using System;

namespace Federation.Core
{
    public interface IScheduleService
    {
        void Initialize(uint callPeriodicitySeconds, uint accuracyMilliseconds);
        void Stop();
        void Start();
        void StartMainTasks();
        DateTime NextWakeupTime { get; }

        ScheduleJob AddJob(ScheduleTask task, DateTime executeTime, bool doRepeat, int? repeatPeriod, bool tryRecover);
        ScheduleJob AddJob(ScheduleTask task, TimeSpan executeAfter, bool doRepeat, int? repeatPeriod, bool tryRecover);

        bool DoWork { get; }
        uint CallPeriodicitySeconds { get; }
        uint AccuracyMilliseconds { get; }
    }
}
