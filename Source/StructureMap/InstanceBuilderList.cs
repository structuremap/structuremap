using System;
using System.Collections.Generic;
using StructureMap.Emitting;
using StructureMap.Graph;

namespace StructureMap
{
    public class InstanceBuilderList
    {
        private readonly Dictionary<Type, InstanceBuilder> _builders = new Dictionary<Type, InstanceBuilder>();
        private readonly Type _pluginType;
        private readonly Dictionary<string, Type> _aliases = new Dictionary<string, Type>();


        public InstanceBuilderList(Type pluginType, IEnumerable<Plugin> plugins)
        {
            _pluginType = pluginType;
            processPlugins(plugins);
        }

        public InstanceBuilderList(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public int BuilderCount
        {
            get { return _builders.Count; }
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
                processPlugin(plugin);

                return _builders[pluggedType];
            }

            return null;
        }

        public InstanceBuilder FindByConcreteKey(string concreteKey)
        {
            if (_aliases.ContainsKey(concreteKey))
            {
                Type pluggedType = _aliases[concreteKey];
                return FindByType(pluggedType);
            }

            return null;
        }

        private void processPlugin(Plugin plugin)
        {
            processPlugins(new Plugin[] { plugin });
        }

        private void processPlugins(IEnumerable<Plugin> plugins)
        {
            foreach (Plugin plugin in plugins)
            {
                if (_aliases.ContainsKey(plugin.ConcreteKey))
                {
                    continue;
                }

                _aliases.Add(plugin.ConcreteKey, plugin.PluggedType);
            }

            List<InstanceBuilder> list = createInstanceBuilders(plugins);
            foreach (InstanceBuilder builder in list)
            {
                _builders.Add(builder.PluggedType, builder);
            }
        }

        private List<InstanceBuilder> createInstanceBuilders(IEnumerable<Plugin> plugins)
        {
            List<Plugin> list = new List<Plugin>();
            foreach (Plugin plugin in plugins)
            {
                if (!_builders.ContainsKey(plugin.PluggedType))
                {
                    list.Add(plugin);
                }
            }

            InstanceBuilderAssembly builderAssembly = new InstanceBuilderAssembly(_pluginType, list);
            return builderAssembly.Compile();
        }

        public void Add(Plugin plugin)
        {
            Add(new Plugin[] {plugin});
        }

        public void Add(IEnumerable<Plugin> plugins)
        {
            processPlugins(plugins);
        }
    }
}