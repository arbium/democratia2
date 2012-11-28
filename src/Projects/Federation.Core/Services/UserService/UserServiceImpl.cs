using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Federation.Core
{
    public class UserServiceImpl : IUserService
    {
        public void NormalizePhoneNumber(User user, bool saveChanges)
        {
            if (user.PhoneNumber == null)
                return;

            var phone = Regex.Replace(user.PhoneNumber, @"\D*", string.Empty);
            var phoneLength = phone.Length;

            if (phoneLength < 10)
            {
                user.PhoneNumber = null;
                user.IsPhoneVerified = false;
                user.IsForeigner = false;
            }
            else if (phoneLength == 10)
            {
                user.PhoneNumber = '7' + phone;
                user.IsForeigner = false;
            }
            else if (phoneLength == 11)
            {
                var codes2 = new[] { "81", "82", "84", "86" };
                var codes3 = new[] { "800", "808", "850", "852", "853", "855", "856", "870", "878", "880", "881", "882", "886" };

                if (phone[0] == '7')
                {
                    user.PhoneNumber = phone;
                    user.IsForeigner = false;
                }
                else if (phone[0] == '8' && !codes2.Contains(phone.Substring(0, 2)) && !codes3.Contains(phone.Substring(0, 3)))
                {
                    user.PhoneNumber = '7' + phone.Remove(0, 1);
                    user.IsForeigner = false;
                }
                else
                {                    
                    user.PhoneNumber = phone;
                    user.IsForeigner = true;
                }
            }
            else
            {                
                user.PhoneNumber = phone;
                user.IsForeigner = true;
            }

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public string NormalizePhoneNumber(string phone)
        {
            if (phone == null)
                return null;

            phone = Regex.Replace(phone, @"\D*", string.Empty);
            var phoneLength = phone.Length;

            if (phoneLength < 10)
                return null;

            if (phoneLength == 10)
                return '7' + phone;

            if (phoneLength == 11)
            {
                var codes2 = new[] { "81", "82", "84", "86" };
                var codes3 = new[] { "800", "808", "850", "852", "853", "855", "856", "870", "878", "880", "881", "882", "886" };

                if (phone[0] == '8' && !codes2.Contains(phone.Substring(0, 2)) && !codes3.Contains(phone.Substring(0, 3)))
                    return '7' + phone.Remove(0, 1);
                
                return phone;
            }

            return phone;
        }

        public void ProfileChangeRequest(Guid userId, string firstname, string surname, string patronymic, string facebook, string liveJournal, string phoneNumber, string comment)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Пользователь не найден!");

            var request = new ProfileChangeRequest
            {
                UserId = userId,
                Firstname = firstname,
                Surname = surname,
                Patronymic = patronymic,
                Facebook = facebook,
                LiveJournal = liveJournal,
                PhoneNumber = phoneNumber,
                Comment = comment,
                Date = DateTime.Now
            };
            DataService.PerThread.ProfileChangeRequestSet.AddObject(request);
            DataService.PerThread.SaveChanges();
        }

        public void UserOutdating()
        {
            var date = DateTime.Now.Add(-ConstHelper.UserOutdatedTime);
            foreach (var user in DataService.PerThread.BaseUserSet.OfType<User>().Where(x => x.LastActivity < date))
                user.IsOutdated = true;
            DataService.PerThread.SaveChanges();
        }
    }
}