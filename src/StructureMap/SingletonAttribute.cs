using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{

    /// <summary>
    /// Makes StructureMap treat a Type as a singleton in the lifecycle scoping
    /// </summary>
    // SAMPLE: SingletonAttribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SingletonAttribute : StructureMapAttribute
    {
        // This method will affect the configuration for the
        // entire plugin type
        public override void Alter(PluginFamily family)
        {
            family.SetLifecycleTo<SingletonLifecycle>();
        }

        // This method will affect single registrations
        public override void Alter(IConfiguredInstance instance)
        {
            instance.SetLifecycleTo<SingletonLifecycle>();
        }
    }
    // ENDSAMPLE
}