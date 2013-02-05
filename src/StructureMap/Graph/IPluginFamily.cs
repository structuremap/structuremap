using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public interface IPluginFamily
    {
        /// <summary>
        /// The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        Type PluginType { get; }

        ILifecycle Lifecycle { get; }
        IEnumerable<Instance> Instances { get; }

        void SetScopeTo(InstanceScope scope);
        void SetScopeTo(ILifecycle lifecycle);
        void SetDefault(Instance instance);
    }
}