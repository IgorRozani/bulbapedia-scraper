using BulbapediaScraper.Runner.Helpers;
using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.ScriptGenerator.Model;
using System;
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

        private readonly List<int> NON_SWITCHABLE_FORMS = new List<int>
        {
            413,
            422,
            423,
            550,
            666,
            669,
            670,
            671,
            710,
            711,
            745,
            801
        };

        public string Generate(ICollection<Pokemon> pokemons)
        {
            var scriptBuilder = new StringBuilder();

            // Nodes
            var types = pokemons.SelectMany(p => p.Types).Distinct(new TypeEqualityComparer()).ToList();
            scriptBuilder.Append(GenerateNodeTypes(types));

            scriptBuilder.AppendLine();

            scriptBuilder.AppendLine(GenerateNodePokemons(pokemons));

            // Relationships

            scriptBuilder.AppendLine();
            scriptBuilder.AppendLine(GenerateRelationships(pokemons));

            return scriptBuilder.ToString();
        }

        private string GenerateNodeTypes(ICollection<Models.Type> types)
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
            var pokemonNodes = new List<Node>();
            var regionalVarianteNodes = new List<Node>();
            var megaEvolutionNodes = new List<Node>();
            var formsNodes = new List<Node>();

            foreach (var pokemon in pokemons)
            {
                pokemonNodes.Add(new Node
                {
                    Id = pokemon.GetCleanName(),
                    Labels = new List<string> { "Pokemon" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", pokemon.Name },
                        { "nationalPokedexNumber", pokemon.NationalPokedexNumber },
                        { "regionalPokedexNumber",  pokemon.RegionalPokedexNumber },
                        { "picture",pokemon.Picture },
                        { "profileUrl", pokemon.ProfileUrl }
                    }
                });

                if (pokemon.RegionalVariants.Any())
                    regionalVarianteNodes.AddRange(GenerateNodesRegionalVariant(pokemon));

                if (pokemon.MegaEvolutions.Any())
                    megaEvolutionNodes.AddRange(GenerateNodesMegaEvolution(pokemon));

                if (pokemon.Forms.Any())
                    formsNodes.AddRange(GenerateNodesForm(pokemon));
            }

            return _neo4jGenerator.CreateNodes(pokemonNodes)
                + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(megaEvolutionNodes)
                + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(regionalVarianteNodes)
                + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(formsNodes);
        }

        private IEnumerable<Node> GenerateNodesMegaEvolution(Pokemon pokemon)
        {
            foreach (var megaEvolution in pokemon.MegaEvolutions)
            {
                yield return new Node
                {
                    Id = megaEvolution.GetCleanName(),
                    Labels = new List<string> { "MegaEvolution" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", megaEvolution.Name },
                        { "picture", megaEvolution.Picture }
                    }
                };
            }
        }

        private IEnumerable<Node> GenerateNodesRegionalVariant(Pokemon pokemon)
        {
            foreach (var regionalVariant in pokemon.RegionalVariants)
            {
                yield return new Node
                {
                    Id = regionalVariant.GetName(pokemon.GetCleanName()),
                    Labels = new List<string> { "Form" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", pokemon.Name },
                        { "picture", regionalVariant.Picture },
                        { "isInterchangeable", false },
                        { "isAlola", true }
                    }
                };
            }
        }

        private IEnumerable<Node> GenerateNodesForm(Pokemon pokemon)
        {
            foreach (var form in pokemon.Forms)
            {
                yield return new Node
                {
                    Id = form.GetCleanName(),
                    Labels = new List<string> { "Form" },
                    Properties = new Dictionary<string, object>
                    {
                        { "name", form.Name },
                        { "picture", form.Picture },
                        { "isInterchangeable", !NON_SWITCHABLE_FORMS.Contains(pokemon.NationalPokedexNumber) },
                        { "isAlola", false }
                    }
                };
            }
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

                if (pokemon.Evolutions.Any())
                {
                    foreach(var evolution in pokemon.Evolutions)
                    {
                        relationships.Add(new Relationship
                        {
                            Labels = new List<string> { "Evolve" },
                            NodeId1 = pokemon.GetCleanName(),
                            NodeId2 = evolution.Pokemon.GetCleanName(),
                            Properties = new Dictionary<string, object>
                            {
                                { "condition", evolution.Condition }
                            }
                        });
                    }
                }

                if (pokemon.RegionalVariants.Any())
                {
                    foreach(var regionalVariant in pokemon.RegionalVariants)
                    {
                        relationships.Add(new Relationship
                        {
                            Labels = new List<string> { "Has" },
                            NodeId1 = pokemon.GetCleanName(),
                            NodeId2 = regionalVariant.GetName(pokemon.GetCleanName())
                        });

                        relationships.AddRange(GenerateRelationshipTypes(regionalVariant.GetName(pokemon.GetCleanName()), regionalVariant.Types));
                    }
                }

                if (pokemon.Forms.Any())
                {
                    foreach(var form in pokemon.Forms)
                    {
                        relationships.Add(new Relationship
                        {
                            Labels = new List<string> { "Has" },
                            NodeId1 = pokemon.GetCleanName(),
                            NodeId2 = form.GetCleanName()
                        });

                        relationships.AddRange(GenerateRelationshipTypes(form.GetCleanName(), form.Types));
                    }
                }
            }

            return _neo4jGenerator.CreateRelationships(relationships);
        }

        private ICollection<Relationship> GenerateRelationshipTypes(string nodeId, ICollection<Models.Type> types)
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
