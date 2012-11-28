using System;

namespace Federation.Core
{
    public interface ICachService
    {
        ICachedViewModel GetViewModel(ICachedViewModel strartModel);
        void DropViewModel(string key);
        void DropViewModelByModel(Guid modelId);
        void AddNewViewModel(ICachedViewModel startModel);
        bool IsCachOutOfDate(string key);
        void ClearOldCach();
    }
}
