using System.Collections.Generic;

namespace BulbapediaScraper.Runner.ScriptGenerator.Model
{
    public class Node
    {
        public Node()
        {
            Labels = new List<string>();
            Properties = new Dictionary<string, object>();
        }

        public string Id { get; set; }
        public List<string> Labels { get; set; }
        public Dictionary<string,object> Properties { get; set; }
    }
}
