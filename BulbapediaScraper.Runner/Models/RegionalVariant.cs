using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{

    public class RegionalVariant
    {
        public RegionalVariant(string picture, ICollection<Type> types)
        {
            Picture = picture;
            Types = types;
        }

        public string Picture { get; set; }

        public ICollection<Type> Types { get; set; }

        public string GetName(string originalPokemon) => originalPokemon + "Alola";
    }
}
