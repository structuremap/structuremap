using System;
using System.Linq.Expressions;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{
    [Obsolete("KILL THE DUPLICATION")]
    public partial class ConfiguredInstance
    {
        public ConfiguredInstance Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        /// Add an interceptor to only this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public ConfiguredInstance InterceptWith(IInterceptor interceptor)
        {
            AddInterceptor(interceptor);
            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Expression<Action<T>> handler)
        {
            return InterceptWith(new ActivatorInterceptor<T>(handler));
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="description">A description of the creation action for diagnostics</param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(string description, Action<T> handler)
        {
            return InterceptWith(InterceptorFactory.ForAction(description, handler));
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Expression<Action<IBuildSession, T>> handler)
        {
            return InterceptWith(new ActivatorInterceptor<T>(handler));
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="description">A description of the creation action for diagnostics</param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(string description, Action<IBuildSession, T> handler)
        {
            return InterceptWith(InterceptorFactory.ForAction(description, handler));
        }

        /// <summary>
        /// Register a Func to potentially decorate or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance DecorateWith<T>(Expression<Func<T, T>> handler)
        {
            return InterceptWith(new DecoratorInterceptor<T>(handler));
        }

        /// <summary>
        /// Register a Func to potentially decorate or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">A description of the creation action for diagnostics</param>
        /// <returns></returns>
        public ConfiguredInstance DecorateWith<T>(string description, Func<T, T> handler)
        {
            return InterceptWith(InterceptorFactory.ForFunc(description, handler));
        }

        /// <summary>
        /// Register a Func to potentially decorate or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public ConfiguredInstance DecorateWith<T>(Expression<Func<IBuildSession, T, T>> func)
        {
            return InterceptWith(new DecoratorInterceptor<T>(func));
        }

        /// <summary>
        /// Register a Func to potentially decorate or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="description">A description of the creation action for diagnostics</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public ConfiguredInstance DecorateWith<T>(string description, Func<IBuildSession, T, T> func)
        {
            return InterceptWith(InterceptorFactory.ForFunc(description, func));
        }
    }
}