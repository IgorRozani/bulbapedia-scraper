using BulbapediaScraper.Runner.Configurations;
using HtmlAgilityPack;

namespace BulbapediaScraper.Runner.Scrapers
{
    public class BaseScraper
    {
        protected HtmlWeb _htmlWeb;
        protected BulbapediaConfiguration _bulbapediaConfiguration;

        public BaseScraper(HtmlWeb htmlWeb, BulbapediaConfiguration bulbapediaConfiguration)
        {
            _htmlWeb = htmlWeb;
            _bulbapediaConfiguration = bulbapediaConfiguration;
        }

        protected string GetImageFullPath(string imagePath) =>
           _bulbapediaConfiguration.BaseImageUrl + imagePath;

        protected string GetSiteFullPath(string path) =>
            _bulbapediaConfiguration.BaseUrl + path;
    }
}
