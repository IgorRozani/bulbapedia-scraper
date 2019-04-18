using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BulbapediaScraper.Runner.ScriptGenerator
{
    public class Neo4jGenerator
    {
        public string CreateNode(string nodeId = null, IDictionary<string, object> properties = null, params string[] labels)
        {
            var scriptBuilder = new StringBuilder();
            scriptBuilder.Append('(').Append(nodeId);

            foreach (var label in labels)
                scriptBuilder.Append(':').Append(label);

            if(properties != null)
            {
                scriptBuilder.Append(" { ");

                foreach (var property in properties)
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

                    if (property.Key != properties.Keys.LastOrDefault())
                        scriptBuilder.Append(", ");
                }

                scriptBuilder.Append("}");
            }

            scriptBuilder.Append(" )");

            return scriptBuilder.ToString();
        }
    }
}
