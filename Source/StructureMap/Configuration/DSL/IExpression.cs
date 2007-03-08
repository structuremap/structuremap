using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public interface IExpression
    {
        void Configure(PluginGraph graph);
    }
}