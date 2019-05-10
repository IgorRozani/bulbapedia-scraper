namespace BulbapediaScraper.Runner.Services.ScriptGenerator.Helper
{
    public static class ScriptHelper
    {
        public static string GetNodeId(string name) =>
            name
                .Replace("!", ((int)'!').ToString())
                .Replace("?", ((int)'?').ToString())
                .Replace("♀", "F")
                .Replace("♂", "M")
                .Replace("é","e")
                .Replace("-", string.Empty)
                .Replace("'", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("%", string.Empty)
                .Replace(".", string.Empty)
                .Replace(":", string.Empty);
    }
}
