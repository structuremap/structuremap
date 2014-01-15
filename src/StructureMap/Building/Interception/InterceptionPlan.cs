using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StructureMap.Building.Interception
{
    public class InterceptionPlan : IDependencySource
    {
        private readonly Type _pluginType;
        private readonly IDependencySource _inner;
        private readonly IEnumerable<IInterceptor> _interceptors;
        private readonly ParameterExpression _variable;

        public InterceptionPlan(Type pluginType, IDependencySource inner, IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _inner = inner;
            _interceptors = interceptors;
            _variable = Expression.Variable(_inner.ReturnedType, "x");

            // TODO -- blow up if any decorates interceptors cannot be cast to pluginType
        }

        public Func<IBuildSession, T> ToBuilder<T>()
        {
            var lambdaType = typeof (Func<IBuildSession, T>);
            var lambda = Expression.Lambda(lambdaType, ToExpression(Parameters.Session), Parameters.Session);
            return (Func<IBuildSession, T>) lambda.Compile();
        } 

        public Expression ToExpression(ParameterExpression session)
        {
            // Seed the plan with the inner value
            var inner = _inner.ToExpression(session);
            var plan = new BlockPlan();
            plan.AddVariable(_variable);
            var assignment = Expression.Assign(_variable, inner);
            plan.Add(assignment);

            addActivations(plan);

            var pluginTypeVariable = addPluginTypeVariable(plan);

            addDecorators(session, pluginTypeVariable, plan);

            createTheReturnValue(pluginTypeVariable, plan);

            return plan.ToExpression();

        }

        public Type ReturnedType
        {
            get
            {
                return _pluginType;
            }
        }

        private void addDecorators(ParameterExpression session, ParameterExpression pluginTypeVariable, BlockPlan plan)
        {
            _interceptors.Where(x => x.Role == InterceptorRole.Decorates).Each(decorator => {
                var decoratedExpression = decorator.ToExpression(session, pluginTypeVariable);
                var wrapped = TryCatchWrapper.WrapFunc<StructureMapInterceptorException>(_pluginType,
                    decoratedExpression, decorator);
                
                plan.Add(Expression.Assign(pluginTypeVariable, wrapped));
            });
        }

        private ParameterExpression addPluginTypeVariable(BlockPlan plan)
        {
            var pluginTypeVariable = plan.FindVariableOfType(_pluginType);
            if (pluginTypeVariable == null)
            {
                pluginTypeVariable = Expression.Variable(_pluginType, "returnValue");
                plan.AddVariable(pluginTypeVariable);

                plan.Add(Expression.Assign(pluginTypeVariable, Expression.Convert(_variable, _pluginType)));
            }
            return pluginTypeVariable;
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

                    yield return
                        TryCatchWrapper.WrapAction<StructureMapInterceptorException>(interceptionExpression, interceptor);

                }
            } 
        }

        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}