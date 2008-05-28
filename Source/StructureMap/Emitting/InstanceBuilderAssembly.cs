using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Manages the IL emitting of a dynamic assembly of InstanceBuilder classes
    /// </summary>
    public class InstanceBuilderAssembly
    {
        private readonly List<string> _classNames = new List<string>();
        private readonly DynamicAssembly _dynamicAssembly;
        private readonly Type _pluginType;

        public InstanceBuilderAssembly(Type pluginType, IEnumerable<Plugin> plugins)
        {
            string assemblyName = guidString() + "InstanceBuilderAssembly";
            _dynamicAssembly = new DynamicAssembly(assemblyName);
            _pluginType = pluginType;

            foreach (Plugin plugin in plugins)
            {
                processPlugin(plugin);
            }
        }

        private static string guidString()
        {
            return Guid.NewGuid().ToString().Replace(".", "");
        }

        /// <summary>
        /// Gets a class name for the InstanceBuilder that will be emitted for this Plugin
        /// </summary>
        /// <returns></returns>
        public static string getInstanceBuilderClassName(Type pluggedType)
        {
            string className = "";

            if (pluggedType.IsGenericType)
            {
                className += escapeClassName(pluggedType);

                Type[] args = pluggedType.GetGenericArguments();
                foreach (Type arg in args)
                {
                    className += escapeClassName(arg);
                }
            }
            else
            {
                className = escapeClassName(pluggedType);
            }

            return className + "InstanceBuilder" + guidString();
        }

        private static string escapeClassName(Type type)
        {
            string typeName = type.Namespace + type.Name;
            string returnValue = typeName.Replace(".", string.Empty);
            return returnValue.Replace("`", string.Empty);
        }

        private void processPlugin(Plugin plugin)
        {
            if (TypeRules.CanBeCast(_pluginType, plugin.PluggedType))
            {
                string className = getInstanceBuilderClassName(plugin.PluggedType);
                ClassBuilder builderClass =
                    _dynamicAssembly.AddClass(className, typeof (InstanceBuilder));

                configureClassBuilder(builderClass, plugin);

                _classNames.Add(className);
            }
            else
            {
                throw new StructureMapException(104, plugin.PluggedType.FullName, _pluginType.FullName);
            }
        }

        public List<InstanceBuilder> Compile()
        {
            Assembly assembly = _dynamicAssembly.Compile();

            return
                _classNames.ConvertAll<InstanceBuilder>(
                    delegate(string typeName) { return (InstanceBuilder) assembly.CreateInstance(typeName); });
        }


        private void configureClassBuilder(ClassBuilder builderClass, Plugin plugin)
        {
            builderClass.AddPluggedTypeGetter(plugin.PluggedType);

            BuildInstanceMethod method = new BuildInstanceMethod(plugin);
            builderClass.AddMethod(method);
        }
    }
}