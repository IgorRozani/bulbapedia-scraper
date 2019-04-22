using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulbapediaScraper.Runner.ScriptGenerator
{
    public class ScriptGenerator
    {
        public string Generate(ICollection<Pokemon> pokemons)
        {
            var neo4jGenerator = new Neo4jGenerator();

            var script = new StringBuilder();
            script.AppendLine("CREATE");
            var scriptTypes = new StringBuilder();
            scriptTypes.AppendLine("CREATE");

            var types = new List<Type>();

            
            foreach(var pokemon in pokemons)
            {
                foreach(var type in pokemon.Types)
                {
                    if (types.All(t => t.Name != type.Name))
                    {
                        types.Add(type);
                        scriptTypes.AppendLine($"({type.Name}:Type) {{ name: '{type.Name}'}}),");
                    }
                        

                }
                script.AppendLine($"({pokemon.GetCleanName()}:Pokemon {{ name:\"{pokemon.Name}\", nationalPokedexNumber:{pokemon.NationalPokedexNumber}, regionalPokedexNumber:'{pokemon.RegionalPokedexNumber}', picture:'{pokemon.Picture}', profileUrl:'{pokemon.ProfileUrl}' }}),");
            }

            return script.ToString();
        }
    }
}
