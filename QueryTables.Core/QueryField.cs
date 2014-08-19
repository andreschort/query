using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using QueryTables.Common.Extension;
using QueryTables.Common.Util;
using QueryTables.Core.Filters;
using QueryTables.Core.Filters.Builders;

namespace QueryTables.Core
{
    [DebuggerDisplay("Name: {Name}, Select:{Select}")]
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

        public bool CaseSensitive { get; set; }

        public bool NullSafe { get; set; }

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
                    operatorFunc = filter.Value is string
                                       ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                                         ((b, f) => b.CompareToThis(f.Value).GreaterThanThis(0))
                                       : ((b, f) => b.GreaterThanThis(f.Value));
                    break;
                case FilterOperator.GreaterThanEqual:
                    operatorFunc = filter.Value is string
                                       ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                                         ((b, f) => b.CompareToThis(f.Value).GreaterThanOrEqualTo(0))
                                       : ((b, f) => b.GreaterThanOrEqualTo(f.Value));
                    break;
                case FilterOperator.LessThan:
                    operatorFunc = filter.Value is string
                                       ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                                         ((b, f) => b.CompareToThis(f.Value).LessThanThis(0))
                                       : ((b, f) => b.LessThanThis(f.Value));
                    break;
                case FilterOperator.LessThanEqual:
                    operatorFunc = filter.Value is string
                                       ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                                         ((b, f) => b.CompareToThis(f.Value).LessThanOrEqualTo(0))
                                       : ((b, f) => b.LessThanOrEqualTo(f.Value));
                    break;
                case FilterOperator.Between:
                    operatorFunc = filter.Value is string
                                       ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                                         ((b, f) => b.CompareToThis(f.Values[0]).GreaterThanOrEqualTo(0)
                                                    .AndAlso(b.CompareToThis(f.Values[1]).LessThanOrEqualTo(0).Expression))
                                       : (b, f) => b.GreaterThanOrEqualTo(f.Values[0])
                                                    .AndAlso(b.LessThanOrEqualTo(f.Values[1]).Expression);
                    break;
                case FilterOperator.Contains:
                case FilterOperator.StartsWith:
                case FilterOperator.EndsWith:
                    operatorFunc = this.CaseSensitive
                        ? (Func<ExpressionBuilder, Filter, ExpressionBuilder>)
                            ((b, f) => b.Call<string>(filter.Operator.ToString(), f.Value.ToString()))
                        : ((b, f) => b.Call<string>("ToLower").Call<string>(filter.Operator.ToString(), f.Value.ToString().ToLower()));

                    break;

                default:
                    throw new Exception("Unkown operator: " + filter.Operator);
            }

            var param = Expression.Parameter(typeof(T));
            var current = ExpressionBuilder.False();
            foreach (var part in this.Where)
            {
                var part_body = ExpressionBuilder.New(param, part.Body.Replace(part.Parameters[0], param));
                var canBeNull = !part.ReturnType.IsValueType || Nullable.GetUnderlyingType(part.ReturnType) != null;
                if (this.NullSafe && canBeNull)
                {
                    var e = Expression.Condition(
                        part_body.NotNull().Expression,
                        operatorFunc(part_body, filter).Expression,
                        Expression.Constant(false));
                    current = current.OrElse(e);
                }
                else
                {
                    current = current.OrElse(operatorFunc(part_body, filter).Expression);
                }
            }

            return query.Where(Expression.Lambda<Func<T, bool>>(current.Expression, param));
        }
    }
}
