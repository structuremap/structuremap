using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using System.Linq;

namespace StructureMap
{

    public class Container : TypeRules, IContainer
    {
        private InterceptorLibrary _interceptorLibrary;
        private Model _model;
        private PipelineGraph _pipelineGraph;
        private PluginGraph _pluginGraph;

        public Container(Action<ConfigurationExpression> action)
        {
            var expression = new ConfigurationExpression();
            action(expression);

            construct(expression.BuildGraph());
        }

        public Container(Registry registry) : this(registry.Build())
        {
        }

        public Container() : this(new PluginGraph())
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

        protected MissingFactoryFunction onMissingFactory
        {
            set { _pipelineGraph.OnMissingFactory = value; }
        }

        #region IContainer Members

        public PluginGraph PluginGraph
        {
            get { return _pluginGraph; }
        }

        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of this Container
        /// </summary>
        public IModel Model
        {
            get { return _model; }
        }

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

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, ExplicitArguments args)
        {
            Instance defaultInstance = _pipelineGraph.GetDefault(pluginType);

            if (defaultInstance == null && pluginType.IsConcrete())
            {
                defaultInstance = new ConfiguredInstance(pluginType);
            }

            BasicInstance basicInstance = defaultInstance as BasicInstance;

            Instance instance = basicInstance == null ? defaultInstance : new ExplicitInstance(pluginType, args, basicInstance);
            BuildSession session = withNewSession(Plugin.DEFAULT);

            args.RegisterDefaults(session);

            return session.CreateInstance(pluginType, instance);
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

            return forType(type).GetAllInstances(session);
        }


        public IList<T> GetAllInstances<T>(ExplicitArguments args)
        {
            BuildSession session = withNewSession(Plugin.DEFAULT);

            args.RegisterDefaults(session);

            return getListOfTypeWithSession<T>(session);
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// PLUGINTYPE.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <param name="instance"></param>
        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            _pipelineGraph.Inject(instance);
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
        /// Injects the given object into a Container by name for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="stub"></param>
        public void Inject<T>(string name, T stub)
        {
            LiteralInstance instance = new LiteralInstance(stub).WithName(name);
            _pipelineGraph.AddInstance<T>(instance);
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
            return _pipelineGraph.ForType(pluginType).FindInstance(instanceKey) == null 
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
            return !_pipelineGraph.PluginTypes.Any(p => p.PluginType == pluginType) 
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
            return (T)(TryGetInstance(typeof (T)) ?? default(T));
        }

        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object
        /// </summary>
        /// <param name="target"></param>
        public void BuildUp(object target)
        {
            var pluggedType = target.GetType();
            IConfiguredInstance instance = _pipelineGraph.GetDefault(pluggedType) as IConfiguredInstance 
                ?? new ConfiguredInstance(pluggedType);

            var builder = PluginCache.FindBuilder(pluggedType);
            builder.BuildUp(instance, withNewSession(Plugin.DEFAULT), target);
        }

        /// <summary>
        /// Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInstance<T>(string instanceKey)
        {
            return (T)(TryGetInstance(typeof(T), instanceKey) ?? default(T));
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
            if (!IsConcrete(type))
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
        /// Injects the given object into a Container as the default for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        public void Inject(Type pluginType, object stub)
        {
            if (!CanBeCast(pluginType, stub.GetType()))
            {
                throw new StructureMapException(220, pluginType.FullName,
                                                stub.GetType().FullName);
            }


            var instance = new LiteralInstance(stub);
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public IList GetAllInstances(Type pluginType)
        {
            return forType(pluginType).GetAllInstances(withNewSession(Plugin.DEFAULT));
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

        #endregion

        private void construct(PluginGraph pluginGraph)
        {
            _interceptorLibrary = pluginGraph.InterceptorLibrary;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            _pluginGraph = pluginGraph;
            pluginGraph.Log.AssertFailures();

            _pipelineGraph = new PipelineGraph(pluginGraph);
            _model = new Model(_pipelineGraph);

            PluginCache.Compile();
        }

        private IList<T> getListOfTypeWithSession<T>(BuildSession session)
        {
            var list = new List<T>();
            foreach (T instance in forType(typeof (T)).GetAllInstances(session))
            {
                list.Add(instance);
            }

            return list;
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

        private BuildSession withNewSession(string name)
        {
            return new BuildSession(_pipelineGraph, _interceptorLibrary){RequestedName = name};
        }


        protected IInstanceFactory forType(Type type)
        {
            return _pipelineGraph.ForType(type);
        }


    }
}