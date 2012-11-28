using System.Net;
using System.Xml;
using Federation.Core;

namespace Federation.Web.Services
{
    // Bing API 2.0 code sample demonstrating the use of the
    // Image SourceType over the XML Protocol.
    static class BingSearchService
    {
        // Replace the following string with the AppId you received from the
        // Bing Developer Center.
        const string AppId = "8EE9425DFAF2A7FADEE0495476DC2C3675348934";

        public static XmlDocument SearchImage(string query)
        {
            HttpWebRequest request = BuildRequest(query);
            XmlDocument result;
            try
            {
                // Send the request; display the response.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = TranslateResponseToXml(response);
            }
            catch (WebException ex)
            {
                throw new BusinessLogicException("Ошибка: " + ex.Message + " внутренняя: " + ex.InnerException);
            }

            return result;
        }

        static HttpWebRequest BuildRequest(string query)
        {
            string requestString = "http://api.bing.net/xml.aspx?"

                // Common request fields (required)
                + "AppId=" + AppId
                + "&Query=" + query
                + "&Sources=Image"

                // Common request fields (optional)
                + "&Version=2.0"
                + "&Market=ru-ru"
                + "&Adult=Strict"

                // Image-specific request fields (optional)
                + "&Image.Count=10"
                + "&Image.Offset=0";

            // Create and initialize the request.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                requestString);

            return request;
        }

        static XmlDocument TranslateResponseToXml(HttpWebResponse response)
        {
            // Load the response into an XmlDocument.
            XmlDocument document = new XmlDocument();
            document.Load(response.GetResponseStream());

            // Add the default namespace to the namespace manager.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("api", "http://schemas.microsoft.com/LiveSearch/2008/04/XML/element");

            //XmlNodeList errors = document.DocumentElement.SelectNodes(
            //    "./api:Errors/api:Error",
            //    nsmgr);

            //if (errors.Count > 0)
            //{
            //    // There are errors in the response. Display error details.
            //    DisplayErrors(errors);
            //}
            //else
            //{
            //    // There were no errors in the response. Display the
            //    // Image results.
            //    DisplayResults(document.DocumentElement, nsmgr);
            //}

            return document;
        }
    }
}