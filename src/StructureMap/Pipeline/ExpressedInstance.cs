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

    public abstract class ExpressedInstance<T, TReturned, TPluginType> : ExpressedInstance<T> where TReturned : TPluginType
    {
        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T OnCreation(Expression<Action<TReturned>> handler)
        {
            AddInterceptor(new ActivatorInterceptor<TReturned>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">A description of the action for diagnostic purposes</param>
        /// <returns></returns>
        public T OnCreation(string description, Action<TReturned> handler)
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
        public T OnCreation(Expression<Action<IContext, TReturned>> handler)
        {
            AddInterceptor(new ActivatorInterceptor<TReturned>(handler));

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
        public T OnCreation(string description, Action<IContext, TReturned> handler)
        {
            AddInterceptor(InterceptorFactory.ForAction(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith(Expression<Func<TPluginType, TPluginType>> handler)
        {
            AddInterceptor(new FuncInterceptor<TPluginType>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith(string description, Func<TPluginType, TPluginType> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith(Expression<Func<IContext, TPluginType, TPluginType>> handler)
        {
            AddInterceptor(new FuncInterceptor<TPluginType>(handler));

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially decorate or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="description">User friendly descriptive message</param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T DecorateWith(string description, Func<IContext, TPluginType, TPluginType> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return thisInstance;
        }

    }
}