namespace Federation.Core
{
    public struct ContentTypeFilter
    {
        public bool Posts;
        public bool Polls;
        public bool Petitions;
        public bool Elections;
    }    

    public struct Personality
    {
        public bool User;
        public bool Group;
    }

    public static class ContentFilterExtension
    {
        public static bool IsSomethingChecked(this ContentTypeFilter contentType)
        {
            if (contentType.Elections || contentType.Petitions || contentType.Polls || contentType.Posts)
                return true;

            return false;
        }

        public static bool IsSomethingChecked(this Personality personality)
        {
            if (personality.Group || personality.User)
                return true;

            return false;
        }
    }
}