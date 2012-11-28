using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Federation.Core
{
    public static class ScheduleService
    {
        private static IScheduleService _current = new ScheduleServiceImpl();

        public static DateTime NextWakeupTime
        { 
            get { return _current.NextWakeupTime; }
        }

        public static void Initialize(uint callPeriodicitySeconds = 120, uint accuracyMilliseconds = 100)
        {
            _current.Initialize(callPeriodicitySeconds, accuracyMilliseconds);
        }

        public static void Start()
        {
            _current.Start();
        }

        public static void Stop()
        {
            _current.Stop();
        }

        public static void StartMainTasks()
        {
            _current.StartMainTasks();
        }

        public static ScheduleJob AddJob(ScheduleTask task, DateTime executeTime, bool doRepeat, int? repeatPeriod, bool tryRecover)
        {
            return _current.AddJob(task, executeTime, doRepeat, repeatPeriod, tryRecover);
        }

        public static ScheduleJob AddJob(ScheduleTask task, TimeSpan executeAfter, bool doRepeat, int? repeatPeriod, bool tryRecover)
        {
            return _current.AddJob(task, executeAfter, doRepeat, repeatPeriod, tryRecover);
        }

        public static bool DoWork { get {return _current.DoWork;} }
        public static uint CallPeriodicitySeconds { get { return _current.CallPeriodicitySeconds; } }
        public static uint AccuracyMilliseconds { get { return _current.AccuracyMilliseconds; } }

        public static ScheduleTask Deserialize( byte[] serializedTask)
        {
            var stream = new MemoryStream(serializedTask);
            stream.Position = 0; //?
            BinaryFormatter bformatter = new BinaryFormatter();
            ScheduleTask resultFunction = bformatter.Deserialize(stream) as ScheduleTask;
            if (resultFunction == null)
                throw new Exception("Неверные данные!");
            return resultFunction;
        }
    }
}
