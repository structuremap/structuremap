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

        public static LambdaExpression ReplaceParameter(Type acceptsType, LambdaExpression expression,
            ParameterExpression newParam)
        {
            var before = expression.Parameters.FirstOrDefault(x => { return x.Type.CanBeCastTo(acceptsType); });

            if (before == null) return expression;

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
            if (node != _before) return node;

            if (_after.Type == _before.Type) return _after;

            return Expression.Convert(_after, _before.Type);
        }
    }
}