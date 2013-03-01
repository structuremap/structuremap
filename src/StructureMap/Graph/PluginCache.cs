using System;
using System.Linq;
using System.Reflection;
using StructureMap.Construction;
using StructureMap.Util;

namespace StructureMap.Graph
{
    [Obsolete("Like this to go away")]
    public static class PluginCache
    {
        private static readonly Cache<Type, IInstanceBuilder> _builders;
        private static readonly Cache<Type, Plugin> _plugins;
        private static readonly SetterRules _setterRules = new SetterRules();

        static PluginCache()
        {
            _plugins = new Cache<Type, Plugin>(t =>
            {
                var plugin = new Plugin(t);
                _setterRules.Configure(plugin);

                return plugin;
            });


            _builders = new Cache<Type, IInstanceBuilder>(t =>
            {
                try
                {
                    Plugin plugin = _plugins[t];
                    return BuilderCompiler.CreateBuilder(plugin);
                }
                catch (Exception e)
                {
                    throw new StructureMapException(245, e, t.AssemblyQualifiedName);
                }
            });
        }

        public static Plugin GetPlugin(Type pluggedType)
        {
            return _plugins[pluggedType];
        }

        public static IInstanceBuilder FindBuilder(Type pluggedType)
        {
            return _builders[pluggedType];
        }

        public static void Store(Type pluggedType, InstanceBuilder builder)
        {
            _builders[pluggedType] = builder;
        }

        public static void ResetAll()
        {
            lock (typeof (PluginCache))
            {
                _setterRules.Clear();
                _builders.ClearAll();
                _plugins.ClearAll();
            }
        }

        public static void AddFilledType(Type type)
        {
            Func<PropertyInfo, bool> predicate = prop => prop.PropertyType == type;
            UseSetterRule(predicate);
        }

        public static void UseSetterRule(Func<PropertyInfo, bool> predicate)
        {
            _setterRules.Add(predicate);
            _plugins.Each(plugin =>
            {
                plugin.UseSetterRule(predicate);

                //does any of the registered plugins have a setter matching the predicate?
                if (plugin.PluggedType.GetProperties().Any(s => predicate(s)))
                {
                    _builders.Remove(plugin.PluggedType);
                }
            });
        }
    }
}