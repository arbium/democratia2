namespace Federation.Core
{
    public enum CandidateStatus : byte
    {
        /// <summary>
        /// Заявлен
        /// </summary>
        Declared = 0,

        /// <summary>
        /// Подтвержден
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Победитель
        /// </summary>
        Winner = 2
    }
}