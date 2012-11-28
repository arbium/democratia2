using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class EditCommentViewModel
    {
        public Guid? GroupId { get; set; }
        public Guid? AuthorId { get; set; }
        public string ReturnUrl { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [AllowHtml]
        public string Text { get; set; }        

        public EditCommentViewModel()
        {
        }

        public EditCommentViewModel(Comment comment)
        {
            if (comment != null)
            {
                GroupId = comment.Content.GroupId;
                AuthorId = comment.Content.AuthorId;
                ReturnUrl = comment.Content.GetUrl() + "#" + comment.Id;
                Id = comment.Id;
                Text = comment.Text;
            }
        }
    }
}