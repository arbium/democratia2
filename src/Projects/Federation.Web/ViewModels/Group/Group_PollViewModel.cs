using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_PollViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsFinished { get; set; }
        public ContentState State { get; set; }

        public Guid GroupId { get; set; }

        public bool IsCreationInProccess { get; set; }
        public bool IsCreationFailed { get; set; }

        public Group_PollStatusViewModel PollStatus { get; set; }
        public List<Group_Poll_ExpertViewModel> Experts { get; set; }

        public Group_PollViewModel()
        {
            Experts = new List<Group_Poll_ExpertViewModel>();
        }

        public Group_PollViewModel(Poll poll)
        {
            Experts = new List<Group_Poll_ExpertViewModel>();

            if (poll != null)
            {
                Id = poll.Id;
                Title = poll.Title;
                Text = poll.Text;
                IsFinished = poll.IsFinished;
                State = (ContentState)poll.State;
                if(poll.GroupId.HasValue)
                    GroupId = poll.GroupId.Value;


                var publishData = VotingService.GetPollPublishData(poll.Id);
                if (publishData != null)
                {
                    IsCreationInProccess = true;
                    IsCreationFailed = publishData.IsFailed;
                }

                PollStatus = new Group_PollStatusViewModel(poll);
                
                var totalBulletins =  poll.Bulletins.Count;
                foreach (var expertBulletin in poll.Bulletins.Where(x => x.Weight > 1).OrderByDescending(x => x.Weight).Take(5))
                    Experts.Add(new Group_Poll_ExpertViewModel(expertBulletin, totalBulletins));
            }
        }
    }

    public class Group_Poll_ExpertViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Avatar { get; set; }
        public int Represent { get; set; }
        public double RelativeRepresent { get; set; }
        public string VoteResult { get; set; }
        public string Comment { get; set; }

        public Group_Poll_ExpertViewModel()
        {
        }

        public Group_Poll_ExpertViewModel(PollBulletin bulletin, int totalBulletins)
        {
            if (bulletin != null)
            {
                Id = bulletin.Owner.UserId;
                var expert = bulletin.Owner.User;
                Name = expert.FirstName;
                Surname = expert.SurName;
                Patronymic = expert.Patronymic;
                Avatar = ImageService.GetImageUrl<User>(expert.Avatar);

                Represent = bulletin.Weight;
                double weight = Represent;
                RelativeRepresent = Math.Round((100 * weight / totalBulletins), 2);
                Comment = bulletin.Comment;

                switch ((VoteOption)bulletin.Result)
                {
                    case VoteOption.Yes: VoteResult = "<span class='vote yes'>За</span>"; break;
                    case VoteOption.No: VoteResult = "<span class='vote no'>Против</span>"; break;
                    case VoteOption.Refrained: VoteResult = "<span class='vote forefit'>Воздержался</span>"; break;
                    case VoteOption.NotVoted: VoteResult = "<span class='vote not'>Не голосовал</span>"; break;
                }
                
            }
        }
    }
}