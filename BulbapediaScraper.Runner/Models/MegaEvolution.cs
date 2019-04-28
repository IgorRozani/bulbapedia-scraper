using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{

    public class MegaEvolution
    {
        public MegaEvolution()
        {
            Types = new List<Type>();
            MegaStone = new MegaStone();
        }

        public string Name { get; set; }

        public MegaStone MegaStone { get; set; }

        public ICollection<Type> Types { get; set; }

        public string Picture { get; set; }
    }
}