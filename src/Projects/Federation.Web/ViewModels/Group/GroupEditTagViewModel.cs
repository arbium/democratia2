using System.ComponentModel.DataAnnotations;
using Federation.Core;
using System.Linq;
using System;

namespace Federation.Web.ViewModels
{
    public class GroupEditTagViewModel
    {
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public Guid GroupId { get; set; }
        public Guid TagId { get; set; }
        
        [Display(Name = "Заголовок темы группы", Description="Должно состоять из одного слова и только из букв или цифр")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание темы группы")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }

        public GroupEditTagViewModel()
        {
        }

        public GroupEditTagViewModel(Group group, Guid tagId)
        {
            if (group != null)
            {
                GroupName = group.Name;
                GroupUrl = group.Url;
                GroupId = group.Id;
                var tag = group.Tags.SingleOrDefault(x=>x.Id == tagId);
                if (tag == null)
                    throw new BusinessLogicException("В данной группе нет тега с таким идентификатором");
                TagId = tagId;
                Title = tag.Title;
                Description = tag.Description;
            }
        }
    }
}