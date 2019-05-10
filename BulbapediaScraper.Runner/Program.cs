using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Scrapers.EvolutionList;
using BulbapediaScraper.Runner.Scrapers.FormsList;
using BulbapediaScraper.Runner.Scrapers.MegaEvolutionList;
using BulbapediaScraper.Runner.Scrapers.MoveList;
using BulbapediaScraper.Runner.Scrapers.PokemonList;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace BulbapediaScraper.Runner
{
    class Program
    {
        private static BulbapediaConfiguration _bulbapediaConfiguration;

        static void Main(string[] args)
        {
            Configure();

            Console.WriteLine("BulbapediaScraper was initialized");
            Console.WriteLine();

            Console.WriteLine("Inform the path and file name:");
            var path = Console.ReadLine();

            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Invalid path");
                return;
            }

            Console.WriteLine();

            var htmlWeb = new HtmlWeb();

            IPokemonListScraper pokemonListScraper = new PokemonList(htmlWeb, _bulbapediaConfiguration);

            Console.WriteLine("Scraping: {0}", pokemonListScraper.GetName());

            var pokemonList = pokemonListScraper.Scrape();

            var scrapers = new List<IListScraper>
            {
                new MegaEvolutionList(htmlWeb, _bulbapediaConfiguration),
                new EvolutionList(htmlWeb, _bulbapediaConfiguration),
                new FormList(htmlWeb, _bulbapediaConfiguration),
                new MoveList(htmlWeb, _bulbapediaConfiguration)
            };
            foreach (var scraper in scrapers)
            {
                Console.WriteLine("Scraping: {0}", scraper.GetName());

                scraper.Scrape(pokemonList);
            }

            Console.WriteLine("Generating script");
            var script = new ScriptGenerator.ScriptGenerator().Generate(pokemonList);

            Console.WriteLine("Saving file");
            GenerateFile(path, script);

            Console.WriteLine();
            Console.WriteLine("BulbapediaScraper was finalized");

            Console.ReadKey();
        }

        private static void GenerateFile(string path, string fileContent)
        {
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, fileContent);
        }

        private static void Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            _bulbapediaConfiguration = configuration.GetSection("BulbapediaConfiguration").Get<BulbapediaConfiguration>();
        }
    }
}
