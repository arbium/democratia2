using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _PetitionStatusViewModel
    {
        public IList<_PetitionStatus_PetitionViewModel> Votings { get; set; }

        public _PetitionStatusViewModel()
        {
            Votings = new List<_PetitionStatus_PetitionViewModel>();
        }

        public _PetitionStatusViewModel(ICollection<Petition> petitions, Guid? userId)
        {
            Votings = new List<_PetitionStatus_PetitionViewModel>();
            if (petitions != null)
            {
                foreach (var record in petitions)
                    Votings.Add(new _PetitionStatus_PetitionViewModel(record, userId));
            }
        }
    }

    public class _PetitionStatus_PetitionViewModel
    {
        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }

        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorSurname { get; set; }
        public string AuthorPatronymic { get; set; }
        public string AuthorAvatar { get; set; }

        public string CommentsCount { get; set; }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public int SignaturesCount { get; set; }
        public bool IsFinished { get; set; }
        public DateTime FinishDate { get; set; }

        public bool IsUserParticipant { get; set; }
        public bool UserSigned { get; set; }

        public _PetitionStatus_PetitionViewModel()
        {
        }

        public _PetitionStatus_PetitionViewModel(Petition record, Guid? userId)
        {
            if (record != null)
            {
                GroupId = record.GroupId;
                if (GroupId.HasValue)
                {
                    GroupName = record.Group.Name;
                    GroupUrl = record.Group.Url;

                    Url = record.GetUrl();
                }

                if (record.AuthorId.HasValue)
                {
                    AuthorId = record.AuthorId.Value;
                    AuthorName = record.Author.FirstName;
                    AuthorPatronymic = record.Author.Patronymic;
                    AuthorSurname = record.Author.SurName;
                    AuthorAvatar = ImageService.GetImageUrl<User>(record.Author.Avatar);

                    if (!GroupId.HasValue)
                        Url = record.GetUrl();
                }

                CommentsCount = DeclinationService.OfNumber(record.Comments.Count, "комментарий", "комментария", "комментариев");

                Id = record.Id;
                Title = record.Title;

                SignaturesCount = record.Signers.Count;
                IsFinished = record.IsFinished;
                if (record.Duration.HasValue && record.PublishDate.HasValue)
                    FinishDate = record.PublishDate.Value.AddDays(record.Duration.Value);

                IsUserParticipant = false;

                if (userId.HasValue && userId != Guid.Empty)
                {
                    if (record.GroupId.HasValue)
                    {
                        var member = GroupService.UserInGroup(userId.Value, record.GroupId.Value);
                        if (member != null)
                        {
                            IsUserParticipant = true;
                            var signature = record.Signers.Count(x=>x.Id == userId);
                            UserSigned = (signature > 0);
                        }
                    }
                    else
                    {
                        IsUserParticipant = true;
                        var signature = record.Signers.Count(x => x.Id == userId);
                        UserSigned = (signature > 0);
                    }
                }
            }
        }
    }
}