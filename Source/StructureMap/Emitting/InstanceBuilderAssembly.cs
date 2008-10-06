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

        public InstanceBuilderAssembly(IEnumerable<Plugin> plugins)
        {
            string assemblyName = "Builders" + guidString();
            _dynamicAssembly = new DynamicAssembly(assemblyName);

            foreach (Plugin plugin in plugins)
            {
                processPlugin(plugin);
            }
        }

        private static string guidString()
        {
            return Guid.NewGuid().ToString().Replace(".", "").Replace("-", "");
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
            string returnValue = typeName.Replace(".", string.Empty).Replace("[]", "Array");
            return returnValue.Replace("`", string.Empty);
        }

        private void processPlugin(Plugin plugin)
        {
            string className = getInstanceBuilderClassName(plugin.PluggedType);
            ClassBuilder builderClass =
                _dynamicAssembly.AddClass(className, typeof (InstanceBuilder));

            configureClassBuilder(builderClass, plugin);

            _classNames.Add(className);
        }

        public List<InstanceBuilder> Compile()
        {
            Assembly assembly = _dynamicAssembly.Compile();


            return
                _classNames.ConvertAll(
                    typeName => (InstanceBuilder) assembly.CreateInstance(typeName));
        }


        private void configureClassBuilder(ClassBuilder builderClass, Plugin plugin)
        {
            builderClass.AddPluggedTypeGetter(plugin.PluggedType);

            var method = new BuildInstanceMethod(plugin);
            builderClass.AddMethod(method);
        }
    }
}