using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using Federation.Core;

namespace Federation.MailService
{
    public class DailyMailTask
    {
        private bool _isRuning;
        private bool _tryStop;
        private DateTime _feedDateBegin;
        private DateTime _feedDateEnd;
        private int _maxNumberOfThreads;
        private int _maxMessagesPerThread;
        private Assembly _assembly;
        private ConcurrentQueue<List<FeedRecord>> _mailList = new ConcurrentQueue<List<FeedRecord>>();
        private ConcurrentStack<int> _threads = new ConcurrentStack<int>();

        private ServiceLogger _log = new ServiceLogger("DailyMail");
        private delegate void SendInvoke(List<FeedRecord> package, int threadId);

        public ServiceLogger Log { get { return _log; } }
        public int MaxNumberOfThreads { get { return _maxNumberOfThreads; } }
        public int AmountOfThreadsProcessing { get { return _maxNumberOfThreads - _threads.Count; } }
        public int MaxMessagesPerThread { get { return _maxMessagesPerThread; } }
        public bool IsRuning { get { return _isRuning; } }
        public DateTime FeedDate { get { return _feedDateBegin; } }

        public DailyMailTask(DateTime feedDate, int maxMessagesPerThread = 50, int maxThreads = 3)
        {
            if (maxMessagesPerThread <= 0)
                throw new Exception("Максимальное количество писем на поток не может быть меньше или равно 0");

            if (maxThreads <= 0)
                throw new Exception("Максимальное количество потоков не может быть меньше или равно 0");

            _maxMessagesPerThread = maxMessagesPerThread;
            _maxNumberOfThreads = maxThreads;
            _feedDateBegin = feedDate.Date;
            _feedDateEnd = _feedDateBegin.Date.AddDays(1);
        }

        #region Core

        public void Initialize(ServiceLogger externalLogger = null)
        {
            if (externalLogger == null)
                externalLogger = new ServiceLogger("DailyMail");

            _log = externalLogger;

            var startMessage = string.Format("----- Инициализация {0} -----", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
            _log.Write(startMessage);

            var reader = new StreamReader(@"Template\MailMessageTemplate.cshtml");
            var messageTemplate = reader.ReadToEnd();
            reader.Close();
            _log.Write(string.Format("Завершено чтение шаблона из Template\\MailMessageTemplate.cshtml"));

            _assembly = RazorBasedTemplateRenderEngine.GenerateAssemblyFromTemplate(messageTemplate);
            _log.Write(string.Format("Шаблон собран и готов к работе"));

            for (int i = _maxNumberOfThreads; i > 0; i--)
            {
                _threads.Push(i);
            }
            _log.Write(string.Format("Добавлены метки возможных потоков"));
        }

        public void Start()
        {
            _isRuning = true;
            _log.Write(string.Format("----- Запуск {0} -----", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));

            PrepareMailList();

            while (!_tryStop)
            {
                var iterationsLeft = _mailList.Count;

                if (iterationsLeft > 0)
                {
                    int readyThreadKey;
                    if (_threads.TryPop(out readyThreadKey) && readyThreadKey != 0)
                    {
                        List<FeedRecord> package;
                        if (_mailList.TryDequeue(out package))
                        {
                            _log.Write(string.Format("Осталось итераций рассылки {0}", iterationsLeft));
                            SendInvoke sendCallback = SendPackage;
                            sendCallback.BeginInvoke(package, readyThreadKey, null, null);
                        }
                    }
                }

                if (_mailList.Count == 0 && _threads.Count == _maxNumberOfThreads)
                    _tryStop = true;

                Thread.Sleep(500);

            }
            _isRuning = false;
        }

        public void Stop()
        {
            _tryStop = true;
        }

        #endregion

        public void PrepareMailList()
        {
            var feed = DataService.PerThread.BaseUserSet.OfType<User>()
                .Where(u => !u.IsOutdated && u.IsEmailVerified && u.SubscriptionSettings.IsSubscribed && u.EncryptedEmail != null && !string.IsNullOrEmpty(u.FirstName) && 
                    (u.MailRecord == null || (u.MailRecord != null && u.MailRecord.Date < _feedDateEnd) || (u.MailRecord != null && !u.MailRecord.IsDelivered))).ToList();
                
            _log.Write(string.Format("----- Пользователей ожидают рассылки {0} -----", feed.Count));
            var innerList = new List<FeedRecord>();

            for (var i = 0; i < feed.Count; i++)
            {
                var k = i / MaxMessagesPerThread;
                if (_mailList.Count < k)
                {
                    _mailList.Enqueue(innerList);
                    innerList = new List<FeedRecord>();
                }

                innerList.Add(new FeedRecord(feed[i].Id, _feedDateBegin, _feedDateEnd));
            }
            if (innerList.Count > 0)
                _mailList.Enqueue(innerList);
        }

        public void SendPackage(IList<FeedRecord> package, int threadId)
        {
            _log.Write(string.Format("----- Поток {0} начинает собирать контент -----", threadId));

            foreach (var record in package)
            {
                try
                {
                    record.PackMailRecord();
                }
                catch (Exception exception)
                {
                    _log.Write(exception);
                    record.PackFailed = true;
                }
            }

            _log.Write(string.Format("----- Поток {0} закончил собирать контент -----", threadId));

            SendMesseges(package, threadId);
            _threads.Push(threadId);
        }

        private void SendMesseges(IList<FeedRecord> package, int threadId)
        {
            var smtp = new SmtpClient();
            var sendedCount = 0;
            var emptyMails = 0;

            _log.Write(string.Format("----- Поток {0} начинает рассылку ({1} сообщений) -----", threadId, package.Count ));

            foreach (var messageContent in package)
            {
                var messageTemplate = RazorBasedTemplateRenderEngine.NewMailMessageTemplate(messageContent, _assembly);
                messageTemplate.Execute();
                var message = new MailMessage();
                message.To.Add(messageTemplate.UserEmail);
                message.Headers.Add("Precedence", "bulk");

                try
                {
                    if (messageContent.PackFailed)
                        throw new Exception("Сборка рассылки для пользователя провалилась");

                    if (messageContent.IsThereUpdates)
                    {
                        message.Subject = "Демократия2. Обновления за " + messageTemplate.Date.ToString("dd MMMM yyyy");
                        message.IsBodyHtml = true;
                        message.Body = messageTemplate.Body;

                        message.To.Add(messageTemplate.UserEmail);
                        message.Headers.Add("Precedence", "bulk");

                        var sendingMessage = "Поток " + threadId + " Отправляем: " + messageTemplate.UserFullName +
                                             " подписка: " + messageContent.Content.Count +
                                             ", сообщений: " + (messageContent.CommentNotices.Count +
                                             messageContent.GroupMemberNotices.Count +
                                             messageContent.GroupModeratorNotices.Count +
                                             messageContent.PetitionNotices.Count +
                                             messageContent.PollNotices.Count +
                                             messageContent.PrivateMessages.Count +
                                             messageContent.SystemMessages.Count);
                        _log.Write(sendingMessage);

                        if (MainClass.TESTING)
                        {
                            if (!Directory.Exists("messages"))
                                Directory.CreateDirectory("messages");
                            File.WriteAllText("messages\\" + messageTemplate.UserId + ".html", message.Body);
                            sendedCount++;
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            smtp.Send(message);
                            sendedCount++;
                            Thread.Sleep(5000);
                        }
                    }
                    else
                    {
                        var sendingMessage = "Поток " + threadId + " Нечего отправлять: " + messageTemplate.UserFullName;
                        _log.Write(sendingMessage);

                        emptyMails++;
                    }


                    var mailRecord = DataService.PerThread.MailRecordSet.SingleOrDefault(mr => mr.User.Id == messageTemplate.UserId);
                    if (mailRecord != null)
                    {
                        mailRecord.Date = DateTime.Now;
                        mailRecord.IsDelivered = true;
                    }
                    else
                    {
                        var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == messageTemplate.UserId);
                        if (user != null)
                        {
                            mailRecord = new MailRecord
                            {
                                Date = DateTime.Now,
                                User = user,
                                IsDelivered = true
                            };
                            DataService.PerThread.MailRecordSet.AddObject(mailRecord);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _log.Write(exception);
                    var mailRecord = DataService.PerThread.MailRecordSet.SingleOrDefault(mr => mr.User.Id == messageTemplate.UserId);
                    if (mailRecord != null)
                    {
                        mailRecord.Date = DateTime.Now;
                        mailRecord.IsDelivered = false;
                    }
                    else
                    {
                        var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == messageTemplate.UserId);
                        if (user != null)
                        {
                            mailRecord = new MailRecord
                            {
                                Date = DateTime.Now,
                                User = user,
                                IsDelivered = false
                            };
                            DataService.PerThread.MailRecordSet.AddObject(mailRecord);
                        }
                    }
                }
                finally
                {
                    message.Dispose();
                    DataService.PerThread.SaveChanges();
                }
            }

            smtp.Dispose();
            _log.Write(string.Format("----- Поток {0} закончил рассылку ({3} сообщений: {1} отправлено, {2} пустых) -----", threadId, sendedCount, emptyMails, package.Count));
        }
    }
}