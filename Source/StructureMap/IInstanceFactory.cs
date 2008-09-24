using System;
using System.Collections;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Interface for a "Factory" pattern class that creates object instances of the PluginType
    /// </summary>
    public interface IInstanceFactory
    {
        Type PluginType { get; }

        void AddInstance(Instance instance);
        Instance AddType<T>();

        IList GetAllInstances(BuildSession session);
        object Build(BuildSession session, Instance instance);
        Instance FindInstance(string name);

        void ForEachInstance(Action<Instance> action);
        void ImportFrom(PluginFamily family);
        void AcceptVisitor(IPipelineGraphVisitor visitor, Instance defaultInstance);
        void EjectAllInstances();
    }
}