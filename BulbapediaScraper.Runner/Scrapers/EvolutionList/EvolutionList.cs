using BulbapediaScraper.Runner.Extensions;
using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Interfaces;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Scrapers.EvolutionList.Enums;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulbapediaScraper.Runner.Scrapers.EvolutionList
{
    public class EvolutionList : IListScraper
    {
        public EvolutionList(HtmlWeb htmlWeb)
        {
            _htmlWeb = htmlWeb;
        }

        private HtmlWeb _htmlWeb;

        public void Scrape(string url, ICollection<Pokemon> pokemonList)
        {
            var htmlPage = _htmlWeb.Load(url);

            var evolutionTables = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");

            foreach (var evolutionTable in evolutionTables)
            {
                var rolls = evolutionTable.SelectNodes("tr");

                var pokemon1 = new Pokemon();
                var pokemon2 = new Pokemon();

                foreach (var roll in rolls)
                {
                    var rollCollumns = roll.SelectNodes("td");
                    if (rollCollumns == null || rollCollumns.Count == 0)
                        continue;

                    if (rollCollumns.Count >= 6 && rollCollumns.Count <= 8)
                    {
                        if (rollCollumns[0].SelectSingleNode("a/img") != null)
                        {
                            var pokemon1Name = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon1Name).SelectSingleNode("a/span").InnerText;

                            if (pokemon1Name.Contains("Unown"))
                            {
                                var form1Picture = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon1Image).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');
                                var form1Name = new StringBuilder();
                                foreach (var element in rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon1Name).SelectNodes("a/span"))
                                    form1Name.Append(" ").Append(element.InnerText);


                                var form2Picture = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon2Image).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');
                                var form2Name = new StringBuilder();
                                foreach (var element in rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon2Name).SelectNodes("a/span"))
                                    form2Name.Append(" ").Append(element.InnerText);

                                pokemon1 = pokemonList.FirstOrDefault(p => p.Name == "Unown");
                                pokemon1.Forms.Add(new Form(form1Name.ToString().Trim(), form1Picture, new List<Type> { new Type("Psychic") }));
                                pokemon1.Forms.Add(new Form(form2Name.ToString().Trim(), form2Picture, new List<Type> { new Type("Psychic") }));

                                if (rollCollumns.Count == 8)
                                {
                                    var form3Picture = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon3Image).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');
                                    var form3Name = new StringBuilder();
                                    foreach (var element in rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon3Name).SelectNodes("a/span"))
                                        form3Name.Append(" ").Append(element.InnerText);
                                    pokemon1.Forms.Add(new Form(form3Name.ToString().Trim(), form3Picture, new List<Type> { new Type("Psychic") }));
                                }
                            }
                            else
                            {
                                var condition1 = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.ConditionEvolution1).InnerText.RemoveSpecialCharacter();
                                var pokemon2Name = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon2Name).SelectSingleNode("a/span").InnerText;

                                pokemon1 = pokemonList.FirstOrDefault(p => p.Name == pokemon1Name);
                                pokemon2 = pokemonList.FirstOrDefault(p => p.Name == pokemon2Name);
                                if (!pokemon1.Evolutions.Any(e => e.Pokemon.Name == pokemon2.Name))
                                    pokemon1.Evolutions.Add(new Evolution(condition1, pokemon2));

                                if (rollCollumns.Count == 8)
                                {
                                    var pokemon3Name = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.Pokemon3Name).SelectSingleNode("a/span").InnerText;
                                    var condition2 = rollCollumns.GetByIndex(ThreeOrTwoEvolutionsRowIndex.ConditionEvolution2).InnerText.RemoveSpecialCharacter();

                                    var pokemon3 = pokemonList.FirstOrDefault(p => p.Name == pokemon3Name);
                                    if (!pokemon2.Evolutions.Any(e => e.Pokemon.Name == pokemon3.Name))
                                        pokemon2.Evolutions.Add(new Evolution(condition2, pokemon3));
                                }
                            }
                        }
                        else
                        {
                            var condition1 = rollCollumns.GetByIndex(MultipleFirstAndSecondEvolutionRowIndex.ConditionEvolution1).InnerText.RemoveSpecialCharacter();
                            var pokemon2Name = rollCollumns.GetByIndex(MultipleFirstAndSecondEvolutionRowIndex.Pokemon2Name).SelectSingleNode("a/span").InnerText;

                            pokemon2 = pokemonList.FirstOrDefault(p => p.Name == pokemon2Name);
                            if(!pokemon1.Evolutions.Any(e => e.Pokemon.Name == pokemon2.Name))
                                pokemon1.Evolutions.Add(new Evolution(condition1, pokemon2));

                            var condition2 = rollCollumns.GetByIndex(MultipleFirstAndSecondEvolutionRowIndex.ConditionEvolution2).InnerText.RemoveSpecialCharacter();
                            var pokemon3Name = rollCollumns.GetByIndex(MultipleFirstAndSecondEvolutionRowIndex.Pokemon2Name).SelectSingleNode("a/span").InnerText;

                            var pokemon3 = pokemonList.FirstOrDefault(p => p.Name == pokemon3Name);
                            if (!pokemon2.Evolutions.Any(e => e.Pokemon.Name == pokemon3.Name))
                                pokemon2.Evolutions.Add(new Evolution(condition2, pokemon3));
                        }
                    }
                    else if (rollCollumns.Count == 4)
                    {
                        if (rollCollumns[0].SelectSingleNode("a/img") == null)
                        {
                            var condition = rollCollumns.GetByIndex(MultipleFirstEvolutionRowIndex.Condition).InnerText.RemoveSpecialCharacter();
                            var pokemonName = rollCollumns.GetByIndex(MultipleFirstEvolutionRowIndex.PokemonName).SelectSingleNode("a/span").InnerText;

                            pokemon2 = pokemonList.FirstOrDefault(p => p.Name == pokemonName);
                            if (!pokemon1.Evolutions.Any(e => e.Pokemon.Name == pokemon2.Name))
                                pokemon1.Evolutions.Add(new Evolution(condition, pokemon2));
                        }
                        // It's a form pokémon, ignore it
                        //else
                        //{
                            //var pokemonName = rollCollumns.GetByIndex(MultipleFormRowIndex.PokemonName).SelectSingleNode("a/span").InnerText;
                            //var picture = rollCollumns.GetByIndex(MultipleFormRowIndex.PokemonImage).SelectSingleNode("a/img").Attributes["src"].Value.Trim('/');

                            //var formRow = rollCollumns.GetByIndex(MultipleFormRowIndex.Form);
                            //var form = formRow.SelectSingleNode("a") != null ? formeRow.SelectSingleNode("a").InnerText : formRow.InnerText;

                            //pokemonList.FirstOrDefault(p => p.Name == pokemonName)?.Forms.Add(new Form(form.RemoveSpecialCharacter(), UrlHelper.GetImageFullPath(picture)));
                        //}
                    }
                    else if (rollCollumns.Count == 3)
                    {
                        if (rollCollumns[0].SelectSingleNode("a/img") == null)
                        {
                            var condition = rollCollumns.GetByIndex(MultipleSecondEvolutionRowIndex.Condition).InnerText.RemoveSpecialCharacter();
                            var pokemonName = rollCollumns.GetByIndex(MultipleSecondEvolutionRowIndex.PokemonName).SelectSingleNode("a/span").InnerText;

                            var pokemon3 = pokemonList.FirstOrDefault(p => p.Name == pokemonName);
                            if (!pokemon2.Evolutions.Any(e => e.Pokemon.Name == pokemon3.Name))
                                pokemon2.Evolutions.Add(new Evolution(condition, pokemon3));
                        }
                        else
                        {
                            var pokemonName = rollCollumns.GetByIndex(WithoutEvolutionRowIndex.PokemonName).SelectSingleNode("a/span").InnerText;
                            // Do nothing
                        }
                    }
                }
            }
        }

        public string GetName() => "Pokémon evolution list";
    }
}
