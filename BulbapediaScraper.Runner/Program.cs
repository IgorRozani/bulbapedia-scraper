using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Services.FileExport.Interfaces;
using BulbapediaScraper.Runner.Services.FileExport.Services;
using BulbapediaScraper.Runner.Services.Scrapers.Interfaces;
using BulbapediaScraper.Runner.Services.Scrapers.Services.EvolutionList;
using BulbapediaScraper.Runner.Services.Scrapers.Services.FormsList;
using BulbapediaScraper.Runner.Services.Scrapers.Services.MegaEvolutionList;
using BulbapediaScraper.Runner.Services.Scrapers.Services.MoveList;
using BulbapediaScraper.Runner.Services.Scrapers.Services.PokemonList;
using BulbapediaScraper.Runner.Services.ScriptGenerator.Interfaces;
using BulbapediaScraper.Runner.Services.ScriptGenerator.Services;
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

            var pokemonListScraper = _serviceProvider.GetService<IPokemonList>();

            PrintScraperName(pokemonListScraper.GetName());

            var pokemonList = pokemonListScraper.Scrape();

            var scrapers = new List<IListScraper>
            {
                _serviceProvider.GetService<IMegaEvolutionList>(),
                _serviceProvider.GetService<IEvolutionList>(),
                _serviceProvider.GetService<IFormList>()
            };
            foreach (var scraper in scrapers)
            {
                PrintScraperName(scraper.GetName());

                scraper.Scrape(pokemonList);
            }

            Console.WriteLine("Generating script");
            var script = _serviceProvider.GetService<IScriptGenerator>().Generate(pokemonList);

            Console.WriteLine("Saving file");
            _serviceProvider.GetService<IFileExport>().Export(script);

            Console.WriteLine();
            Console.WriteLine("BulbapediaScraper was finalized");
        }

        private static void PrintScraperName(string scraperName) =>
            Console.WriteLine("Scraping: {0}", scraperName);

        private static void Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var bulbapediaConfiguration = configuration.GetSection("bulbapediaConfiguration").Get<BulbapediaConfiguration>();
            var fileExport = configuration.GetSection("fileExportConfiguration").Get<FileExportConfiguration>();
            var htmlWeb = new HtmlWeb();

            _serviceProvider = new ServiceCollection()
                .AddSingleton(bulbapediaConfiguration)
                .AddSingleton(fileExport)
                .AddSingleton(htmlWeb)
                .AddSingleton<IPokemonList, PokemonList>()
                .AddSingleton<IEvolutionList, EvolutionList>()
                .AddSingleton<IFormList, FormList>()
                .AddSingleton<IMegaEvolutionList, MegaEvolutionList>()
                .AddSingleton<IMoveList, MoveList>()
                .AddSingleton<IFileExport, FileExport>()
                .AddSingleton<INeo4jGenerator, Neo4jGenerator>()
                .AddSingleton<IScriptGenerator, ScriptGenerator>()
                .BuildServiceProvider();
        }
    }
}
