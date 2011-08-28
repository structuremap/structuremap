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
    public class CreatePluginFamilyExpression<PLUGINTYPE>
    {
        private readonly List<Action<PluginFamily>> _alterations = new List<Action<PluginFamily>>();
        private readonly List<Action<PluginGraph>> _children = new List<Action<PluginGraph>>();
        private readonly Type _pluginType;

        public CreatePluginFamilyExpression(Registry registry)
        {
            _pluginType = typeof (PLUGINTYPE);

            registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);

                _children.ForEach(action => action(graph));
                _alterations.ForEach(action => action(family));
            });
        }

        /// <summary>
        /// Define the Default Instance for this PluginType
        /// </summary>
        [Obsolete("Prefer the Use() methods")]
        public IsExpression<PLUGINTYPE> TheDefault { get { return new InstanceExpression<PLUGINTYPE>(i => registerDefault(i)); } }

        public InstanceExpression<PLUGINTYPE> MissingNamedInstanceIs { get { return new InstanceExpression<PLUGINTYPE>(i => _alterations.Add(family => family.MissingInstance = i)); } }

        /// <summary>
        /// Add multiple Instance's to this PluginType
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstances(Action<IInstanceExpression<PLUGINTYPE>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<PLUGINTYPE>(i => list.Add(i));
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
        /// Conditional binding of instances
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ConditionalInstance<PLUGINTYPE> ConditionallyUse(
            Action<ConditionalInstance<PLUGINTYPE>.ConditionalInstanceExpression> configuration)
        {
            var instance = new ConditionalInstance<PLUGINTYPE>(configuration);
            Use(instance);
            
            return instance;
        }

        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> UseSpecial(Action<IInstanceExpression<PLUGINTYPE>> configure)
        {
            var expression = new InstanceExpression<PLUGINTYPE>(i => Use(i));
            configure(expression);

            return this;
        }


        /// <summary>
        /// Access to all of the uncommon Instance types
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddSpecial(Action<IInstanceExpression<PLUGINTYPE>> configure)
        {
            var expression = new InstanceExpression<PLUGINTYPE>(i => Add(i));
            configure(expression);

            return this;
        }


        private CreatePluginFamilyExpression<PLUGINTYPE> alterAndContinue(Action<PluginFamily> action)
        {
            _alterations.Add(action);
            return this;
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Prefer the usage For<ISomething>().Use<Something>()")]
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIsConcreteType<CONCRETETYPE>()
            where CONCRETETYPE : PLUGINTYPE
        {
            Type concreteType = typeof (CONCRETETYPE);

            ExpressionValidator.ValidatePluggabilityOf(concreteType).IntoPluginType(_pluginType);

            if (!PluginCache.GetPlugin(concreteType).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }

            return alterAndContinue(family =>
            {
                ConfiguredInstance instance =
                    new ConfiguredInstance(concreteType).WithName(concreteType.AssemblyQualifiedName);
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        /// <summary>
        /// Shorthand way of saying Use<>
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        public SmartInstance<CONCRETETYPE> Use<CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            // This is *my* team's naming convention for generic parameters
            // I know you may not like it, but it's my article so there
            var instance = new SmartInstance<CONCRETETYPE>();

            registerDefault(instance);

            return instance;
        }

        /// <summary>
        /// Shorthand to say TheDefault.Is.ConstructedBy(func)
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<PLUGINTYPE> Use(Func<IContext, PLUGINTYPE> func)
        {
            return TheDefault.Is.ConstructedBy(func);
        }

        /// <summary>
        /// Shorthand to say TheDefault.Is.ConstructedBy(func)
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<PLUGINTYPE> Use(Func<PLUGINTYPE> func)
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
        public ObjectInstance Use(PLUGINTYPE @object)
        {
            return TheDefault.IsThis(@object);
        }

        /// <summary>
        /// Sets the object creation of the instances of the PluginType.  For example:  PerRequest,
        /// Singleton, ThreadLocal, HttpContext, or Hybrid
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
		[Obsolete("Change to LifecycleIs(scope) or use Singleton(), HttpContextScoped(), HybridHttpOrThreadLocalScoped()")]
        public CreatePluginFamilyExpression<PLUGINTYPE> CacheBy(InstanceScope scope)
        {
            return alterAndContinue(family => family.SetScopeTo(scope));
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> Singleton()
        {
            return lifecycleIs(InstanceScope.Singleton);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Transient
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> Transient()
        {
            return lifecycleIs(InstanceScope.Transient);
        }

        private CreatePluginFamilyExpression<PLUGINTYPE> lifecycleIs(InstanceScope lifecycle)
        {
            _alterations.Add(family => family.SetScopeTo(lifecycle));
            return this;
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Hybrid lifecycle
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> HybridHttpOrThreadLocalScoped()
        {
            return lifecycleIs(InstanceScope.Hybrid);
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as HttpContext scoped
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> HttpContextScoped()
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
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(Action<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((PLUGINTYPE) target);
                        return target;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), (c, o) =>
                    {
                        handler((PLUGINTYPE) o);
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
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreationForAll(Action<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((PLUGINTYPE)target);
                        return target;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), (c, o) =>
                    {
                        handler((PLUGINTYPE)o);
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
        public CreatePluginFamilyExpression<PLUGINTYPE> InterceptWith(InstanceInterceptor interceptor)
        {
            _children.Add(
                graph =>
                {
                    var typeInterceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE),
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
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(Action<IContext, PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (c, o) =>
                    {
                        handler(c, (PLUGINTYPE) o);
                        return o;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);

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
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreationForAll(Action<IContext, PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (c, o) =>
                    {
                        handler(c, (PLUGINTYPE)o);
                        return o;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), function);

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [Obsolete("Change to EnrichAllWith() -- eliminates confusion")]
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(EnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (context, target) => handler((PLUGINTYPE) target);

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichAllWith(EnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (context, target) => handler((PLUGINTYPE)target);

                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [Obsolete("Change to EnrichAllWith -- it's to avoid confusion")]
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(ContextEnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE),
                                                                (c, o) => handler(c, (PLUGINTYPE) o));
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichAllWith(ContextEnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE),
                                                                (c, o) => handler(c, (PLUGINTYPE)o));
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }


        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  This will only work if the Concrete Type
        /// has no primitive constructor or mandatory Setter arguments.
        /// </summary>
        /// <typeparam name="PLUGGEDTYPE"></typeparam>
        /// <returns></returns>
        [Obsolete("Change to Add<T>()")]
        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<PLUGGEDTYPE>()
        {
            if (!PluginCache.GetPlugin(typeof (PLUGGEDTYPE)).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }

            _alterations.Add(family =>
            {
                string name = PluginCache.GetPlugin(typeof (PLUGGEDTYPE)).ConcreteKey;
                SmartInstance<PLUGGEDTYPE> instance = new SmartInstance<PLUGGEDTYPE>().WithName(name);
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
        public CreatePluginFamilyExpression<PLUGINTYPE> LifecycleIs(ILifecycle lifecycle)
        {
            _alterations.Add(family => family.SetScopeTo(lifecycle));
            return this;
        }


        /// <summary>
        /// Largely deprecated and unnecessary with the ability to add Xml configuration files
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstancesFrom(MementoSource source)
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
        public CreatePluginFamilyExpression<PLUGINTYPE> AlwaysUnique()
        {
            return LifecycleIs(new UniquePerRequestLifecycle());
        }

        /// <summary>
        /// Adds the object to to the PLUGINTYPE
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public ObjectInstance Add(PLUGINTYPE @object)
        {
            var instance = new ObjectInstance(@object);
            Add(instance);

            return instance;
        }

        public SmartInstance<PLUGGEDTYPE> Add<PLUGGEDTYPE>()
        {
            var instance = new SmartInstance<PLUGGEDTYPE>();
            Add(instance);

            return instance;
        }

        /// <summary>
        /// Add an Instance to this type created by a Lambda
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public LambdaInstance<PLUGINTYPE> Add(Func<IContext, PLUGINTYPE> func)
        {
            var instance = new LambdaInstance<PLUGINTYPE>(func);
            Add(instance);

            return instance;
        }


        public void Add(Instance instance)
        {
            _alterations.Add(f => f.AddInstance(instance));
        }
    }
}