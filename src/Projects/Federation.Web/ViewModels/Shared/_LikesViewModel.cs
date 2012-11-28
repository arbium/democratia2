using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{    
    public class _LikesViewModel
    {
        public Guid TargetId { get; set; }
        public byte TargetType { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public bool? Vote { get; set; }
        public bool IsAuthor { get; set; }

        public _LikesViewModel(Content content, Guid? userId)
        {
            if (content != null)
            {
                TargetId = content.Id;
                TargetType = (byte)WtfLikes.Content;

                if (content.AuthorId.HasValue)
                    IsAuthor = content.AuthorId.Value == userId;

                var likes = content.Likes;

                LikesCount = likes.Count(l => l.Value);
                DislikesCount = likes.Count(l => l.Value == false);

                var like = likes.SingleOrDefault(l => l.ContentId == content.Id && l.User.Id == userId);

                if (like != null)
                    Vote = like.Value;
            }
        }

        public _LikesViewModel(Guid targetId, Guid? userId, WtfLikes targetType)
        {
            Like like = null;
            ICollection<Like> likes = null;

            switch (targetType)
            {
                case WtfLikes.AlbumItem:
                    var albumItem = DataService.PerThread.AlbumItemSet.SingleOrDefault(x => x.Id == targetId);
                    if (albumItem == null)
                        return;

                    if (albumItem.Album.UserId.HasValue)
                        IsAuthor = albumItem.Album.UserId.Value == userId;

                    likes = albumItem.Likes;                    
                    like = likes.SingleOrDefault(l => l.AlbumItemId == targetId && l.User.Id == userId);
                    break;

                case WtfLikes.Content:
                    var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == targetId);
                    if (content == null)
                        return;

                    if (content.AuthorId.HasValue)
                        IsAuthor = content.AuthorId.Value == userId;

                    likes = content.Likes;
                    like = likes.SingleOrDefault(l => l.ContentId == targetId && l.User.Id == userId);
                    break;

                case WtfLikes.Comment:
                    var comment = DataService.PerThread.CommentSet.SingleOrDefault(x => x.Id == targetId);
                    if (comment == null)
                        return;

                    IsAuthor = comment.UserId == userId;

                    likes = comment.Likes;
                    like = likes.SingleOrDefault(l => l.CommentId == targetId && l.User.Id == userId);
                    break;
            }

            if (likes != null)
            {
                LikesCount = likes.Count(l => l.Value);
                DislikesCount = likes.Count(l => l.Value == false);
            }

            TargetId = targetId;
            TargetType = (byte)targetType;

            if (like != null)
                Vote = like.Value;
        }
    }
}