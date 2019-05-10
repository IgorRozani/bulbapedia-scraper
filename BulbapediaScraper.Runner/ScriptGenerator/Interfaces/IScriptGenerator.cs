using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.ScriptGenerator.Interfaces
{
    public interface IScriptGenerator
    {
        string Generate(ICollection<Pokemon> pokemons);
    }
}
