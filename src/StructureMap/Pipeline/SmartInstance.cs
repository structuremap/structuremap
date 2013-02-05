using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    /// <summary>
    ///     Instance that builds objects with by calling constructor functions and using setter properties
    /// </summary>
    /// <typeparam name="T">The concrete type constructed by SmartInstance</typeparam>
    public class SmartInstance<T> : ConstructorInstance<SmartInstance<T>>
    {
        private readonly List<Action<T>> _actions = new List<Action<T>>();

        public SmartInstance()
            : base(typeof (T))
        {
        }

        /// <summary>
        ///     Sets the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SmartInstance<T> Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(Action<T> handler)
        {
            var interceptor = new StartupInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        protected override SmartInstance<T> thisObject()
        {
            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(Action<IContext, T> handler)
        {
            var interceptor = new StartupInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(EnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(EnrichmentHandler<TPluginType> handler)
        {
            var interceptor = new EnrichmentInterceptor<TPluginType>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(ContextEnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(ContextEnrichmentHandler<TPluginType> handler)
        {
            var interceptor = new EnrichmentInterceptor<TPluginType>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        ///     Register an <see cref="InstanceInterceptor">InstanceInterceptor</see> with this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public SmartInstance<T> InterceptWith(InstanceInterceptor interceptor)
        {
            Interceptor = interceptor;
            return this;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            var builtTarget = (T) base.build(pluginType, session);
            foreach (var action in _actions)
            {
                action(builtTarget);
            }

            return builtTarget;
        }

        /// <summary>
        ///     Set simple setter properties
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public SmartInstance<T> SetProperty(Action<T> action)
        {
            _actions.Add(action);
            return this;
        }


        /// <summary>
        ///     Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="TSettertype"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DependencyExpression<SmartInstance<T>, TSettertype> Setter<TSettertype>(
            Expression<Func<T, TSettertype>> expression)
        {
            string propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<SmartInstance<T>, TSettertype>(this, propertyName);
        }



    }
}