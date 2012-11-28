using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace LJService
{
    public static class LiveJournalService
    {
        public static HttpWebResponse SendXMLCode(XmlDocument doc)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(doc.OuterXml.ToString());
            string requestString = "http://www.livejournal.com/interface/xmlrpc";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestString);
            request.Method = "POST";
            request.UserAgent = "XMLRPC Client 1.0";
            request.Host = "www.livejournal.com";
            request.ContentType = "text/xml";
            request.ContentLength = byteArray.Length;
            request.KeepAlive = false;
            var dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            return (HttpWebResponse)request.GetResponse();
        }

        public static string GetChalenge()
        {
            var query = new XmlDocument();
            var structure = PrepareXMLRPCDocument("getchallenge", query);

            var response = SendXMLCode(query);
            string result = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var dataStream = response.GetResponseStream();

                XmlDocument doc = new XmlDocument();
                doc.Load(dataStream);
                dataStream.Close();

                var resultNodes = doc.GetElementsByTagName("name");
                if (resultNodes.Count > 0)
                {
                    foreach (XmlNode node in resultNodes)
                    {
                        if (node.InnerText == "challenge")
                        {
                            result = node.NextSibling.FirstChild.InnerText;
                            break;
                        }
                    }
                }
            }

            response.Close();
            return result;
        }

        public static bool Post(string username, string password, string title, string text, DateTime date)
        {
            var key = GetChalenge();
            var query = new XmlDocument();
            var structure = PrepareXMLRPCDocument("postevent", query);

            structure.AppendChild(AddStringParam("username", username, query));
            structure.AppendChild(AddStringParam("auth_method", "challenge", query));
            structure.AppendChild(AddStringParam("auth_challenge", key, query));
            structure.AppendChild(AddStringParam("auth_response", EncodePassword(password, key), query));
            structure.AppendChild(AddStringParam("event", text, query));
            structure.AppendChild(AddStringParam("subject", title, query));
            structure.AppendChild(AddStringParam("lineendings", "pc", query));
            structure.AppendChild(AddIntParam("year", date.Year, query));
            structure.AppendChild(AddIntParam("mon", date.Month, query));
            structure.AppendChild(AddIntParam("day", date.Day, query));
            structure.AppendChild(AddIntParam("hour", date.Hour, query));
            structure.AppendChild(AddIntParam("min", date.Minute, query));

            var response = SendXMLCode(query);

            var dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            dataStream.Close();
            bool result = response.StatusCode == HttpStatusCode.OK;
            response.Close();

            //TODO: Анализировать ответ и возвращать

            return result;
        }


        public static List<ExternalContent> GetContentByDate(string username, string password, string userJournal, DateTime date)
        {
            //auth_method=challenge&auth_challenge={0}&auth_response={1}
            var key = GetChalenge();
            var query = new XmlDocument();
            var structure = PrepareXMLRPCDocument("getevents", query);

            structure.AppendChild(AddStringParam("username", username, query));
            structure.AppendChild(AddStringParam("auth_method", "challenge", query));
            structure.AppendChild(AddStringParam("auth_challenge", key, query));
            structure.AppendChild(AddStringParam("auth_response", EncodePassword(password, key), query));
            structure.AppendChild(AddStringParam("lineendings", "pc", query));
            structure.AppendChild(AddStringParam("selecttype", "day", query));
            structure.AppendChild(AddIntParam("year", date.Year, query));
            structure.AppendChild(AddIntParam("month", date.Month, query));
            structure.AppendChild(AddIntParam("day", date.Day, query));


            structure.AppendChild(AddIntParam("ver", 1, query));

            structure.AppendChild(AddStringParam("usejournal", userJournal, query));

            var response = SendXMLCode(query);

            var dataStream = response.GetResponseStream();

            XmlDocument doc = new XmlDocument();
            doc.Load(dataStream);
            List<ExternalContent> resultList = new List<ExternalContent>();
            var namenodes = doc.GetElementsByTagName("data");
            foreach (XmlNode node in namenodes)
            {
                foreach (XmlNode element in node.ChildNodes)
                {
                    if (element.Name == "value")
                    {
                        ExternalContent newElement = new ExternalContent();

                        if (element.FirstChild != null && element.FirstChild.Name == "struct")
                        {
                            var parentDataNode = node.FirstChild.FirstChild;

                            foreach (XmlNode valueNode in parentDataNode.ChildNodes)
                            {
                                if (valueNode.Name == "member" && valueNode.ChildNodes.Count == 2)
                                {
                                    string name = null;
                                    string value = null;

                                    if (valueNode.ChildNodes[0].Name == "name")
                                    {
                                        name = valueNode.ChildNodes[0].InnerText;

                                        if (valueNode.ChildNodes[1].Name == "value" && valueNode.ChildNodes[1].FirstChild != null)
                                        {
                                            if (valueNode.ChildNodes[1].FirstChild.ChildNodes.Count == 1)
                                            {
                                                value = valueNode.ChildNodes[1].FirstChild.InnerText;
                                                newElement.ComposeProperty(name, value);
                                                //Console.WriteLine(name + ": " + value);
                                            }
                                            else if (valueNode.ChildNodes[1].FirstChild.ChildNodes.Count > 1)
                                            {
                                                foreach (XmlNode innerprop in valueNode.ChildNodes[1].FirstChild.ChildNodes)
                                                {
                                                    if (innerprop.ChildNodes[0].Name == "name")
                                                    {
                                                        name = innerprop.ChildNodes[0].InnerText;

                                                        if (innerprop.ChildNodes[1].Name == "value" && innerprop.ChildNodes[1].FirstChild != null)
                                                        {
                                                            value = innerprop.ChildNodes[1].FirstChild.InnerText;
                                                            newElement.ComposeProperty(name, value);
                                                            //Console.WriteLine(name + ": " + value);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        resultList.Add(newElement);
                    }
                }
            }


            dataStream.Close();
            response.Close();

            //TODO: Анализировать ответ и возвращать

            return resultList;
        }

        private static XmlNode AddStringParam(string name, string value, XmlDocument document)
        {
            var member = document.CreateElement("member", null);

            var membername = document.CreateElement("name", null);
            member.AppendChild(membername);
            membername.AppendChild(document.CreateTextNode(name));

            var membervalue = document.CreateElement("value", null);
            member.AppendChild(membervalue);

            var membertype = document.CreateElement("string", null);
            membervalue.AppendChild(membertype);
            membertype.AppendChild(document.CreateTextNode(value));

            return member;
        }

        private static XmlNode AddIntParam(string name, int value, XmlDocument document)
        {
            var member = document.CreateElement("member", null);

            var membername = document.CreateElement("name", null);
            member.AppendChild(membername);
            membername.AppendChild(document.CreateTextNode(name));

            var membervalue = document.CreateElement("value", null);
            member.AppendChild(membervalue);

            var membertype = document.CreateElement("int", null);
            membervalue.AppendChild(membertype);
            membertype.AppendChild(document.CreateTextNode(value.ToString()));

            return member;
        }

        private static XmlNode PrepareXMLRPCDocument(string method, XmlDocument query)
        {
            var doctype = query.CreateNode(XmlNodeType.XmlDeclaration, "version=\"1.0\"", null);
            query.AppendChild(doctype);

            var methodCall = query.CreateElement("methodCall", null);
            query.AppendChild(methodCall);

            var methodName = query.CreateElement("methodName", null);
            methodCall.AppendChild(methodName);
            methodName.AppendChild(query.CreateTextNode("LJ.XMLRPC." + method));

            var parametrs = query.CreateElement("params", null);
            methodCall.AppendChild(parametrs);
            var param = query.CreateElement("param", null);
            parametrs.AppendChild(param);
            var value = query.CreateElement("value", null);
            param.AppendChild(value);
            var structure = query.CreateElement("struct", null);
            value.AppendChild(structure);

            return structure;
        }

        public static string EncodePassword(string password, string chalenge)
        {
            string result = null;

            result = EncodeHelper.ToMD5(chalenge + EncodeHelper.ToMD5(password));

            return result;
        }
    }
}
