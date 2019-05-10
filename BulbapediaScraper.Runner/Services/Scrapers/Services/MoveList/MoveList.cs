using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Services.Scrapers.Interfaces;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Services.Scrapers.Services.MoveList
{
    public class MoveList : BaseScraper, IMoveList
    {
        public MoveList(HtmlAgilityPack.HtmlWeb htmlWeb, BulbapediaConfiguration bulbapediaConfiguration) : base(htmlWeb, bulbapediaConfiguration)
        {
        }

        public string GetName() => "Move list";

        public void Scrape(ICollection<Pokemon> pokemonList)
        {
            var htmlPage = _htmlWeb.Load(GetSiteFullPath(_bulbapediaConfiguration.MovesListPath));

            var evolutionTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");
            foreach (var evolutionTable in evolutionTables)
            {
                if (evolutionTable.SelectSingleNode("tr/td/table") == null)
                    continue;


            }
        }
    }
}
