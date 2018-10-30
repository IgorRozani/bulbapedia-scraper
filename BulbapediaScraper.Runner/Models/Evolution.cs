namespace BulbapediaScraper.Runner.Models
{
    public class Evolution
    {
        public Evolution(Pokemon pokemon)
        {
            Pokemon = pokemon;
        }

        public Evolution(string requirement, Pokemon pokemon) : this(pokemon)
        {
            Requirement = requirement;
        }

        public string Requirement { get; set; }

        public Pokemon Pokemon { get; set; }
    }
}
