
using System.Text.RegularExpressions;

namespace Federation.Core
{
    public static class ValidationHelper
    {
        public static bool IsCorrectEmail(string checkString)
        {
            Regex regex = new Regex(ConstHelper.EmailRegexp);
            return regex.IsMatch(checkString);
        }

        public static bool IsCorrectLogin(string checkString)
        {
            return true;
        }

        public static bool IsCorrectPassword(string checkString)
        {
            return true;
        }

        //public static bool GetPasswordStrong()
        //{
            
        //}
    }
}
