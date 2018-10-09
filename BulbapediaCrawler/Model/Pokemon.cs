using System.Collections.Generic;

namespace BulbapediaCrawler.Model
{
    public class Pokemon
    {
        public int NationalPokedexNumber { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public RegionalVariant RegionalVariant { get; set; }

        public ICollection<Is> IsTypes { get; set; }
        public List<Evolve> Evolves { get; set; }
    }
}
