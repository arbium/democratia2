using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditCandidatePetitionViewModel
    {
        private string _cleanText;

        public Guid CandidateId { get; set; }
        public bool IsConfirmed { get; set; }

        [Display(Name = "Текст петиции")]
        [Required(ErrorMessage = "Обязательное поле")]
        [AllowHtml]
        public string Text
        {
            get { return _cleanText; }
            set { _cleanText = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }

        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }

        public GroupEditCandidatePetitionViewModel()
        {
        }

        public GroupEditCandidatePetitionViewModel(Candidate candidate)
        {
            if (candidate != null)
            {
                CandidateId = candidate.Id;
                IsConfirmed = (CandidateStatus)candidate.Status == CandidateStatus.Confirmed;
                if (candidate.Petition != null)
                    Text = candidate.Petition.Text;
                GroupId = candidate.GroupMember.GroupId;
                GroupName = candidate.GroupMember.Group.Name;
                GroupUrl = candidate.GroupMember.Group.Url;
            }
        }
    }
}
