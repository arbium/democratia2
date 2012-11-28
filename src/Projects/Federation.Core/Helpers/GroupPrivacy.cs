using System;

namespace Federation.Core
{
    /// <summary>
    /// Настройки приватности
    /// </summary>
    [Flags]
    public enum GroupPrivacy : short
    {
        /// <summary>
        /// Модерация членов группы
        /// </summary>
        MemberModeration = 1,

        /// <summary>
        /// Модерация контента группы
        /// </summary>
        ContentModeration = 2,

        /// <summary>
        /// Группа-невидимка
        /// </summary>
        Invisible = 4,

        /// <summary>
        /// Комментирование только для членов группы
        /// </summary>
        PrivateDiscussion = 8
    }
}