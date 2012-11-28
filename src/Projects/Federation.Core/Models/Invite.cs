using System;

namespace Federation.Core
{
    public partial class Invite
    {
        public string FullName
        {
            get
            {
                string username = string.Empty;

                if (!string.IsNullOrEmpty(Surname))
                    username += Surname + " ";
                if (!string.IsNullOrEmpty(Name))
                    username += Name + " ";
                if (!string.IsNullOrEmpty(Patronymic))
                    username += Patronymic;

                if (string.IsNullOrWhiteSpace(username))
                    username = Id.ToString();

                return username;
            }
        }

        public Invite()
        {
            Id = Guid.NewGuid();
        }

        public string GetStateSting()
        {
            switch ((InviteState)State)
            {
                case InviteState.NotSent: return "Приглашение не выслано";
                case InviteState.Requested: return "Запрос приглашения";
                case InviteState.Sent: return "Приглашение выслано";
                case InviteState.Used: return "Пользователь зарегистрирован";
                case InviteState.Blocked: return "Приглашение заблокировано";
            }

            return "Ошибка";
        }
    }
}