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

        private readonly Neo4jGenerator _neo4jGenerator;

        public string Generate(ICollection<Pokemon> pokemons)
        {
            var scriptBuilder = new StringBuilder();

            var types = pokemons.SelectMany(p => p.Types).Distinct(new TypeEqualityComparer()).ToList();
            scriptBuilder.Append(GenerateNodeTypes(types));

            scriptBuilder.AppendLine();

            scriptBuilder.AppendLine(GenerateNodePokemons(pokemons));

            scriptBuilder.AppendLine();
            var megaEvolutions = pokemons.SelectMany(p => p.MegaEvolutions).ToList();
            scriptBuilder.AppendLine(GenerateNodeMegaEvolution(megaEvolutions));

            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine(GenerateRelationships(pokemons));

            return scriptBuilder.ToString();
        }

        private string GenerateNodeTypes(ICollection<Type> types)
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

        private string GenerateNodePokemons(ICollection<Pokemon> pokemons)
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

        private string GenerateNodeMegaEvolution(ICollection<MegaEvolution> megaEvolutions)
        {
            var nodes = new List<Node>();

            foreach (var megaEvolution in megaEvolutions)
            {
                nodes.Add(new Node
                {
                    Id = megaEvolution.GetCleanName(),
                    Labels = new List<string> { "MegaEvolution" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", megaEvolution.Name },
                        { "picture", megaEvolution.Picture }
                    }
                });
            }

            return _neo4jGenerator.CreateNodes(nodes);
        }

        private string GenerateRelationships(ICollection<Pokemon> pokemons)
        {
            var relationships = new List<Relationship>();

            foreach (var pokemon in pokemons)
            {
                relationships.AddRange(GenerateRelationshipTypes(pokemon.GetCleanName(), pokemon.Types));

                if (pokemon.MegaEvolutions.Any())
                {
                    foreach (var megaEvolution in pokemon.MegaEvolutions)
                    {
                        relationships.Add(new Relationship
                        {
                            Labels = new List<string> { "MegaEvolve" },
                            NodeId1 = pokemon.GetCleanName(),
                            NodeId2 = megaEvolution.GetCleanName(),
                            Properties = new Dictionary<string, object>
                            {
                                { "megaStone", megaEvolution.MegaStone.Name },
                                { "image", megaEvolution.Picture }
                            }
                        });

                        relationships.AddRange(GenerateRelationshipTypes(megaEvolution.GetCleanName(), megaEvolution.Types));
                    }
                }
            }

            return _neo4jGenerator.CreateRelationships(relationships);
        }

        private ICollection<Relationship> GenerateRelationshipTypes(string nodeId, ICollection<Type> types)
        {
            var relationships = new List<Relationship>();

            foreach (var type in types)
            {
                relationships.Add(new Relationship
                {
                    Labels = new List<string> { "Is" },
                    NodeId1 = nodeId,
                    NodeId2 = type.Name
                });
            }
            return relationships;
        }
    }
}
