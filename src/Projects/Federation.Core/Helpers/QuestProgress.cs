using System;

namespace Federation.Core
{
    /// <summary>
    /// Прохождение квеста
    /// </summary>
    [Flags]
    public enum QuestProgress : short
    {
        /// <summary>
        /// Нихера не пройдено
        /// </summary>
        None = 0,

        /// <summary>
        /// Установка фотографии на аватар
        /// </summary>
        Avatar = 1,

        /// <summary>
        /// Заполнение информации о себе
        /// </summary>
        Info = 2,

        /// <summary>
        /// Вступление в 3 интересных сообщества (группы)
        /// </summary>
        GroupsJoin = 4,

        /// <summary>
        /// Участие в голосованиях
        /// </summary>
        Voting = 8,

        /// <summary>
        /// Делегирование или объявление себя экспертом
        /// </summary>
        Delegating = 16,

        /// <summary>
        /// Написание комментария
        /// </summary>
        Commenting = 32,

        /// <summary>
        /// Завершено
        /// </summary>
        Completed = 64
    }

    public static class QuestProgressExtension
    {
        public static byte Percents(this QuestProgress questProgress)
        {
            const int questStagesCount = 6;
            var questCompletedStages = 0;

            if (questProgress.HasFlag(QuestProgress.Avatar))
                questCompletedStages++;
            if (questProgress.HasFlag(QuestProgress.Commenting))
                questCompletedStages++;
            if (questProgress.HasFlag(QuestProgress.Delegating))
                questCompletedStages++;
            if (questProgress.HasFlag(QuestProgress.GroupsJoin))
                questCompletedStages++;
            if (questProgress.HasFlag(QuestProgress.Info))
                questCompletedStages++;
            if (questProgress.HasFlag(QuestProgress.Voting))
                questCompletedStages++;

            if (questCompletedStages == 0)
                return 0;

            return (byte)(100 * questCompletedStages / questStagesCount);            
        }
    }
}