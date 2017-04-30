using System.Collections.Generic;
using System.Linq;
using QueryTables.Common;

namespace QueryTables.Core.Extensions {
    public static class QueryExtensions {

        public static BindedQuery<T> For<T>(this Query<T> query, IQueryable<T> source) {
            return new BindedQuery<T>(query, source);
        }
    }

    public class BindedQuery<T> {
        public Query<T> Query { get; set; }
        
        public IQueryable<T> Source { get; set; }

        public BindedQuery(Query<T> query, IQueryable<T> source) {
            this.Query = query;
            this.Source = source;
        }

        public BindedQuery<T> Filter(Dictionary<string, string> filters) {
            if (filters != null && filters.Any()) {
                this.Source = this.Query.Filter(this.Source, filters);
            }

            return this;
        }

        public BindedQuery<T> OrderBy(List<KeyValuePair<string, SortDirection>> sortings) {
            if (sortings != null && sortings.Any()) {
                this.Source = this.Query.OrderBy(this.Source, sortings);
            }
            
            return this;
        }

        public IQueryable<TResult> Project<TResult>() {
            return this.Query.Project<TResult>(this.Source);
        }
    }
}