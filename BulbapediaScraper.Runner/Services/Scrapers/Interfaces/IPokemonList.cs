using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Services.Scrapers.Interfaces
{
    public interface IPokemonList : IBaseScraper
    {
        ICollection<Pokemon> Scrape();
    }
}
