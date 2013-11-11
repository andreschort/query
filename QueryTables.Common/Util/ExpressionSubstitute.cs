using System.Linq.Expressions;

namespace QueryTables.Common.Util
{
    public class ExpressionSubstitute : ExpressionVisitor
    {
        private readonly Expression from, to;

        public ExpressionSubstitute(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            if (node == this.from)
            {
                return this.to;
            }

            return base.Visit(node);
        }
    }
}
