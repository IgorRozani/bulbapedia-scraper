using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Models
{
    public class Form
    {
        public Form(string name, string picture)
        {
            Name = name;
            Picture = picture;
        }

        public Form(string picture, ICollection<Type> types)
        {
            Picture = picture;
            Types = types;
        }

        public Form(string name, string picture, ICollection<Type> types) : this(name,picture)
        {
            Types = types;
        }

        public string Name { get; set; }

        public string Picture { get; set; }

        public ICollection<Type> Types { get; set; }

        public string GetName(string pokemonName) => pokemonName + Name.Replace("!", ((int)'!').ToString()).Replace("?", ((int)'?').ToString()).Replace(" ", string.Empty);
    }
}
