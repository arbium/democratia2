using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupPetitionSignersViewModel
    {
        public Guid GroupId { get; set; }
        public Guid PetitionId { get; set; }
        public string PetitionTitle { get; set; }

        public IList<GroupPetitionSigners_UserViewModel> Signers = new List<GroupPetitionSigners_UserViewModel>();

        public GroupPetitionSignersViewModel()
        {
        }

        public GroupPetitionSignersViewModel(Petition petition)
        {
            if (petition != null)
            {
                if (petition.GroupId.HasValue)
                    GroupId = petition.GroupId.Value;

                PetitionId = petition.Id;
                PetitionTitle = petition.Title;
                Signers = petition.Signers.Select(x => new GroupPetitionSigners_UserViewModel(x)).ToList();
            }
        }
    }

    public class GroupPetitionSigners_UserViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public GroupPetitionSigners_UserViewModel()
        {
        }

        public GroupPetitionSigners_UserViewModel(User user)
        {
            Avatar = ImageService.GetImageUrl<User>(user.Avatar);
            Id = user.Id;
            Name = user.FullName;
        }
    }
}