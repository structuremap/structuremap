using System;
using StructureMap.Pipeline;

namespace StructureMap
{
    // SAMPLE: IInstancePolicy
    /// <summary>
    /// Custom policy on Instance construction that is evaluated
    /// as part of creating a "build plan"
    /// </summary>
    public interface IInstancePolicy
    {
        /// <summary>
        /// Apply any conventional changes to the configuration
        /// of a single Instance
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        void Apply(Type pluginType, Instance instance);
    }
    // ENDSAMPLE
}