using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.Diagnostics;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public interface IInterceptionPlan : IDependencySource, IBuildPlanVisitable
    {
    }

    public class InterceptionPlan : IInterceptionPlan
    {
        private readonly Policies _policies;
        private readonly IEnumerable<IInterceptor> _interceptors;
        private readonly ParameterExpression _variable;
        private readonly IEnumerable<ActivatorGroup> _activators;
        private readonly IEnumerable<IInterceptor> _decorators;

        public InterceptionPlan(Type pluginType, IDependencySource inner, Policies policies,
            IEnumerable<IInterceptor> interceptors)
        {
            ReturnedType = pluginType;
            Inner = inner;
            _policies = policies;
            _interceptors = interceptors;
            _variable = Expression.Variable(Inner.ReturnedType, "x");

            

            _activators = findActivatorGroups();
            _decorators = _interceptors.Where(x => x.Role == InterceptorRole.Decorates);

            var badDecorators = _decorators.Where(x => x.Accepts != pluginType || x.Returns != pluginType).ToArray();
            if (badDecorators.Any())
            {
                var decoratorDescription = string.Join(", ", badDecorators.Select(x => x.Description));

                var description = "Invalid decorators for type {0}: {1}".ToFormat(pluginType.GetTypeName(), decoratorDescription);
                throw new StructureMapBuildPlanException(description);
            }
        }

        public IDependencySource Inner { get; }

        public Func<IBuildSession, IContext, T> ToBuilder<T>()
        {
            var lambdaType = typeof (Func<IBuildSession, IContext, T>);
            var lambda = Expression.Lambda(lambdaType, ToExpression(Parameters.Session, Parameters.Context),
                Parameters.Session, Parameters.Context);
            return (Func<IBuildSession, IContext, T>) lambda.Compile();
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            // Seed the plan with the inner value
            var inner = Inner.ToExpression(session, context);
            var plan = new BlockPlan();
            plan.AddVariable(_variable);
            var assignment = Expression.Assign(_variable, inner);
            plan.Add(assignment);

            addActivations(plan);

            var pluginTypeVariable = addPluginTypeVariable(plan);

            addDecorators(context, pluginTypeVariable, plan);

            createTheReturnValue(pluginTypeVariable, plan);

            return plan.ToExpression();
        }

        public Type ReturnedType { get; }

        void IDependencySource.AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Dependency(this);
        }

        public void AcceptVisitor(IBuildPlanVisitor visitor)
        {
            _decorators.Each(visitor.Decorator);
            _activators.SelectMany(x => x.Interceptors).Each(visitor.Activator);
        }

        private void addDecorators(ParameterExpression context, ParameterExpression pluginTypeVariable, BlockPlan plan)
        {
            _decorators.Each(decorator => {
                var decoratedExpression = decorator.ToExpression(_policies, context, pluginTypeVariable);
                var wrapped =
                    TryCatchWrapper.WrapFunc<StructureMapInterceptorException>(
                        "Decorator Interceptor failed during object construction.  See the inner exception", ReturnedType,
                        decoratedExpression, decorator);

                plan.Add(Expression.Assign(pluginTypeVariable, wrapped));
            });
        }

        private ParameterExpression addPluginTypeVariable(BlockPlan plan)
        {
            var pluginTypeVariable = plan.FindVariableOfType(ReturnedType);
            if (pluginTypeVariable == null)
            {
                pluginTypeVariable = Expression.Variable(ReturnedType, "returnValue");
                plan.AddVariable(pluginTypeVariable);

                plan.Add(Expression.Assign(pluginTypeVariable, Expression.Convert(_variable, ReturnedType)));
            }
            return pluginTypeVariable;
        }

        private void createTheReturnValue(ParameterExpression variable, BlockPlan plan)
        {
            var label = Expression.Label(ReturnedType);
            Expression returnTarget = variable;
            plan.Add(Expression.Return(label, returnTarget, ReturnedType));
            plan.Add(Expression.Label(label, variable));
        }

        private void addActivations(BlockPlan plan)
        {
            plan.AddVariables(_activators.SelectMany(x => x.ToParameterExpressions(Inner.ReturnedType)));
            plan.Add(_activators.SelectMany(x => x.CreateExpressions(_policies, _variable)).ToArray());
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

            public IEnumerable<IInterceptor> Interceptors
            {
                get { return _group; }
            }

            public IEnumerable<ParameterExpression> ToParameterExpressions(Type returnedType)
            {
                if (returnedType != _group.Key)
                {
                    _variable = Expression.Variable(_group.Key, "_" + _group.Key.Name.ToLower());
                    yield return _variable;
                }
            }

            public IEnumerable<Expression> CreateExpressions(Policies policies, ParameterExpression originalVariable)
            {
                var variable = _variable ?? originalVariable;

                if (_variable != null)
                {
                    yield return Expression.Assign(_variable, Expression.Convert(originalVariable, _group.Key));
                }

                foreach (var interceptor in _group)
                {
                    var interceptionExpression = interceptor.ToExpression(policies, Parameters.Context, variable);

                    yield return
                        TryCatchWrapper.WrapAction<StructureMapInterceptorException>(
                            "Activator interceptor failed during object creation.  See the inner exception for details.",
                            interceptionExpression, interceptor);
                }
            }
        }

        public string Description
        {
            get { return "Interceptor Plan for " + Inner.Description; }
        }
    }
}