namespace Federation.Web
{
    public enum ContentViewType
    {
        UserPost,
        GroupPost,
        UserPetition,
        GroupPetition,
        Poll,
        Election,
        Survey
    }

    public static class ContentViewTypeExtension
    {
        public static string GetTitle(this ContentViewType cvt)
        {
            var result = string.Empty;

            switch (cvt)
            {
                case ContentViewType.UserPost:
                    result = "личный пост";
                    break;

                case ContentViewType.GroupPost:
                    result = "групповой пост";
                    break;

                case ContentViewType.UserPetition:
                    result = "личная петиция";
                    break;

                case ContentViewType.GroupPetition:
                    result = "групповая петиция";
                    break;

                case ContentViewType.Poll:
                    result = "голосование";
                    break;

                case ContentViewType.Election:
                    result = "выборы";
                    break;

                case ContentViewType.Survey:
                    result = "опрос";
                    break;
            }

            return result;
        }
    }
}