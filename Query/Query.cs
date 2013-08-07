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
            var fields = this.Fields.ToDictionary(x => x.Name, y => y.SelectElse == null ? y.Select.ReturnType : y.SelectElse.GetType());
            var dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(fields);

            ParameterExpression parameter = Expression.Parameter(queryable.ElementType, "t");
            var bindings = dynamicType.GetFields()
                                      .Select(field =>
                                          {
                                              var queryField = this.Fields.First(x => x.Name.Equals(field.Name));

                                              // Replace original parameter with our new parameter
                                              var originalParameter = queryField.Select.Parameters[0];
                                              var expression = queryField.Select.Body.Replace(originalParameter, parameter);

                                              // Ensure the expression return type and the field type match
                                              //expression = Expression.Convert(expression, field.FieldType);

                                              if (queryField.SelectElse != null)
                                              {
                                                  expression = this.CreateSelectWhen(
                                                      expression,
                                                      new Dictionary<object, object>(queryField.SelectWhen),
                                                      queryField.SelectElse);
                                              }

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
