﻿using BulbapediaScraper.Runner.Models;
using BulbapediaScraper.Runner.Models.Comparers;
using BulbapediaScraper.Runner.Services.ScriptGenerator.Helper;
using BulbapediaScraper.Runner.Services.ScriptGenerator.Interfaces;
using BulbapediaScraper.Runner.Services.ScriptGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulbapediaScraper.Runner.Services.ScriptGenerator.Services
{
    public class ScriptGenerator : IScriptGenerator
    {
        private readonly INeo4jGenerator _neo4jGenerator;

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

        public ScriptGenerator(INeo4jGenerator neo4jGenerator)
        {
            _neo4jGenerator = neo4jGenerator;
        }

        public string Generate(ICollection<Pokemon> pokemons)
        {
            var scriptBuilder = new StringBuilder();

            // Nodes
            var types = pokemons.SelectMany(p => p.Types).Distinct(new TypeEqualityComparer()).ToList();
            scriptBuilder.AppendLine(GenerateNodeTypes(types));

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
                        { "name",type.Name }
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
                    Id = ScriptHelper.GetNodeId(pokemon.Name),
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
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(megaEvolutionNodes)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(regionalVarianteNodes)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateNodes(formsNodes);
        }

        private IEnumerable<Node> GenerateNodesMegaEvolution(Pokemon pokemon)
        {
            foreach (var megaEvolution in pokemon.MegaEvolutions)
            {
                yield return new Node
                {
                    Id = ScriptHelper.GetNodeId(megaEvolution.Name),
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
                    Id = ScriptHelper.GetNodeId(regionalVariant.GetName(pokemon.Name)),
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
                    Id = ScriptHelper.GetNodeId(form.GetName(pokemon.Name)),
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
            var typeRelationships = new List<Relationship>();
            var megaEvolutionRelationships = new List<Relationship>();
            var evolutionRelatioships = new List<Relationship>();
            var regionalVariantRelationships = new List<Relationship>();
            var formRelationships = new List<Relationship>();

            foreach (var pokemon in pokemons)
            {
                typeRelationships.AddRange(GenerateRelationshipTypes(ScriptHelper.GetNodeId(pokemon.Name), pokemon.Types));

                if (pokemon.MegaEvolutions.Any())
                {
                    foreach (var megaEvolution in pokemon.MegaEvolutions)
                    {
                        megaEvolutionRelationships.Add(new Relationship
                        {
                            Labels = new List<string> { "MEGA_EVOLVE" },
                            NodeId1 = ScriptHelper.GetNodeId(pokemon.Name),
                            NodeId2 = ScriptHelper.GetNodeId(megaEvolution.Name),
                            Properties = new Dictionary<string, object>
                            {
                                { "megaStone", megaEvolution.MegaStone.Name },
                                { "image", megaEvolution.Picture }
                            }
                        });

                        typeRelationships.AddRange(GenerateRelationshipTypes(ScriptHelper.GetNodeId(megaEvolution.Name), megaEvolution.Types));
                    }
                }

                if (pokemon.Evolutions.Any())
                {
                    foreach (var evolution in pokemon.Evolutions)
                    {
                        evolutionRelatioships.Add(new Relationship
                        {
                            Labels = new List<string> { "EVOLVE" },
                            NodeId1 = ScriptHelper.GetNodeId(pokemon.Name),
                            NodeId2 = ScriptHelper.GetNodeId(evolution.Pokemon.Name),
                            Properties = new Dictionary<string, object>
                            {
                                { "condition", evolution.Condition }
                            }
                        });
                    }
                }

                if (pokemon.RegionalVariants.Any())
                {
                    foreach (var regionalVariant in pokemon.RegionalVariants)
                    {
                        regionalVariantRelationships.Add(new Relationship
                        {
                            Labels = new List<string> { "HAS" },
                            NodeId1 = ScriptHelper.GetNodeId(pokemon.Name),
                            NodeId2 = regionalVariant.GetName(ScriptHelper.GetNodeId(pokemon.Name))
                        });

                        typeRelationships.AddRange(GenerateRelationshipTypes(ScriptHelper.GetNodeId(regionalVariant.GetName(pokemon.Name)), regionalVariant.Types));
                    }
                }

                if (pokemon.Forms.Any())
                {
                    foreach (var form in pokemon.Forms)
                    {
                        formRelationships.Add(new Relationship
                        {
                            Labels = new List<string> { "HAS" },
                            NodeId1 = ScriptHelper.GetNodeId(pokemon.Name),
                            NodeId2 = ScriptHelper.GetNodeId(form.GetName(pokemon.Name))
                        });

                        typeRelationships.AddRange(GenerateRelationshipTypes(ScriptHelper.GetNodeId(form.GetName(pokemon.Name)), form.Types));
                    }
                }
            }

            return _neo4jGenerator.CreateRelationships(evolutionRelatioships)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateRelationships(megaEvolutionRelationships)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateRelationships(regionalVariantRelationships)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateRelationships(formRelationships)
                + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + _neo4jGenerator.CreateRelationships(typeRelationships);
        }

        private ICollection<Relationship> GenerateRelationshipTypes(string nodeId, ICollection<Models.Type> types)
        {
            var relationships = new List<Relationship>();

            foreach (var type in types)
            {
                relationships.Add(new Relationship
                {
                    Labels = new List<string> { "IS" },
                    NodeId1 = nodeId,
                    NodeId2 = type.Name
                });
            }
            return relationships;
        }
    }
}
