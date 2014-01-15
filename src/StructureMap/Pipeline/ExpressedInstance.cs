using System;
using System.Linq.Expressions;
using StructureMap.Building.Interception;

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
        public T OnCreation<THandler>(Expression<Action<THandler>> handler)
        {
            AddInterceptor(new ActivatorInterceptor<THandler>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <param name="description">A description of the action for diagnostic purposes</param>
        /// <returns></returns>
        public T OnCreation<THandler>(string description, Action<THandler> handler)
        {
            AddInterceptor(InterceptorFactory.ForAction(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(Expression<Func<THandler, THandler>> handler)
        {
            AddInterceptor(new FuncInterceptor<THandler>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(string description, Func<THandler, THandler> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(Expression<Func<IBuildSession, THandler, THandler>> handler)
        {
            AddInterceptor(new FuncInterceptor<THandler>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(string description, Func<IBuildSession, THandler, THandler> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register an <see cref="IInterceptor">IInterceptor</see> with this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public T InterceptWith(IInterceptor interceptor)
        {
            AddInterceptor(interceptor);
            return thisInstance;
        }
    }
}