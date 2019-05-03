using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Scrapers.MoveList
{
    public class MoveList : BaseScraper, IListScraper
    {
        public MoveList(HtmlAgilityPack.HtmlWeb htmlWeb) : base(htmlWeb)
        {
        }

        public string GetName() => "Move list";

        public void Scrape(string url, ICollection<Pokemon> pokemonList)
        {
            var htmlPage = _htmlWeb.Load(url);

            var evolutionTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");
            foreach (var evolutionTable in evolutionTables)
            {
                if (evolutionTable.SelectSingleNode("tr/td/table") == null)
                    continue;


            }
        }
    }
}
