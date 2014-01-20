using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

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

        // TOO -- do we need this?

        IPipelineGraph ForProfile(string profile);

        IPipelineGraph ToNestedGraph();

        void RegisterContainer(IContainer container);


        void Configure(Action<ConfigurationExpression> configure);
    }
}