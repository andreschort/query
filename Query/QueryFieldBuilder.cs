using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Extension;
using Common.Util;

namespace Query
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

        public QueryFieldBuilder<T> SelectWhen(Dictionary<object, object> transformation)
        {
            this.Instance.SelectWhen = transformation;
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
                this.Instance.FilterType = this.GetFilterType(where.ReturnType);
            }

            this.withManualWhere = true;
            this.Instance.Where.Add(where);

            return this;
        }

        public QueryFieldBuilder<T> FilterAs(FilterType filterType)
        {
            this.Instance.FilterType = filterType;
            this.withManualFilterType = true;
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

        private FilterType GetFilterType(Type type)
        {
            if (type == null)
            {
                return FilterType.None;
            }

            if (type == typeof(string))
            {
                return FilterType.Text;
            }

            if (type == typeof(int))
            {
                return FilterType.Integer;
            }

            if (type == typeof(bool))
            {
                return FilterType.Boolean;
            }

            if (type.IsEnum)
            {
                return FilterType.List;
            }

            if (type == typeof(decimal) || type == typeof(double))
            {
                return FilterType.Decimal;
            }

            if (type == typeof (DateTime))
            {
                return FilterType.Date;
            }

            return FilterType.List;
        }
    }
}