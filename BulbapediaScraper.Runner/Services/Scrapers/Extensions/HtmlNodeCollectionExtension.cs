using HtmlAgilityPack;
using System;

namespace BulbapediaScraper.Runner.Services.Scrapers.Extensions
{
    public static class HtmlNodeCollectionExtension
    {
        public static HtmlNode GetByIndex(this HtmlNodeCollection elements, Enum index)
        {
            return elements[Convert.ToInt32(index)];
        }
    }
}
