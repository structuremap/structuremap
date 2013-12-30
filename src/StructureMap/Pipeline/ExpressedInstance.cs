using System;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    /// <summary>
    ///     Base class for many of the Instance subclasses to support
    ///     method chaining in the Registry DSL for common options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ExpressedInstance<T> : Instance
    {
        protected abstract T thisInstance { get; }


        /// <summary>
        ///     Set the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public T Named(string instanceKey)
        {
            Name = instanceKey;
            return thisInstance;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T OnCreation<THandler>(Action<THandler> handler)
        {
            var interceptor = new StartupInterceptor<THandler>((c, o) => handler(o));
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(EnrichmentHandler<THandler> handler)
        {
            var interceptor = new EnrichmentInterceptor<THandler>((c, o) => handler(o));
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(ContextEnrichmentHandler<THandler> handler)
        {
            var interceptor = new EnrichmentInterceptor<THandler>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register an <see cref="InstanceInterceptor">InstanceInterceptor</see> with this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public T InterceptWith(InstanceInterceptor interceptor)
        {
            Interceptor = interceptor;
            return thisInstance;
        }
    }
}