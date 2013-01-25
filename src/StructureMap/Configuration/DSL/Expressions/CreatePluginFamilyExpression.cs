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

        /// <summary>
        /// Define the Default Instance for this PluginType
        /// </summary>
        [Obsolete("Prefer the Use() methods")]
        public IsExpression<TPluginType> TheDefault { get { return new InstanceExpression<TPluginType>(i => registerDefault(i)); } }

        public InstanceExpression<TPluginType> MissingNamedInstanceIs { get { return new InstanceExpression<TPluginType>(i => _alterations.Add(family => family.MissingInstance = i)); } }

        /// <summary>
        /// Add multiple Instance's to this PluginType
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> AddInstances(Action<IInstanceExpression<TPluginType>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<TPluginType>(i => list.Add(i));
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
            var expression = new InstanceExpression<TPluginType>(i => Use(i));
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
            var expression = new InstanceExpression<TPluginType>(i => Add(i));
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
        /// Shorthand to say TheDefault.Is.ConstructedBy(func)
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<TPluginType> Use(Func<IContext, TPluginType> func)
        {
            return TheDefault.Is.ConstructedBy(func);
        }

        /// <summary>
        /// Shorthand to say TheDefault.Is.ConstructedBy(func)
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<TPluginType> Use(Func<TPluginType> func)
        {
            return TheDefault.Is.ConstructedBy(func);
        }

        public void Use(Instance instance)
        {
            TheDefault.IsThis(instance);
        }

        /// <summary>
        /// Shorthand to say TheDefault.IsThis(@object)
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public ObjectInstance Use(TPluginType @object)
        {
            return TheDefault.IsThis(@object);
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
        [Obsolete("Change to OnCreationForAll")]
        public CreatePluginFamilyExpression<TPluginType> OnCreation(Action<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((TPluginType) target);
                        return target;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof (TPluginType), (c, o) =>
                    {
                        handler((TPluginType) o);
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
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> OnCreationForAll(Action<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((TPluginType)target);
                        return target;
                    };

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
        [Obsolete("Change to OnCreationForAll")]
        public CreatePluginFamilyExpression<TPluginType> OnCreation(Action<IContext, TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (c, o) =>
                    {
                        handler(c, (TPluginType) o);
                        return o;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof (TPluginType), function);

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
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
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{TPluginType})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [Obsolete("Change to EnrichAllWith() -- eliminates confusion")]
        public CreatePluginFamilyExpression<TPluginType> EnrichWith(EnrichmentHandler<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (context, target) => handler((TPluginType) target);

                    var interceptor = new PluginTypeInterceptor(typeof (TPluginType), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{TPluginType})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
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
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,TPluginType})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [Obsolete("Change to EnrichAllWith -- it's to avoid confusion")]
        public CreatePluginFamilyExpression<TPluginType> EnrichWith(ContextEnrichmentHandler<TPluginType> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof (TPluginType),
                                                                (c, o) => handler(c, (TPluginType) o));
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,TPluginType})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
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
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  This will only work if the Concrete Type
        /// has no primitive constructor or mandatory Setter arguments.
        /// </summary>
        /// <typeparam name="TPluggedType"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to Add<T>()")]
        public CreatePluginFamilyExpression<TPluginType> AddConcreteType<TPluggedType>()
        {
            if (!PluginCache.GetPlugin(typeof (TPluggedType)).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }

            _alterations.Add(family =>
            {
                string name = PluginCache.GetPlugin(typeof (TPluggedType)).ConcreteKey;
                SmartInstance<TPluggedType> instance = new SmartInstance<TPluggedType>().WithName(name);
                family.AddInstance(instance);
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


        /// <summary>
        /// Largely deprecated and unnecessary with the ability to add Xml configuration files
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<TPluginType> AddInstancesFrom(MementoSource source)
        {
            _alterations.Add(family => family.AddMementoSource(source));

            return this;
        }

        private void registerDefault(Instance instance)
        {
            _alterations.Add(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
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