using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IInstanceGraph
    {
        Instance GetDefault(Type pluginType);
        bool HasDefaultForPluginType(Type pluginType);
        bool HasInstance(Type pluginType, string instanceKey);
        void EachInstance(Action<Type, Instance> action);
        IEnumerable<Instance> GetAllInstances();
        IEnumerable<Instance> GetAllInstances(Type pluginType);
        Instance FindInstance(Type pluginType, string name);
        IEnumerable<PluginFamily> UniqueFamilies();

        ILifecycle DefaultLifecycleFor(Type pluginType);
        ContainerRole Role { get; }
        IEnumerable<Instance> ImmediateInstances();

        PluginGraph ImmediatePluginGraph { get; }
    }
}