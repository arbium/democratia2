using System;

namespace LJService
{
    public class ExternalContent
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime PublishDate { get; set; }
        public string ExternalLink { get; set; }
        public string Tags { get; set; }

        public void ComposeProperty(string name, string value)
        {
            switch (name)
            {
                case "subject": Title = EncodeHelper.Base64Decode(value); break;
                case "event": Body = EncodeHelper.Base64Decode(value); break;
                case "eventtime": PublishDate = DateTime.Parse(value); break;
                case "taglist": Tags = EncodeHelper.Base64Decode(value); break;
                case "url": ExternalLink = value; break;
            }
        }
    }
}
