using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDraftsViewModel
    {        
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }

        public readonly IList<UserDrafts_ContentViewModel> Posts = new List<UserDrafts_ContentViewModel>();
        public readonly IList<UserDrafts_ContentViewModel> Surveys = new List<UserDrafts_ContentViewModel>();
        public readonly IList<UserDrafts_PetitionViewModel> Petitions = new List<UserDrafts_PetitionViewModel>();
        public readonly IList<UserDrafts_PetitionViewModel> CoauthorPetitions = new List<UserDrafts_PetitionViewModel>();

        public UserDraftsViewModel()
        {
        }

        public UserDraftsViewModel(User user)
        {
            if (user != null)
            {
                var drafts = DataService.PerThread.ContentSet.Where(p => p.AuthorId == user.Id && p.State == (byte)ContentState.Draft);

                AuthorId = user.Id;
                AuthorName = user.FullName;

                IList<Post> posts = drafts.OfType<Post>().ToList();
                IList<Survey> surveys = drafts.OfType<Survey>().ToList();
                IList<Petition> petitions = drafts.OfType<Petition>().ToList();

                Posts = posts.Select(x => new UserDrafts_ContentViewModel(x)).ToList();
                Surveys = surveys.Select(x => new UserDrafts_ContentViewModel(x)).ToList();
                Petitions = petitions.Select(x => new UserDrafts_PetitionViewModel(x)).ToList();
                CoauthorPetitions = new List<UserDrafts_PetitionViewModel>();

                IList<Coauthor> coauthors = DataService.PerThread.CoauthorSet
                    .Where(c => c.UserId == user.Id && c.IsAccepted.HasValue).Where(c => c.IsAccepted.Value).ToList();

                foreach (var coauthor in coauthors)
                    if (coauthor.Petition.State == (byte)ContentState.Draft)
                        CoauthorPetitions.Add(new UserDrafts_PetitionViewModel(coauthor.Petition));
            }
        }
    }
}
