using BulbapediaScraper.Runner.Enums;
using BulbapediaScraper.Runner.Extensions;
using BulbapediaScraper.Runner.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulbapediaScraper.Runner
{
    class Program
    {
        private const string POKEMON_LIST_PATH = "List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number";
        private const string POKEMON_EVOLUTION_LIST_PATH = "List_of_Pokémon_by_evolution_family";
        private const string BASE_URL = "https://bulbapedia.bulbagarden.net/w/index.php?title=";
        private const string BASE_IMAGE_URL = "https://";

        private static HtmlWeb _htmlWeb;

        static void Main(string[] args)
        {
            Console.WriteLine("BulbapediaScraper was initialized");
            Console.WriteLine();

            _htmlWeb = new HtmlWeb();

            var pokemonList = ScrapePokemonList(GetFullPath(POKEMON_LIST_PATH));

            ScrapePokemonEvolutionList(GetFullPath(POKEMON_EVOLUTION_LIST_PATH), ref pokemonList);

            Console.WriteLine();
            Console.WriteLine("BulbapediaScraper was finalized");
        }

        private static Dictionary<int, Pokemon> ScrapePokemonList(string url)
        {
            Console.WriteLine("Scraping: Pokémon list");

            var htmlPage = DownloadPage(url);

            var pokemonTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");

            var pokemonList = new Dictionary<int, Pokemon>();

            foreach (var pokemonTable in pokemonTables)
            {
                var rolls = pokemonTable.SelectNodes("tr");

                // It's not a pokemon table
                if (rolls == null || rolls.Count < 2)
                    continue;

                foreach (var roll in rolls)
                {
                    if (roll == rolls.FirstOrDefault())
                        continue;

                    var rollCollumns = roll.SelectNodes("td");

                    var rdex = rollCollumns.GetByIndex(PokemonListTableIndex.Rdex).InnerText.Replace('\n', ' ').Replace("#", "").Trim();
                    var ndex = rollCollumns.GetByIndex(PokemonListTableIndex.Ndex).InnerText.Replace('\n', ' ').Replace("#", "").Trim();
                    var picture = rollCollumns.GetByIndex(PokemonListTableIndex.Picture).SelectSingleNode("a/img")
                        .Attributes["src"].Value.Trim('/');
                    var name = rollCollumns.GetByIndex(PokemonListTableIndex.PokemonName).SelectSingleNode("a").InnerText;
                    var profileUrl = rollCollumns.GetByIndex(PokemonListTableIndex.PokemonName).SelectSingleNode("a").Attributes["href"].Value.Replace("/wiki/", "");

                    var types = new List<Models.Type>
                    {
                        new Models.Type(rollCollumns.GetByIndex(PokemonListTableIndex.Type1).SelectSingleNode("a/span").InnerText)
                    };
                    if (rollCollumns.Count > 5)
                        types.Add(new Models.Type(rollCollumns.GetByIndex(PokemonListTableIndex.Type2).SelectSingleNode("a/span").InnerText));

                    // New pokemons without numbers are not supported
                    if (!int.TryParse(ndex, out int nationalPokedexNumber))
                        continue;

                    if (pokemonList.ContainsKey(nationalPokedexNumber))
                    {
                        pokemonList[nationalPokedexNumber].RegionalVariants.Add(
                            new RegionalVariant(GetImageFullPath(picture), types));
                    }
                    else
                    {
                        pokemonList.Add(nationalPokedexNumber, new Pokemon
                        {
                            Name = name,
                            RegionalPokedexNumber = rdex == "&160;" ? null : rdex,
                            NationalPokedexNumber = nationalPokedexNumber,
                            Picture = GetImageFullPath(picture),
                            Types = types,
                            ProfileUrl = GetFullPath(profileUrl)
                        });
                    }
                }
            }

            return pokemonList;
        }

        private static void ScrapePokemonEvolutionList(string url, ref Dictionary<int, Pokemon> pokemonList)
        {
            Console.WriteLine("Scraping: Pokémon evolution list");

            var htmlPage = DownloadPage(url);

            var evolutionTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");

            foreach (var evolutionTable in evolutionTables)
            {
                var rolls = evolutionTable.SelectNodes("tr");

                foreach (var roll in rolls)
                {
                    var rollCollumns = roll.SelectNodes("td");

                    if (rollCollumns.Count == 0)
                        continue;


                }
            }
        }

        private static HtmlDocument DownloadPage(string url)
            => _htmlWeb.Load(url);

        private static string GetImageFullPath(string imagePath) =>
            $"{BASE_IMAGE_URL}{imagePath}";

        private static string GetFullPath(string path) =>
            $"{BASE_URL}{path}";
    }
}
