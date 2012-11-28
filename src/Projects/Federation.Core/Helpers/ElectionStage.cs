namespace Federation.Core
{
    public enum ElectionStage : byte
    {
        /// <summary>
        /// Агитация
        /// </summary>
        Agitation = 0,

        /// <summary>
        /// Голосование
        /// </summary>
        Voting = 1,

        /// <summary>
        /// Завершены
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Провалены
        /// </summary>
        Failed = 3
    }
}