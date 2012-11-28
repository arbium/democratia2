using System.Net;
using Federation.Core;

namespace Federation.ControllerExecuter
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = ConstHelper.AppUrl + args[0].ToString();

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            httpWebResponse.Close();
        }
    }
}