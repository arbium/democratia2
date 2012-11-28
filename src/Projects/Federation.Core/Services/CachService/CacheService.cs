using System;

namespace Federation.Core
{
    public static class CachService
    {
        private static ICachService _current = ServiceLocator.Current.GetNewInstance<ICachService>();

        public static ICachedViewModel GetViewModel(ICachedViewModel startModel)
        {
            return _current.GetViewModel(startModel);
        }

        public static void DropViewModel(string key)
        {
            _current.DropViewModel(key);
        }

        public static void DropViewModelByModel(Guid modelId)
        {
            _current.DropViewModelByModel(modelId);
        }

        public static void AddNewViewModel(ICachedViewModel startModel)
        {
            _current.AddNewViewModel(startModel);
        }

        public static bool IsCachOutOfDate(string key)
        {
            return _current.IsCachOutOfDate(key);
        }

        public static void ClearOldCach()
        {
            _current.ClearOldCach();
        }
    }
}
