using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LJService;
using System.Text.RegularExpressions;

namespace Federation.Core
{
    public static class ContentTransferService
    {
        private static List<TransferAgent> GetLJSynchranizationList(DateTime date)
        {
            DateTime startTime = date.AddDays(-1).Date;
            DateTime endTime = date.Date;            
            List<TransferAgent> result = new List<TransferAgent>();

            var userlist = DataService.PerThread.BaseUserSet.OfType<User>().Where(x => x.LiveJournal != null && x.LiveJournalSindication && ((x.LiveJournalSynchDate >= startTime && x.LiveJournalSynchDate < endTime) || x.LiveJournalSynchDate == null)).ToList();
            
            foreach (var user in userlist)
            {
                var userJournalName = Regex.Matches(user.LiveJournal, @"((?<=http://(?!www)).*(?=.livejournal.com))|((?<=http://www.).*(?=.livejournal.com))|((?<=http://(www.)?livejournal.com/users/)\w+(?!/.*).)");
                if (userJournalName.Count == 1)
                {
                    result.Add(new TransferAgent { UserId = user.Id, TransferKey = userJournalName[0].ToString(), PublishAsDraft = user.LiveJournalSindicateAsDraft, TransferDate = startTime });
                }
            }

            return result;
        }

        private struct TransferAgent
        {
            public Guid UserId { get; set; }
            public string TransferKey { get; set; }
            public bool PublishAsDraft { get; set; }
            public DateTime TransferDate { get; set; }
        }

        public static void StartSynchronization()
        {
            var sycnchDate = DateTime.Now;
            var synchList = GetLJSynchranizationList(sycnchDate);

            foreach (var item in synchList)
            {
                var transferContent = LiveJournalService.GetContentByDate("supoved","flatronl192ws",item.TransferKey, item.TransferDate);
                var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == item.UserId);
                
                if (user != null)
                {
                    foreach (var content in transferContent)
                    {
                        if (!string.IsNullOrEmpty(content.Body))
                        {
                            content.Body = content.Body.Replace("\r\n", "<br/>");
                        }
                        ContentService.CreatePost(null, item.UserId, content.Title, content.Body, content.Tags, item.PublishAsDraft, false);
                    }

                    user.LiveJournalSynchDate = DateTime.Now;
                    DataService.PerThread.SaveChanges();
                }
            }
        }
    }
}
