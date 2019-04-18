namespace BulbapediaScraper.Runner.Helpers
{
    public class UrlHelper
    {
        private const string BASE_IMAGE_URL = "https://";
        private const string BASE_URL = "https://bulbapedia.bulbagarden.net/w/index.php?title=";

        public static string GetImageFullPath(string imagePath) =>
            BASE_IMAGE_URL + imagePath;

        public static string GetFullPath(string path) =>
            BASE_URL + path;
    }
}
