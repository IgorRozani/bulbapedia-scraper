using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Scrapers.Interfaces
{
    public interface IPokemonListScraper : IBaseScraper
    {
        ICollection<Pokemon> Scrape();
    }
}
