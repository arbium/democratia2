using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core.Services.ScheduleService.Tasks
{
    [Serializable]
    public class SendCommentsToMailTask : ScheduleTask
    {
        public override void Execute()
        {
            DateTime lastTime = DateTime.Now.AddMinutes(-30);

            var comments = DataService.PerThread.CommentSet.Where(x => x.DateTime.CompareTo(lastTime) > 0 && x.ParentComment != null);

            Dictionary<Guid, List<Guid>> UsersCommentsToMail = new Dictionary<Guid,List<Guid>>();

            foreach(var comment in comments)
            {
                if (UsersCommentsToMail.ContainsKey(comment.ParentComment.UserId))
                {
                    int index = UsersCommentsToMail.Keys.ToList().IndexOf(comment.ParentComment.UserId);
                    UsersCommentsToMail.Values.ElementAt(index).Add(comment.Id);
                }
                else 
                {
                    List<Guid> newComments = new List<Guid>();
                    newComments.Add(comment.Id);
                    UsersCommentsToMail.Add(comment.ParentComment.UserId, newComments);
                }
            }

            // вызываем композер, которого еще нет
            // сендим на меил

        }
    }
}
