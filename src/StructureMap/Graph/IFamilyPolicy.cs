using System;

namespace StructureMap.Graph
{
    // SAMPLE: IFamilyPolicy

    /// <summary>
    /// Allows StructureMap to fill in missing registrations by unknown plugin types
    /// at runtime
    /// </summary>
    public interface IFamilyPolicy
    {
        /// <summary>
        /// Allows you to create missing registrations for an unknown plugin type
        /// at runtime.
        /// Return null if this policy does not apply to the given type
        /// </summary>
        PluginFamily Build(Type type);

        /// <summary>
        /// Should this policy be used to determine whether or not the Container has
        /// registrations for a plugin type in the PluginGraph.HasFamily(type) method
        /// </summary>
        bool AppliesToHasFamilyChecks { get; }
    }
    // ENDSAMPLE
}