using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public interface IMementoBuilder
    {
        InstanceMemento BuildMemento(PluginFamily family);
        InstanceMemento BuildMemento(PluginGraph graph);
        void SetInstanceName(string instanceKey);
    }
}