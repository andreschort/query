using System.Collections.Generic;
using System.Linq;

namespace QueryTables.Common.Extension
{
    public static class QueryableExtensions
    {
        public static List<dynamic> ToDynamic(this IQueryable list)
        {
            return Enumerable.Cast<object>(list).Cast<dynamic>().ToList();
        }
    }
}
