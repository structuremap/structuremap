using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Building.Interception;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    // TODO -- evaluate ALL Xml comments.  Missing parameters

    /// <summary>
    /// Expression Builder that has grammars for defining policies at the 
    /// PluginType level
    /// </summary>
    public class CreatePluginFamilyExpression<TPluginType>
    {
        private readonly List<Action<PluginFamily>> _alterations = new List<Action<PluginFamily>>();
        private readonly List<Action<PluginGraph>> _children = new List<Action<PluginGraph>>();
        private readonly Type _pluginType;

        public CreatePluginFamilyExpression(Registry registry, ILifecycle scope)
        {
            _pluginType = typeof (TPluginType);

            registry.alter = graph => {
                var family = graph.Families[_pluginType];

                _children.Each(action => action(graph));
                _alterations.Each(action => action(family));
            };

            if (scope != null)
            {
                lifecycleIs(scope);
            }
        }

        public InstanceExpression<TPluginType> MissingNamedInstanceIs
        {
            get
            {
                return new InstanceExpression<TPluginType>(i => _alterations.Add(family => family.MissingInstance = i));
            }
        }

        /// <summary>
        /// Add multiple Instances to this PluginType
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> AddInstances(Action<IInstanceExpression<TPluginType>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<TPluginType>(list.Add);
            action(child);

            alter = family => list.Each(family.AddInstance);
            return this;
        }


        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> UseSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(UseInstance);
            configure(expression);

            return this;
        }


        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> AddSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(AddInstance);
            configure(expression);

            return this;
        }


        /// <summary>
        /// Shorthand way of saying Use<>
        /// </summary>
        public SmartInstance<TConcreteType, TPluginType> Use<TConcreteType>() where TConcreteType : TPluginType
        {
            // This is *my* team's naming convention for generic parameters
            // I know you may not like it, but it's my article so there
            var instance = new SmartInstance<TConcreteType, TPluginType>();

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda using the IContext to construct the default instance of the Plugin type
        /// 
        /// </summary>
        public LambdaInstance<T, TPluginType> Use<T>(Expression<Func<IContext, T>> expression) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(expression);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda using the IContext to construct the default instance of the Plugin type
        /// Use this signature if your Func is too complicated to be an Expression
        /// </summary>
        /// <param name="description">Diagnostic description of the func</param>
        public LambdaInstance<T, TPluginType> Use<T>(string description, Func<IContext, T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda to construct the default instance of the Plugin type
        /// </summary>
        public LambdaInstance<T, TPluginType> Use<T>(Expression<Func<T>> expression) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(expression);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda to construct the default instance of the Plugin type
        /// Use this overload if your func is too complicated to be an expression
        /// </summary>
        /// <param name="description">Diagnostic description of the func</param>
        public LambdaInstance<T, TPluginType> Use<T>(string description, Func<T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Makes the supplied instance the default Instance for 
        /// TPluginType
        /// </summary>
        public void UseInstance(Instance instance)
        {
            registerDefault(instance);
        }

        /// <summary>
        /// Shorthand to say TheDefault.IsThis(@object)
        /// </summary>
        public ObjectInstance<TReturned, TPluginType> Use<TReturned>(TReturned @object) where TReturned : class, TPluginType
        {
            var instance = new ObjectInstance<TReturned, TPluginType>(@object);
            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Makes the default instance of TPluginType the named
        /// instance
        /// </summary>
        public ReferencedInstance Use(string instanceName)
        {
            var instance = new ReferencedInstance(instanceName);
            UseInstance(instance);

            return instance;
        }

        /// <summary>
        /// Defines a fallback instance in case no default was defined for TPluginType
        /// </summary>
        public SmartInstance<TConcreteType, TPluginType> UseIfNone<TConcreteType>() where TConcreteType : TPluginType
        {
            var instance = new SmartInstance<TConcreteType, TPluginType>();
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<T, TPluginType> UseIfNone<T>(Expression<Func<IContext, T>> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(func);
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<T, TPluginType> UseIfNone<T>(string description, Func<IContext, T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<T, TPluginType> UseIfNone<T>(Expression<Func<T>> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(func);
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<T, TPluginType> UseIfNone<T>(string description, Func<T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);
            registerFallBack(instance);
            return instance;
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> Singleton()
        {
            return lifecycleIs(Lifecycles.Singleton);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Transient
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> Transient()
        {
            return lifecycleIs(Lifecycles.Transient);
        }

        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(Expression<Action<TPluginType>> handler,
            Func<Instance, bool> filter = null)
        {
            return InterceptWith(new ActivatorInterceptor<TPluginType>(handler), filter);
        }

        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="description">Descriptive text for diagnostics</param>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(string description,
            Action<TPluginType> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(InterceptorFactory.ForAction(description, handler), filter);
        }


        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(
            Expression<Action<IContext, TPluginType>> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(new ActivatorInterceptor<TPluginType>(handler), filter);
        }

        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="description">Descriptive text for diagnostics</param>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(string description,
            Action<IContext, TPluginType> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(InterceptorFactory.ForAction(description, handler), filter);
        }

        /// <summary>
        /// Adds an Interceptor to only this PluginType
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> InterceptWith(IInterceptor interceptor,
            Func<Instance, bool> filter = null)
        {
            _children.Add(
                graph => { graph.Policies.Interceptors.Add<TPluginType>(interceptor, filter); });

            return this;
        }

        /// <summary>
        /// Decorates all instances of TPluginType with the concrete type TDecoratorType
        /// </summary>
        public SmartInstance<TDecoratorType, TPluginType> DecorateAllWith<TDecoratorType>(Func<Instance, bool> filter = null)
            where TDecoratorType : TPluginType
        {
            var instance = new SmartInstance<TDecoratorType, TPluginType>();
            var interceptor = new DecoratorInterceptor(typeof (TPluginType), instance);
            var policy = new InterceptorPolicy<TPluginType>(interceptor, filter);

            alter = graph => graph.Policies.Interceptors.Add(policy);

            return instance;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike OnCreationForAll(),
        /// DecorateAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> DecorateAllWith(
            Expression<Func<TPluginType, TPluginType>> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(new FuncInterceptor<TPluginType>(handler), filter);
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike OnCreationForAll(),
        /// DecorateAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="description">Descriptive text for diagnostics</param>
        public CreatePluginFamilyExpression<TPluginType> DecorateAllWith(string description,
            Func<TPluginType, TPluginType> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(InterceptorFactory.ForFunc(description, handler), filter);
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike OnCreationForAll(),
        /// DecorateAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> DecorateAllWith(
            Expression<Func<IContext, TPluginType, TPluginType>> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(new FuncInterceptor<TPluginType>(handler), filter);
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike OnCreationForAll(),
        /// DecorateAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="description">Descriptive text for diagnostics</param>
        /// <param name="handler">Function that will create a decorator for the plugin type</param>
        public CreatePluginFamilyExpression<TPluginType> DecorateAllWith(string description,
            Func<IContext, TPluginType, TPluginType> handler, Func<Instance, bool> filter = null)
        {
            return InterceptWith(InterceptorFactory.ForFunc(description, handler), filter);
        }

        /// <summary>
        /// Registers an ILifecycle for this Plugin Type that executes before
        /// any object of this PluginType is created.  ILifecycle's can be
        /// used to create a custom scope
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> LifecycleIs(ILifecycle lifecycle)
        {
            lifecycleIs(lifecycle);
            return this;
        }

        /// <summary>
        /// Forces StructureMap to always use a unique instance to
        /// stop the "BuildSession" caching
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> AlwaysUnique()
        {
            return LifecycleIs(new UniquePerRequestLifecycle());
        }

        /// <summary>
        /// Adds the object to to the TPluginType
        /// </summary>
        public ObjectInstance<TReturned, TPluginType> Add<TReturned>(TReturned @object) where TReturned : class, TPluginType
        {
            var instance = new ObjectInstance<TReturned, TPluginType>(@object);
            AddInstance(instance);

            return instance;
        }

        public SmartInstance<TPluggedType, TPluginType> Add<TPluggedType>() where TPluggedType : TPluginType
        {
            var instance = new SmartInstance<TPluggedType, TPluginType>();
            AddInstance(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        public LambdaInstance<T, TPluginType> Add<T>(Expression<Func<T>> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(func);
            AddInstance(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        public LambdaInstance<T, TPluginType> Add<T>(string description, Func<T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);
            AddInstance(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        public LambdaInstance<T, TPluginType> Add<T>(Expression<Func<IContext, T>> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(func);
            AddInstance(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        public LambdaInstance<T, TPluginType> Add<T>(string description, Func<IContext, T> func) where T : TPluginType
        {
            var instance = new LambdaInstance<T, TPluginType>(description, func);
            AddInstance(instance);

            return instance;
        }

        public void AddInstance(Instance instance)
        {
            alter = f => f.AddInstance(instance);
        }

        private CreatePluginFamilyExpression<TPluginType> lifecycleIs(ILifecycle lifecycle)
        {
            alter = family => family.SetLifecycleTo(lifecycle);
            return this;
        }

        private void registerDefault(Instance instance)
        {
            alter = family => family.SetDefault(instance);
        }

        private void registerFallBack(Instance instance)
        {
            alter = family => family.SetFallback(instance);
        }

        private Action<PluginFamily> alter
        {
            set { _alterations.Add(value); }
        }
    }
}