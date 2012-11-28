using System;

namespace Federation.Core.Services.MonitoringService
{
    class MonitoringServiceImpl : IMonitoringService
    {
        private delegate void AsynchDelegate();

        public void AsynchLogUserAuthentification(string userName, string ip, bool status)
        {
            var userAuthentificationLog = new UserAuthentificationLog
            {
                UserName = userName,
                IpAddress = ip,
                Status = status,
                DateTime = DateTime.Now
            };

            AsynchDelegate aDelegate = () =>
            {
                DataService.PerThread.UserAuthentificationLogSet.AddObject(userAuthentificationLog);
                DataService.PerThread.SaveChanges();
            };

            var asynchRes = aDelegate.BeginInvoke(null, null);
            aDelegate.EndInvoke(asynchRes);
        }

        public void AsynchLogUserAuthorization(Guid? userId, string ip, bool status)
        {
            var userAuthorizationLog = new UserAuthorizationLog
            {
                UserId = userId.Value,
                IpAddress = ip,
                Status = status,
                DateTime = DateTime.Now
            };

            AsynchDelegate aDelegate = () =>
            {
                DataService.PerThread.UserAuthorizationLogSet.AddObject(userAuthorizationLog);
                DataService.PerThread.SaveChanges();
            };

            var asynchRes = aDelegate.BeginInvoke(null, null);
            aDelegate.EndInvoke(asynchRes);
        }

        public void AsynchLogUserPollVote(Guid? userId, Guid groupMemberId, Guid pollId, string result)
        {
            var userPollVoteLog = new UserPollVoteLog
            {
                UserId = userId.Value,
                GroupMemberId = groupMemberId,
                PollId = pollId,
                Result = result,
                DateTime = DateTime.Now
            };

            AsynchDelegate aDelegate = () =>
            {
                DataService.PerThread.UserPollVoteLogSet.AddObject(userPollVoteLog);
                DataService.PerThread.SaveChanges();
            };

            var asynchRes = aDelegate.BeginInvoke(null, null);
            aDelegate.EndInvoke(asynchRes);
        }

        public void AsynchLogUserRegistration(Guid? userId, string ip)
        {
            var userRegistrationLog = new UserRegistrationLog
            {
                UserId = userId.Value,
                IpAddress = ip,
                DateTime = DateTime.Now
            };

            AsynchDelegate aDelegate = () =>
            {
                DataService.PerThread.UserRegistrationLogSet.AddObject(userRegistrationLog);
                DataService.PerThread.SaveChanges();
            };

            var asynchRes = aDelegate.BeginInvoke(null, null);
            aDelegate.EndInvoke(asynchRes);
        }
    }
}
