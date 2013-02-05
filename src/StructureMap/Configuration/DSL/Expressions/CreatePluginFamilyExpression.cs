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
        // TODO -- do the set trick that pisses off Dru here to make it cleaner
        private readonly List<Action<PluginFamily>> _alterations = new List<Action<PluginFamily>>();
        private readonly List<Action<PluginGraph>> _children = new List<Action<PluginGraph>>();
        private readonly Type _pluginType;

        public CreatePluginFamilyExpression(Registry registry, InstanceScope? scope)
        {
            _pluginType = typeof (TPluginType);

            registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);

                _children.ForEach(action => action(graph));
                _alterations.ForEach(action => action(family));
            });

            if (scope != null)
            {
                _alterations.Add(family => family.SetScopeTo(scope.Value));
            }
        }

        public InstanceExpression<TPluginType> MissingNamedInstanceIs { get { return new InstanceExpression<TPluginType>(i => _alterations.Add(family => family.MissingInstance = i)); } }

        /// <summary>
        /// Add multiple Instance's to this PluginType
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> AddInstances(Action<IInstanceExpression<TPluginType>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<TPluginType>(list.Add);
            action(child);

            return alterAndContinue(family =>
            {
                foreach (Instance instance in list)
                {
                    family.AddInstance(instance);
                }
            });
        }

        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> UseSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(Use);
            configure(expression);

            return this;
        }


        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> AddSpecial(Action<IInstanceExpression<TPluginType>> configure)
        {
            var expression = new InstanceExpression<TPluginType>(Add);
            configure(expression);

            return this;
        }


        private CreatePluginFamilyExpression<TPluginType> alterAndContinue(Action<PluginFamily> action)
        {
            _alterations.Add(action);
            return this;
        }

        /// <summary>
        /// Shorthand way of saying Use<>
        /// </summary>
        /// <typeparam name="TConcreteType"></typeparam>
        /// <returns></returns>
        public SmartInstance<TConcreteType> Use<TConcreteType>() where TConcreteType : TPluginType
        {
            // This is *my* team's naming convention for generic parameters
            // I know you may not like it, but it's my article so there
            var instance = new SmartInstance<TConcreteType>();

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda using the IContext to construct the default instance of the plugin type
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<TPluginType> Use(Func<IContext, TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Use a lambda to construct the default instance of the plugin type
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
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
        /// <param name="instance"></param>
        public void Use(Instance instance)
        {
            registerDefault(instance);
        }

        /// <summary>
        /// Shorthand to say TheDefault.IsThis(@object)
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
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
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public ReferencedInstance Use(string instanceName)
        {
            var instance = new ReferencedInstance(instanceName);
            Use(instance);

            return instance;
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> Singleton()
        {
            return lifecycleIs(InstanceScope.Singleton);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Transient
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> Transient()
        {
            return lifecycleIs(InstanceScope.Transient);
        }

        private CreatePluginFamilyExpression<TPluginType> lifecycleIs(InstanceScope lifecycle)
        {
            _alterations.Add(family => family.SetScopeTo(lifecycle));
            return this;
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Hybrid lifecycle
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> HybridHttpOrThreadLocalScoped()
        {
            return lifecycleIs(InstanceScope.Hybrid);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as HttpContext scoped
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> HttpContextScoped()
        {
            return lifecycleIs(InstanceScope.HttpContext);
        }



        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
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
        /// Adds an Interceptor to only this PluginType
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
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
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
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
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{TPluginType})">OnCreationForAll()</see>,
        /// EnrichAllWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
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
        /// <param name="handler"></param>
        /// <returns></returns>
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
        /// <param name="lifecycle"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> LifecycleIs(ILifecycle lifecycle)
        {
            _alterations.Add(family => family.SetScopeTo(lifecycle));
            return this;
        }

        private void registerDefault(Instance instance)
        {
            _alterations.Add(family => family.SetDefault(instance));
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
        /// <param name="object"></param>
        /// <returns></returns>
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
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<TPluginType> Add(Func<IContext, TPluginType> func)
        {
            var instance = new LambdaInstance<TPluginType>(func);
            Add(instance);

            return instance;
        }


        public void Add(Instance instance)
        {
            _alterations.Add(f => f.AddInstance(instance));
        }
    }
}