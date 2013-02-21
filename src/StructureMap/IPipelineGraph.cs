using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public interface IPipelineGraph : ILifecycleContext
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

        [Obsolete("This needs to go away.  We'll just have Container.Configure write directly to the PluginGraph")]
        void ImportFrom(PluginGraph graph);

        void Dispose();

        // This is borderline awful. 
        [Obsolete("replace with new Model class")]
        IEnumerable<IPluginTypeConfiguration> GetPluginTypes(IContainer container);

        


        IPipelineGraph ToNestedGraph();

        IEnumerable<PluginGraph> AllGraphs();
        PluginGraph Outer { get; }
    }
}