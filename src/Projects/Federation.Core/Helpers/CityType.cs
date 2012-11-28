namespace Federation.Core
{
    /// <summary>
    /// Тип населенного пункта
    /// </summary>
    public enum CityType : byte
    {
        /// <summary>
        /// Город (г.)
        /// </summary>
        City = 0,

        /// <summary>
        /// Поселок (п.)
        /// </summary>
        Settlement = 1,

        /// <summary>
        /// Село (с.)
        /// </summary>
        Village = 2
    }
}