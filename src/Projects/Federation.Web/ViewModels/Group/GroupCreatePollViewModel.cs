using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupCreatePollViewModel
    {
        private string _cleanText = string.Empty;

        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public bool IsContentModeration { get; set; }

        [Display(Name = "Заголовок вопроса")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание вопроса вынесенного на голосование")]
        [Required(ErrorMessage = "Обязательное поле")]
        [AllowHtml]
        public string Text
        {
            get { return _cleanText; }
            set { _cleanText = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }

        [Display(Name = "Дополнительные темы")]
        public string TagTitles { get; set; }

        [Display(Name = "Основные темы")]
        public IList<TagViewModel> Topics { get; set; }

        [Display(Name = "Черновик")]
        public bool IsDraft { get; set; }

        [Display(Name = "Открытое голосование")]
        public bool HasOpenProtocol { get; set; }

        [Display(Name = "Длительность, дней")]
        public short Duration { get; set; }

        public GroupCreatePollViewModel()
        {
            Duration = 7;
        }

        public GroupCreatePollViewModel(Group group)
        {
            Duration = 7;

            if (group != null)
            {
                GroupId = group.Id;
                GroupName = group.Name;
                GroupUrl = group.Url;
                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);

                if (GroupId == new Guid(ConstHelper.KsGroupId))
                {
                    HasOpenProtocol = true;
                }

                Topics = group.Tags.Where(t => t.TopicState == (byte)TopicState.GroupTopic).Select(x => new TagViewModel(x)).ToList();
            }
            else
            {
                Topics = new List<TagViewModel>();
            }
        }
    }
}