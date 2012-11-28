namespace Federation.Core
{
    public enum ContentState : byte
    {
        /// <summary>
        /// Ожидает модерации
        /// </summary>
        Premoderated = 0,

        /// <summary>
        /// Одобрено
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 2,

        /// <summary>
        /// Заблокировано
        /// </summary>
        Blocked = 3,

        /// <summary>
        /// Удалено
        /// </summary>
        Deleted = 4
    }
}