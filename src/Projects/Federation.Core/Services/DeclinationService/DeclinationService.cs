namespace Federation.Core
{
    public static class DeclinationService
    {
        public static string OfNumber(int number, string titleOf1, string titleOf3, string titleOf5)
        {
            string[] titles = { titleOf1, titleOf3, titleOf5 };
            int[] cases = { 2, 0, 1, 1, 1, 2 };

            return number + " " +  titles[(number % 100 > 4 && number % 100 < 20) ? 2 : cases[(number % 10 < 5) ? number % 10 : 5]];
        }
    }
}