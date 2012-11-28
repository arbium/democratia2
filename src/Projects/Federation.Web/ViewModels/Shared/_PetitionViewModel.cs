using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _PetitionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsUserSigned { get; set; }
        public ContentState State { get; set; }
        public IList<_Petition_CoauthorViewModel> Coauthors { get; set; }
        public bool IsFinished { get; set; }
        public bool IsPrivate { get; set; }
        public string Privacy { get; set; }
        public string SignersCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? GroupId { get; set; }

        public _PetitionViewModel()
        {
        }

        public _PetitionViewModel(Petition petition, Guid? userId)
        {
            if (petition != null)
            {
                Id = petition.Id;
                Title = petition.Title;
                Text = petition.Text;                
                IsFinished = petition.IsFinished;
                IsPrivate = petition.IsPrivate;
                StartDate = petition.PublishDate;
                State = (ContentState)petition.State;

                GroupId = petition.GroupId;

                Coauthors = petition.Coauthors.Where(c => c.IsAccepted.HasValue && c.IsAccepted.Value).Select(c => new _Petition_CoauthorViewModel(c)).ToList();
                SignersCount = DeclinationService.OfNumber(petition.Signers.Count, "человек", "человека", "человек");

                if (userId.HasValue)
                    IsUserSigned = petition.Signers.Count(x => x.Id == userId.Value) != 0;

                if (petition.PublishDate.HasValue && petition.Duration.HasValue)
                    EndDate = petition.PublishDate.Value.AddDays(petition.Duration.Value);                               
                
                if (petition.IsPrivate && petition.GroupId.HasValue)
                    Privacy = "только для членов группы";                
            }
        }

        public class _Petition_CoauthorViewModel
        {
            public string Name { get; set; }
            public string Avatar { get; set; }
            public Guid UserId { get; set; }

            public _Petition_CoauthorViewModel()
            {
            }

            public _Petition_CoauthorViewModel(Coauthor coauthor)
            {
                if (coauthor != null)
                {
                    Name = coauthor.User.FullName;
                    Avatar = ImageService.GetImageUrl<User>(coauthor.User.Avatar);
                    UserId = coauthor.UserId;
                }
            }
        }
    }
}