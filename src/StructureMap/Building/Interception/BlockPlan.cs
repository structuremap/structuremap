using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class BlockPlan
    {
        private readonly List<Expression> _expressions = new List<Expression>();
        private readonly List<ParameterExpression> _variables = new List<ParameterExpression>();

        public static BlockPlan operator +(BlockPlan plan, Expression expression)
        {
            plan._expressions.Add(expression);

            return plan;
        }

        public void Add(params Expression[] expressions)
        {
            _expressions.AddRange(expressions);
        }

        public BlockExpression ToExpression()
        {
            return Expression.Block(_variables, _expressions.ToArray());
        }

        public void AddVariable(ParameterExpression variable)
        {
            _variables.Add(variable);
        }

        public void AddVariables(IEnumerable<ParameterExpression> variables)
        {
            _variables.AddRange(variables);
        }

        public IEnumerable<ParameterExpression> Variables
        {
            get { return _variables; }
        }

        public ParameterExpression FindVariableOfType(Type type)
        {
            return _variables.LastOrDefault(x => x.Type == type);
        }
    }
}