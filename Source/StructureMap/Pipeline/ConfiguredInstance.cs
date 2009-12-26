using System;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// An Instance class that builds objects by calling a constructor function on a concrete type
    /// and filling setter properties.  ConfiguredInstance should only be used for open generic types.
    /// Favor <see cref="SmartInstance{T}">SmartInstance{T}</see> for all other usages.
    /// </summary>
    public partial class ConfiguredInstance : ConstructorInstance
    {
        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType) : base(memento.FindPlugin(graph.FindFamily(pluginType)))
        {
            read(memento, graph, pluginType);
        }

        private void read(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            var reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
            plugin.VisitArguments(reader);
        }

        public ConfiguredInstance(Type pluggedType, string name) : base(pluggedType, name)
        {
        }


        public ConfiguredInstance(Type pluggedType) : base(pluggedType)
        {

        }

        public ConfiguredInstance(Type templateType, params Type[] types) : base(templateType.MakeGenericType(types))
        {
        }
    }
}