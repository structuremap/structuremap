using System;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class ParameterRewriter : ExpressionVisitor
    {
        private readonly Type _acceptsType;
        private readonly ParameterExpression _before;
        private readonly ParameterExpression _after;

        public static LambdaExpression ReplaceParameter(Type acceptsType, LambdaExpression expression, ParameterExpression newParam)
        {
            var before = expression.Parameters.Single(x => x.Type == newParam.Type);

            var rewriter = new ParameterRewriter(acceptsType, before, newParam);
            return rewriter.VisitAndConvert(expression, "Activator");
        }

        public ParameterRewriter(Type acceptsType, ParameterExpression before, ParameterExpression after)
        {
            _acceptsType = acceptsType;
            _before = before;
            _after = after;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _before ? _after : node;
        }
    }
}