namespace Federation.Core
{
    public enum GroupState : byte
    {
        /// <summary>
        /// Несформированная группа - заготовка группы, у которой совет модераторов еще не набрался
        /// </summary>
        Blank = 0,

        /// <summary>
        /// Группа на учредительном этапе - действующая группа, не набравшая достаточного числа членов
        /// </summary>
        Founding = 1,

        /// <summary>
        /// Действующая группа, набравшая достаточное число членов
        /// </summary>
        Main = 2,

        /// <summary>
        /// Архивная группа
        /// </summary>
        Archive = 3
    }
}