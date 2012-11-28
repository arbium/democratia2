namespace Federation.Core
{
    public static class ValueAnalizer
    {
        public static bool? GetGenderFromString(string textgender)
        {
            if (textgender != null)
            {
                textgender = textgender.ToLower().Trim();
                if (textgender == "male")
                    return true;
                if (textgender == "female")
                    return false;
            }

            return null;
        }
    }
}