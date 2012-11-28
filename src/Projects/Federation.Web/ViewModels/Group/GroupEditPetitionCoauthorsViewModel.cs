using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditPetitionCoauthorsViewModel
    {
        public Guid GroupId { get; set; }

        public readonly _EditPetitionCoauthorsViewModel EditPetitionCoauthors = new _EditPetitionCoauthorsViewModel();

        public GroupEditPetitionCoauthorsViewModel()
        {
        }

        public GroupEditPetitionCoauthorsViewModel(Petition petition)
        {
            if (petition != null)
            {
                if (petition.GroupId.HasValue)
                    GroupId = petition.GroupId.Value;

                EditPetitionCoauthors = new _EditPetitionCoauthorsViewModel(petition);
            }
        }
    }
}