using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Helper base class for instance policies on registrations
    /// of concrete classes using the IConfiguredInstance interface
    /// </summary>
    // SAMPLE: ConfiguredInstancePolicy
    public abstract class ConfiguredInstancePolicy : IInstancePolicy
    {
        /// <summary>
        /// Applies explicit configuration to an IConfiguredInstance
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public void Apply(Type pluginType, Instance instance)
        {
            var configured = instance as IConfiguredInstance;
            if (configured != null)
            {
                apply(pluginType, configured);
            }
        }

        /// <summary>
        /// This method is called against any Instance that implements the
        /// IConfiguredInstance interface
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        protected abstract void apply(Type pluginType, IConfiguredInstance instance);
    }
    // ENDSAMPLE
}