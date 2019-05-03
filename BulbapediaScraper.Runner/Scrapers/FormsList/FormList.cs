using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulbapediaScraper.Runner.Scrapers.FormsList
{
    public class FormList : BaseScraper, IListScraper
    {
        public FormList(HtmlWeb htmlWeb) : base(htmlWeb)
        {
        }

        private readonly List<string> _ignoreLines = new List<string>
        {
            " Pikachu&#160;Electric&#160;\n",
            " Pikachu&#160;Electric&#160;\n",
            " Pichu&#160;Electric&#160;\n",
            " A\n",
            " Squirtle&#160;Water&#160;\n",
            " Wartortle&#160;Water&#160;\n",
            " Blastoise&#160;Water&#160;\n",
            " Pichu&#160;Electric&#160;\n",
            " Pikachu♂&#160;Electric&#160;\n",
            " Raichu♂&#160;Electric&#160;\n",
            " Eevee&#160;Normal&#160;\n",
            " Vaporeon&#160;Water&#160;\n",
            " Jolteon&#160;Electric&#160;\n",
            " Flareon&#160;Fire&#160;\n",
            " Espeon&#160;Psychic&#160;\n",
            " Umbreon&#160;Dark&#160;\n",
            " Pattern 1&#160;Normal&#160;\n",
            "Spoiler warning: this article may contain major plot or ending details.",
            "\nForm\n\n\n\n\n\n\n\n\n Language\n\n Title\n\n\n  Japanese\n\n すがた Form\n\n\n\n\n\n\n\n\n\n\n Chinese\n\n Cantonese\n\n 樣子 Yeuhngjí\n\n\n Mandarin\n\n 樣子 / 样子 Yàngzi\n\n\n\n\n\n\n\n\n\n\n  French\n\n Forme\n\n\n\n  German\n\n Form\n\n\n\n\n\n\n\n  Indonesian\n\n Bentuk\n\n\n  Italian\n\n Forma\n\n\n  Korean\n\n 모습 Moseup\n\n\n\n\n\n  Malaysian\n\n Bentuk\n\n\n\n\n  Polish\n\n Forma\n\n\n\n\n\n\n\n\n\n  Spanish\n\n Forma\n\n\n\n\n\n\n  Thai\n\n ร่าง Rang\n\n\n\n\n\n  Vietnamese\n\n Dạng\n\n\n\n\n\n",
            "\n Variant Pokémon\n List of Pokémon with gender differences\n Mega Evolution\n Primal Reversion\n",
            "\n\n\n\n\n\n\n\n Lists of Pokémon\n\n\n by National Pokédex no.\n\n English • Japanese • German • French • Korean • Chinese\n\n\n\n\n\n by regional Pokédex no.\n\n Kanto • New • Johto • Hoenn (Gen III • Gen VI) • Sinnoh • Unova • New Unova • Kalos • Alola (SM • USUM)Unown Mode • in no regional Pokédex • in every regional Pokédex\n\n\n\n\n\n by regional Browser no.\n\n Fiore • Almia • Oblivia • in no regional Browser • in every regional Browser\n\n\n\n\n\n by index number\n\n Generation I • Generation II • Generation III • Generation IV • Generation V • Generation VI • Generation VII\n\n\n\n\n\n by other numbering systems\n\n DPBP • PokéPark Pad • Ransei Gallery • Shuffle list • Picross list • Duel Library • Google Maps: Pokémon Challenge\n\n\n\n\n\n by attributes\n\n Ability • category • habitat • color • IQ group • gender differences • form differencesEgg Group • body style • height • weight • unique type combinations\n\n\n\n\n\n by evolution\n\n evolution family (GO) • no evolution family • branched • cross-generation • levels\n\n\n\n\n\n by in-game stats\n\n base stats (Gen I • Gen II-V • Gen VI • fully evolved • unique base stat totals • GO)performance stats • catch rate • EVs given in battle (Gen III-IV • Gen V-VI) • gender ratiosteps to hatch • availability • wild held item (Gen II) • experience type • base friendship • call rate\n\n\n\n\n\n miscellaneous\n\n alphabetically • field moves (Gen I • Gen II • Gen III • Gen IV • Gen V • Gen VI)Shadow Pokémon • Pal Park area • Pokéwalker • debut episode • glitchreleased with a Hidden Ability (Gen V • Gen VI • Gen VII) • ST Energy Shot • petting\n\n\n"
        };

        public string GetName() => "Form list";

        public void Scrape(string url, ICollection<Pokemon> pokemonList)
        {
            var htmlPage = _htmlWeb.Load(url);

            var formTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");

            foreach (var formTable in formTables)
            {
                if (formTable.Attributes["align"]?.Value == "center")
                    continue;

                var rolls = formTable.SelectNodes("tr");

                var firstRoll = rolls.FirstOrDefault();

                if (firstRoll.SelectNodes("td") == null || firstRoll.SelectNodes("td").Count == 0)
                    continue;

                var firstTd = firstRoll.SelectSingleNode("td").InnerText;

                if (_ignoreLines.Contains(firstTd))
                    continue;

                if (firstRoll.SelectNodes("th") != null)
                {
                    // do nothing
                    continue;
                }

                foreach (var roll in rolls)
                {
                    if (firstRoll.SelectNodes("td/table") != null)
                    {
                        foreach (var tableRow in roll.SelectNodes("td/table"))
                        {
                            var trs = tableRow.SelectNodes("tr");

                            if (trs.Count <= 3)
                                throw new Exception();

                            var formImage = trs[0].SelectSingleNode("td/a/img").Attributes["src"].Value.Trim('/');
                            var pokemonName = trs[1].SelectSingleNode("th/small/a/span").InnerText;
                            var formName = trs[1].SelectSingleNode("th/small").InnerText.Substring(pokemonName.Length);
                            if (string.IsNullOrEmpty(formName))
                                formName = pokemonName;

                            var types = new List<Models.Type>();
                            foreach (var cellType in trs[2].SelectSingleNode("th/small").SelectNodes("span/a/span"))
                            {
                                types.Add(new Models.Type(cellType.InnerText.Replace("&#160;", string.Empty)));
                            }

                            var pokemon = pokemonList.FirstOrDefault(p => p.Name == pokemonName);
                            if (!pokemon.Forms.Any(f => f.Name == formName))
                                pokemon.Forms.Add(new Form(formName, UrlHelper.GetImageFullPath(formImage), types));
                        }
                    }
                    else
                    {
                        // Castform case
                        foreach (var td in roll.SelectNodes("td"))
                        {
                            var formName = td.InnerText.Split("&#160;")[0].Trim();
                            if (string.IsNullOrEmpty(formName))
                            {
                                formName = td.SelectSingleNode("span/a/span").InnerText.Replace("&#160;", string.Empty).Trim();
                            }

                            var imgs = td.SelectNodes("a/img");

                            for (var i = 0; i < imgs.Count; i++)
                            {
                                var formPicture = imgs[i].Attributes["src"].Value.Trim('/');
                                int pokemonNumber;
                                var imgAlt = imgs[i].Attributes["alt"].Value;
                                if (imgAlt.StartsWith("Spr 7s 716"))
                                    pokemonNumber = 716;
                                else if (imgAlt.EndsWith("Magearna M19.png"))
                                    pokemonNumber = 801;
                                else
                                    pokemonNumber = Convert.ToInt32(imgAlt.Substring(0, 3));
                                var types = new List<Models.Type>();
                                var rollsTypes = td.SelectNodes("small");
                                if (rollsTypes != null)
                                {
                                    foreach (var rollTypes in rollsTypes[i].SelectNodes("span/a/span"))
                                        types.Add(new Models.Type(rollTypes.InnerText.Replace("&#160;", string.Empty)));
                                }
                                else if (pokemonNumber != 718)
                                {
                                    types.Add(new Models.Type(formName));
                                }

                                var pokemon = pokemonList.FirstOrDefault(p => p.NationalPokedexNumber == pokemonNumber);
                                if (!pokemon.Forms.Any(f => f.Name == formName))
                                    pokemon.Forms.Add(new Form(formName, UrlHelper.GetImageFullPath(formPicture), types));
                            }
                        }
                    }
                }
            }

            var pokemonsWithForms = pokemonList.Where(p => p.Forms.Any() && p.Name != "Unown").ToList();
        }
    }
}
