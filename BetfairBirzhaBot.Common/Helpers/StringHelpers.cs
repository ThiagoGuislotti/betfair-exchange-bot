namespace BetfairBirzhaBot.Common.Helpers
{
    public static class StringHelpers
    {
        public static string ToUpperFirstLetters(this string text, string separator = "-")
        {
            string[] words = text.Split(separator);
            return string.Join(separator, words.Select(x => char.ToUpper(x[0]) + x.Substring(1)));
        }


        public static string GetLeagueFromUrl(this string url)
        {
            string[] urlData = url.Split('/');

            return urlData[7].Replace("-", " ");
        }


        public static string GetSeparatorFromTitle(this string title)
        {
            var separators = new List<string>
            {
                " v ",
                " vs ",
                " - "
            };

            return separators.Find(x => separators.Contains(x));
        }
    }
}
