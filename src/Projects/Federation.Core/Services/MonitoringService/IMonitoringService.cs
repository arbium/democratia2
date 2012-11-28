using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core.Services.MonitoringService
{
    public interface IMonitoringService
    {
        void AsynchLogUserAuthentification(string userName, string ip, bool status);
        void AsynchLogUserAuthorization(Guid? userId, string ip, bool status);
        void AsynchLogUserPollVote(Guid? userId, Guid groupMemberId, Guid poolId, string result);
        void AsynchLogUserRegistration(Guid? userId, string ip);
    }
}
