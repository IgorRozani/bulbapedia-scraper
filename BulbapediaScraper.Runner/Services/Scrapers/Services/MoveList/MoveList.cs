using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Services.Scrapers.Extensions;
using BulbapediaScraper.Runner.Services.Scrapers.Interfaces;
using BulbapediaScraper.Runner.Services.Scrapers.Services.MoveList.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BulbapediaScraper.Runner.Services.Scrapers.Services.MoveList
{
    public class MoveList : BaseScraper, IMoveList
    {
        public MoveList(HtmlAgilityPack.HtmlWeb htmlWeb, BulbapediaConfiguration bulbapediaConfiguration) : base(htmlWeb, bulbapediaConfiguration)
        {
        }

        public string GetName() => "Move list";

        public void Scrape(ICollection<Pokemon> pokemonList)
        {
            var moves = new List<Move>();

            var htmlPage = _htmlWeb.Load(GetSiteFullPath(_bulbapediaConfiguration.MovesListPath));

            var movesTable = htmlPage.DocumentNode.SelectNodes("//body/div[@id='globalWrapper']/div[@id='column-content']/div[@id='content']/div[@id='outercontentbox']/div[@id='contentbox']/div[@id='bodyContent']/div[@id='mw-content-text']/table");
            foreach (var moveTable in movesTable)
            {
                if (moveTable.SelectSingleNode("tr/td/table") == null)
                    continue;

                foreach (var roll in moveTable.SelectNodes("tr"))
                {
                    var subTableRolls = roll.SelectNodes("td/table/tr");

                    var firstRoll = subTableRolls.FirstOrDefault();

                    foreach (var subTableRoll in subTableRolls)
                    {
                        // First row is always the header
                        if (subTableRoll == firstRoll)
                            continue;

                        var rollCollumns = subTableRoll.SelectNodes("td");

                        if(rollCollumns.Count == 7)
                        {
                            var moveId = rollCollumns.GetByIndex(ShadowMoveRow.Index).InnerText.RemoveSpecialCharacter();
                            var nameCell = rollCollumns.GetByIndex(ShadowMoveRow.Name).SelectSingleNode("a");
                            var name = nameCell.InnerText;
                            var moveLink = nameCell.Attributes["href"].Value.Replace("/wiki/", "");
                            var type = rollCollumns.GetByIndex(ShadowMoveRow.Type).SelectSingleNode("a/span").InnerText;
                            var category = rollCollumns.GetByIndex(ShadowMoveRow.Category).SelectSingleNode("a/span").InnerText;
                            var powerPointValue = rollCollumns.GetByIndex(ShadowMoveRow.PowerPoint).InnerText.RemoveSpecialCharacter().Replace("*", "");
                            var powerValue = rollCollumns.GetByIndex(ShadowMoveRow.Power).InnerText.RemoveSpecialCharacter().Replace("*", "");
                            var accuracyValue = rollCollumns.GetByIndex(ShadowMoveRow.Accuracy).InnerText.RemoveSpecialCharacter().Replace("%", "").Replace("*", "");

                            var fullMoveLink = GetSiteFullPath(moveLink);

                            moves.Add(new Move
                            {
                                Name = name,
                                PowerPoint = int.TryParse(powerPointValue, out int powerPoint) ? powerPoint : (int?)null,
                                Acurracy = int.TryParse(accuracyValue, out int accuracy) ? accuracy / 100.0m : (decimal?)null,
                                Power = int.TryParse(powerValue, out int power) ? power : (int?)null,
                                Type = !string.IsNullOrEmpty(type) ? new Type(type) : null,
                                Category = new Category(category),
                                IsColosseumOrXDExclusive = true
                            });
                        }
                        else if (rollCollumns.Count == 6)
                        {
                            var moveId = rollCollumns.GetByIndex(MysteryDungeonMoveRow.Index).InnerText.RemoveSpecialCharacter();
                            var nameCell = rollCollumns.GetByIndex(MysteryDungeonMoveRow.Name).SelectSingleNode("a");
                            var name = nameCell.InnerText;
                            var moveLink = nameCell.Attributes["href"].Value.Replace("/wiki/", "");
                            var type = rollCollumns.GetByIndex(MysteryDungeonMoveRow.Type).SelectSingleNode("a/span").InnerText.Replace("?","");
                            var powerPointValue = rollCollumns.GetByIndex(MysteryDungeonMoveRow.PowerPoint).InnerText.RemoveSpecialCharacter().Replace("*", "");
                            var powerValue = rollCollumns.GetByIndex(MysteryDungeonMoveRow.Power).InnerText.RemoveSpecialCharacter().Replace("*", "").Replace("☆","").Replace("?", "");
                            var accuracyValue = rollCollumns.GetByIndex(MysteryDungeonMoveRow.Accuracy).InnerText.RemoveSpecialCharacter().Replace("%", "").Replace("*", "").Replace("☆", "").Replace("?", "");

                            var fullMoveLink = GetSiteFullPath(moveLink);

                            moves.Add(new Move
                            {
                                Name = name,
                                PowerPoint = int.TryParse(powerPointValue, out int powerPoint) ? powerPoint : (int?)null,
                                Acurracy = int.TryParse(accuracyValue, out int accuracy) ? accuracy : (decimal?)null,
                                Power = int.TryParse(powerValue, out int power) ? power : (int?)null,
                                Type = !string.IsNullOrEmpty(type) ? new Type(type) : null,
                                IsMysteryDungeonExclusive = true
                            });
                        }
                        else 
                        {
                            var moveId = rollCollumns.GetByIndex(MoveRow.Index).InnerText.RemoveSpecialCharacter();
                            var nameCell = rollCollumns.GetByIndex(MoveRow.Name).SelectSingleNode("a");
                            var name = nameCell.InnerText;
                            var moveLink = nameCell.Attributes["href"].Value.Replace("/wiki/", "");
                            var type = rollCollumns.GetByIndex(MoveRow.Type).SelectSingleNode("a/span").InnerText;
                            var category = rollCollumns.GetByIndex(MoveRow.Category).SelectSingleNode("a/span").InnerText;
                            var contest = rollCollumns.GetByIndex(MoveRow.Contest).SelectSingleNode("a/span").InnerText.Replace("?", "");
                            var powerPointValue = rollCollumns.GetByIndex(MoveRow.PowerPoint).InnerText.RemoveSpecialCharacter().Replace("*", "");
                            var powerValue = rollCollumns.GetByIndex(MoveRow.Power).InnerText.RemoveSpecialCharacter().Replace("*", "");
                            var accuracyValue = rollCollumns.GetByIndex(MoveRow.Accuracy).InnerText.RemoveSpecialCharacter().Replace("%", "").Replace("*", "");

                            var fullMoveLink = GetSiteFullPath(moveLink);

                            moves.Add(new Move
                            {
                                Name = name,
                                PowerPoint = int.TryParse(powerPointValue, out int powerPoint) ? powerPoint : (int?)null,
                                Acurracy = int.TryParse(accuracyValue, out int accuracy) ? accuracy / 100.0m : (decimal?)null,
                                Power = int.TryParse(powerValue, out int power) ? power : (int?)null,
                                Type = !string.IsNullOrEmpty(type) ? new Type(type) : null,
                                Category = new Category(category),
                                Contest = !string.IsNullOrEmpty(contest) ? new Contest(contest) : null
                            });
                        }
                    }
                }
            }
        }
    }
}
