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
            new GreediestConstructorSelector()
        };

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            if (instance.Constructor == null)
            {
                instance.Constructor = Select(instance.PluggedType);
            }
        }

        public void Add(IConstructorSelector selector)
        {
            _selectors.Add(selector);
        }

        public ConstructorInfo Select(Type pluggedType)
        {
            return _selectors.Union(_defaults).FirstValue(x => x.Find(pluggedType, _graph));
        }
    }
}