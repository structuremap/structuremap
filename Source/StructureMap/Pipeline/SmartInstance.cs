using System;
using System.Collections.Generic;
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
    public class SmartInstance<T> : ConfiguredInstanceBase<SmartInstance<T>>
    {
        private readonly List<Action<T>> _actions = new List<Action<T>>();

        public SmartInstance() : base(typeof (T))
        {
        }

        /// <summary>
        /// Sets the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SmartInstance<T> WithName(string instanceKey)
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
        public SmartInstance<T> OnCreation(Action<T> handler)
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
        public SmartInstance<T> EnrichWith<PLUGINTYPE>(EnrichmentHandler<PLUGINTYPE> handler)
        {
            var interceptor = new EnrichmentInterceptor<PLUGINTYPE>((c, o) => handler(o));
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
        public SmartInstance<T> EnrichWith<PLUGINTYPE>(ContextEnrichmentHandler<PLUGINTYPE> handler)
        {
            var interceptor = new EnrichmentInterceptor<PLUGINTYPE>(handler);
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

        protected override string getDescription()
        {
            return "Smart Instance for " + getConcreteType(null).FullName;
        }

        /// <summary>
        /// Define a primitive constructor argument
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        public PropertyExpression<SmartInstance<T>> WithCtorArg(string argumentName)
        {
            return new PropertyExpression<SmartInstance<T>>(this, argumentName);
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
        /// Define a primitive setter property by specifying the property name with
        /// an expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public PropertyExpression<SmartInstance<T>> WithProperty(Expression<Func<T, object>> expression)
        {
            string propertyName = ReflectionHelper.GetProperty(expression).Name;
            return WithProperty(propertyName);
        }

        /// <summary>
        /// Define a primitive setter property by specifying the property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyExpression<SmartInstance<T>> WithProperty(string propertyName)
        {
            return new PropertyExpression<SmartInstance<T>>(this, propertyName);
        }

        /// <summary>
        /// Inline definition of a constructor dependency.  Select the constructor argument by type.  Do not
        /// use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="CTORTYPE"></typeparam>
        /// <returns></returns>
        public DependencyExpression<T, CTORTYPE> CtorDependency<CTORTYPE>()
        {
            string constructorArg = getArgumentNameForType<CTORTYPE>();
            return CtorDependency<CTORTYPE>(constructorArg);
        }

        private string getArgumentNameForType<CTORTYPE>()
        {
            Plugin plugin = PluginCache.GetPlugin(getConcreteType(null));
            return plugin.FindArgumentNameForType<CTORTYPE>();
        }

        /// <summary>
        /// Inline definition of a constructor dependency.  Select the constructor argument by type and constructor name.  
        /// Use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="CTORTYPE"></typeparam>
        /// <param name="constructorArg"></param>
        /// <returns></returns>
        public DependencyExpression<T, CTORTYPE> CtorDependency<CTORTYPE>(string constructorArg)
        {
            return new DependencyExpression<T, CTORTYPE>(this, constructorArg);
        }

        /// <summary>
        /// Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DependencyExpression<T, SETTERTYPE> SetterDependency<SETTERTYPE>(
            Expression<Func<T, SETTERTYPE>> expression)
        {
            string propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<T, SETTERTYPE>(this, propertyName);
        }

        /// <summary>
        /// Inline definition of a setter dependency.  Only use this method if there
        /// is only a single property of the SETTERTYPE
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <returns></returns>
        public DependencyExpression<T, SETTERTYPE> SetterDependency<SETTERTYPE>()
        {
            return CtorDependency<SETTERTYPE>();
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<T, CHILD> TheArrayOf<CHILD>()
        {
            if (typeof (CHILD).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            Plugin plugin = PluginCache.GetPlugin(typeof (T));
            string propertyName = plugin.FindArgumentNameForType(typeof (CHILD).MakeArrayType());

            return TheArrayOf<CHILD>(propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<T, CHILD> TheArrayOf<CHILD>(string ctorOrPropertyName)
        {
            return new ArrayDefinitionExpression<T, CHILD>(this, ctorOrPropertyName);
        }

        #region Nested type: ArrayDefinitionExpression

        /// <summary>
        /// Expression Builder to help define multiple Instances for an Array dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ARRAY"></typeparam>
        public class ArrayDefinitionExpression<T, ARRAY>
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

                var child = new InstanceExpression<ARRAY>(i => list.Add(i));
                action(child);

                _instance.setChildArray(_propertyName, list.ToArray());

                return _instance;
            }

            /// <summary>
            /// Specify an array of Instance objects directly for an Array dependency
            /// </summary>
            /// <param name="arrayInstances"></param>
            /// <returns></returns>
            public SmartInstance<T> Contains(params Instance[] arrayInstances)
            {
                _instance.setChildArray(_propertyName, arrayInstances);

                return _instance;
            }
        }

        #endregion

        #region Nested type: DependencyExpression

        /// <summary>
        /// Expression Builder that helps to define child dependencies inline 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="CHILD"></typeparam>
        public class DependencyExpression<T, CHILD>
        {
            private readonly SmartInstance<T> _instance;
            private readonly string _propertyName;

            internal DependencyExpression(SmartInstance<T> instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Nested Closure to define a child dependency inline
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(Action<IInstanceExpression<CHILD>> action)
            {
                var expression = new InstanceExpression<CHILD>(i => _instance.setChild(_propertyName, i));
                action(expression);

                return _instance;
            }

            /// <summary>
            /// Shortcut to set an inline dependency to an Instance
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(Instance instance)
            {
                _instance.setChild(_propertyName, instance);
                return _instance;
            }

            /// <summary>
            /// Shortcut to set an inline dependency to a designated object
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(CHILD value)
            {
                return Is(new LiteralInstance(value));
            }

            /// <summary>
            /// Set an Inline dependency to the Default Instance of the Property type
            /// Used mostly to force an optional Setter property to be filled by
            /// StructureMap
            /// </summary>
            /// <returns></returns>
            public SmartInstance<T> IsTheDefault()
            {
                return Is(new DefaultInstance());
            }

            /// <summary>
            /// Shortcut method to define a child dependency inline
            /// </summary>
            /// <typeparam name="CONCRETETYPE"></typeparam>
            /// <returns></returns>
            public SmartInstance<T> Is<CONCRETETYPE>() where CONCRETETYPE : CHILD
            {
                return Is(new SmartInstance<CONCRETETYPE>());
            }
        }

        #endregion
    }
}