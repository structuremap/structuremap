using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Emitting;
using StructureMap.Graph;

namespace StructureMap
{
    public class InstanceBuilderList
    {
        private readonly Dictionary<Type, InstanceBuilder> _builders = new Dictionary<Type, InstanceBuilder>();
        private readonly Type _pluginType;


        public InstanceBuilderList(Type pluginType, IEnumerable<Plugin> plugins)
        {
            _pluginType = pluginType;
            processPlugins(plugins);
        }


        public InstanceBuilder FindByType(Type pluggedType)
        {
            if (pluggedType == null)
            {
                return null;
            }

            if (_builders.ContainsKey(pluggedType))
            {
                return _builders[pluggedType];
            }

            // Add a missing PluggedType if we can
            if (TypeRules.CanBeCast(_pluginType, pluggedType))
            {
                Plugin plugin = new Plugin(pluggedType);
                processPlugins(new Plugin[]{plugin});

                return _builders[pluggedType];
            }

            return null;
        }

        public InstanceBuilder FindByConcreteKey(string concreteKey)
        {
            foreach (KeyValuePair<Type, InstanceBuilder> pair in _builders)
            {
                if (pair.Value.ConcreteTypeKey == concreteKey)
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public void Add(Type pluggedType, InstanceBuilder builder)
        {
            _builders.Add(pluggedType, builder);
        }

        private void processPlugins(IEnumerable<Plugin> plugins)
        {
            List<InstanceBuilder> list = createInstanceBuilders(plugins);
            foreach (InstanceBuilder builder in list)
            {
                _builders.Add(builder.PluggedType, builder);
            }
        }

        private List<InstanceBuilder> createInstanceBuilders(IEnumerable<Plugin> plugins)
        {
            InstanceBuilderAssembly builderAssembly = new InstanceBuilderAssembly(_pluginType, plugins);
            return builderAssembly.Compile();
        }
    }
}