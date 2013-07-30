using System;
using System.Linq.Expressions;
using Common.Extension;

namespace Query
{
    public class QueryFieldBuilder<T>
    {
        private bool withManualWhere;

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

            if (!this.withManualWhere)
            {
                this.Where(select);
                this.withManualWhere = false;
            }

            return this;
        }

        public QueryFieldBuilder<T> Where<E>(Expression<Func<T, E>> where)
        {
            if (!this.withManualWhere)
            {
                this.Instance.Where.Clear();
            }

            this.withManualWhere = true;
            this.Instance.Where.Add(where);

            return this;
        }
    }
}