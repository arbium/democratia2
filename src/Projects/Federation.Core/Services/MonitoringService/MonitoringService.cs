using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core.Services.MonitoringService
{
    public static class MonitoringService
    {
        private static IMonitoringService _current = new MonitoringServiceImpl();

        public static void AsynchLogUserAuthentification(string userName, string ip, bool status)
        {
            _current.AsynchLogUserAuthentification(userName, ip, status);
        }

        public static void AsynchLogUserAuthorization(Guid? userId, string ip, bool status)
        {
            _current.AsynchLogUserAuthorization(userId, ip, status);
        }

        public static void AsynchLogUserPollVote(Guid? userId, Guid groupMemberId, Guid poolId, string result)
        {
            _current.AsynchLogUserPollVote(userId, groupMemberId, poolId, result);
        }

        public static void AsynchLogUserRegistration(Guid? userId, string ip)
        {
            _current.AsynchLogUserRegistration(userId, ip);
        }
    }
}
