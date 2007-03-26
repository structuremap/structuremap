using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public interface IMementoBuilder : IExpression
    {
        InstanceMemento BuildMemento(PluginFamily family);
        InstanceMemento BuildMemento(PluginGraph graph);
        void SetInstanceName(string instanceKey);

        void ValidatePluggability(Type pluginType);
    }
}