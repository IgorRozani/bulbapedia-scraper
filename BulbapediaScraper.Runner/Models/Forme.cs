namespace BulbapediaScraper.Runner.Models
{
    public class Forme
    {
        public Forme(string name, string picture)
        {
            Name = name;
            Picture = picture;
        }

        public string Name { get; set; }

        public string Picture { get; set; }
    }
}
