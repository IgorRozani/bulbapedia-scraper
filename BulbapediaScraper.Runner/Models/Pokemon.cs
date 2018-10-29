using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{
    public class Pokemon
    {
        public Pokemon()
        {
            RegionalVariants = new List<RegionalVariant>();
        }

        public int NationalPokedexNumber { get; set; }

        public string RegionalPokedexNumber { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        public List<RegionalVariant> RegionalVariants { get; set; }

        public ICollection<Type> Types { get; set; }
    }
}
