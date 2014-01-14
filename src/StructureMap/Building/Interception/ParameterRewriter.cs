using System;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class ParameterRewriter : ExpressionVisitor
    {
        private readonly ParameterExpression _before;
        private readonly ParameterExpression _after;

        public static LambdaExpression ReplaceParameter(Type acceptsType, LambdaExpression expression, ParameterExpression newParam)
        {
            var before = expression.Parameters.Single(x => {
                return x.Type.CanBeCastTo(acceptsType);
            });

            var rewriter = new ParameterRewriter(before, newParam);
            return rewriter.VisitAndConvert(expression, "Activator");
        }

        public ParameterRewriter(ParameterExpression before, ParameterExpression after)
        {
            _before = before;
            _after = after;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _before ? _after : node;
        }


    }
}