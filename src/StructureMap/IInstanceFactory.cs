using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Interface for a "Factory" pattern class that creates object instances of the PluginType
    /// </summary>
    [Obsolete]
    public interface IInstanceFactory : IDisposable
    {
        Type PluginType { get; }

        Instance MissingInstance { get; }

        Instance[] AllInstances { get; }
        ILifecycle Lifecycle { get; }

        // need to override this
        void AddInstance(Instance instance);


        Instance FindInstance(string name);
        void ImportFrom(PluginFamily family);


        void EjectAllInstances(PipelineGraph pipelineGraph);

        IInstanceFactory Clone();
        void RemoveInstance(Instance instance);
    }
}