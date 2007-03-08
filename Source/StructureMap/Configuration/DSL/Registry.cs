using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private List<IExpression> _expressions = new List<IExpression>();
        private PluginGraph _graph;

        public Registry(PluginGraph graph)
        {
            _graph = graph;
        }

        public Registry()
        {
            _graph = new PluginGraph();
        }


        /// <summary>
        /// Implement this method to 
        /// </summary>
        protected virtual void configure()
        {
            // no-op;
        }

        protected void addExpression(IExpression expression)
        {
            _expressions.Add(expression);
        }

        private void configurePluginGraph(PluginGraph graph)
        {
            foreach (IExpression expression in _expressions)
            {
                expression.Configure(graph);
            }
        }


        public void Dispose()
        {
            configurePluginGraph(_graph);
        }

        public ScanAssembliesExpression ScanAssemblies()
        {
            ScanAssembliesExpression expression = new ScanAssembliesExpression();
            addExpression(expression);

            return expression;
        }

        public CreatePluginFamilyExpression BuildInstancesOf<T>()
        {
            CreatePluginFamilyExpression expression = new CreatePluginFamilyExpression(typeof (T));
            addExpression(expression);

            return expression;
        }

        public InstanceManager BuildInstanceManager()
        {
            PluginGraph graph = new PluginGraph();
            configurePluginGraph(graph);
            return new InstanceManager(graph);
        }

        public InstanceExpression AddInstanceOf<T>()
        {
            InstanceExpression expression = new InstanceExpression(typeof (T));
            addExpression(expression);
            return expression;
        }

        public static InstanceExpression Instance<T>()
        {
            return new InstanceExpression(typeof (T));
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
    }
}