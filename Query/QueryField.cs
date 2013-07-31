using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Extension;
using Common.Util;

namespace Query
{
    public class QueryField<T>
    {
        public string Name { get; set; }

        public FilterType FilterType { get; set; }

        public LambdaExpression Select { get; set; }

        /// <summary>
        /// Key: Filter value
        /// Value: Expression to apply as a filter to the IQueryable
        /// </summary>
        public Dictionary<object, Expression<Func<T, bool>>> When { get; set; }

        /// <summary>
        /// List of lambda expressions of the form {x => x.Property} or {x => someWorkOver(x)}.
        /// The filter value will be compared to the return of each expression according to the filter's operator.
        /// The final expression applied is the concatenation of each expression using an OR operator.
        /// </summary>
        public List<LambdaExpression> Where { get; set; }

        public QueryField()
        {
            this.When = new Dictionary<object, Expression<Func<T, bool>>>();
            this.Where = new List<LambdaExpression>();
        }

        public IQueryable<T> Filter(IQueryable<T> query, Filter filter)
        {
            if (!filter.Valid)
            {
                return query;
            }

            if (filter.Values.Any(value => value != null && this.When.ContainsKey(value)))
            {
                return query.Where(this.When[filter.Value]);
            }

            Func<ExpressionBuilder, Filter, ExpressionBuilder> operatorFunction;

            switch (filter.Operator)
            {
                case FilterOperator.Equal:
                    operatorFunction = (b, f) => b.EqualTo(f.Value);
                    break;
                case FilterOperator.NotEqual:
                    operatorFunction = (b, f) => b.NotEqualTo(f.Value);
                    break;
                case FilterOperator.GreaterThan:
                    operatorFunction = (b, f) => b.GreaterThanThis(f.Value);
                    break;
                case FilterOperator.GreaterThanEqual:
                    operatorFunction = (b, f) => b.GreaterThanOrEqualTo(f.Value);
                    break;
                case FilterOperator.LessThan:
                    operatorFunction = (b, f) => b.LessThanThis(f.Value);
                    break;
                case FilterOperator.LessThanEqual:
                    operatorFunction = (b, f) => b.LessThanOrEqualTo(f.Value);
                    break;
                case FilterOperator.Between:
                    operatorFunction = (b, f) => b.Between(f.Values[0], f.Values[1]);
                    break;
                case FilterOperator.Contains:
                    operatorFunction = (b, f) => b.Call<string>("ToLower").Call<string>("Contains", f.Value.ToString().ToLower());
                    break;
                case FilterOperator.StartsWith:
                    operatorFunction = (b, f) => b.Call<string>("ToLower").Call<string>("StartsWith", f.Value.ToString().ToLower());
                    break;
                case FilterOperator.EndsWith:
                    operatorFunction = (b, f) => b.Call<string>("ToLower").Call<string>("EndsWith", f.Value.ToString().ToLower());
                    break;
                default:
                    throw new Exception("Unkown operator: " + filter.Operator);
            }

            var builder = ExpressionBuilder.Parameter<T>();
            ExpressionBuilder body = this.Where.Aggregate(
                ExpressionBuilder.False(),
                (current, key) =>
                current.OrElse(
                    operatorFunction(
                        ExpressionBuilder.New(builder.Param, key.Body.Replace(key.Parameters[0], builder.Param)),
                        filter)
                    .Expression));

            return query.Where(builder.Lambda<T, bool>(body.Expression));
        }
    }
}
