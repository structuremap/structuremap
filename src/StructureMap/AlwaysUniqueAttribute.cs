using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    #pragma warning disable 1591
    /// <summary>
    /// Makes StructureMap treat a Type with the AlwaysUnique lifecycle
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AlwaysUniqueAttribute : StructureMapAttribute
    {

        public override void Alter(PluginFamily family)
        {
            family.SetLifecycleTo<UniquePerRequestLifecycle>();
        }

        public override void Alter(IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<UniquePerRequestLifecycle>();
        }
    }
    #pragma warning restore 1591
}