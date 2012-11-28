using System.Linq;
using System.Text;

namespace Federation.Core
{
    public class ErrorServiceImpl : IErrorService
    {//TODO: Покрыть тестами и дать нормальные имена переменным
        public string Add(string tmp)
        {
            uint num = (uint)tmp.GetHashCode();
            StringBuilder stringBuilder = new StringBuilder();

            while (num >= 26)
            {
                uint modulo = num % 26;
                char tmpChar = (char)('a' + modulo);
                stringBuilder.Append(tmpChar);

                num = (uint)(num / 26);//отброс дробной части
            }

            string f = stringBuilder.ToString();
            string f2 = f;

            int i = 0;
            ErrorText errorText = DataService.PerThread.ErrorTextSet.Where(x => x.Key == f2).SingleOrDefault();
            while (errorText != null)
            {
                if (errorText.Text == tmp)
                    return f2;

                f2 = f + i.ToString();
                i++;
            }

            ErrorText addingErrorText = new ErrorText() {Key = f2, Text = tmp};
            DataService.PerThread.ErrorTextSet.AddObject(addingErrorText);
            DataService.PerThread.SaveChanges();
            return f2;
        }

        public string Get(string key)
        {
            ErrorText errorText = DataService.PerThread.ErrorTextSet.Where(x => x.Key == key).SingleOrDefault();
            if (errorText == null)
                return "";
            return errorText.Text;
        }
    }
}
