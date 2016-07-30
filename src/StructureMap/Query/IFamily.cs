using System;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public interface IFamily
    {
        /// <summary>
        /// The resulting object from this Instance will be evicted from its
        /// lifecycle if it has already been created and cached
        /// </summary>
        /// <param name="instance"></param>
        void Eject(Instance instance);

        /// <summary>
        /// Ejects any existing object for this Instance from its lifecycle
        /// and permanently removes the configured Instance from the container
        /// </summary>
        /// <param name="instance"></param>
        void EjectAndRemove(Instance instance);

        /// <summary>
        /// Builds the object
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object Build(Instance instance);

        /// <summary>
        /// Queries the lifecycle if it has been created
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool HasBeenCreated(Instance instance);

        /// <summary>
        /// The default lifecycle for this PluginType/Family
        /// </summary>
        ILifecycle Lifecycle { get; }

        Type PluginType { get; }

        /// <summary>
        /// A reference to the underlying container runtime model.  Doing any direct manipulation
        /// against this service will void the warranty on StructureMap.
        /// </summary>
        IPipelineGraph Pipeline { get; }
    }
}