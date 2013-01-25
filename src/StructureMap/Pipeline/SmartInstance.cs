using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class SmartInstance<T> : ConstructorInstance
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
        [Obsolete("Please change to Named(instanceKey)")]
        public SmartInstance<T> WithName(string instanceKey)
        {
            Name = instanceKey;
            return this;
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

        /// <summary>
        /// Define a primitive constructor argument
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        [Obsolete("Use Ctor() instead")]
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
        [Obsolete("Change to Ctor<>()")]
        public DependencyExpression<CTORTYPE> CtorDependency<CTORTYPE>()
        {
            string constructorArg = getArgumentNameForType<CTORTYPE>();
            return CtorDependency<CTORTYPE>(constructorArg);
        }

        /// <summary>
        /// Inline definition of a constructor dependency.  Select the constructor argument by type.  Do not
        /// use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="CTORTYPE"></typeparam>
        /// <returns></returns>
        public DependencyExpression<CTORTYPE> Ctor<CTORTYPE>()
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
        [Obsolete("Use Ctor<CTORTYPE>(constructorArg)")]
        public DependencyExpression<CTORTYPE> CtorDependency<CTORTYPE>(string constructorArg)
        {
            return new DependencyExpression<CTORTYPE>(this, constructorArg);
        }

        /// <summary>
        /// Inline definition of a constructor dependency.  Select the constructor argument by type and constructor name.  
        /// Use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="CTORTYPE"></typeparam>
        /// <param name="constructorArg"></param>
        /// <returns></returns>
        public DependencyExpression<CTORTYPE> Ctor<CTORTYPE>(string constructorArg)
        {
            return new DependencyExpression<CTORTYPE>(this, constructorArg);
        }

        /// <summary>
        /// Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        [Obsolete("Use Setter()")]
        public DependencyExpression<SETTERTYPE> SetterDependency<SETTERTYPE>(
            Expression<Func<T, SETTERTYPE>> expression)
        {
            string propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<SETTERTYPE>(this, propertyName);
        }


        /// <summary>
        /// Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DependencyExpression<SETTERTYPE> Setter<SETTERTYPE>(
            Expression<Func<T, SETTERTYPE>> expression)
        {
            string propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<SETTERTYPE>(this, propertyName);
        }

        /// <summary>
        /// Inline definition of a setter dependency.  Only use this method if there
        /// is only a single property of the SETTERTYPE
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Use Setter<>()")]
        public DependencyExpression<SETTERTYPE> SetterDependency<SETTERTYPE>()
        {
            return CtorDependency<SETTERTYPE>();
        }


        /// <summary>
        /// Inline definition of a setter dependency.  Only use this method if there
        /// is only a single property of the SETTERTYPE
        /// </summary>
        /// <typeparam name="SETTERTYPE"></typeparam>
        /// <returns></returns>
        public DependencyExpression<SETTERTYPE> Setter<SETTERTYPE>()
        {
            return CtorDependency<SETTERTYPE>();
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <returns></returns>
        [Obsolete("Use EnumerableOf<>")]
        public ArrayDefinitionExpression<CHILD> TheArrayOf<CHILD>()
        {
            if (typeof (CHILD).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            Plugin plugin = PluginCache.GetPlugin(typeof (T));
            string propertyName = plugin.FindArgumentNameForEnumerableOf(typeof (CHILD));
            
            return TheArrayOf<CHILD>(propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        [Obsolete("Use EnumerableOf<>")]
        public ArrayDefinitionExpression<CHILD> TheArrayOf<CHILD>(string ctorOrPropertyName)
        {
            if (ctorOrPropertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof(CHILD).FullName, typeof(T).FullName);
            }
            return new ArrayDefinitionExpression<CHILD>(this, ctorOrPropertyName);
        }


        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<CHILD> EnumerableOf<CHILD>()
        {
            if (typeof(CHILD).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            Plugin plugin = PluginCache.GetPlugin(typeof(T));
            string propertyName = plugin.FindArgumentNameForEnumerableOf(typeof(CHILD));

            return TheArrayOf<CHILD>(propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        /// This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<CHILD> EnumerableOf<CHILD>(string ctorOrPropertyName)
        {
            if (ctorOrPropertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof(CHILD).FullName, typeof(T).FullName);
            }
            return new ArrayDefinitionExpression<CHILD>(this, ctorOrPropertyName);
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

        /// <summary>
        /// Expression Builder that helps to define child dependencies inline 
        /// </summary>
        /// <typeparam name="CHILD"></typeparam>
        public class DependencyExpression<CHILD>
        {
            private readonly SmartInstance<T> _instance;
            private readonly string _propertyName;

            internal DependencyExpression(SmartInstance<T> instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Sets the value of the constructor argument to the key/value in the 
            /// AppSettings
            /// </summary>
            /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
            /// <returns></returns>
            public SmartInstance<T> EqualToAppSetting(string appSettingKey)
            {
                return EqualToAppSetting(appSettingKey, null);
            }

            /// <summary>
            /// Sets the value of the constructor argument to the key/value in the 
            /// AppSettings when it exists. Otherwise uses the provided default value.
            /// </summary>
            /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
            /// <param name="defaultValue">The value to use if an entry for <paramref name="appSettingKey"/> does not exist in the appSettings section.</param>
            /// <returns></returns>
            public SmartInstance<T> EqualToAppSetting(string appSettingKey, string defaultValue)
            {
                string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
                if (propertyValue == null) propertyValue = defaultValue;
                _instance.SetValue(_propertyName, propertyValue);
                return _instance;
            }

            /// <summary>
            /// Nested Closure to define a child dependency inline
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(Action<IInstanceExpression<CHILD>> action)
            {
                var expression = new InstanceExpression<CHILD>(i => _instance.SetChild(_propertyName, i));
                action(expression);

                return _instance;
            }

            public SmartInstance<T> Is(Func<IContext, CHILD> func)
            {
                var child = new LambdaInstance<CHILD>(func);
                return Is(child);
            }

            /// <summary>
            /// Shortcut to set an inline dependency to an Instance
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(Instance instance)
            {
                _instance.SetChild(_propertyName, instance);
                return _instance;
            }

            /// <summary>
            /// Shortcut to set an inline dependency to a designated object
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public SmartInstance<T> Is(CHILD value)
            {
                _instance.SetValue(_propertyName, value);
                return _instance;
            }


            /// <summary>
            /// Shortcut to set an inline dependency to a designated object
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            [Obsolete("Change to Is()")]
            public SmartInstance<T> EqualTo(CHILD value)
            {
                _instance.SetValue(_propertyName, value);
                return _instance;
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
            /// <typeparam name="TConcreteType"></typeparam>
            /// <returns></returns>
            public SmartInstance<T> Is<TConcreteType>() where TConcreteType : CHILD
            {
                return Is(new SmartInstance<TConcreteType>());
            }


            /// <summary>
            /// Shortcut method to define a child dependency inline and configure
            /// the child dependency
            /// </summary>
            /// <typeparam name="TConcreteType"></typeparam>
            /// <returns></returns>
            public SmartInstance<T> Is<TConcreteType>(Action<SmartInstance<TConcreteType>> configure) where TConcreteType : CHILD
            {
                var instance = new SmartInstance<TConcreteType>();
                configure(instance);
                return Is(instance);
            }

            public SmartInstance<T> Named(string name)
            {
                return Is(c => c.GetInstance<CHILD>(name));
            }
        }

        #endregion
    }
}