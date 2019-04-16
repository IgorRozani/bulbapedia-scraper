using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Interfaces
{

    public interface IListScraper : IBaseScraper
    {
        void Scrape(string url, ICollection<Pokemon> pokemonList);
    }
}
