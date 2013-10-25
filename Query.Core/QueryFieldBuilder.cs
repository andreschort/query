using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Query.Common.Extension;
using Query.Common.Util;
using Query.Core.Filters;
using Query.Core.Filters.Builders;

namespace Query.Core
{
    public class QueryFieldBuilder<T>
    {
        private bool withManualWhere;

        private bool withManualFilterType;

        public QueryField<T> Instance { get; set; }

        public QueryFieldBuilder<T> Create(string name)
        {
            this.Instance = new QueryField<T> {Name = name};
            
            this.withManualFilterType = false;
            this.withManualWhere = false;
            
            return this;
        }

        public QueryFieldBuilder<T> Create<E>(Expression<Func<T, E>> select)
        {
            this.Instance = new QueryField<T> {Name = @select.GetPropertyInfo().Name};

            this.withManualFilterType = false;
            this.withManualWhere = false;

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

        public QueryFieldBuilder<T> SelectWhen(object value, object result)
        {
            this.Instance.SelectWhen.Add(value, result);
            return this;
        }

        public QueryFieldBuilder<T> SelectWhen(Dictionary<object, object> transformation, object selectElse)
        {
            this.Instance.SelectWhen = transformation;
            this.Instance.SelectElse = selectElse;
            return this;
        }

        public QueryFieldBuilder<T> SelectElse(object selectElse)
        {
            this.Instance.SelectElse = selectElse;
            return this;
        }

        public QueryFieldBuilder<T> Where<E>(Expression<Func<T, E>> where)
        {
            if (!this.withManualWhere)
            {
                this.Instance.Where.Clear();
            }
            
            if (!this.withManualFilterType)
            {
                this.Instance.FilterBuilder = this.GetFilterBuilder(where.ReturnType);
            }

            this.withManualWhere = true;
            this.Instance.Where.Add(where);

            return this;
        }

        public QueryFieldBuilder<T> AddWhere<E>(Expression<Func<T, E>> where)
        {
            this.Instance.Where.Add(where);

            return this;
        }
        
        public QueryFieldBuilder<T> FilterAs(IFilterBuilder filterBuilder)
        {
            this.Instance.FilterBuilder = filterBuilder;
            return this;
        }

        public QueryFieldBuilder<T> FilterAsText()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof (string));
            return this;
        }

        public QueryFieldBuilder<T> FilterAsInteger()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof(int));
            return this;
        }

        public QueryFieldBuilder<T> FilterAsDecimal()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof(decimal));
            return this;
        }

        public QueryFieldBuilder<T> FilterAsDate()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof(DateTime));
            return this;
        }

        public QueryFieldBuilder<T> FilterAsBoolean()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof(bool));
            return this;
        }

        public QueryFieldBuilder<T> FilterAsList()
        {
            this.Instance.FilterBuilder = this.GetFilterBuilder(typeof (Enum));
            return this;
        }

        public QueryFieldBuilder<T> FilterWhen(object filterValue, Expression<Func<T, bool>> when)
        {
            this.Instance.When[filterValue] = when;

            return this;
        }

        public QueryFieldBuilder<T> TruncateTime()
        {
            var truncatedExpression =
                ExpressionBuilder.New(this.Instance.Select.Parameters[0], this.Instance.Select.Body).TruncateTime().Lambda();

            this.Instance.Select = truncatedExpression;
            this.Instance.Where.Clear();
            this.Instance.Where.Add(truncatedExpression);

            return this;
        }

        private IFilterBuilder GetFilterBuilder(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (type == typeof(string))
            {
                return new TextFilterBuilder();
            }

            if (type == typeof(int) || type == typeof(int?))
            {
                return new NumericFilterBuilder(type);
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return new BooleanFilterBuilder();
            }

            if (type.IsEnum)
            {
                return new ListFilterBuilder();
            }

            if (type == typeof(decimal) || type == typeof(double) || type == typeof(decimal?) || type == typeof(double?))
            {
                return new NumericFilterBuilder(type);
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DateFilterBuilder();
            }

            return new ListFilterBuilder();
        }

        public QueryFieldBuilder<T> CaseSensitive()
        {
            this.Instance.CaseSensitive = true;

            return this;
        }
    }
}