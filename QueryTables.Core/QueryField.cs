using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;

namespace QueryTables.Core
{
    public class QueryField<T>
    {
        public QueryField()
        {
            this.When = new Dictionary<object, Expression<Func<T, bool>>>();
            this.Where = new List<LambdaExpression>();
            this.SelectWhen = new Dictionary<object, object>();
        }

        public string Name { get; set; }

        public IFilterBuilder FilterBuilder { get; set; }

        public LambdaExpression Select { get; set; }

        public Dictionary<object, object> SelectWhen { get; set; }

        public object SelectElse { get; set; }

        /// <summary>
        /// Key: Filter value
        /// Value: Expression to apply as a filter to the  expression
        /// </summary>
        public Dictionary<object, Expression<Func<T, bool>>> When { get; set; }

        /// <summary>
        /// List of lambda expressions of the form {x => x.Property} or {x => someWorkOver(x)}.
        /// The filter value will be compared to the return of each expression according to the filter's operator.
        /// The final expression applied is the concatenation of each expression using an OR operator.
        /// </summary>
        public List<LambdaExpression> Where { get; set; }

        public bool? CaseSensitive { get; set; }

        public IQueryable<T> Filter(IQueryable<T> query, Filter filter)
        {
            if (!filter.Valid)
            {
                return query;
            }

            if (filter.Values.Where(v => v != null).Any(v => this.When.ContainsKey(v)))
            {
                return query.Where(this.When[filter.Value]);
            }

            Func<ExpressionBuilder, Filter, ExpressionBuilder> operatorFunc;
            switch (filter.Operator)
            {
                case FilterOperator.Equal:
                    operatorFunc = (b, f) => b.EqualTo(f.Value);
                    break;
                case FilterOperator.NotEqual:
                    operatorFunc = (b, f) => b.NotEqualTo(f.Value);
                    break;
                case FilterOperator.GreaterThan:
                    operatorFunc = (b, f) => b.GreaterThanThis(f.Value);
                    break;
                case FilterOperator.GreaterThanEqual:
                    operatorFunc = (b, f) => b.GreaterThanOrEqualTo(f.Value);
                    break;
                case FilterOperator.LessThan:
                    operatorFunc = (b, f) => b.LessThanThis(f.Value);
                    break;
                case FilterOperator.LessThanEqual:
                    operatorFunc = (b, f) => b.LessThanOrEqualTo(f.Value);
                    break;
                case FilterOperator.Between:
                    operatorFunc = (b, f) => b.Between(f.Values[0], f.Values[1]);
                    break;
                case FilterOperator.Contains:
                    if (this.CaseSensitive.GetValueOrDefault())
                    {
                        operatorFunc = (b, f) => b.Call<string>("Contains", f.Value.ToString());
                    }
                    else
                    {
                        operatorFunc = (b, f) => b.Call<string>("ToLower").Call<string>("Contains", f.Value.ToString().ToLower());
                    }

                    break;
                case FilterOperator.StartsWith:
                    if (this.CaseSensitive.GetValueOrDefault())
                    {
                        operatorFunc = (b, f) => b.Call<string>("StartsWith", f.Value.ToString());
                    }
                    else
                    {
                        operatorFunc = (b, f) => b.Call<string>("ToLower").Call<string>("StartsWith", f.Value.ToString().ToLower());
                    }

                    break;
                case FilterOperator.EndsWith:
                    if (this.CaseSensitive.GetValueOrDefault())
                    {
                        operatorFunc = (b, f) => b.Call<string>("EndsWith", f.Value.ToString());
                    }
                    else
                    {
                        operatorFunc = (b, f) => b.Call<string>("ToLower").Call<string>("EndsWith", f.Value.ToString().ToLower());
                    }

                    break;
                default:
                    throw new Exception("Unkown operator: " + filter.Operator);
            }

            var builder = ExpressionBuilder.Parameter<T>();
            var body = this.Where.Aggregate(
                ExpressionBuilder.False(),
                (current, key) =>
                    current.OrElse(
                        operatorFunc(
                            ExpressionBuilder.New(builder.Param, key.Body.Replace(key.Parameters[0], builder.Param)),
                            filter)
                            .Expression));

            return query.Where(builder.Lambda<T, bool>(body.Expression));
        }
    }
}
