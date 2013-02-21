using System;

namespace StructureMap.Graph
{
    public interface IFamilyPolicy
    {
        PluginFamily Build(Type type);
        bool AppliesToHasFamilyChecks { get; }
    }
}