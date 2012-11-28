using System;

namespace Federation.Core
{
    public interface IContentService
    {
        Content Attach(Guid contentId, Guid? currUserId, AttachDetachTarget target);
        Content Detach(Guid contentId, Guid? currUserId, AttachDetachTarget target);
        Post CreatePost(Guid? groupId, Guid? authorId, string title, string text, string tagsString, bool isDraft, bool isExternal);
        Post UpdatePost(Guid postId, Guid? editorId, string newTitle, string newText, string newTagsString, bool isDraft);
        void ToggleDiscussion(bool show, Guid contentId, Guid userId);        
        void ToggleDiscussion(bool show, Content content, Guid userId);        
        void Delete(Guid contentId, Guid userId);        
        void Delete(Content content, Guid userId);        
        void Restore(Guid contentId, Guid userId);        
        void Restore(Content content, Guid userId);
        void ToggleBlock(bool block, Guid contentId, Guid userId);        
        void ToggleBlock(bool block, Content content, Guid userId);
        Content Publish(Guid contentId, Guid userId);        
        void Publish(Content content, Guid userId);        
        void Unpublish(Guid contentId, Guid userId);
        void Unpublish(Content content, Guid userId);        
        bool IsAuthor(Guid contentId, Guid userId);        
        bool IsAuthor(Content content, Guid userId);
        void ClearOldDeletedContent();
    }
}