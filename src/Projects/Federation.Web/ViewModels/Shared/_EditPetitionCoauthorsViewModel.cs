using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _EditPetitionCoauthorsViewModel
    {
        public Guid PetitionId { get; set; }
        public string PetitionController { get; set; }
        public string PetitionTitle { get; set; }

        public IList<_EditPetitionCoauthors_Coauthor> Coauthors { get; set; }

        [Display(Name = "ФИО (полностью)")]
        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"^\w{1,}\s\w{1,}\s\w{1,}$")]
        public string UserNameForInvite { get; set; }

        public _EditPetitionCoauthorsViewModel()
        {
            Coauthors = new List<_EditPetitionCoauthors_Coauthor>();
        }

        public _EditPetitionCoauthorsViewModel(Petition petition)
        {
            if (petition != null)
            {
                PetitionId = petition.Id;
                PetitionController = petition.Controller;
                PetitionTitle = petition.Title;
                Coauthors = petition.Coauthors.Select(x => new _EditPetitionCoauthors_Coauthor(x)).ToList();
            }
            else
                Coauthors = new List<_EditPetitionCoauthors_Coauthor>();
        }
    }

    public class _EditPetitionCoauthors_Coauthor
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }

        public _EditPetitionCoauthors_Coauthor()
        {
        }

        public _EditPetitionCoauthors_Coauthor(Coauthor coauthor)
        {
            Id = coauthor.Id;
            Name = coauthor.User.FullName;
            UserId = coauthor.UserId;

            if (!coauthor.IsAccepted.HasValue)
                Status = "нет ответа";
            else if (coauthor.IsAccepted.Value)
                Status = "принято";
            else
                Status = "отклонено";
        }
    }
}