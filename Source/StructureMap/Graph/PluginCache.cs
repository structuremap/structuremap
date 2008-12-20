using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Emitting;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public static class PluginCache
    {
        private static readonly Cache<Type, InstanceBuilder> _builders;
        private static readonly List<Type> _filledTypes = new List<Type>();
        private static readonly Cache<Type, Plugin> _plugins;

        static PluginCache()
        {
            _plugins = new Cache<Type, Plugin>(t => new Plugin(t));
            _builders = new Cache<Type, InstanceBuilder>(t =>
            {
                Plugin plugin = _plugins[t];
                plugin.SetFilledTypes(_filledTypes);
                return new InstanceBuilderAssembly(new[] {plugin}).Compile()[0];
            });
        }

        public static Plugin GetPlugin(Type pluggedType)
        {
            return _plugins[pluggedType];
        }

        public static InstanceBuilder FindBuilder(Type pluggedType)
        {
            return _builders[pluggedType];
        }

        public static void Compile()
        {
            lock (typeof (PluginCache))
            {
                IEnumerable<Plugin> plugins =
                    _plugins.Where(plugin => pluginHasNoBuilder(plugin) && plugin.CanBeCreated());
                createAndStoreBuilders(plugins);
            }
        }

        private static void createAndStoreBuilders(IEnumerable<Plugin> plugins)
        {
            foreach (Plugin plugin in plugins)
            {
                plugin.SetFilledTypes(_filledTypes);
            }

            var assembly = new InstanceBuilderAssembly(plugins);
            assembly.Compile().ForEach(b => _builders[b.PluggedType] = b);
        }

        private static bool pluginHasNoBuilder(Plugin plugin)
        {
            return !_builders.Has(plugin.PluggedType);
        }

        public static void Store(Type pluggedType, InstanceBuilder builder)
        {
            _builders[pluggedType] = builder;
        }

        internal static void ResetAll()
        {
            lock (typeof (PluginCache))
            {
                _builders.Clear();
                _plugins.Clear();
            }
        }

        public static void AddFilledType(Type type)
        {
            _filledTypes.Add(type);
        }
    }
}