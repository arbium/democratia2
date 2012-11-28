using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditPollViewModel
    {
        private string _cleanText;

        public Guid PollId { get; set; }

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

        [Display(Name = "Длительность, в днях")]
        public short Duration { get; set; }

        public GroupEditPollViewModel()
        {
        }

        public GroupEditPollViewModel(Poll poll)
        {
            var group = poll.Group;

            if (group != null)
            {
                GroupId = group.Id;
                GroupName = group.Name;
                GroupUrl = group.Url;
                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);

                Topics = group.Tags.Where(t => t.TopicState == (byte)TopicState.GroupTopic).Select(x => new TagViewModel(x)).ToList();
                TagTitles = TagsHelper.ConvertTagListToString(poll.Tags.Where(t => t.TopicState != (byte)TopicState.GroupTopic).ToList());
            }

            foreach (var tag in poll.Tags)
            {
                if (tag.TopicState == (byte)TopicState.GroupTopic)
                {
                    var tagVm = Topics.SingleOrDefault(x => x.Id == tag.Id);
                    if (tagVm != null)
                        tagVm.IsChecked = true;
                }
            }

            PollId = poll.Id;
            Title = poll.Title;
            Text = poll.Text;
            IsDraft = poll.State == (byte)ContentState.Draft;

            if (poll.Duration.HasValue)
                Duration = poll.Duration.Value;
        }
    }
}