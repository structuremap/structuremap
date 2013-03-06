using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
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

            registry.alter = graph =>
            {
                PluginFamily family = graph.Families[_pluginType];

                _children.Each(action => action(graph));
                _alterations.Each(action => action(family));
            };

            if (scope != null)
            {
                lifecycleIs(scope);
            }
        }

        public InstanceExpression<TPluginType> MissingNamedInstanceIs { get { return new InstanceExpression<TPluginType>(i => _alterations.Add(family => family.MissingInstance = i)); } }

        /// <summary>
        /// Add multiple Instances to this PluginType
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> AddInstances(Action<IInstanceExpression<TPluginType>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<TPluginType>(list.Add);
            action(child);

            alter = family => list.ForEach(family.AddInstance);
            return this;
        }

        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> UseSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(Use);
            configure(expression);

            return this;
        }


        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> AddSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(Add);
            configure(expression);

            return this;
        }


        /// <summary>
        /// Shorthand way of saying Use<>
        /// </summary>
        public SmartInstance<TConcreteType> Use<TConcreteType>() where TConcreteType : TPluginType
        {
            // This is *my* team's naming convention for generic parameters
            // I know you may not like it, but it's my article so there
            var instance = new SmartInstance<TConcreteType>();

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda using the IContext to construct the default instance of the Plugin type
        /// 
        /// </summary>
        public LambdaInstance<TPluginType> Use(Func<IContext, TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda to construct the default instance of the Plugin type
        /// </summary>
        public LambdaInstance<TPluginType> Use(Func<TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Makes the supplied instance the default Instance for 
        /// TPluginType
        /// </summary>
        public void Use(Instance instance)
        {
            registerDefault(instance);
        }

        /// <summary>
        /// Shorthand to say TheDefault.IsThis(@object)
        /// </summary>
        public ObjectInstance Use(TPluginType @object)
        {
            var instance = new ObjectInstance(@object);
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
            Use(instance);

            return instance;
        }

        /// <summary>
        /// Defines a fallback instance in case no default was defined for <see cref="TPluginType"/>
        /// </summary>
        public SmartInstance<TConcreteType> UseIfNone<TConcreteType>() where TConcreteType : TPluginType
        {
            var instance = new SmartInstance<TConcreteType>();
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<TPluginType> UseIfNone(Func<IContext, TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);
            registerFallBack(instance);
            return instance;
        }

        public LambdaInstance<TPluginType> UseIfNone(Func<TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);
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
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(Action<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof(TPluginType), (c, o) =>
                    {
                        handler((TPluginType)o);
                        return o;
                    });

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(Action<IContext, TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (c, o) =>
                    {
                        handler(c, (TPluginType)o);
                        return o;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof(TPluginType), function);

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Adds an Interceptor to only this PluginType
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> InterceptWith(InstanceInterceptor interceptor)
        {
            _children.Add(
                graph =>
                {
                    var typeInterceptor = new PluginTypeInterceptor(typeof (TPluginType),
                                                                    (c, o) => interceptor.Process(o, c));
                    graph.InterceptorLibrary.AddInterceptor(typeInterceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{TPluginType})">OnCreationForAll()</see>,
        /// EnrichAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> EnrichAllWith(EnrichmentHandler<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (context, target) => handler((TPluginType)target);

                    var interceptor = new PluginTypeInterceptor(typeof(TPluginType), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,TPluginType})">OnCreationForAll()</see>,
        /// EnrichAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        public CreatePluginFamilyExpression<TPluginType> EnrichAllWith(ContextEnrichmentHandler<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof(TPluginType),
                                                                (c, o) => handler(c, (TPluginType)o));
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
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
        public ObjectInstance Add(TPluginType @object)
        {
            var instance = new ObjectInstance(@object);
            Add(instance);

            return instance;
        }

        public SmartInstance<TPluggedType> Add<TPluggedType>()
        {
            var instance = new SmartInstance<TPluggedType>();
            Add(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        public LambdaInstance<TPluginType> Add(Func<IContext, TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);
            Add(instance);

            return instance;
        }

        public void Add(Instance instance)
        {
            alter = f => f.AddInstance(instance);
        }

        private CreatePluginFamilyExpression<TPluginType> lifecycleIs(ILifecycle lifecycle)
        {
            alter = family => family.SetScopeTo(lifecycle);
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