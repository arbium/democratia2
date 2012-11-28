using System;

namespace Federation.Core
{
    public interface ICachedViewModel
    {
        string CachKey { get; }
        string CachLabel { get; set; }
        Guid CachUserId { get; }
        Guid RelativeModelId { get; }
        TimeSpan CachDropPeriod { get; }
        bool CachUserRelated { get; }
        DateTime CachLastAccess { get; set; }

        ICachedViewModel Initialize();
        bool CachIsOutOfDate();
    }
}
