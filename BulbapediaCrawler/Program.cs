using System;
using HtmlAgilityPack;

namespace BulbapediaCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pokemonListURL =
                @"https://bulbapedia.bulbagarden.net/w/index.php?title=List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number&oldid=2853096";

            var htmlWeb = new HtmlWeb();
            var htmlPage = htmlWeb.Load(pokemonListURL);

            var pokemonTable = htmlPage.DocumentNode.SelectSingleNode("//body/div[@id='globalWrapper']");
        }
    }
}
