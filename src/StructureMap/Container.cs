using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class Container : IContainer
    {
        private IPipelineGraph _pipelineGraph;
        private readonly object _syncLock = new object();

        public static IContainer For<T>() where T : Registry, new()
        {
            return new Container(new T());
        }

        public Container(Action<ConfigurationExpression> action) : this(PipelineGraph.For(action))
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
        public Container(PluginGraph pluginGraph) : this(PipelineGraph.BuildRoot(pluginGraph))
        {
        }

        internal Container(IPipelineGraph pipelineGraph)
        {
            if (pipelineGraph == null) throw new ArgumentNullException("pipelineGraph");

            Name = Guid.NewGuid().ToString();

            _pipelineGraph = pipelineGraph;
            _pipelineGraph.RegisterContainer(this);
        }

        /// <summary>
        ///     Provides queryable access to the configured PluginType's and Instances of this Container
        /// </summary>
        public IModel Model
        {
            get { return _pipelineGraph.ToModel(); }
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

        /// <summary>
        /// Gets the default instance of T, but built with the overridden
        /// arguments from args
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetInstance<T>(ExplicitArguments args, string name)
        {
            return (T) GetInstance(typeof (T), args, name);
        }

        /// <summary>
        ///     Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, ExplicitArguments args)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var defaultInstance = _pipelineGraph.Instances.GetDefault(pluginType);
                var requestedName = BuildSession.DEFAULT;

                return buildInstanceWithArgs(pluginType, defaultInstance, args, requestedName);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0} ,{1})", pluginType.GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Gets the named instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, ExplicitArguments args, string name)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var namedInstance = _pipelineGraph.Instances.FindInstance(pluginType, name);
                return buildInstanceWithArgs(pluginType, namedInstance, args, name);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>({1}, '{2}')", pluginType.GetFullName(), args, name);
                throw;
            }
        }

        /// <summary>
        ///     Gets all configured instances of type T using explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable GetAllInstances(Type type, ExplicitArguments args)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var session = new BuildSession(_pipelineGraph, BuildSession.DEFAULT, args);
                return session.GetAllInstances(type);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances({0}, {1})", type.GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Gets the default instance of type T using the explicitly configured arguments from the "args"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAllInstances<T>(ExplicitArguments args)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var session = new BuildSession(_pipelineGraph, BuildSession.DEFAULT, args);
                return session.GetAllInstances<T>();
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances<{0}>({1})", typeof (T).GetFullName(), args);
                throw;
            }
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var session = new BuildSession(_pipelineGraph);
                return session.GetAllInstances<T>();
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances<{0}>()", typeof (T).GetFullName());
                throw;
            }
        }

        /// <summary>
        ///     Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, string instanceKey)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                return new BuildSession(_pipelineGraph, instanceKey).CreateInstance(pluginType, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, '{1}')", pluginType.GetFullName(), instanceKey);
                throw;
            }
        }

        /// <summary>
        ///     Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public object TryGetInstance(Type pluginType, string instanceKey)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                return !_pipelineGraph.Instances.HasInstance(pluginType, instanceKey)
                    ? null
                    : GetInstance(pluginType, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0}, '{1}')", pluginType.GetFullName(), instanceKey);
                throw;
            }
        }

        /// <summary>
        ///     Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public object TryGetInstance(Type pluginType)
        {
            try
            {
                return !_pipelineGraph.Instances.HasDefaultForPluginType(pluginType)
                    ? null
                    : GetInstance(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0})", pluginType.GetFullName());
                throw;
            }
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                new BuildSession(_pipelineGraph).BuildUp(target);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.BuildUp({0})", target);
                throw;
            }
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                return new BuildSession(_pipelineGraph).GetInstance(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0})", pluginType.GetFullName());
                throw;
            }
        }


        /// <summary>
        ///     Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetInstance(Type pluginType, Instance instance)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            try
            {
                var session = new BuildSession(_pipelineGraph, instance.Name) {RootType = instance.ReturnedType};
                return session.FindObject(pluginType, instance);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, Instance: {1})", pluginType.GetFullName(), instance.Description);
                throw;
            }
        }

        /// <summary>
        ///     Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public IEnumerable GetAllInstances(Type pluginType)
        {
            try
            {
                return new BuildSession(_pipelineGraph).GetAllInstances(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances({0})", pluginType.GetFullName());
                throw;
            }
        }

        /// <summary>
        ///     Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure"></param>
        public void Configure(Action<ConfigurationExpression> configure)
        {
            lock (_syncLock)
            {
                _pipelineGraph.Configure(configure);


                if (Role == ContainerRole.Nested)
                {
                    _pipelineGraph.ValidateValidNestedScoping();
                }
            }
        }


        /// <summary>
        /// Get the child container for the named profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public IContainer GetProfile(string profileName)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            var pipeline = _pipelineGraph.Profiles.For(profileName);
            return new Container(pipeline);
        }

        /// <summary>
        /// Creates a new, anonymous child container
        /// </summary>
        /// <returns></returns>
        public IContainer CreateChildContainer()
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            var pipeline = _pipelineGraph.Profiles.NewChild();
            return new Container(pipeline);
        }

        /// <summary>
        /// The profile name of this container
        /// </summary>
        public string ProfileName
        {
            get
            {
                if (_disposedLatch)
                    throw new ObjectDisposedException(Name);

                return _pipelineGraph.Profile;
            }
        }

        /// <summary>
        ///     Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        /// <param name="pluginType">Optional parameter to filter the results down to just this plugin type</param>
        /// <param name="assembly">Optional parameter to filter the results down to only plugin types from this Assembly</param>
        /// <param name="@namespace">Optional parameter to filter the results down to only plugin types from this namespace</param>
        /// <param name="typeName">Optional parameter to filter the results down to any plugin type whose name contains this text</param>
        public string WhatDoIHave(Type pluginType = null, Assembly assembly = null, string @namespace = null,
            string typeName = null)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            var writer = new WhatDoIHaveWriter(_pipelineGraph);
            return writer.GetText(new ModelQuery
            {
                Assembly = assembly,
                Namespace = @namespace,
                PluginType = pluginType,
                TypeName = typeName
            });
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            return new ExplicitArgsExpression(this).With(argName);
        }


        /// <summary>
        ///     Use with caution!  Does a full environment test of the configuration of this container.  Will try to create every configured
        ///     instance and afterward calls any methods marked with the [ValidationMethod] attribute
        /// </summary>
        public void AssertConfigurationIsValid()
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            PipelineGraphValidator.AssertNoErrors(_pipelineGraph);
        }

        /// <summary>
        ///     Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EjectAllInstancesOf<T>()
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            _pipelineGraph.Ejector.EjectAllInstancesOf<T>();
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            return new CloseGenericTypeExpression(subject, this);
        }

        /// <summary>
        ///     Starts a "Nested" Container for atomic, isolated access
        /// </summary>
        /// <returns></returns>
        public IContainer GetNestedContainer()
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            var container = new Container(_pipelineGraph.ToNestedGraph());
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
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            var pipeline = _pipelineGraph.Profiles.For(profileName).ToNestedGraph();
            return new Container(pipeline);
        }

        private bool _disposedLatch;

        public void Dispose()
        {
            if (_disposedLatch) return;
            _disposedLatch = true;

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
        public void Inject<TPluginType>(TPluginType instance) where TPluginType : class
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            Configure(x => x.For<TPluginType>().Use(instance));
        }


        /// <summary>
        ///     Injects the given object into a Container as the default for the designated
        ///     pluginType.  Mostly used for temporarily setting up return values of the Container
        ///     to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        public void Inject(Type pluginType, object @object)
        {
            if (_disposedLatch)
                throw new ObjectDisposedException(Name);

            Configure(x => x.For(pluginType).Use(@object));
        }

        private object buildInstanceWithArgs(Type pluginType, Instance defaultInstance, ExplicitArguments args,
            string requestedName)
        {
            if (defaultInstance == null && pluginType.IsConcrete())
            {
                defaultInstance = new ConfiguredInstance(pluginType);
            }

            var basicInstance = defaultInstance as IConfiguredInstance;

            var instance = basicInstance == null
                ? defaultInstance
                : basicInstance.Override(args);

            if (instance == null)
            {
                throw new StructureMapConfigurationException("No default instance or named instance '{0}' for requested plugin type {1}", requestedName, pluginType.GetFullName());
            }

            var session = new BuildSession(_pipelineGraph, requestedName, args)
            {
                RootType = instance.ReturnedType
            };

            

            return session.FindObject(pluginType, instance);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured
        /// arguments
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With(Action<IExplicitArgsExpression> action)
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

        /// <summary>
        /// Is this container the root, a profile or child, or a nested container?
        /// </summary>
        public ContainerRole Role
        {
            get { return _pipelineGraph.Role; }
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
                    throw new StructureMapConfigurationException(
                        "Type '{0}' is not an open generic type".ToFormat(templateType.GetFullName()));
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
