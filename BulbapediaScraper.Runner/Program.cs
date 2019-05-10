using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Scrapers.Interfaces;
using BulbapediaScraper.Runner.Scrapers.Services.EvolutionList;
using BulbapediaScraper.Runner.Scrapers.Services.FormsList;
using BulbapediaScraper.Runner.Scrapers.Services.MegaEvolutionList;
using BulbapediaScraper.Runner.Scrapers.Services.MoveList;
using BulbapediaScraper.Runner.Scrapers.Services.PokemonList;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace BulbapediaScraper.Runner
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

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

            var pokemonListScraper = _serviceProvider.GetService<IPokemonListScraper>();

            Console.WriteLine("Scraping: {0}", pokemonListScraper.GetName());

            var pokemonList = pokemonListScraper.Scrape();

            var scrapers = new List<IListScraper>
            {
                _serviceProvider.GetService<IMegaEvolutionList>(),
                _serviceProvider.GetService<IEvolutionList>(),
                _serviceProvider.GetService<IFormList>(),
                _serviceProvider.GetService<IMoveList>()
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

            var bulbapediaConfiguration = configuration.GetSection("BulbapediaConfiguration").Get<BulbapediaConfiguration>();

            var htmlWeb = new HtmlWeb();

            _serviceProvider = new ServiceCollection()
                .AddSingleton(bulbapediaConfiguration)
                .AddSingleton(htmlWeb)
                .AddSingleton<IPokemonListScraper, PokemonList>()
                .AddSingleton<IEvolutionList, EvolutionList>()
                .AddSingleton<IFormList, FormList>()
                .AddSingleton<IMegaEvolutionList, MegaEvolutionList>()
                .AddSingleton<IMoveList, MoveList>()
                .BuildServiceProvider();
        }
    }
}
