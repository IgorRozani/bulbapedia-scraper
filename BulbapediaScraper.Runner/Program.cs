using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Scrapers.EvolutionList;
using BulbapediaScraper.Runner.Scrapers.MegaEvolutionList;
using BulbapediaScraper.Runner.Scrapers.PokemonList;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner
{
    class Program
    {
        private const string POKEMON_LIST_PATH = "List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number";
        private const string POKEMON_EVOLUTION_LIST_PATH = "List_of_Pokémon_by_evolution_family";
        private const string POKEMON_FORMS_LIST_PATH = "List_of_Pokémon_with_form_differences";
        private const string POKEMON_MEGA_EVOLUTION_LIST_PATH = "Mega_Evolution";

        private static HtmlWeb _htmlWeb;

        static void Main(string[] args)
        {
            Console.WriteLine("BulbapediaScraper was initialized");
            Console.WriteLine();

            var htmlWeb = new HtmlWeb();


            IPokemonListScraper pokemonListScraper = new PokemonList(htmlWeb);

            Console.WriteLine("Scraping: {0}", pokemonListScraper.GetName());

            var pokemonList = pokemonListScraper.Scrape(UrlHelper.GetFullPath(POKEMON_LIST_PATH));

            var scrapers = new List<IListScraper>
            {
                new EvolutionList(htmlWeb),
                new MegaEvolutionList(htmlWeb)
            };
            foreach (var scraper in scrapers)
            {
                Console.WriteLine("Scraping: {0}", scraper.GetName());

                scraper.Scrape(GetScraperUrl(scraper), pokemonList);
            }

            Console.WriteLine();
            Console.WriteLine("BulbapediaScraper was finalized");

            Console.ReadKey();
        }

        private static string GetScraperUrl(IListScraper scraper)
        {
            if (scraper is EvolutionList)
                return UrlHelper.GetFullPath(POKEMON_EVOLUTION_LIST_PATH);

            if (scraper is MegaEvolutionList)
                return UrlHelper.GetFullPath(POKEMON_MEGA_EVOLUTION_LIST_PATH);

            return string.Empty;
        }
    }
}
