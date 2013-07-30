using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Extension;
using Common.Util;

namespace Query
{
    public class Query<T>
    {
        public Query()
        {
            this.Fields = new List<QueryField<T>>();
        }

        public List<QueryField<T>> Fields { get; set; }

        public IQueryable Apply(IQueryable<T> query, Dictionary<string, string> values)
        {
            FilterBuilder filterBuilder = new FilterBuilder();
            var filters = from value in values
                          let field = this.Fields.Find(x => x.Name.Equals(value.Key))
                          select filterBuilder.Create(field, value.Value);

            return this.Apply(query, filters);
        }

        public IQueryable Apply(IQueryable<T> query, IEnumerable<Filter> filters)
        {
            return this.Project(this.Filter(query, filters));
        }

        /// <summary>
        /// Create a select new {} expression and apply it to queryable.
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public IQueryable Project(IQueryable<T> queryable)
        {
            var propertyInfos = this.Fields.ToDictionary(x => x.Name, y => y.Select.ReturnType);
            var dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(propertyInfos);
            var selectExpressions = this.Fields.ToDictionary(x => x.Name, x => x.Select);

            ParameterExpression parameter = Expression.Parameter(queryable.ElementType, "t");
            var bindings = dynamicType.GetFields()
                                      .Select(field =>
                                          {
                                              // Replace original parameter with our new parameter
                                              var originalParameter = selectExpressions[field.Name].Parameters[0];
                                              var expression =
                                                  selectExpressions[field.Name].Body.Replace(originalParameter,
                                                                                             parameter);

                                              // Ensure the expression return type and the field type match
                                              //expression = Expression.Convert(expression, field.FieldType);

                                              // Bind field with expression
                                              return Expression.Bind(field, expression);
                                          });

            Expression selector = Expression.Lambda(
                Expression.MemberInit(Expression.New(dynamicType), bindings),
                parameter);

            return queryable.Provider.CreateQuery(Expression.Call(typeof (Queryable), "Select",
                                                                  new[] {queryable.ElementType, dynamicType},
                                                                  Expression.Constant(queryable), selector));
        }

        public IQueryable<T> Filter(IQueryable<T> query, IEnumerable<Filter> filters)
        {
            return filters.Aggregate(query, this.Filter);
        }

        public IQueryable<T> Filter(IQueryable<T> query, Filter filter)
        {
            var field = this.Fields.FirstOrDefault(x => x.Name.Equals(filter.Name));

            return field == null ? query : field.Filter(query, filter);
        }
    }
}
