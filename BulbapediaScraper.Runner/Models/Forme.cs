using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{
    public class Forme
    {
        public Forme(string name, string picture)
        {
            Name = name;
            Picture = picture;
        }

        public Forme(string picture, ICollection<Type> types)
        {
            Picture = picture;
            Types = types;
        }

        public Forme(string name, string picture, ICollection<Type> types) : this(name,picture)
        {
            Types = types;
        }

        public string Name { get; set; }

        public string Picture { get; set; }

        public ICollection<Type> Types { get; set; }
    }
}
