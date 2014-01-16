using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IPipelineGraph : ILifecycleContext, IDisposable
    {
        [Obsolete("This smells to high heaven.  Nasty coupling, law of demeter")]
        PluginGraph PluginGraph { get; }

        /// <summary>
        ///     Unwraps a nested container and/or profiles?
        /// </summary>
        /// <returns></returns>
        IPipelineGraph Root();

        Instance GetDefault(Type pluginType);
        bool HasDefaultForPluginType(Type pluginType);
        bool HasInstance(Type pluginType, string instanceKey);
        void EachInstance(Action<Type, Instance> action);
        IEnumerable<Instance> GetAllInstances();
        IEnumerable<Instance> GetAllInstances(Type pluginType);
        Instance FindInstance(Type pluginType, string name);

        IPipelineGraph ForProfile(string profile);

        IPipelineGraph ToNestedGraph();


        IEnumerable<PluginGraph> AllGraphs();


        IEnumerable<PluginFamily> UniqueFamilies();
    }
}