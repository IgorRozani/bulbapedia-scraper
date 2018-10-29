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
        private const string POKEMON_LIST_URL = "https://bulbapedia.bulbagarden.net/w/index.php?title=List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number&oldid=2853096";

        static void Main(string[] args)
        {
            Console.WriteLine("BulbapediaScraper was initialized");

            Console.WriteLine();

            Console.WriteLine("Scraping: Pokémon list");

            var htmlWeb = new HtmlWeb();
            var htmlPage = htmlWeb.Load(POKEMON_LIST_URL);

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

                    var types = new List<Models.Type>
                    {
                        new Models.Type(rollCollumns.GetByIndex(PokemonListTableIndex.Type1).SelectSingleNode("a/span").InnerText)
                    };
                    if (rollCollumns.Count > 5)
                        types.Add(new Models.Type(rollCollumns.GetByIndex(PokemonListTableIndex.Type2).SelectSingleNode("a/span").InnerText));

                    var nationalPokedexNumber = Convert.ToInt32(ndex);
                    if (pokemonList.ContainsKey(nationalPokedexNumber))
                    {
                        pokemonList[nationalPokedexNumber].RegionalVariants.Add(
                            new RegionalVariant($"https://{picture}", types));
                    }
                    else
                    {
                        pokemonList.Add(nationalPokedexNumber, new Pokemon
                        {
                            Name = name,
                            RegionalPokedexNumber = rdex == "&160;" ? null : rdex,
                            NationalPokedexNumber = nationalPokedexNumber,
                            Picture = $"https://{picture}",
                            Types = types
                        });
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("BulbapediaScraper was finalized");
        }
    }
}
