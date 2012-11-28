using System;

namespace Federation.Core
{    
    public enum AttachDetachTarget : byte { Group = 1, User = 0 }

    public static class ContentService
    {
        private static IContentService _current = new ContentServiceImpl();

        public static Post CreatePost(Guid? groupId, Guid? authorId, string title, string text, string tagsString, bool isDraft, bool isExternal)
        {
            return _current.CreatePost(groupId, authorId, title, text, tagsString, isDraft, isExternal);
        }
        public static Post UpdatePost(Guid postId, Guid? editorId, string newTitle, string newText, string newTagsString, bool isDraft)
        {
            return _current.UpdatePost(postId, editorId, newTitle, newText, newTagsString, isDraft);
        }

        public static Content Attach(Guid contentId, Guid? currUserId, AttachDetachTarget target)
        {
            return _current.Attach(contentId, currUserId, target);
        }
        public static Content Detach(Guid contentId, Guid? currUserId, AttachDetachTarget target)
        {
            return _current.Detach(contentId, currUserId, target);
        }

        public static void ToggleDiscussion(bool show, Guid contentId, Guid userId)
        {
            _current.ToggleDiscussion(show, contentId, userId);
        }
        public static void ToggleDiscussion(bool show, Content content, Guid userId)
        {
            _current.ToggleDiscussion(show, content, userId);
        }

        public static void Delete(Guid contentId, Guid userId)
        {
            _current.Delete(contentId, userId);
        }
        public static void Delete(Content content, Guid userId)
        {
            _current.Delete(content, userId);
        }

        public static void Restore(Guid contentId, Guid userId)
        {
            _current.Restore(contentId, userId);
        }
        public static void Restore(Content content, Guid userId)
        {
            _current.Restore(content, userId);
        }

        public static void ToggleBlock(bool block, Guid contentId, Guid userId)
        {
            _current.ToggleBlock(block, contentId, userId);
        }
        public static void ToggleBlock(bool block, Content content, Guid userId)
        {
            _current.ToggleBlock(block, content, userId);
        }

        public static void Publish(Guid contentId, Guid userId)
        {
            _current.Publish(contentId, userId);
        }
        public static void Publish(Content content, Guid userId)
        {
            _current.Publish(content, userId);
        }

        public static void Unpublish(Guid contentId, Guid userId)
        {
            _current.Unpublish(contentId, userId);
        }
        public static void Unpublish(Content content, Guid userId)
        {
            _current.Unpublish(content, userId);
        }

        public static bool IsAuthor(Guid contentId, Guid userId)
        {
            return _current.IsAuthor(contentId, userId);
        }
        public static bool IsAuthor(Content content, Guid userId)
        {
            return _current.IsAuthor(content, userId);
        }

        public static void ClearOldDeletedContent()
        {
            _current.ClearOldDeletedContent();
        }
    }
}
