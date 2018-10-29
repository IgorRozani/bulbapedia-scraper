using BulbapediaScraper.Runner.Enums;
using HtmlAgilityPack;

namespace BulbapediaScraper.Runner.Extensions
{
    public static class HtmlNodeCollectionExtension
    {
        public static HtmlNode GetByIndex(this HtmlNodeCollection elements, PokemonListTableIndex index) =>
            elements[(int)index];
    }
}
