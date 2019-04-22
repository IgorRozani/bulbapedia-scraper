using BulbapediaScraper.Runner.Models;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Helpers
{

    public class TypeEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y) => x.Name == y.Name;

        public int GetHashCode(Type obj) => obj.Name.GetHashCode();
    }
}
