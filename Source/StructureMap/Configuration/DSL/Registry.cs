using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private List<IExpression> _expressions = new List<IExpression>();
        private PluginGraph _graph;

        public Registry(PluginGraph graph) : this()
        {
            _graph = graph;
        }

        public Registry()
        {
            _graph = new PluginGraph();
            configure();
        }


        /// <summary>
        /// Implement this method to 
        /// </summary>
        protected virtual void configure()
        {
            // no-op;
        }

        protected internal void addExpression(IExpression expression)
        {
            _expressions.Add(expression);
        }

        internal void ConfigurePluginGraph(PluginGraph graph)
        {
            foreach (IExpression expression in _expressions)
            {
                expression.Configure(graph);
            }
        }


        public void Dispose()
        {
            ConfigurePluginGraph(_graph);
        }


        public CreatePluginFamilyExpression BuildInstancesOf<T>()
        {
            CreatePluginFamilyExpression expression = new CreatePluginFamilyExpression(typeof (T));
            addExpression(expression);

            return expression;
        }

        public IInstanceManager BuildInstanceManager()
        {
            ConfigurePluginGraph(_graph);
            _graph.ReadDefaults();
            return new InstanceManager(_graph);
        }

        public InstanceExpression.InstanceTypeExpression AddInstanceOf<T>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (T));
            addExpression(expression);
            return expression.TypeExpression();
        }

        public static InstanceExpression.InstanceTypeExpression Instance<T>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (T));
            return expression.TypeExpression();
        }

        public static PrototypeExpression<T> Prototype<T>(T prototype)
        {
            return new PrototypeExpression<T>(prototype);
        }

        public static LiteralExpression<T> Object<T>(T instance)
        {
            return new LiteralExpression<T>(instance);
        }

        public LiteralExpression<T> AddInstanceOf<T>(T target)
        {
            LiteralExpression<T> literal = new LiteralExpression<T>(target);
            addExpression(literal);

            return literal;
        }

        public PrototypeExpression<T> AddPrototypeInstanceOf<T>(T prototype)
        {
            PrototypeExpression<T> expression = new PrototypeExpression<T>(prototype);
            addExpression(expression);

            return expression;
        }

        public ProfileExpression CreateProfile(string profileName)
        {
            ProfileExpression expression = new ProfileExpression(profileName);
            addExpression(expression);

            return expression;
        }

        public static bool IsPublicRegistry(Type type)
        {
            if (!typeof (Registry).IsAssignableFrom(type))
            {
                return false;
            }

            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                return false;
            }

            return (type.GetConstructor(new Type[0]) != null);
        }
    }
}