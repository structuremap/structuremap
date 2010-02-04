using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Construction;
using StructureMap.Diagnostics;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class Container : IContainer
    {
        private InterceptorLibrary _interceptorLibrary;
        private PipelineGraph _pipelineGraph;
        private PluginGraph _pluginGraph;

        public Container(Action<ConfigurationExpression> action)
        {
            var expression = new ConfigurationExpression();
            action(expression);

            // As explained later in the article,
            // PluginGraph is part of the Semantic Model
            // of StructureMap
            PluginGraph graph = expression.BuildGraph();

            // Take the PluginGraph object graph and
            // dynamically emit classes to build the
            // configured objects
            construct(graph);
        }

        public Container(Registry registry)
            : this(registry.Build())
        {
        }

        public Container()
            : this(new PluginGraph())
        {
        }

        /// <summary>
        /// Constructor to create an Container
        /// </summary>
        /// <param name="pluginGraph">PluginGraph containing the instance and type definitions 
        /// for the Container</param>
        public Container(PluginGraph pluginGraph)
        {
            construct(pluginGraph);
        }

        protected MissingFactoryFunction onMissingFactory { set { _pipelineGraph.OnMissingFactory = value; } }

        public PluginGraph PluginGraph { get { return _pluginGraph; } }

        #region IContainer Members

        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of this Container
        /// </summary>
        public IModel Model { get { return new Model(_pipelineGraph, this); } }

        /// <summary>
        /// Creates or finds the named instance of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public T GetInstance<T>(string instanceKey)
        {
            return (T) GetInstance(typeof (T), instanceKey);
        }

        /// <summary>
        /// Creates a new instance of the requested type T using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T GetInstance<T>(Instance instance)
        {
            return (T) GetInstance(typeof (T), instance);
        }

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public PLUGINTYPE GetInstance<PLUGINTYPE>(ExplicitArguments args)
        {
            return (PLUGINTYPE) GetInstance(typeof (PLUGINTYPE), args);
        }

        public T GetInstance<T>(ExplicitArguments args, string name)
        {
            Instance namedInstance = _pipelineGraph.ForType(typeof (T)).FindInstance(name);
            return (T) buildInstanceWithArgs(typeof (T), namedInstance, args, name);
        }

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, ExplicitArguments args)
        {
            Instance defaultInstance = _pipelineGraph.GetDefault(pluginType);
            string requestedName = Plugin.DEFAULT;

            return buildInstanceWithArgs(pluginType, defaultInstance, args, requestedName);
        }


        /// <summary>
        /// Gets all configured instances of type T using explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IList GetAllInstances(Type type, ExplicitArguments args)
        {
            BuildSession session = withNewSession(Plugin.DEFAULT);

            args.RegisterDefaults(session);

            Array instances = session.CreateInstanceArray(type, null);
            return new ArrayList(instances);
        }


        public IList<T> GetAllInstances<T>(ExplicitArguments args)
        {
            BuildSession session = withNewSession(Plugin.DEFAULT);

            args.RegisterDefaults(session);

            return getListOfTypeWithSession<T>(session);
        }


        /// <summary>
        /// Creates or finds the default instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            return (T) GetInstance(typeof (T));
        }

        [Obsolete("Please use GetInstance<T>() instead.")]
        public T FillDependencies<T>()
        {
            return (T) FillDependencies(typeof (T));
        }


        /// <summary>
        /// Creates or resolves all registered instances of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetAllInstances<T>()
        {
            BuildSession session = withNewSession(Plugin.DEFAULT);
            return getListOfTypeWithSession<T>(session);
        }

        /// <summary>
        /// Sets the default instance for all PluginType's to the designated Profile.
        /// </summary>
        /// <param name="profile"></param>
        public void SetDefaultsToProfile(string profile)
        {
            _pipelineGraph.CurrentProfile = profile;
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, string instanceKey)
        {
            return withNewSession(instanceKey).CreateInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object TryGetInstance(Type pluginType, string instanceKey)
        {
            return !_pipelineGraph.HasInstance(pluginType, instanceKey)
                       ? null
                       : GetInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object TryGetInstance(Type pluginType)
        {
            return !_pipelineGraph.HasDefaultForPluginType(pluginType)
                       ? null
                       : GetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of type T. Returns the default value of T if it is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInstance<T>()
        {
            return (T) (TryGetInstance(typeof (T)) ?? default(T));
        }

        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object
        /// </summary>
        /// <param name="target"></param>
        public void BuildUp(object target)
        {
            Type pluggedType = target.GetType();
            IConfiguredInstance instance = _pipelineGraph.GetDefault(pluggedType) as IConfiguredInstance
                                           ?? new ConfiguredInstance(pluggedType);

            IInstanceBuilder builder = PluginCache.FindBuilder(pluggedType);
            var arguments = new Arguments(instance, withNewSession(Plugin.DEFAULT));
            builder.BuildUp(arguments, target);
        }

        /// <summary>
        /// Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInstance<T>(string instanceKey)
        {
            return (T) (TryGetInstance(typeof (T), instanceKey) ?? default(T));
        }

        /// <summary>
        /// Creates or finds the default instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType)
        {
            return withNewSession(Plugin.DEFAULT).CreateInstance(pluginType);
        }


        /// <summary>
        /// Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, Instance instance)
        {
            return withNewSession(instance.Name).CreateInstance(pluginType, instance);
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        [Obsolete("Please use GetInstance(Type) instead")]
        public object FillDependencies(Type type)
        {
            if (!type.IsConcrete())
            {
                throw new StructureMapException(230, type.FullName);
            }

            var plugin = new Plugin(type);
            if (!plugin.CanBeAutoFilled)
            {
                throw new StructureMapException(230, type.FullName);
            }

            return GetInstance(type);
        }


        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public IList GetAllInstances(Type pluginType)
        {
            Array instances = withNewSession(Plugin.DEFAULT).CreateInstanceArray(pluginType, null);
            return new ArrayList(instances);
        }

        /// <summary>
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure"></param>
        public void Configure(Action<ConfigurationExpression> configure)
        {
            lock (this)
            {
                var registry = new ConfigurationExpression();
                configure(registry);

                PluginGraph graph = registry.BuildGraph();

                graph.Log.AssertFailures();

                _interceptorLibrary.ImportFrom(graph.InterceptorLibrary);
                _pipelineGraph.ImportFrom(graph);
            }
        }

        /// <summary>
        /// Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        /// <returns></returns>
        public string WhatDoIHave()
        {
            var writer = new WhatDoIHaveWriter(_pipelineGraph);
            return writer.GetText();
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With<T>(T arg)
        {
            return new ExplicitArgsExpression(this).With(arg);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With(Type pluginType, object arg)
        {
            return new ExplicitArgsExpression(this).With(pluginType, arg);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency or primitive argument
        /// with the designated name should be the next value.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public IExplicitProperty With(string argName)
        {
            return new ExplicitArgsExpression(this).With(argName);
        }


        /// <summary>
        /// Use with caution!  Does a full environment test of the configuration of this container.  Will try to create every configured
        /// instance and afterward calls any methods marked with the [ValidationMethod] attribute
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            var session = new ValidationBuildSession(_pipelineGraph, _interceptorLibrary);
            session.PerformValidations();

            if (!session.Success)
            {
                throw new StructureMapConfigurationException(session.BuildErrorMessages());
            }
        }

        /// <summary>
        /// Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EjectAllInstancesOf<T>()
        {
            _pipelineGraph.EjectAllInstancesOf<T>();
        }

        /// <summary>
        /// Convenience method to request an object using an Open Generic
        /// Type and its parameter Types
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        /// <example>
        /// IFlattener flattener1 = container.ForGenericType(typeof (IFlattener&lt;&gt;))
        ///     .WithParameters(typeof (Address)).GetInstanceAs&lt;IFlattener&gt;();
        /// </example>
        public OpenGenericTypeExpression ForGenericType(Type templateType)
        {
            return new OpenGenericTypeExpression(templateType, this);
        }

        /// <summary>
        /// Shortcut syntax for using an object to find a service that handles
        /// that type of object by using an open generic type
        /// </summary>
        /// <example>
        /// IHandler handler = container.ForObject(shipment)
        ///                        .GetClosedTypeOf(typeof (IHandler<>))
        ///                        .As<IHandler>();
        /// </example>
        /// <param name="subject"></param>
        /// <returns></returns>
        public CloseGenericTypeExpression ForObject(object subject)
        {
            return new CloseGenericTypeExpression(subject, this);
        }

        /// <summary>
        /// Starts a "Nested" Container for atomic, isolated access
        /// </summary>
        /// <returns></returns>
        public IContainer GetNestedContainer()
        {
            var container = new Container
            {
                _interceptorLibrary = _interceptorLibrary,
                _pipelineGraph = _pipelineGraph.ToNestedGraph(),
                _onDispose = nestedDispose
            };

            // Fixes a mild bug.  The child container should inject itself
            container.Configure(x => x.For<IContainer>().Use(container));

            return container;
        }

        /// <summary>
        /// Starts a new "Nested" Container for atomic, isolated service location.  Opens 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public IContainer GetNestedContainer(string profileName)
        {
            IContainer container = GetNestedContainer();
            container.SetDefaultsToProfile(profileName);

            return container;
        }


        private Action<Container> _onDispose = fullDispose;
        public void Dispose()
        {
            _onDispose(this);
        }

        private static void fullDispose(Container c)
        {
            c.Model.AllInstances.Each(i => i.EjectObject());

            nestedDispose(c);
        }

        private static void nestedDispose(Container c)
        {
            c._pipelineGraph.Dispose();
        }

        #endregion

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// PLUGINTYPE.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            Configure(x => x.For<PLUGINTYPE>().Use(instance));
        }

        public void Inject<PLUGINTYPE>(string name, PLUGINTYPE value)
        {
            Configure(x => x.For<PLUGINTYPE>().Use(value).Named(name));
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="object"></param>
        public void Inject(Type pluginType, object @object)
        {
            Configure(x => x.For(pluginType).Use(@object));
        }

        private object buildInstanceWithArgs(Type pluginType, Instance defaultInstance, ExplicitArguments args,
                                             string requestedName)
        {
            if (defaultInstance == null && pluginType.IsConcrete())
            {
                defaultInstance = new ConfiguredInstance(pluginType);
            }

            var basicInstance = defaultInstance as ConstructorInstance;

            Instance instance = basicInstance == null
                                    ? defaultInstance
                                    : basicInstance.Override(args);

            BuildSession session = withNewSession(requestedName);

            args.RegisterDefaults(session);

            return session.CreateInstance(pluginType, instance);
        }

        public ExplicitArgsExpression With(Action<ExplicitArgsExpression> action)
        {
            var expression = new ExplicitArgsExpression(this);
            action(expression);

            return expression;
        }

        private void construct(PluginGraph pluginGraph)
        {
            _interceptorLibrary = pluginGraph.InterceptorLibrary;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            _pluginGraph = pluginGraph;

            var thisInstance = new ObjectInstance(this);
            _pluginGraph.FindFamily(typeof (IContainer)).AddInstance(thisInstance);
            _pluginGraph.ProfileManager.SetDefault(typeof (IContainer), thisInstance);

            var funcInstance = new FactoryTemplate(typeof (LazyInstance<>));
            _pluginGraph.FindFamily(typeof(Func<>)).AddInstance(funcInstance);
            _pluginGraph.ProfileManager.SetDefault(typeof(Func<>), funcInstance);



            pluginGraph.Log.AssertFailures();

            _pipelineGraph = new PipelineGraph(pluginGraph);


        }

        [Obsolete("delegate to something cleaner in BuildSession")]
        private IList<T> getListOfTypeWithSession<T>(BuildSession session)
        {
            var list = new List<T>();
            foreach (T instance in session.CreateInstanceArray(typeof (T), null))
            {
                list.Add(instance);
            }

            return list;
        }


        private BuildSession withNewSession(string name)
        {
            return new BuildSession(_pipelineGraph, _interceptorLibrary)
            {
                RequestedName = name
            };
        }

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public void Inject(Type pluginType, Instance instance)
        {
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        #region Nested type: GetInstanceAsExpression

        public interface GetInstanceAsExpression
        {
            T GetInstanceAs<T>();
        }

        #endregion

        #region Nested type: OpenGenericTypeExpression

        public class OpenGenericTypeExpression : GetInstanceAsExpression
        {
            private readonly Container _container;
            private readonly Type _templateType;
            private Type _pluginType;

            public OpenGenericTypeExpression(Type templateType, Container container)
            {
                if (!templateType.IsOpenGeneric())
                {
                    throw new StructureMapException(285);
                }

                _templateType = templateType;
                _container = container;
            }

            #region GetInstanceAsExpression Members

            public T GetInstanceAs<T>()
            {
                return (T) _container.GetInstance(_pluginType);
            }

            #endregion

            public GetInstanceAsExpression WithParameters(params Type[] parameterTypes)
            {
                _pluginType = _templateType.MakeGenericType(parameterTypes);
                return this;
            }
        }

        #endregion
    }
}