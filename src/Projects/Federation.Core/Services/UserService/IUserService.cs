using System;

namespace Federation.Core
{
    public interface IUserService
    {
        void UserOutdating();
        void ProfileChangeRequest(Guid userId, string firstname, string surname, string patronymic, string facebook, string liveJournal, string phoneNumber, string comment);
        void NormalizePhoneNumber(User user, bool saveChanges);
        string NormalizePhoneNumber(string phone);
    }
}