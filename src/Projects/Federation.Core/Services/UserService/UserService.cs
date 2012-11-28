using System;

namespace Federation.Core
{
    public static class UserService
    {
        private static IUserService _current = new UserServiceImpl();

        public static void NormalizePhoneNumber(User user, bool saveChanges = false)
        {
            _current.NormalizePhoneNumber(user, saveChanges);
        }

        public static string NormalizePhoneNumber(string phone)
        {
            return _current.NormalizePhoneNumber(phone);
        }

        public static void ProfileChangeRequest(Guid userId, string firstname, string surname, string patronymic, string facebook, string liveJournal, string phoneNumber, string comment)
        {
            _current.ProfileChangeRequest(userId, firstname, surname, patronymic, facebook, liveJournal, phoneNumber, comment);
        }

        public static void UserOutdating()
        {
            _current.UserOutdating();
        }
    }
}