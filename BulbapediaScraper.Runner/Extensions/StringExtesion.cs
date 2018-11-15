namespace BulbapediaScraper.Runner.Extensions
{
    public static class StringExtesion
    {
        public static string RemoveSpecialCharacter(this string value) =>
            value.Replace('\n', ' ').Replace("→", "").Replace("#", "").Trim();

    }
}
