using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap
{
    public class InitializationExpression : ConfigurationExpression
    {
        internal InitializationExpression()
        {
            _parserBuilder.IgnoreDefaultFile = false;
            DefaultProfileName = string.Empty;
        }

        public bool UseDefaultStructureMapConfigFile
        {
            set { _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value; }
        }

        public bool IgnoreStructureMapConfig
        {
            set { _parserBuilder.IgnoreDefaultFile = value; }
        }

        public bool PullConfigurationFromAppConfig
        {
            set { _parserBuilder.PullConfigurationFromAppConfig = value; }
        }

        public string DefaultProfileName { get; set; }
    }

    public class ConfigurationExpression : Registry
    {
        protected readonly GraphLog _log = new GraphLog();
        private readonly List<Registry> _registries = new List<Registry>();
        protected readonly ConfigurationParserBuilder _parserBuilder;

        internal ConfigurationExpression()
        {
            _parserBuilder = new ConfigurationParserBuilder(_log);
            _parserBuilder.IgnoreDefaultFile = true;
            _parserBuilder.PullConfigurationFromAppConfig = false;

            _registries.Add(this);
            
        }

        public void AddRegistry<T>() where T : Registry, new()
        {
            AddRegistry(new T());
        }

        public void AddRegistry(Registry registry)
        {
            _registries.Add(registry);
        }

        public void AddConfigurationFromXmlFile(string fileName)
        {
            _parserBuilder.IncludeFile(fileName);
        }

        public void AddConfigurationFromNode(XmlNode node)
        {
            _parserBuilder.IncludeNode(node, "Xml configuration");
        }

        public bool IncludeConfigurationFromConfigFile
        {
            set
            {
                _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value;
            }
            
        }

        internal PluginGraph BuildGraph()
        {
            var parsers = _parserBuilder.GetParsers();
            PluginGraphBuilder builder = new PluginGraphBuilder(parsers, _registries.ToArray(), _log);

            return builder.Build();
        }
    }

    /// <summary>
    /// A collection of IInstanceFactory's.
    /// </summary>
    public class Container : TypeRules, IContainer
    {
        private InterceptorLibrary _interceptorLibrary;
        private PipelineGraph _pipelineGraph;

        public Container(Action<ConfigurationExpression> action)
        {
            ConfigurationExpression expression = new ConfigurationExpression();
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
        /// <param name="failOnException">Flags the Container to fail or trap exceptions</param>
        public Container(PluginGraph pluginGraph)
        {
            construct(pluginGraph);
        }

        private void construct(PluginGraph pluginGraph)
        {
            _interceptorLibrary = pluginGraph.InterceptorLibrary;

            if (!pluginGraph.IsSealed)
            {
                pluginGraph.Seal();
            }

            pluginGraph.Log.AssertFailures();

            _pipelineGraph = new PipelineGraph(pluginGraph);

            PluginCache.Compile();
        }

        protected MissingFactoryFunction onMissingFactory
        {
            set { _pipelineGraph.OnMissingFactory = value; }
        }

        #region IContainer Members

        public T GetInstance<T>(string instanceKey)
        {
            return (T) GetInstance(typeof (T), instanceKey);
        }

        public T GetInstance<T>(Instance instance)
        {
            return (T) GetInstance(typeof (T), instance);
        }

        public PLUGINTYPE GetInstance<PLUGINTYPE>(ExplicitArguments args)
        {
            return (PLUGINTYPE) GetInstance(typeof(PLUGINTYPE), args);
        }

        public object GetInstance(Type type, ExplicitArguments args)
        {
            Instance defaultInstance = _pipelineGraph.GetDefault(type);

            Instance instance = new ExplicitInstance(type, args, defaultInstance);
            BuildSession session = withNewSession();

            args.RegisterDefaults(session);

            return session.CreateInstance(type, instance);
        }

        public void Inject<PLUGINTYPE>(PLUGINTYPE instance)
        {
            _pipelineGraph.Inject(instance);
        }

        public T GetInstance<T>()
        {
            return (T) GetInstance(typeof (T));
        }

        public T FillDependencies<T>()
        {
            return (T) FillDependencies(typeof (T));
        }


        public void Inject<T>(string name, T stub)
        {
            LiteralInstance instance = new LiteralInstance(stub).WithName(name);
            _pipelineGraph.AddInstance<T>(instance);
        }

        public IList<T> GetAllInstances<T>()
        {
            List<T> list = new List<T>();

            BuildSession session = withNewSession();

            foreach (T instance in forType(typeof (T)).GetAllInstances(session))
            {
                list.Add(instance);
            }

            return list;
        }

        public void SetDefaultsToProfile(string profile)
        {
            _pipelineGraph.CurrentProfile = profile;
        }

        /// <summary>
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, string instanceKey)
        {
            return withNewSession().CreateInstance(pluginType, instanceKey);
        }


        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType)
        {
            return withNewSession().CreateInstance(pluginType);
        }


        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, Instance instance)
        {
            return withNewSession().CreateInstance(pluginType, instance);
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

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        public void SetDefault(Type pluginType, string instanceKey)
        {
            ReferencedInstance reference = new ReferencedInstance(instanceKey);
            _pipelineGraph.SetDefault(pluginType, reference);
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        public void SetDefault<T>(Instance instance)
        {
            SetDefault(typeof(T), instance);
        }

        public void SetDefault<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE
        {
            SetDefault<PLUGINTYPE>(new ConfiguredInstance(typeof(CONCRETETYPE)));
        }


        /// <summary>
        /// Attempts to create a new instance of the requested type.  Automatically inserts the default
        /// configured instance for each dependency in the StructureMap constructor function.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object FillDependencies(Type type)
        {
            if (!IsConcrete(type))
            {
                throw new StructureMapException(230, type.FullName);
            }

            Plugin plugin = new Plugin(type);
            if (!plugin.CanBeAutoFilled)
            {
                throw new StructureMapException(230, type.FullName);
            }

            return GetInstance(type);
        }

        /// <summary>
        /// Sets up the Container to return the object in the "stub" argument anytime
        /// any instance of the PluginType is requested
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


            LiteralInstance instance = new LiteralInstance(stub);
            _pipelineGraph.SetDefault(pluginType, instance);
        }

        public IList GetAllInstances(Type type)
        {
            return forType(type).GetAllInstances(withNewSession());
        }

        public void Configure(Action<Registry> configure)
        {
            lock (this)
            {
                Registry registry = new Registry();
                configure(registry);

                PluginGraph graph = registry.Build();

                graph.Log.AssertFailures();

                _interceptorLibrary.ImportFrom(graph.InterceptorLibrary);
                _pipelineGraph.ImportFrom(graph);
            }
        }

        public string WhatDoIHave()
        {
            WhatDoIHaveWriter writer = new WhatDoIHaveWriter(_pipelineGraph);
            return writer.GetText();
        }

        public ExplicitArgsExpression With<T>(T arg)
        {
            return new ExplicitArgsExpression(this).With<T>(arg);
        }

        public IExplicitProperty With(string argName)
        {
            return new ExplicitArgsExpression(this).With(argName);
        }

        public void AssertConfigurationIsValid()
        {
            ValidationBuildSession session = new ValidationBuildSession(_pipelineGraph, _interceptorLibrary);
            session.PerformValidations();

            if (!session.Success)
            {
                throw new StructureMapConfigurationException(session.BuildErrorMessages());
            }
        }



        #endregion

        private BuildSession withNewSession()
        {
            return new BuildSession(_pipelineGraph, _interceptorLibrary);
        }


        protected IInstanceFactory forType(Type type)
        {
            return _pipelineGraph.ForType(type);
        }

    }
}