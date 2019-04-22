using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.ScriptGenerator.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulbapediaScraper.Runner.ScriptGenerator
{
    public class ScriptGenerator
    {
        public ScriptGenerator()
        {
            _neo4jGenerator = new Neo4jGenerator();
        }

        private Neo4jGenerator _neo4jGenerator;

        public string Generate(ICollection<Pokemon> pokemons)
        {
            var scriptBuilder = new StringBuilder();

            var types = pokemons.SelectMany(p => p.Types).Distinct(new TypeEqualityComparer()).ToList();
            scriptBuilder.Append(GenerateTypes(types));

            scriptBuilder.AppendLine();

            scriptBuilder.AppendLine(GeneratePokemons(pokemons));

            return scriptBuilder.ToString();
        }

        private string GenerateTypes(ICollection<Type> types)
        {
            var nodes = new List<Node>();
            foreach (var type in types)
            {
                nodes.Add(new Node
                {
                    Id = type.Name,
                    Labels = new List<string> { "Type" },
                    Properties = new Dictionary<string, object>
                    {
                        { "Name",type.Name }
                    }
                });
            }

            return _neo4jGenerator.CreateNodes(nodes);
        }

        private string GeneratePokemons(ICollection<Pokemon> pokemons)
        {
            var nodes = new List<Node>();
            foreach (var pokemon in pokemons)
            {
                nodes.Add(new Node
                {
                    Id = pokemon.GetCleanName(),
                    Labels = new List<string> { "Pokemon" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", pokemon.Name },
                        {"nationalPokedexNumber", pokemon.NationalPokedexNumber},
                        { "regionalPokedexNumber",  pokemon.RegionalPokedexNumber},
                        {"picture",pokemon.Picture},
                        {"profileUrl", pokemon.ProfileUrl}
                    }
                });
            }

            return _neo4jGenerator.CreateNodes(nodes);
        }
    }
}
