using StructureMap.Diagnostics;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public interface IDiagnosticInstance
    {
        bool CanBePartOfPluginFamily(PluginFamily family);
        InstanceToken CreateToken();
    }
}