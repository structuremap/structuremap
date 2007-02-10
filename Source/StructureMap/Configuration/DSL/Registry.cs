using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class Registry : IDisposable
    {
        private List<IExpression> _expressions = new List<IExpression>();
        private readonly PluginGraph _graph;

        public Registry(PluginGraph graph)
        {
            _graph = graph;
        }

        public Registry()
        {

        }

        protected void addExpression(IExpression expression)
        {
            _expressions.Add(expression);
        }

        public void Configure(PluginGraph graph)
        {
            foreach (IExpression expression in _expressions)
            {
                expression.Configure(graph);
            }
        }

        public void Dispose()
        {
            Configure(_graph);
        }

        public ScanAssembliesExpression ScanAssemblies()
        {
            ScanAssembliesExpression expression = new ScanAssembliesExpression();
            addExpression(expression);

            return expression;
        }

        public CreatePluginFamilyExpression BuildInstancesOfType<T>()
        {
            CreatePluginFamilyExpression expression = new CreatePluginFamilyExpression(typeof(T));
            addExpression(expression);

            return expression;
        }

        public InstanceManager BuildInstanceManager()
        {
            PluginGraph graph = new PluginGraph();
            Configure(graph);
            return new InstanceManager(graph);
        }
    }
}
