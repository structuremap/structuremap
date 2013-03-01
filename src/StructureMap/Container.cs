using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Construction;
using StructureMap.Diagnostics;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class Container : IContainer
    {
        private IPipelineGraph _pipelineGraph;

        public Container(Action<ConfigurationExpression> action) : this(RootPipelineGraph.For(action))
        {
        }

        public Container(Registry registry)
            : this(new PluginGraphBuilder().Add(registry).Build())
        {
        }

        public Container()
            : this(new PluginGraphBuilder().Build())
        {
        }

        /// <summary>
        ///     Constructor to create an Container
        /// </summary>
        /// <param name="pluginGraph">
        ///     PluginGraph containing the instance and type definitions
        ///     for the Container
        /// </param>
        public Container(PluginGraph pluginGraph) : this(new RootPipelineGraph(pluginGraph))
        {
        }

        internal Container(IPipelineGraph pipelineGraph)
        {
            Name = Guid.NewGuid().ToString();

            _pipelineGraph = pipelineGraph;
            _pipelineGraph.Outer.Families[typeof(IContainer)].SetDefault(new ObjectInstance(this));
        }

        /// <summary>
        ///     Provides queryable access to the configured PluginType's and Instances of this Container
        /// </summary>
        public IModel Model
        {
            get { return new Model(_pipelineGraph); }
        }

        /// <summary>
        ///     Creates or finds the named instance of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public T GetInstance<T>(string instanceKey)
        {
            return (T) GetInstance(typeof (T), instanceKey);
        }

        /// <summary>
        ///     Creates a new instance of the requested type T using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T GetInstance<T>(Instance instance)
        {
            return (T) GetInstance(typeof (T), instance);
        }

        /// <summary>
        ///     Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TPluginType GetInstance<TPluginType>(ExplicitArguments args)
        {
            return (TPluginType) GetInstance(typeof (TPluginType), args);
        }

        public T GetInstance<T>(ExplicitArguments args, string name)
        {
            Instance namedInstance = _pipelineGraph.FindInstance(typeof (T), name);
            return (T) buildInstanceWithArgs(typeof (T), namedInstance, args, name);
        }

        /// <summary>
        ///     Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
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
        ///     Gets all configured instances of type T using explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable GetAllInstances(Type type, ExplicitArguments args)
        {
            var session = new BuildSession(_pipelineGraph, Plugin.DEFAULT, args);
            return session.GetAllInstances(type);
        }


        public IEnumerable<T> GetAllInstances<T>(ExplicitArguments args)
        {
            var session = new BuildSession(_pipelineGraph, Plugin.DEFAULT, args);
            return session.GetAllInstances<T>();
        }


        /// <summary>
        ///     Creates or finds the default instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            return (T) GetInstance(typeof (T));
        }

        /// <summary>
        ///     Creates or resolves all registered instances of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllInstances<T>()
        {
            var session = new BuildSession(_pipelineGraph);
            return session.GetAllInstances<T>();
        }

        /// <summary>
        ///     Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, string instanceKey)
        {
            return new BuildSession(_pipelineGraph, instanceKey).CreateInstance(pluginType, instanceKey);
        }

        /// <summary>
        ///     Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
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
        ///     Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
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
        ///     Creates or finds the default instance of type T. Returns the default value of T if it is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInstance<T>()
        {
            return (T) (TryGetInstance(typeof (T)) ?? default(T));
        }

        /// <summary>
        ///     The "BuildUp" method takes in an already constructed object
        ///     and uses Setter Injection to push in configured dependencies
        ///     of that object
        /// </summary>
        /// <param name="target"></param>
        public void BuildUp(object target)
        {
            Type pluggedType = target.GetType();
            IConfiguredInstance instance = _pipelineGraph.GetDefault(pluggedType) as IConfiguredInstance
                                           ?? new ConfiguredInstance(pluggedType);

            IInstanceBuilder builder = PluginCache.FindBuilder(pluggedType);
            var arguments = new Arguments(instance, new BuildSession(_pipelineGraph));
            builder.BuildUp(arguments, target);
        }

        /// <summary>
        ///     Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T TryGetInstance<T>(string instanceKey)
        {
            return (T) (TryGetInstance(typeof (T), instanceKey) ?? default(T));
        }

        /// <summary>
        ///     Creates or finds the default instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType)
        {
            return new BuildSession(_pipelineGraph).GetInstance(pluginType);
        }


        /// <summary>
        ///     Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, Instance instance)
        {
            var session = new BuildSession(_pipelineGraph, instance.Name);
            return session.FindObject(pluginType, instance);
        }

        /// <summary>
        ///     Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public IEnumerable GetAllInstances(Type pluginType)
        {
            return new BuildSession(_pipelineGraph).GetAllInstances(pluginType);
        }

        /// <summary>
        ///     Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure"></param>
        public void Configure(Action<ConfigurationExpression> configure)
        {
            lock (this)
            {
                var registry = new ConfigurationExpression();
                configure(registry);

                var builder = new PluginGraphBuilder(_pipelineGraph.Outer);
                builder.Add(registry);

                builder.RunConfigurations();
            }
        }

        public IContainer GetProfile(string profileName)
        {
            var pipeline = _pipelineGraph.ForProfile(profileName);
            return new Container(pipeline);
        }

        /// <summary>
        ///     Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        public string WhatDoIHave()
        {
            var writer = new WhatDoIHaveWriter(_pipelineGraph);
            return writer.GetText();
        }

        /// <summary>
        ///     Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        ///     of type T should be "arg"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With<T>(T arg)
        {
            return new ExplicitArgsExpression(this).With(arg);
        }

        /// <summary>
        ///     Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        ///     of type T should be "arg"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With(Type pluginType, object arg)
        {
            return new ExplicitArgsExpression(this).With(pluginType, arg);
        }

        /// <summary>
        ///     Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency or primitive argument
        ///     with the designated name should be the next value.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public IExplicitProperty With(string argName)
        {
            return new ExplicitArgsExpression(this).With(argName);
        }


        /// <summary>
        ///     Use with caution!  Does a full environment test of the configuration of this container.  Will try to create every configured
        ///     instance and afterward calls any methods marked with the [ValidationMethod] attribute
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            var session = new ValidationBuildSession(_pipelineGraph);
            session.PerformValidations();

            if (!session.Success)
            {
                throw new StructureMapConfigurationException(session.BuildErrorMessages());
            }
        }

        /// <summary>
        ///     Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EjectAllInstancesOf<T>()
        {
            new GraphEjector(_pipelineGraph.Outer).EjectAllInstancesOf<T>();
        }

        /// <summary>
        ///     Convenience method to request an object using an Open Generic
        ///     Type and its parameter Types
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        /// <example>
        ///     IFlattener flattener1 = container.ForGenericType(typeof (IFlattener&lt;&gt;))
        ///     .WithParameters(typeof (Address)).GetInstanceAs&lt;IFlattener&gt;();
        /// </example>
        public OpenGenericTypeExpression ForGenericType(Type templateType)
        {
            return new OpenGenericTypeExpression(templateType, this);
        }

        /// <summary>
        ///     Shortcut syntax for using an object to find a service that handles
        ///     that type of object by using an open generic type
        /// </summary>
        /// <example>
        ///     IHandler handler = container.ForObject(shipment)
        ///     .GetClosedTypeOf(typeof (IHandler<>))
        ///     .As<IHandler>();
        /// </example>
        /// <param name="subject"></param>
        /// <returns></returns>
        public CloseGenericTypeExpression ForObject(object subject)
        {
            return new CloseGenericTypeExpression(subject, this);
        }

        /// <summary>
        ///     Starts a "Nested" Container for atomic, isolated access
        /// </summary>
        /// <returns></returns>
        public IContainer GetNestedContainer()
        {
            var container =  new Container(_pipelineGraph.ToNestedGraph());
            container.Name = "Nested-" + container.Name;

            return container;
        }

        /// <summary>
        ///     Starts a new "Nested" Container for atomic, isolated service location.  Opens
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public IContainer GetNestedContainer(string profileName)
        {
            var pipeine = _pipelineGraph.ForProfile(profileName).ToNestedGraph();
            return new Container(pipeine);
        }

        public void Dispose()
        {
            _pipelineGraph.SafeDispose();
            _pipelineGraph = null;
        }

        /// <summary>
        ///     The name of the container. By default this is set to
        ///     a random Guid. This is a convience property to
        ///     assist with debugging. Feel free to set to anything,
        ///     as this is not used in any logic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Injects the given object into a Container as the default for the designated
        ///     TPluginType.  Mostly used for temporarily setting up return values of the Container
        ///     to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="instance"></param>
        public void Inject<TPluginType>(TPluginType instance)
        {
            Configure(x => x.For<TPluginType>().Use(instance));
        }

        public void Inject<TPluginType>(string name, TPluginType value)
        {
            Configure(x => x.For<TPluginType>().Use(value).Named(name));
        }

        /// <summary>
        ///     Injects the given object into a Container as the default for the designated
        ///     pluginType.  Mostly used for temporarily setting up return values of the Container
        ///     to introduce mocks or stubs during automated testing scenarios
        /// </summary>
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

            var instance = basicInstance == null
                                    ? defaultInstance
                                    : basicInstance.Override(args);

            var session = new BuildSession(_pipelineGraph, requestedName, args);

            return session.FindObject(pluginType, instance);
        }

        public ExplicitArgsExpression With(Action<ExplicitArgsExpression> action)
        {
            var expression = new ExplicitArgsExpression(this);
            action(expression);

            return expression;
        }

        /// <summary>
        ///     Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        public void Inject(Type pluginType, Instance instance)
        {
            Configure(x => x.For(pluginType).Use(instance));
        }

        private void nameContainer(IContainer container)
        {
            container.Name = "Nested-" + container.Name;
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