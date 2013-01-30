using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Instance that builds objects with by calling constructor functions and using setter properties
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
        /// Sets the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SmartInstance<T> Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
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
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
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
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
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
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
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
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
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
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
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
        /// Register an <see cref="InstanceInterceptor">InstanceInterceptor</see> with this Instance
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
        /// Set simple setter properties
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public SmartInstance<T> SetProperty(Action<T> action)
        {
            _actions.Add(action);
            return this;
        }


        /// <summary>
        /// Inline definition of a setter dependency.  The property name is specified with an Expression
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

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<TChild> EnumerableOf<TChild>()
        {
            if (typeof(TChild).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            Plugin plugin = PluginCache.GetPlugin(typeof(T));
            string propertyName = plugin.FindArgumentNameForEnumerableOf(typeof(TChild));

            if (propertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof(TChild).FullName, typeof(T).FullName);
            }
            return new ArrayDefinitionExpression<TChild>(this, propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<TChild> EnumerableOf<TChild>(string ctorOrPropertyName)
        {
            if (ctorOrPropertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof(TChild).FullName, typeof(T).FullName);
            }
            return new ArrayDefinitionExpression<TChild>(this, ctorOrPropertyName);
        }


        #region Nested type: ArrayDefinitionExpression

        /// <summary>
        /// Expression Builder to help define multiple Instances for an Array dependency
        /// </summary>
        /// <typeparam name="ARRAY"></typeparam>
        public class ArrayDefinitionExpression<ARRAY>
        {
            private readonly SmartInstance<T> _instance;
            private readonly string _propertyName;

            internal ArrayDefinitionExpression(SmartInstance<T> instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Nested Closure that allows you to add an unlimited number of child Instances
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public SmartInstance<T> Contains(Action<IInstanceExpression<ARRAY>> action)
            {
                var list = new List<Instance>();

                var child = new InstanceExpression<ARRAY>(list.Add);
                action(child);

                _instance.SetCollection(_propertyName, list);

                return _instance;
            }

            /// <summary>
            /// Specify an array of Instance objects directly for an Array dependency
            /// </summary>
            /// <param name="children"></param>
            /// <returns></returns>
            public SmartInstance<T> Contains(params Instance[] children)
            {
                _instance.SetCollection(_propertyName, children);

                return _instance;
            }
        }

        #endregion

        #region Nested type: DependencyExpression

        #endregion
    }
}