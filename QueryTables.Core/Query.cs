using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryTables.Common;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core.Filters;

namespace QueryTables.Core
{
    public class Query<T>
    {
        public Query()
        {
            this.Fields = new List<QueryField<T>>();
        }

        public List<QueryField<T>> Fields { get; set; }

        public Type ResultType { get; private set; }

        public bool NullSafe { get; set; }

        /// <summary>
        /// Create a select new {} expression and apply it to lambda expression.
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public IQueryable Project(IQueryable<T> queryable)
        {
            var fields = this.Fields.ToDictionary(x => x.Name, y => y.SelectElse == null ? y.Select.ReturnType : y.SelectElse.GetType());

            return this.Project(queryable, LinqRuntimeTypeBuilder.GetDynamicType(fields));
        }

        public IQueryable<E> Project<E>(IQueryable<T> queryable)
        {
            return (IQueryable<E>)this.Project(queryable, typeof(E));
        }

        public IQueryable<T> Filter(IQueryable<T> query, Dictionary<string, string> values)
        {
            if (values == null || !values.Any())
            {
                return query;
            }

            var fieldsNotFound = values.Keys.Except(this.Fields.Select(x => x.Name)).ToList();

            if (fieldsNotFound.Any())
            {
                throw new Exception("Fields not found: " + fieldsNotFound.Aggregate((c, a) => c + ", " + a));
            }

            var filters = from value in values
                          let field = this.Fields.Find(x => x.Name.Equals(value.Key))
                          select field.FilterBuilder.Create(field, value.Value);

            return this.Filter(query, filters);
        }

        public IQueryable<T> Filter(IQueryable<T> query, IEnumerable<Filter> filters)
        {
            return filters.Aggregate(query, this.Filter);
        }

        public IQueryable<T> Filter(IQueryable<T> query, Filter filter)
        {
            if (filter == null)
            {
                return query;
            }

            var field = this.Fields.FirstOrDefault(x => x.Name.Equals(filter.Name));

            return field == null ? query : field.Filter(query, filter);
        }

        public IQueryable<T> OrderBy(IQueryable<T> query, List<KeyValuePair<string, SortDirection>> sortings)
        {
            return this.OrderBy<int>(query, sortings);
        }

        public IQueryable<T> OrderBy<E>(IQueryable<T> query, List<KeyValuePair<string, SortDirection>> sortings, Expression<Func<T, E>> defaultSort = null)
        {
            if (sortings == null || !sortings.Any())
            {
                return defaultSort == null ? query : query.OrderBy(defaultSort);
            }

            // first sorting
            var orderedQuery = this.OrderBy(query, sortings[0].Key, sortings[0].Value);

            // the rest
            return sortings.GetRange(1, sortings.Count - 1)
                .Aggregate(orderedQuery, (current, sorting) => this.ThenBy(current, sorting.Key, sorting.Value));
        }

        public IOrderedQueryable<T> OrderBy(IQueryable<T> query, string fieldName, SortDirection sortDirection)
        {
            return this.OrderBy(query, fieldName, sortDirection, "OrderBy");
        }

        public IOrderedQueryable<T> ThenBy(IQueryable<T> query, string fieldName, SortDirection sortDirection)
        {
            return this.OrderBy(query, fieldName, sortDirection, "ThenBy");
        }

        public QueryFieldBuilder<T> AddField(string name)
        {
            var builder = new QueryFieldBuilder<T>();

            var field = builder.Create(name).Instance;
            field.NullSafe = this.NullSafe;
            this.Fields.Add(field);

            return builder;
        }

        public QueryFieldBuilder<T> AddField<E>(Expression<Func<T, E>> select)
        {
            var builder = new QueryFieldBuilder<T>();

            var field = builder.Create(@select).Instance;
            field.NullSafe = this.NullSafe;
            this.Fields.Add(field);

            return builder;
        }

        private IOrderedQueryable<T> OrderBy(IQueryable<T> query, string fieldName, SortDirection sortDirection, string methodName)
        {
            var field = this.Fields.First(x => x.Name.Equals(fieldName));
            
            if (sortDirection.Equals(SortDirection.Descending))
            {
                methodName += "Descending";
            }

            var methodInfo = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2);

            return (IOrderedQueryable<T>)methodInfo.MakeGenericMethod(typeof(T), field.Select.ReturnType).Invoke(null, new object[] { query, field.Select });
        }
        
        private IQueryable Project(IQueryable<T> queryable, Type resultType)
        {
            this.ResultType = resultType;
            var parameter = Expression.Parameter(queryable.ElementType, "t");

            var bindings = this.Fields.Select(field =>
                {
                    var property = this.ResultType.GetProperty(field.Name);

                    if (property == null)
                    {
                        throw new Exception(string.Format("Property {0} not found in type {1}", field.Name, this.ResultType.FullName));
                    }

                    // Replace original parameter with our new parameter
                    var originalParameter = field.Select.Parameters[0];
                    var expression = field.Select.Body.Replace(originalParameter, parameter);

                    if (field.SelectElse == null)
                    {
                        // Ensure the expression return type and the field type match
                        expression = Expression.Convert(expression, property.PropertyType);
                    }
                    else
                    {
                        expression = this.CreateSelectWhen(
                            expression,
                            new Dictionary<object, object>(field.SelectWhen),
                            field.SelectElse);
                    }

                    // Bind field with expression
                    return Expression.Bind(property, expression);
                });

            var selector = Expression.Lambda(
                Expression.MemberInit(Expression.New(this.ResultType), bindings),
                parameter);

            var selectExpression = Expression.Call(
                typeof(Queryable),
                "Select",
                new[] { queryable.ElementType, this.ResultType },
                queryable.Expression,
                Expression.Quote(selector));

            return queryable.Provider.CreateQuery(selectExpression);
        }

        /// <summary>
        /// Creates an Expression of the form:
        /// target.Equals(key1)
        ///     ? value1
        ///     : target.Equals(key2)
        ///         ? value2
        ///         : selectElse
        /// </summary>
        /// <param name="target"></param>
        /// <param name="selectWhen"></param>
        /// <param name="selectElse"></param>
        /// <returns></returns>
        private Expression CreateSelectWhen(Expression target, Dictionary<object, object> selectWhen, object selectElse)
        {
            if (!selectWhen.Any())
            {
                return Expression.Constant(selectElse);
            }

            var keyValuePair = selectWhen.ElementAt(0);
            selectWhen.Remove(keyValuePair.Key);

            return Expression.Condition(
                Expression.Equal(target, Expression.Constant(keyValuePair.Key)),
                Expression.Constant(keyValuePair.Value),
                this.CreateSelectWhen(target, selectWhen, selectElse));
        }
    }
}
