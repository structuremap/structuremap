using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class ConstructorSelector : ConfiguredInstancePolicy
    {
        private readonly PluginGraph _graph;
        private readonly IList<IConstructorSelector> _selectors = new List<IConstructorSelector>();

        public ConstructorSelector(PluginGraph graph)
        {
            _graph = graph;
        }

        private readonly IConstructorSelector[] _defaults = new IConstructorSelector[]
        {
            new AttributeConstructorSelector(),
            new GreediestConstructorSelector(),
            new FirstConstructor()
        };

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            if (instance.Constructor == null)
            {
                instance.Constructor = Select(instance.PluggedType, instance.Dependencies);
            }
        }

        public void Add(IConstructorSelector selector)
        {
            _selectors.Insert(0, selector);
        }

        public ConstructorInfo Select(Type pluggedType, DependencyCollection dependencies)
        {
            foreach (var selector in _selectors)
            {
                var ctor = selector.Find(pluggedType, dependencies, _graph);
                if (ctor != null) return ctor;
            }

            foreach (var @default in _defaults)
            {
                var ctor = @default.Find(pluggedType, dependencies, _graph);
                if (ctor != null) return ctor;
            }

            return null;
        }
    }

    public class FirstConstructor : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            return pluggedType.GetConstructors().FirstOrDefault();
        }
    }
}