using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{
    public class Pokemon
    {
        public Pokemon()
        {
            RegionalVariants = new List<RegionalVariant>();
            Evolutions = new List<Evolution>();
            Types = new List<Type>();
            Formes = new List<Forme>();
            MegaEvolutions = new List<MegaEvolution>();
        }

        public int NationalPokedexNumber { get; set; }

        public string RegionalPokedexNumber { get; set; }

        public string Name { get; set; }

        public string Picture { get; set; }

        public string ProfileUrl { get; set; }

        public List<RegionalVariant> RegionalVariants { get; set; }

        public ICollection<Type> Types { get; set; }

        public ICollection<Evolution> Evolutions { get; set; }

        public ICollection<Forme> Formes { get; set; }

        public ICollection<MegaEvolution> MegaEvolutions { get; set; }

        public string GetCleanName()
        {
            return Name.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty)
                .Replace("♀", "F")
                .Replace("♂", "M")
                .Replace("'",string.Empty);
        }
    }
}