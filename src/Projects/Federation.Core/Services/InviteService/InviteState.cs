namespace Federation.Core
{
    public enum InviteState: byte
    {
        /// <summary>
        /// Приглашение выслано
        /// </summary>
        Sent,

        /// <summary>
        /// Приглашение не выслано
        /// </summary>
        NotSent,

        /// <summary>
        /// Пользователь зарегистрирован
        /// </summary>
        Used,

        /// <summary>
        /// Запрос приглашения
        /// </summary>
        Requested,

        /// <summary>
        /// Приглашение заблокировано
        /// </summary>
        Blocked
    }
}
