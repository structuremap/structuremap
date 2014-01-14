using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class InterceptionPlan : IBuildPlan
    {
        private readonly Type _pluginType;
        private readonly IBuildPlan _inner;
        private readonly IEnumerable<IInterceptor> _interceptors;
        private ParameterExpression _variable;

        public InterceptionPlan(Type pluginType, IBuildPlan inner, IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _inner = inner;
            _interceptors = interceptors;
            _variable = Expression.Variable(_inner.ReturnedType, "x");
        }

        public object Build(IBuildSession session)
        {
            throw new NotImplementedException();
        }

        public Func<IBuildSession, T> ToBuilder<T>()
        {
            var lambdaType = typeof (Func<IBuildSession, T>);
            var lambda = Expression.Lambda(lambdaType, ToExpression(Parameters.Session), Parameters.Session);
            return (Func<IBuildSession, T>) lambda.Compile();
        } 

        // TODO -- wrap with exception wrappers on all
        public Expression ToExpression(ParameterExpression session)
        {
            // Seed the plan with the inner value
            var inner = _inner.ToExpression(session);
            var plan = new BlockPlan();
            plan.AddVariable(_variable);
            var assignment = Expression.Assign(_variable, inner);
            plan.Add(assignment);

            addActivations(plan);
            createTheReturnValue(_variable, plan);

            return plan.ToExpression();

        }

        private void createTheReturnValue(ParameterExpression variable, BlockPlan plan)
        {
            var label = Expression.Label(_pluginType);
            Expression returnTarget = variable;
            plan.Add(Expression.Return(label, returnTarget, _pluginType));
            plan.Add(Expression.Label(label, variable));
        }

        private void addActivations(BlockPlan plan)
        {
            var activators = findActivatorGroups();
            plan.AddVariables(activators.SelectMany(x => x.ToParameterExpressions(_inner.ReturnedType)));
            plan.Add(activators.SelectMany(x => x.CreateExpressions(_variable)).ToArray());
        }

        private IEnumerable<ActivatorGroup> findActivatorGroups()
        {
            return _interceptors
                .Where(x => x.Role == InterceptorRole.Activates)
                .GroupBy(x => x.Accepts)
                .Select(g => new ActivatorGroup(g))
                .ToArray();
        }

        public class ActivatorGroup
        {
            private readonly IGrouping<Type, IInterceptor> _group;
            private ParameterExpression _variable;

            public ActivatorGroup(IGrouping<Type, IInterceptor> group)
            {
                _group = @group;
            }

            public IEnumerable<ParameterExpression> ToParameterExpressions(Type returnedType)
            {
                if (returnedType != _group.Key)
                {
                    _variable = Expression.Variable(_group.Key, "_" + _group.Key.Name.ToLower());
                    yield return _variable;
                }
            }

            public IEnumerable<Expression> CreateExpressions(ParameterExpression originalVariable)
            {
                var variable = _variable ?? originalVariable;

                if (_variable != null)
                {
                    yield return Expression.Assign(_variable, Expression.Convert(originalVariable, _group.Key));
                }

                foreach (var interceptor in _group)
                {
                    var interceptionExpression = interceptor.ToExpression(Parameters.Session, variable);

                    yield return interceptionExpression;
                }
            } 
        }

        public Type ReturnedType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Description { get; private set; }
    }
}