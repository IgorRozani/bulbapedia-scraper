using BulbapediaScraper.Runner.Extensions;
using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Scrapers.MegaEvolutionList.Enums;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulbapediaScraper.Runner.Scrapers.MegaEvolutionList
{
    public class MegaEvolutionList : BaseScraper, IListScraper
    {
        public MegaEvolutionList(HtmlWeb htmlWeb) : base (htmlWeb)
        {
        }

        public void Scrape(string url, ICollection<Pokemon> pokemonList)
        {
            var htmlPage = _htmlWeb.Load(url);

            var megaEvolutionTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");

            foreach (var megaEvolutionTable in megaEvolutionTables)
            {
                var rolls = megaEvolutionTable.SelectNodes("tr");

                // It's not a pokemon table
                if (rolls == null || rolls.Count < 2)
                    continue;

                var firstRoll = rolls.FirstOrDefault();

                if (firstRoll.SelectSingleNode("th").InnerText.Trim() != "Pokémon")
                    continue;

                var pokemon = new Pokemon();

                foreach (var roll in rolls)
                {
                    // First row is always the header
                    if (roll == firstRoll)
                        continue;

                    var rollCollumns = roll.SelectNodes("td");
                    if (rollCollumns == null || rollCollumns.Count == 0)
                        continue;

                    if (rollCollumns.Count == 8)
                    {
                        var pokemonName = rollCollumns.GetByIndex(FullRow.PokemonName).SelectSingleNode("a").InnerText.Trim();

                        var megaEvolutionPicture = rollCollumns.GetByIndex(FullRow.MegaEvolutionImage).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');

                        var megaEvolutionTypes = new List<string>();
                        foreach (var type in rollCollumns.GetByIndex(FullRow.MegaEvolutionType).SelectNodes("span/a/span"))
                            megaEvolutionTypes.Add(type.InnerText.Replace("&#160;", string.Empty));

                        var megaStoneTd = rollCollumns.GetByIndex(FullRow.MegaStone).SelectNodes("a");
                        var megaStoneName = string.Empty;
                        var megaStoneImage = string.Empty;
                        foreach (var link in megaStoneTd)
                        {
                            if (link.SelectSingleNode("img") != null)
                                megaStoneImage = link.SelectSingleNode("img").Attributes["src"].Value.Trim('/');
                            else
                                megaStoneName = link.InnerText;
                        }

                        pokemon = pokemonList.FirstOrDefault(p => p.Name == pokemonName);
                        pokemon.MegaEvolutions.Add(new MegaEvolution
                        {
                            Name = GetMegaEvolutionName(pokemon.Name, megaStoneName),
                            Picture = UrlHelper.GetImageFullPath(megaEvolutionPicture),
                            Types = megaEvolutionTypes.Select(m => new Models.Type(m)).ToList(),
                            MegaStone = new MegaStone
                            {
                                Name = megaStoneName,
                                Image = UrlHelper.GetImageFullPath(megaStoneImage)
                            }
                        });
                    }
                    else if (rollCollumns.Count == 4)
                    {
                        var megaEvolutionPicture = rollCollumns.GetByIndex(PartialRow.MegaEvolutionImage).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');

                        var megaEvolutionTypes = new List<string>();
                        foreach (var type in rollCollumns.GetByIndex(PartialRow.MegaEvolutionType).SelectNodes("span/a/span"))
                            megaEvolutionTypes.Add(type.InnerText.Replace("&#160;", string.Empty));

                        var megaStoneTd = rollCollumns.GetByIndex(PartialRow.MegaStone).SelectNodes("a");
                        var megaStoneName = string.Empty;
                        var megaStoneImage = string.Empty;
                        foreach (var link in megaStoneTd)
                        {
                            if (link.SelectSingleNode("img") != null)
                                megaStoneImage = link.SelectSingleNode("img").Attributes["src"].Value.Trim('/');
                            else
                                megaStoneName = link.InnerText;
                        }

                        pokemon.MegaEvolutions.Add(new MegaEvolution
                        {
                            Name = GetMegaEvolutionName(pokemon.Name, megaStoneName),
                            Picture = UrlHelper.GetImageFullPath(megaEvolutionPicture),
                            Types = megaEvolutionTypes.Select(m => new Models.Type(m)).ToList(),
                            MegaStone = new MegaStone
                            {
                                Name = megaStoneName,
                                Image = UrlHelper.GetImageFullPath(megaStoneImage)
                            }
                        });
                    }
                    else
                        throw new NotImplementedException();
                }
            }
        }

        private string GetMegaEvolutionName(string pokemonName, string megaStoneName)
        {
            var name = $"Mega {pokemonName}";

            if (megaStoneName.Split(' ').Length > 1)
                name += $" { megaStoneName.Split(' ').LastOrDefault()}";
            return name;
        }

        public string GetName() => "Mega evolution list";
    }
}
