using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Services.ScriptGenerator.Model
{

    public class Relationship
    {
        public Relationship()
        {
            Labels = new List<string>();
            Properties = new Dictionary<string, object>();
        }

        public string NodeId1 { get; set; }
        public string NodeId2 { get; set; }
        public List<string> Labels { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
