using System;
using System.Linq.Expressions;
using System.Reflection;
using Query.Common;

namespace Common.Extension
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Lambda)
            {
                LambdaExpression lambdaExpression = (LambdaExpression)expression;
                return GetPropertyInfo(lambdaExpression.Body);
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (PropertyInfo)((MemberExpression)expression).Member;
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                return GetPropertyInfo(((UnaryExpression)expression).Operand);
            }

            if (expression.NodeType == ExpressionType.Call)
            {
                GetPropertyInfo(((MethodCallExpression)expression).Object);
            }

            throw new InvalidOperationException("Invalid type of expression.");
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Lambda)
            {
                LambdaExpression lambdaExpression = (LambdaExpression)expression;
                return GetMemberInfo(lambdaExpression.Body);
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression)expression).Member;
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                return GetMemberInfo(((UnaryExpression)expression).Operand);
            }

            if (expression.NodeType == ExpressionType.Call)
            {
                GetMemberInfo(((MethodCallExpression)expression).Object);
            }

            if (expression.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression)expression).Method;
            }

            return expression.Type;
        }

        public static Expression Replace(this Expression body, Expression from, Expression to)
        {
            var swap = new ExpressionSubstitute(from, to);
            return swap.Visit(body);
        }
    }
}