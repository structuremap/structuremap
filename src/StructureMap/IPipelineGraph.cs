using System;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public interface IPipelineGraph : ILifecycleContext, IDisposable
    {
        IInstanceGraph Instances { get; }

        IModel ToModel();

        string Profile { get; }
        ContainerRole Role { get; }

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