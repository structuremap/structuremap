using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public class PipelineGraph
    {
        private readonly PluginGraph _pluginGraph;
        private readonly IInstanceGraph _instances;
        private readonly IPipelineGraph _root;

        public PipelineGraph(PluginGraph pluginGraph, IInstanceGraph instances, IPipelineGraph root)
        {
            _pluginGraph = pluginGraph;
            _instances = instances;
            _root = root;
        }

        public IPipelineGraph Root()
        {
            return _root;
        }
    }



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
    }

    public interface IPipelineGraph : ILifecycleContext, IDisposable
    {
        IInstanceGraph Instances { get; }

        IModel ToModel();

        string Profile { get; }

        IGraphEjector Ejector { get; }

        Policies Policies { get; }

        /// <summary>
        ///     Unwraps a nested container and/or profiles?
        /// </summary>
        /// <returns></returns>
        IPipelineGraph Root();

        Profiles Profiles { get; }

        IPipelineGraph ToNestedGraph();

        void RegisterContainer(IContainer container);


        void Configure(Action<ConfigurationExpression> configure);
    }
}