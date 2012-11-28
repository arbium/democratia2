namespace Federation.Core
{
    /// <summary>
    /// Категория группы
    /// </summary>
    public enum GroupType : byte
    {
        /// <summary>
        /// Не указана
        /// </summary>
        None = 0,

        /// <summary>
        /// Корневая группа
        /// </summary>
        Root = 1,

        /// <summary>
        /// Разработка
        /// </summary>
        Development = 2,

        /// <summary>
        /// СМИ
        /// </summary>
        Media = 3
    }
}