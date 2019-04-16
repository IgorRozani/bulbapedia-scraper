using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Interfaces
{
    public interface IPokemonListScraper : IBaseScraper
    {
        ICollection<Pokemon> Scrape(string url);
    }
}
