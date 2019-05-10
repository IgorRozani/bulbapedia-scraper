using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Services.Scrapers.Interfaces
{
    public interface IListScraper : IBaseScraper
    {
        void Scrape(ICollection<Pokemon> pokemonList);
    }
}
