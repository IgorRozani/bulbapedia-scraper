namespace BulbapediaScraper.Runner.Models
{
    public class Move
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Category Category { get; set; }
        public Contest Contest { get; set; }
        public int? PowerPoint { get; set; }
        public int? Power { get; set; }
        public decimal? Acurracy { get; set; }
        public bool IsMysteryDungeonExclusive { get; set; }
        public bool IsColosseumOrXDExclusive { get; set; }
    }
}
