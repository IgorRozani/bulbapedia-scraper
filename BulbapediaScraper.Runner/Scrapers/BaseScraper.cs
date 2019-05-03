using HtmlAgilityPack;

namespace BulbapediaScraper.Runner.Scrapers
{
    public class BaseScraper
    {
        protected HtmlWeb _htmlWeb;

        public BaseScraper(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }
    }
}
