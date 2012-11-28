using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditPetitionViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public bool IsContentModeration { get; set; }

        public readonly _EditPetitionViewModel EditPetition = new _EditPetitionViewModel();

        public GroupEditPetitionViewModel()
        {
        }

        public GroupEditPetitionViewModel(Petition petition)
        {
            if (petition != null)
            {
                if (petition.GroupId.HasValue)
                {
                    GroupId = petition.GroupId.Value;
                    GroupName = petition.Group.Name;
                    GroupUrl = petition.Group.Url;
                    IsContentModeration = petition.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
                }

                EditPetition = new _EditPetitionViewModel(petition);
            }
        }
    }
}