using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserPetitionNoticesViewModel
    {
        public IList<UserPetitionNotices_ItemViewModel> Items { get; set; }

        public UserPetitionNoticesViewModel()
        {
            Items = new List<UserPetitionNotices_ItemViewModel>();
        }

        public UserPetitionNoticesViewModel(Guid userId)
        {
            var items = DataService.PerThread.CoauthorSet.Where(c => !c.IsAccepted.HasValue && c.UserId == userId);
            Items = new List<UserPetitionNotices_ItemViewModel>();

            foreach (var item in items)
                Items.Add(new UserPetitionNotices_ItemViewModel(item));
        }
    }

    public class UserPetitionNotices_ItemViewModel
    {
        public Guid CoauthorId { get; set; }
        public Guid PetitionId { get; set; }
        public string PetitionController { get; set; }
        public string PetitionTitle { get; set; }

        public UserPetitionNotices_ItemViewModel()
        {
        }

        public UserPetitionNotices_ItemViewModel(Coauthor coauthor)
        {
            if (coauthor != null)
            {
                CoauthorId = coauthor.Id;
                PetitionId = coauthor.Petition.Id;
                PetitionController = coauthor.Petition.Controller;
                PetitionTitle = coauthor.Petition.Title;
            }
        }
    }
}