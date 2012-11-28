namespace Federation.Core
{
    //Мы считаем что ошибки(критические и некритические) - часть выволнения программы
    public enum JobState : byte
    {
        ReadyToStart,
        ReadyToRecover,
        Started,
        Ended,
        Error,
        Aborted
    }
}
