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
        private Type _pluginType;


        public InstanceBuilderList(Type pluginType, Plugin[] plugins)
        {
            _pluginType = pluginType;
            processPlugins(plugins);
        }


        public InstanceBuilder FindByType(Type pluggedType)
        {
            if (_builders.ContainsKey(pluggedType))
            {
                return _builders[pluggedType];
            }

            // Add a missing PluggedType if we can
            if (Plugin.CanBeCast(_pluginType, pluggedType))
            {
                Plugin plugin = Plugin.CreateImplicitPlugin(pluggedType);
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
            Assembly assembly = createInstanceBuilderAssembly(plugins);
            foreach (Plugin plugin in plugins)
            {
                addPlugin(assembly, plugin);
            }
        }

        private Assembly createInstanceBuilderAssembly(IEnumerable<Plugin> plugins)
        {
            string assemblyName = Guid.NewGuid().ToString().Replace(".", "") + "InstanceBuilderAssembly";
            InstanceBuilderAssembly builderAssembly = new InstanceBuilderAssembly(assemblyName, _pluginType);

            foreach (Plugin plugin in plugins)
            {
                builderAssembly.AddPlugin(plugin);
            }

            return builderAssembly.Compile();
        }


        private void addPlugin(Assembly assembly, Plugin plugin)
        {
            string instanceBuilderClassName = plugin.GetInstanceBuilderClassName();
            InstanceBuilder builder = (InstanceBuilder)assembly.CreateInstance(instanceBuilderClassName);

            _builders.Add(plugin.PluggedType, builder);
        }
    }
}