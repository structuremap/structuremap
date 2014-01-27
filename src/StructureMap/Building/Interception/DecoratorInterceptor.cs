using System;
using System.Linq.Expressions;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class DecoratorInterceptor : IInterceptor
    {
        private readonly Type _pluginType;
        private readonly IConfiguredInstance _instance;

        public DecoratorInterceptor(Type pluginType, IConfiguredInstance instance)
        {
            _pluginType = pluginType;
            _instance = instance;
        }

        public DecoratorInterceptor(Type pluginType, Type pluggedType)
            : this(pluginType, new ConstructorInstance(pluggedType))
        {
            
        }

        public string Description
        {
            get
            {
                return "Decorator of type " + _instance.PluggedType.GetFullName();
            }
        }

        public InterceptorRole Role
        {
            get
            {
                return InterceptorRole.Decorates;
            }
        }

        public Expression ToExpression(Policies policies, ParameterExpression session, ParameterExpression variable)
        {
            var dependencies = _instance.Dependencies.Clone();
            dependencies.Add(_pluginType, new LiteralDependencySource(variable, _pluginType));

            var build = ConcreteType.BuildSource(_instance.PluggedType, _instance.Constructor, dependencies, policies);
            var builder = build.ToExpression(session, Parameters.Context);

            return Expression.Convert(builder, _pluginType);
        }

        public Type Accepts { get { return _pluginType; } }
        public Type Returns { get { return _pluginType; } }



        public class LiteralDependencySource : IDependencySource
        {
            private readonly Expression _expression;

            public LiteralDependencySource(Expression expression, Type returnedType)
            {
                _expression = expression;
                ReturnedType = returnedType;
            }

            public string Description
            {
                get
                {
                    return _expression.ToString();
                }
            }
            public Expression ToExpression(ParameterExpression session, ParameterExpression context)
            {
                return _expression;
            }

            public Type ReturnedType { get; private set; }
        }
    }


}