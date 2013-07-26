using System;
using System.Linq.Expressions;
using Common.Extension;

namespace Query
{
    public class QueryFieldBuilder<T>
    {
        public QueryField<T> Instance { get; set; }

        public QueryFieldBuilder<T> Create(string name)
        {
            this.Instance = new QueryField<T> {Name = name};
            return this;
        }

        public QueryFieldBuilder<T> Create<E>(Expression<Func<T, E>> select)
        {
            this.Instance = new QueryField<T> {Name = @select.GetPropertyInfo().Name};

            return this.Select(select);
        }

        public QueryFieldBuilder<T> Select<E>(Expression<Func<T, E>> select)
        {
            this.Instance.Select = select;

            return this;
        }
    }
}