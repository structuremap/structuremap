using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Manages the IL emitting of a dynamic assembly of InstanceBuilder classes
    /// </summary>
    public class InstanceBuilderAssembly
    {
        private DynamicAssembly _dynamicAssembly;
        private Type _pluginType;

        public InstanceBuilderAssembly(string assemblyName, Type pluginType)
        {
            _dynamicAssembly = new DynamicAssembly(assemblyName);
            _pluginType = pluginType;
        }

        public void AddPlugin(Plugin plugin)
        {
            if (Plugin.CanBeCast(_pluginType, plugin.PluggedType))
            {
                ClassBuilder builderClass =
                    _dynamicAssembly.AddClass(plugin.GetInstanceBuilderClassName(), typeof (InstanceBuilder));

                configureClassBuilder(builderClass, plugin);
            }
            else
            {
                throw new StructureMapException(104, plugin.PluggedType.FullName, _pluginType.FullName);
            }
        }

        public Assembly Compile()
        {
            return _dynamicAssembly.Compile();
        }


        private void configureClassBuilder(ClassBuilder builderClass, Plugin plugin)
        {
            builderClass.AddReadonlyStringProperty("ConcreteTypeKey", plugin.ConcreteKey, true);
            builderClass.AddReadonlyStringProperty("PluginType", TypePath.GetAssemblyQualifiedName(_pluginType), true);
            builderClass.AddReadonlyStringProperty("PluggedType", TypePath.GetAssemblyQualifiedName(plugin.PluggedType),
                                                   true);

            BuildInstanceMethod method = new BuildInstanceMethod(plugin);
            builderClass.AddMethod(method);
        }
    }
}