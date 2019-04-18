using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;
using System.Text;

namespace BulbapediaScraper.Runner.ScriptGenerator
{
    public class ScriptGenerator
    {
        public string Generate(ICollection<Pokemon> pokemons)
        {
            var script = new StringBuilder();

            script.AppendLine("CREATE");
            foreach(var pokemon in pokemons)
            {
                script.AppendLine($"({pokemon.GetCleanName()}:Pokemon {{ name:\"{pokemon.Name}\", nationalPokedexNumber:{pokemon.NationalPokedexNumber}, regionalPokedexNumber:'{pokemon.RegionalPokedexNumber}', picture:'{pokemon.Picture}', profileUrl:'{pokemon.ProfileUrl}' }}),");
            }

            return script.ToString();
        }
    }
}
