using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// An Instance class that builds objects by calling a constructor function on a concrete type
    /// and filling setter properties.  ConfiguredInstance should only be used for open generic types.
    /// Favor <see cref="SmartInstance{T}">SmartInstance{T}</see> for all other usages.
    /// </summary>
    public partial class ConfiguredInstance : ConstructorInstance<ConfiguredInstance>
    {
        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType)
            : base(memento.FindPlugin(graph.FindFamily(pluginType)))
        {
            read(memento, graph, pluginType);
        }

        public ConfiguredInstance(Type TPluggedType, string name)
            : base(TPluggedType, name)
        {
        }


        public ConfiguredInstance(Type TPluggedType)
            : base(TPluggedType)
        {
        }

        public ConfiguredInstance(Type templateType, params Type[] types)
            : base(templateType.MakeGenericType(types))
        {
        }

        private void read(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            var reader = new InstanceMementoPropertyReader(this, memento, graph);
            plugin.VisitArguments(reader);
        }

        protected override ConfiguredInstance thisObject()
        {
            return this;
        }
    }
}