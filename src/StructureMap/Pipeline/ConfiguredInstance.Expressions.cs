using System;
using StructureMap.Configuration.DSL;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance
    {
        public ConfiguredInstance Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Action<T> handler)
        {
            var interceptor = new StartupInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Action<IContext, T> handler)
        {
            var interceptor = new StartupInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<T>(EnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<T>(ContextEnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }
    }
}