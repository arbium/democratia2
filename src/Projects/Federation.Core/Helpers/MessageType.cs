namespace Federation.Core
{
    public enum MessageType : byte
    {
        /// <summary>
        /// Системное сообщение
        /// </summary>
        SystemMessage = 0,

        /// <summary>
        /// Личное сообщение
        /// </summary>
        PrivateMessage = 1,

        /// <summary>
        /// Уведомление касаемо комментариев
        /// </summary>
        CommentNotice = 2,

        /// <summary>
        /// Уведомление члену группы
        /// </summary>
        GroupMemberNotice = 8,

        /// <summary>
        /// Уведомление модератору группы
        /// </summary>
        GroupModeratorNotice = 3,

        /// <summary>
        /// Уведомление касаемо петиций
        /// </summary>
        PetitionNotice = 4,

        /// <summary>
        /// Уведомление касаемо голосований
        /// </summary>
        PollNotice = 5,

        /// <summary>
        /// Уведомление касаемо выборов
        /// </summary>
        ElectionNotice = 6,

        /// <summary>
        /// Уведомление касаемо Опросов
        /// </summary>
        SurveyNotice = 7
    }
}
