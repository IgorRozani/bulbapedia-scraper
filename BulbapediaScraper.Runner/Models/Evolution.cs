namespace BulbapediaScraper.Runner.Models
{
    public class Evolution
    {
        public Evolution(Pokemon pokemon)
        {
            Pokemon = pokemon;
        }

        public Evolution(string condition, Pokemon pokemon) : this(pokemon)
        {
            Condition = condition;
        }

        public string Condition { get; set; }

        public Pokemon Pokemon { get; set; }
    }
}
