using BulbapediaScraper.Runner.ScriptGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulbapediaScraper.Runner.ScriptGenerator
{
    public class Neo4jGenerator
    {
        private const string CREATE = "CREATE ";

        public string CreateNodes(IList<Node> nodes)
        {
            var scriptStringBuilder = new StringBuilder();
            scriptStringBuilder.AppendLine(CREATE);
            foreach (var node in nodes)
            {
                scriptStringBuilder.Append(GenerateNode(node));
                if (node != nodes.LastOrDefault())
                    scriptStringBuilder.AppendLine(", ");
            }
            return scriptStringBuilder.ToString();
        }

        public string CreateNode(Node node) =>
            CREATE + GenerateNode(node);

        private string GenerateNode(Node node)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.Append('(').Append(node.Id);

            foreach (var label in node.Labels)
                scriptBuilder.Append(':').Append(label);

            if (node.Properties.Any())
            {
                scriptBuilder.Append(" { ");

                foreach (var property in node.Properties)
                {
                    if (property.Value == null)
                        continue;

                    scriptBuilder.Append(property.Key).Append(":");

                    switch (Type.GetTypeCode(property.Value.GetType()))
                    {
                        case TypeCode.String:
                            scriptBuilder.Append("\"").Append(property.Value).Append("\"");
                            break;
                        case TypeCode.DateTime:
                            var datetime = (DateTime)property.Value;
                            scriptBuilder.Append("datetime({year:").Append(datetime.Year).Append(", month:").Append(datetime.Month).Append(", day:").Append(datetime.Day).Append(", hour:").Append(datetime.Hour).Append(", minute:").Append(datetime.Minute).Append(", second:").Append(datetime.Second).Append("})");
                            break;
                        default:
                            scriptBuilder.Append(property.Value);
                            break;
                    }

                    if (property.Key != node.Properties.Keys.LastOrDefault())
                        scriptBuilder.Append(", ");
                }

                scriptBuilder.Append("}");
            }

            scriptBuilder.Append(")");

            return scriptBuilder.ToString();
        }


        public string CreateRelationships(IList<Relationship> relationships)
        {
            var scriptStringBuilder = new StringBuilder();
            scriptStringBuilder.AppendLine(CREATE);
            foreach (var relationship in relationships)
            {
                scriptStringBuilder.Append(GenerateRelationship(relationship));
                if (relationship != relationships.LastOrDefault())
                    scriptStringBuilder.AppendLine(", ");
            }
            return scriptStringBuilder.ToString();
        }

        public string CreateRelationship(Relationship relationship) =>
            CREATE + GenerateRelationship(relationship);

        private string GenerateRelationship(Relationship relationship)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.Append('(').Append(relationship.NodeId1).Append(")-[");

            foreach (var label in relationship.Labels)
                scriptBuilder.Append(':').Append(label);

            if (relationship.Properties.Any())
            {
                scriptBuilder.Append(" { ");

                foreach (var property in relationship.Properties)
                {
                    scriptBuilder.Append(property.Key).Append(":");

                    switch (Type.GetTypeCode(property.Value.GetType()))
                    {
                        case TypeCode.String:
                            scriptBuilder.Append("'").Append(property.Value).Append("'");
                            break;
                        case TypeCode.DateTime:
                            var datetime = (DateTime)property.Value;
                            scriptBuilder.Append("datetime({year:").Append(datetime.Year).Append(", month:").Append(datetime.Month).Append(", day:").Append(datetime.Day).Append(", hour:").Append(datetime.Hour).Append(", minute:").Append(datetime.Minute).Append(", second:").Append(datetime.Second).Append("})");
                            break;
                        default:
                            scriptBuilder.Append(property.Value);
                            break;
                    }

                    if (property.Key != relationship.Properties.Keys.LastOrDefault())
                        scriptBuilder.Append(", ");
                }

                scriptBuilder.Append("}");
            }

            scriptBuilder.Append("]->(").Append(relationship.NodeId2).Append(")");

            return scriptBuilder.ToString();
        }
    }
}
