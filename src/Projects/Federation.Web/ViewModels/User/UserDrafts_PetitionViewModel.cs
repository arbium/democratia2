using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDrafts_PetitionViewModel
    {
        public Guid Id { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string Controller { get; set; }
        public string Privacy { get; set; }
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }        
        public IList<TagViewModel> Tags { get; set; }

        public UserDrafts_PetitionViewModel()
        {
        }

        public UserDrafts_PetitionViewModel(Petition petition)
        {
            if (petition != null)
            {
                Id = petition.Id;
                AuthorId = petition.AuthorId;
                GroupId = petition.GroupId;

                if (petition.GroupId.HasValue)
                {
                    GroupName = petition.Group.Name;
                    GroupUrl = petition.Group.Url;

                    if (petition.IsPrivate)
                        Privacy = "только для членов группы";
                }

                Controller = petition.Controller;
                CreationDate = petition.CreationDate;
                Title = petition.Title;                
                Tags = petition.Tags.Select(x => new TagViewModel(x)).ToList();

                Summary = TextHelper.CleanTags(petition.Text);
                if (Summary.Length > ConstHelper.MiniSummaryLength)
                    Summary = Summary.Substring(0, ConstHelper.MiniSummaryLength) + "…";
            }
        }
    }
}