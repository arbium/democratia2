using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    //TODO етсь бага когда два таск стоят оооочень близко на временной шкале, допустим с разницей в несколько тактов процессорного времени
    public class ScheduleServiceImpl : IScheduleService
    {
        private static readonly ConcurrentQueue<ScheduleJob> _queue = new ConcurrentQueue<ScheduleJob>();
        private ScheduleJob _nextJob;
        private Thread _scheduleThread = null;
        private uint _callPeriodicitySeconds = 0;
        private uint _accuracyMilliseconds = 0;//Поидеи на чуууточку больше чем время выполнения функции Scheduling в худшем случае
        private bool _DoWork = false;
        private DateTime _nextWakeupTime = DateTime.MinValue;
        private static ServiceLogger _log = new ServiceLogger("Schedule");

        #region Api

        public DateTime NextWakeupTime { get { return _nextWakeupTime; } }

        public void Initialize(uint callPeriodicitySeconds, uint accuracyMilliseconds)
        {
            if (_scheduleThread == null)
            {
                _log.EnableAutoSave(300);
                _log.Write("Инициализация Шедуллера " + DateTime.Now);
                _callPeriodicitySeconds = callPeriodicitySeconds;
                _accuracyMilliseconds = accuracyMilliseconds;

                _scheduleThread = new Thread(new ThreadStart(Scheduling));
                _scheduleThread.IsBackground = true;
                _scheduleThread.Start();
                _DoWork = true;
                StartMainTasks();
            }
            //не уверен что нужно выкидывать ошибку 
        }

        public void StartMainTasks()
        {
            ScheduleService.AddJob(new AdminKeysClearTask(), new TimeSpan(1, 0, 0), true, 3600, false);
            ScheduleService.AddJob(new ConfirmationClearTask(), new TimeSpan(1, 0, 0), true, 3600, false);
            ScheduleService.AddJob(new EmailVerificationCodesClearTask(), new TimeSpan(1, 0, 0), true, 3600 * 24 * 7, false);
            ScheduleService.AddJob(new PasswordRecoveriesClearTask(), DateTime.Now, true, 3600 * 24 * 7, false);
            ScheduleService.AddJob(new SecretCodesClearTask(), DateTime.Now, true, 3600 * 24 * 2, false);
            ScheduleService.AddJob(new WasteGroupsClearTask(), new TimeSpan(1, 0, 0), true, 3600 * 24, false);
            ScheduleService.AddJob(new ClearOutdatedContext(), DateTime.Now, true, 900, false);
            ScheduleService.AddJob(new CheckNotFinishedPollsTask(), DateTime.Now.AddMinutes(5), true, 3600 * 24, false);
            ScheduleService.AddJob(new UserOutdatingTask(), new TimeSpan(1, 0, 0, 0), true, 3600 * 24, false);
            ScheduleService.AddJob(new UpdateTagsWeightTask(), DateTime.Now, true, 3600 * 12, false);
            ScheduleService.AddJob(new ServiceMsgsClearTask(), DateTime.Now, true, 3600 * 24, false);
            ScheduleService.AddJob(new UpdateGroupsRatingTask(), DateTime.Now, true, 3600 * 24, false);            
        }

        public void Start()
        {
            _log.Write("Шедуллер запущен");
            //if (_scheduleThread == null)
            //    throw new ScheduleNotInitializeException("Шедуллер не инициализирован. Прежде чем запускать шедуллер - инициализируйте");
            //_DoWork = true;
        }

        public void Stop()
        {
            _log.Write("Шедуллер остановлен");
            //if (_scheduleThread == null)
            //    throw new ScheduleNotInitializeException("Шедуллер не инициализирован. Прежде чем останавливать шедуллер - инициализируйте");
            //_DoWork = true;
        }

        public bool DoWork { get { return _DoWork; } }
        public uint CallPeriodicitySeconds { get { return _callPeriodicitySeconds; } }
        public uint AccuracyMilliseconds { get { return _accuracyMilliseconds; } }

        public ScheduleJob AddJob(ScheduleTask task, DateTime executionTime, bool doRepeat, int? repeatPeriod, bool tryRecover)
        {
            _log.Write("Добавление нового таска " + task.Name);
            ScheduleJob newJob = new ScheduleJob
                                     {
                                         CreationDate = DateTime.Now,
                                         ExecutionDate = DateTime.Now,
                                         NextExecutionDate = executionTime,
                                         Task = task.Serialize(),
                                         State = (byte)JobState.ReadyToStart,
                                         RepeatPeriod = repeatPeriod,
                                         DoRepeat = doRepeat,
                                         TryRecover = tryRecover,
                                         IsUnique = task.IsUnique,
                                         Title = task.Name,
                                         Type = task.GetType().FullName
                                     };

            if (newJob.IsUnique)
            {
                var oldjob = DataService.PerThread.ScheduleJobSet.FirstOrDefault(x => x.Type == newJob.Type && (x.State == (byte)JobState.ReadyToStart || x.State == (byte)JobState.ReadyToRecover || x.State == (byte)JobState.Started));
                if (oldjob != null)
                {
                    if (oldjob.State == (byte)JobState.Started)
                    {
                    }
                    else
                    {
                        oldjob.RepeatPeriod = newJob.RepeatPeriod;
                        oldjob.DoRepeat = newJob.DoRepeat;
                        oldjob.TryRecover = newJob.TryRecover;
                        oldjob.Title = newJob.Title;
                        DataService.PerThread.SaveChanges();                        
                    }

                    return oldjob;
                }
            }

            SaveJobsToDb(newJob);
            return newJob;
        }

        public ScheduleJob AddJob(ScheduleTask task, TimeSpan executionAfter, bool doRepeat, int? repeatPeriod, bool tryRecover)
        {
            return AddJob(task, DateTime.Now + executionAfter, doRepeat, repeatPeriod, tryRecover);
        }

        private void AbbortJob(Guid id)
        {
            RemoveJob(id);
        }

        #endregion

        #region Главный метод шедуллера

        private void Scheduling()
        {
            while (true)
            {
                _log.Write("Начало основного цикла");
                if (_nextJob != null)
                {
                    _log.Write("У меня есть назначенная задача id " + _nextJob.Id);
                    //Смотрим настало ли время для выполнения таска        
                    if ((_nextJob.NextExecutionDate - DateTime.Now).TotalMilliseconds < _accuracyMilliseconds)
                    {
                        ExecuteJob(_nextJob);
                        _nextJob = null;
                    }
                    else
                    {
                        Thread.Sleep((int)((_nextJob.NextExecutionDate - DateTime.Now).TotalMilliseconds)+1);
                    }
                }

                //проверяем не пуст ли список предстоящих работ
                if (_queue.Count == 0)
                {
                    var newJobs = GetIncomingJobs();
                    foreach (var job in newJobs)
                    {
                        _queue.Enqueue(job);
                    }
                }                

                //Получаем следующую работу
                if (_queue.Count != 0)
                {
                    _queue.TryDequeue(out _nextJob);
                }
                else
                {
                    //Расчет времени сна
                    int sleepingTime = (int)(_callPeriodicitySeconds);
                    if (_nextJob != null && (_nextJob.NextExecutionDate - DateTime.Now).TotalSeconds < _callPeriodicitySeconds)
                    {
                        sleepingTime = (int)((_nextJob.NextExecutionDate - DateTime.Now).TotalSeconds);
                    }

                    if (sleepingTime > 0)
                    {
                        _nextWakeupTime = DateTime.Now.AddSeconds(sleepingTime);
                        Thread.Sleep((sleepingTime * 1000));
                    }
                }
            }
        }

        #endregion

        #region Выполнение работы

        private void ExecuteJob(ScheduleJob executeJob)
        {
            _log.Write("Приступаю к выполнению джобы id: " + executeJob.Id);
            ScheduleTask executeTask = ScheduleService.Deserialize(executeJob.Task);
            Action execute = executeTask.Execute;
            executeJob.ExecutionDate = DateTime.Now;
            executeJob.State = (byte)JobState.Started;
            SaveJobStatusToDb(executeJob);
            Tuple<Guid, Action, DateTime> args = new Tuple<Guid, Action, DateTime>(executeJob.Id, execute, DateTime.Now);
            execute.BeginInvoke(JobIsDone, args);
        }

        private static void JobIsDone(IAsyncResult asyncResult)
        {
            Tuple<Guid, Action, DateTime> args = asyncResult.AsyncState as Tuple<Guid, Action, DateTime>;

            ScheduleJob job = DataService.PerThread.ScheduleJobSet.SingleOrDefault(x => x.Id == args.Item1);
            Action executeTask = args.Item2;
            DateTime executedTime = args.Item3;
            DateTime endTime = DateTime.Now;

            ScheduleTaskState taskState = ScheduleTaskState.Success;
            Exception exception = null;
            StringBuilder info = new StringBuilder();

            try
            {
                _log.Write("Начинается Асинхронное выполнение таска джобы " + job.Id);
                executeTask.EndInvoke(asyncResult);
            }
            catch (Exception e)
            {
                exception = e;
                _log.Write(e, "Выполнение джобы " + job.Id + " ПРЕРВАНО, возникла системная ОШИБКА");
                taskState = ScheduleTaskState.EpicFail;
            }
            finally
            {
                if (exception != null)
                {
                    info.AppendLine(exception.Message);
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                    }

                    _log.Write(exception, "Выполнение джобы " + job.Id + " завершено с ошибкой");
                    info.AppendLine("InnerException: " + exception.Message);
                    job.State = (byte)JobState.Error;
                }
                else
                {
                    _log.Write("Выполнение джобы " + job.Id + " завершено успешно");
                    if (job.DoRepeat && job.RepeatPeriod.HasValue)
                    {
                        job.State = (byte)JobState.ReadyToStart;
                        var d = DateTime.Now.AddSeconds(job.RepeatPeriod.Value);
                        job.NextExecutionDate = d;
                        _log.Write("Следующее выполнение джобы назначено на " + d);
                    }
                    else
                        job.State = (byte)JobState.Ended;
                }
                ReSaveJobToDb(job);

                Log(job, executedTime, endTime, info.ToString(), taskState);
            }

        }

        #endregion

        #region Взаимодействие с БД

        private static ScheduleJob GetJob(Guid id)
        {
            var jobFromDb = DataService.PerThread.ScheduleJobSet.SingleOrDefault(x => x.Id == id);
            return jobFromDb;
        }

        private void RemoveJob(Guid id)
        {
            var job = GetJob(id);
            job.State = (byte)JobState.Aborted;
            DataService.PerThread.SaveChanges();
        }

        private List<ScheduleJob> GetIncomingJobs()
        {
            _log.Write("Получение нового списка тасков из БД");

            var wakeupPoint = DateTime.Now.AddSeconds(_callPeriodicitySeconds);
            return DataService.PerThread.ScheduleJobSet.Where(x => x.NextExecutionDate <= wakeupPoint && (x.State == (byte)JobState.ReadyToStart || x.State == (byte)JobState.ReadyToRecover)).OrderBy(x => x.NextExecutionDate).ToList();
        }

        private static ScheduleJob GetImmediateJob()
        {
            var jobFromDb = DataService.PerThread.ScheduleJobSet.Where(x => x.State == (byte)JobState.ReadyToStart || x.State == (byte)JobState.ReadyToRecover).OrderBy(x => x.NextExecutionDate).FirstOrDefault();
            return jobFromDb;
        }

        private static void Log(ScheduleJob scheduleJob, DateTime startDate, DateTime endDate, string info, ScheduleTaskState state)
        {
            ScheduleExecutionInfo log = new ScheduleExecutionInfo()
            {
                StartDate = startDate,
                EndDate = endDate,
                ScheduleJobId = scheduleJob.Id,
                Info = info,
                State = (byte)state
            };
            DataService.PerThread.ScheduleExecutionInfoSet.AddObject(log);
            DataService.PerThread.SaveChanges();
        }

        private static void SaveJobsToDb(ScheduleJob job)
        {
            DataService.PerThread.ScheduleJobSet.AddObject(job);
            DataService.PerThread.SaveChanges();
        }

        private static void SaveJobStatusToDb(ScheduleJob job)//отвратительно
        {
            var jobFromDb = DataService.PerThread.ScheduleJobSet.FirstOrDefault(x => x.Id == job.Id);
            jobFromDb.State = job.State;
            jobFromDb.ExecutionDate = job.ExecutionDate;

            DataService.PerThread.SaveChanges();
        }

        private static void ReSaveJobToDb(ScheduleJob job)//отвратительно
        {
            var jobFromDb = DataService.PerThread.ScheduleJobSet.FirstOrDefault(x => x.Id == job.Id);
            jobFromDb.State = job.State;
            jobFromDb.NextExecutionDate = job.NextExecutionDate;

            DataService.PerThread.SaveChanges();
        }

        //private static void SaveJobStatusToDb(ScheduleJob job)//отвратительно
        //{
        //    var dataContext = DataService.GetDataServiceInstance();
        //    dataContext.BeginWork();
        //    var jobFromDb = dataContext.ScheduleJobSet.Where(x => x.Id == job.Id).FirstOrDefault();
        //    jobFromDb.State = job.State;
        //    dataContext.SaveChanges();
        //    dataContext.EndWork();
        //}

        #endregion
    }
}
