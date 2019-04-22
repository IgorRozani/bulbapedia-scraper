using BulbapediaScraper.Runner.Extensions;
using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Scrapers.PokemonList.Enums;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace BulbapediaScraper.Runner.Scrapers.PokemonList
{
    public class PokemonList : IPokemonListScraper
    {
        public PokemonList(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        private HtmlWeb _htmlWeb;

        public ICollection<Pokemon> Scrape(string url)
        {
            var htmlPage = _htmlWeb.Load(url);

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
                    // First row is always the header
                    if (roll == rolls.FirstOrDefault())
                        continue;

                    var rollCollumns = roll.SelectNodes("td");

                    var rdex = rollCollumns.GetByIndex(TableIndex.Rdex).InnerText.RemoveSpecialCharacter();
                    var ndex = rollCollumns.GetByIndex(TableIndex.Ndex).InnerText.RemoveSpecialCharacter();
                    var picture = rollCollumns.GetByIndex(TableIndex.Picture).SelectSingleNode("a/img")
                        .Attributes["src"].Value.Trim('/');
                    var name = rollCollumns.GetByIndex(TableIndex.PokemonName).SelectSingleNode("a").InnerText;
                    var profileUrl = rollCollumns.GetByIndex(TableIndex.PokemonName).SelectSingleNode("a").Attributes["href"].Value.Replace("/wiki/", "");

                    var types = new List<Type>
                    {
                        new Type(rollCollumns.GetByIndex(TableIndex.Type1).SelectSingleNode("a/span").InnerText)
                    };
                    if (rollCollumns.Count > 5)
                        types.Add(new Type(rollCollumns.GetByIndex(TableIndex.Type2).SelectSingleNode("a/span").InnerText));

                    // New pokemons without numbers are not supported
                    if (!int.TryParse(ndex, out int nationalPokedexNumber))
                        continue;

                    if (pokemonList.ContainsKey(nationalPokedexNumber))
                    {
                        var pictureUrl = UrlHelper.GetImageFullPath(picture);
                        if (nationalPokedexNumber > 151)
                        {
                            pokemonList[nationalPokedexNumber].Formes.Add(new Forme(pictureUrl, types));
                        }
                        else
                        {
                            pokemonList[nationalPokedexNumber].RegionalVariants.Add(
                               new RegionalVariant(pictureUrl, types));
                        }
                    }
                    else
                    {
                        pokemonList.Add(nationalPokedexNumber, new Pokemon
                        {
                            Name = name,
                            RegionalPokedexNumber = rdex == "&160;" ? null : rdex,
                            NationalPokedexNumber = nationalPokedexNumber,
                            Picture = UrlHelper.GetImageFullPath(picture),
                            Types = types,
                            ProfileUrl = UrlHelper.GetFullPath(profileUrl)
                        });
                    }
                }
            }

            return pokemonList.Values.ToList();
        }

        public string GetName() => "Pokémon list";
    }
}
