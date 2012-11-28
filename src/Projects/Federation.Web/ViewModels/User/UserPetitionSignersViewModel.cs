using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserPetitionSignersViewModel
    {
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid PetitionId { get; set; }
        public string PetitionTitle { get; set; }

        public readonly IList<UserPetitionSigners_UserViewModel> Signers = new List<UserPetitionSigners_UserViewModel>();

        public UserPetitionSignersViewModel()
        {
        }

        public UserPetitionSignersViewModel(Petition petition)
        {
            if (petition != null)
            {
                if (petition.AuthorId.HasValue)
                {
                    AuthorId = petition.AuthorId.Value;
                    AuthorName = petition.Author.FullName;
                }

                PetitionId = petition.Id;
                PetitionTitle = petition.Title;
                Signers = petition.Signers.Select(x => new UserPetitionSigners_UserViewModel(x)).ToList();
            }
        }
    }

    public class UserPetitionSigners_UserViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public UserPetitionSigners_UserViewModel()
        {
        }

        public UserPetitionSigners_UserViewModel(User user)
        {
            Avatar = ImageService.GetImageUrl<User>(user.Avatar);
            Id = user.Id;
            Name = user.FullName;
        }
    }
}