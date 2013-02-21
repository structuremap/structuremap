using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public interface IPipelineGraph : ILifecycleContext, IDisposable
    {
        /// <summary>
        /// Unwraps a nested container and/or profiles?
        /// </summary>
        /// <returns></returns>
        IPipelineGraph Root(); // TODO -- I think this will need to be surfaced somehow so that it builds in the right Profile (?)

        InstanceInterceptor FindInterceptor(Type concreteType);

        Instance GetDefault(Type pluginType);
        bool HasDefaultForPluginType(Type pluginType);
        bool HasInstance(Type pluginType, string instanceKey);
        void EachInstance(Action<Type, Instance> action);
        IEnumerable<Instance> GetAllInstances();
        IEnumerable<Instance> GetAllInstances(Type pluginType);
        Instance FindInstance(Type pluginType, string name);

        IPipelineGraph ForProfile(string profile);

        IEnumerable<IPluginTypeConfiguration> GetPluginTypes();

        IPipelineGraph ToNestedGraph();

        IEnumerable<PluginGraph> AllGraphs();
        PluginGraph Outer { get; }
    }
}