using System;
using System.Linq.Expressions;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class DecoratorInterceptor : IInterceptor
    {
        private readonly Type _pluginType;
        private bool _hasAppliedPolicies;

        public DecoratorInterceptor(Type pluginType, IConfiguredInstance instance)
        {
            _pluginType = pluginType;
            Instance = instance;
        }

        public DecoratorInterceptor(Type pluginType, Type pluggedType)
            : this(pluginType, new ConstructorInstance(pluggedType))
        {
        }

        public string Description => "Decorator of type " + Instance.PluggedType.GetFullName();

        public IConfiguredInstance Instance { get; }

        public InterceptorRole Role => InterceptorRole.Decorates;

        public Expression ToExpression(Policies policies, ParameterExpression session, ParameterExpression variable)
        {
            var build = ToConcreteBuild(policies, variable);
            var builder = build.ToExpression(session, Parameters.Context);

            return Expression.Convert(builder, _pluginType);
        }

        public ConcreteBuild ToConcreteBuild(Policies policies, ParameterExpression variable)
        {
            variable = variable ?? Expression.Variable(_pluginType, "Inner");

            if (!_hasAppliedPolicies)
            {
                Instance.As<Instance>().ApplyAllPolicies(_pluginType, policies);
                _hasAppliedPolicies = true;
            }
            
            var dependencies = Instance.Dependencies.Clone();
            dependencies.Add(_pluginType, new LiteralDependencySource(variable, _pluginType));

            var build = ConcreteType.BuildSource(Instance.PluggedType, Instance.Constructor, dependencies, policies);
            return build;
        }


        public Type Accepts => _pluginType;

        public Type Returns => _pluginType;


        public class LiteralDependencySource : IDependencySource
        {
            private readonly Expression _expression;

            public LiteralDependencySource(Expression expression, Type returnedType)
            {
                _expression = expression;
                ReturnedType = returnedType;
            }

            public string Description => "The inner " + ReturnedType.GetName();

            public Expression ToExpression(ParameterExpression session, ParameterExpression context)
            {
                return _expression;
            }

            public Type ReturnedType { get; private set; }
            public void AcceptVisitor(IDependencyVisitor visitor)
            {
                visitor.Dependency(this);
            }
        }

    }

}