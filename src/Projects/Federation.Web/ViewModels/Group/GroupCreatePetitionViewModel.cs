using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupCreatePetitionViewModel
    {        
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public bool IsContentModeration { get; set; }

        public _CreatePetitionViewModel CreatePetition { get; set; }

        public GroupCreatePetitionViewModel()
        {
            CreatePetition = new _CreatePetitionViewModel();
        }

        public GroupCreatePetitionViewModel(Group group)
        {
            CreatePetition = new _CreatePetitionViewModel();

            if (group != null)
            {                
                GroupId = group.Id;
                GroupName = group.Name;
                GroupUrl = group.Url;
                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);

                CreatePetition.GroupId = group.Id;
            }
        }
    }
}