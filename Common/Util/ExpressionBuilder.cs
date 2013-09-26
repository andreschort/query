using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Util
{
    public class ExpressionBuilder
    {
        private ExpressionBuilder()
        {
        }

        private ExpressionBuilder(ParameterExpression param, Expression expression)
        {
            this.Param = param;
            this.Expression = expression;
        }

        public ParameterExpression Param { get; private set; }

        public Expression Expression { get; private set; }

        public static ExpressionBuilder New(ParameterExpression param, Expression expression)
        {
            return new ExpressionBuilder(param, expression);
        }

        public static ExpressionBuilder Parameter<T>()
        {
            return new ExpressionBuilder
                {
                    Param = Expression.Parameter(typeof(T), string.Empty)
                };
        }

        public static ExpressionBuilder Constant<T>(T constant)
        {
            return new ExpressionBuilder { Expression = Expression.Convert(Expression.Constant(constant), typeof(T)) };
        }

        public static ExpressionBuilder True()
        {
            return new ExpressionBuilder { Expression = Expression.Constant(true) };
        }

        public static ExpressionBuilder False()
        {
            return new ExpressionBuilder { Expression = Expression.Constant(false) };
        }

        public static LambdaExpression Build<T, E>(Expression<Func<T, E>> expression)
        {
            return expression;
        }

        public static Expression<Func<TModel, TToProperty>> Cast<TModel, TFromProperty, TToProperty>(Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }

        public ExpressionBuilder PropertyOrField(string name)
        {
            return new ExpressionBuilder(this.Param, BuildPropertyExpression(this.Param, name));
        }

        public ExpressionBuilder PropertyOrField<T>(string name)
        {
            return this.PropertyOrField(name, typeof(T));
        }

        public ExpressionBuilder PropertyOrField(string name, Type type)
        {
            return new ExpressionBuilder(
                this.Param,
                Expression.Convert(this.PropertyOrField(name).Expression, type));
        }

        public ExpressionBuilder Call<T>(string name)
        {
            return this.Call(typeof(T), name);
        }

        public ExpressionBuilder Call(Type type, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ExpressionBuilder(this.Param, this.Expression);
            }

            return this.Call(type.GetMethod(name, new Type[] { }));
        }

        public ExpressionBuilder Call(MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                return new ExpressionBuilder(this.Param, this.Expression);
            }

            var methodCallExpression = methodInfo.IsStatic
                                           ? Expression.Call(methodInfo, this.Expression)
                                           : Expression.Call(this.Expression, methodInfo);

            return new ExpressionBuilder(this.Param, methodCallExpression);
        }

        public ExpressionBuilder Call<T>(string name, object arg)
        {
            var exp = Expression.Call(this.Expression, typeof(T).GetMethod(name, new[] { arg.GetType() }), Expression.Constant(arg));

            return new ExpressionBuilder(this.Param, this.Expression = exp);
        }

        public ExpressionBuilder TruncateTime()
        {
            var exp = Expression.Call(typeof (EntityFunctions).GetMethod("TruncateTime", new[] {typeof (DateTime?)}),
                                      Expression.Convert(this.Expression, typeof (DateTime?)));

            return new ExpressionBuilder(this.Param, exp);
        }

        public ExpressionBuilder Convert<T>()
        {
            return new ExpressionBuilder(this.Param, Expression.Convert(this.Expression, typeof(T)));
        }

        public ExpressionBuilder Equal(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.Equal(this.Expression, converted));
        }

        public ExpressionBuilder EqualTo<T>(T right)
        {
            return this.Equal(Expression.Constant(right));
        }

        public ExpressionBuilder NotEqual(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.NotEqual(this.Expression, converted));
        }

        public ExpressionBuilder NotEqualTo<T>(T right)
        {
            return this.NotEqual(Expression.Constant(right));
        }

        public ExpressionBuilder GreaterThan(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.GreaterThan(this.Expression, converted));
        }

        public ExpressionBuilder GreaterThanThis<T>(T right)
        {
            return this.GreaterThan(Expression.Constant(right));
        }

        public ExpressionBuilder GreaterThanOrEqual(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.GreaterThanOrEqual(this.Expression, converted));
        }

        public ExpressionBuilder GreaterThanOrEqualTo<T>(T right)
        {
            return this.GreaterThanOrEqual(Expression.Constant(right));
        }

        public ExpressionBuilder LessThan(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.LessThan(this.Expression, converted));
        }

        public ExpressionBuilder LessThanThis<T>(T right)
        {
            return this.LessThan(Expression.Constant(right));
        }

        public ExpressionBuilder LessThanOrEqual(Expression right)
        {
            var converted = Expression.Convert(right, this.Expression.Type); // convert to ensure the expressions types match
            return new ExpressionBuilder(this.Param, Expression.LessThanOrEqual(this.Expression, converted));
        }

        public ExpressionBuilder LessThanOrEqualTo<T>(T right)
        {
            return this.LessThanOrEqual(Expression.Constant(right));
        }

        public ExpressionBuilder AndAlso(Expression right)
        {
            return this.AndAlso(this.Expression, right);
        }

        public ExpressionBuilder OrElse(Expression right)
        {
            return this.OrElse(this.Expression, right);
        }

        public ExpressionBuilder AndAlso(Expression left, Expression right)
        {
            return new ExpressionBuilder(this.Param, Expression.And(left, right));
        }

        public ExpressionBuilder OrElse(Expression left, Expression right)
        {
            return new ExpressionBuilder(this.Param, Expression.OrElse(left, right));
        }

        public ExpressionBuilder Between<T>(T left, T right)
        {
            var expression = this.GreaterThanOrEqualTo(left).Expression;
            var expression1 = this.LessThanOrEqualTo(right).Expression;

            return this.AndAlso(expression, expression1);
        }

        public Expression<Func<T, E>> Lambda<T, E>()
        {
            return this.Lambda<T, E>(this.Expression);
        }

        public Expression<Func<T, E>> Lambda<T, E>(Expression body)
        {
            return Expression.Lambda<Func<T, E>>(body, this.Param);
        }

        public LambdaExpression Lambda()
        {
            return Expression.Lambda(this.Expression, this.Param);
        }

        private static MemberExpression BuildPropertyExpression(ParameterExpression param, string propertyName)
        {
            var parts = string.IsNullOrEmpty(propertyName)
                            ? new Queue<string>()
                            : new Queue<string>(propertyName.Split('.'));

            MemberExpression property = Expression.PropertyOrField(param, parts.Dequeue());

            if (parts.Any())
            {
                property = parts.Aggregate(property, Expression.PropertyOrField);
            }

            return property;
        }
    }
}
