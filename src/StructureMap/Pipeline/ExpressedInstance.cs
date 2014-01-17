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
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T OnCreation<THandler>(Expression<Action<IBuildSession, THandler>> handler)
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
        public T OnCreation<THandler>(string description, Action<IBuildSession, THandler> handler)
        {
            AddInterceptor(InterceptorFactory.ForAction(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith<THandler>(Expression<Func<THandler, THandler>> handler)
        {
            AddInterceptor(new DecoratorInterceptor<THandler>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith<THandler>(string description, Func<THandler, THandler> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith<THandler>(Expression<Func<IBuildSession, THandler, THandler>> handler)
        {
            AddInterceptor(new DecoratorInterceptor<THandler>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith<THandler>(string description, Func<IBuildSession, THandler, THandler> handler)
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

        /// <summary>
        /// Makes this and only this Instance a Singleton
        /// </summary>
        /// <returns></returns>
        public T Singleton()
        {
            SetLifecycleTo<SingletonLifecycle>();
            return thisInstance;
        }

        /// <summary>
        /// Makes this and only this Instance "always unique"
        /// </summary>
        /// <returns></returns>
        public T AlwaysUnique()
        {
            SetLifecycleTo<UniquePerRequestLifecycle>();
            return thisInstance;
        }

        /// <summary>
        /// Makes this and only this Instance a transient
        /// </summary>
        /// <returns></returns>
        public T Transient()
        {
            SetLifecycleTo<TransientLifecycle>();
            return thisInstance;
        }

        /// <summary>
        /// Override the lifecycle on only this Instance
        /// </summary>
        /// <typeparam name="TLifecycle"></typeparam>
        /// <returns></returns>
        public T LifecycleIs<TLifecycle>() where TLifecycle : ILifecycle, new()
        {
            SetLifecycleTo<TLifecycle>();
            return thisInstance;
        }
    }
}