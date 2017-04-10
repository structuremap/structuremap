using System.IO;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Pipeline;
using StructureMap.Query;
using StructureMap.TypeRules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace StructureMap
{
    public class Container : IContainer
    {
        private readonly ConcurrentBag<Container> _children = new ConcurrentBag<Container>();
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

        /// <summary>
        /// Asserts that this container is not disposed yet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        private void assertNotDisposed()
        {
            if (!_disposedLatch) return;

            switch (Role)
            {
                case ContainerRole.Root:
                    throw new ObjectDisposedException("StructureMap Application Root Container");

                case ContainerRole.Nested:
                    throw new ObjectDisposedException("StructureMap Nested Container");

                case ContainerRole.ProfileOrChild:
                    throw new ObjectDisposedException("StructureMap Child/Profile Container");
            }
        }

        internal Container(IPipelineGraph pipelineGraph)
        {
            _role = pipelineGraph.Role;
            Name = Guid.NewGuid().ToString();

            _pipelineGraph = pipelineGraph;
            _pipelineGraph.RegisterContainer(this);
        }

        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of this Container.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IModel Model
        {
            get
            {
                assertNotDisposed();
                return _pipelineGraph.ToModel();
            }
        }

        /// <summary>
        /// Creates or finds the named instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="instanceKey"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T GetInstance<T>(string instanceKey)
        {
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return (T)DoGetInstance(typeof(T), instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>('{1}')", typeof(T).GetFullName(), instanceKey);
                throw;
            }
        }

        /// <summary>
        /// Creates a new instance of the requested type <typeparamref name="T"/> using the supplied
        /// <see cref="Instance"/>. Mostly used internally.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instance">The instance of <see cref="Instance"/> used for creating of
        /// a <typeparamref name="T"/> instance.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T GetInstance<T>(Instance instance)
        {
            ArgumentChecker.ThrowIfNull("instance", instance);
            assertNotDisposed();

            try
            {
                return (T)DoGetInstance(typeof(T), instance);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>({1})", typeof(T).GetFullName(), instance.Description);
                throw;
            }
        }

        /// <summary>
        /// Gets the default instance of <typeparamref name="T"/>, but built with the overridden arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T GetInstance<T>(ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                return (T)DoGetInstance(typeof(T), args);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>({1})", typeof(T).GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Gets the named instance of <typeparamref name="T"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> or <paramref name="instanceKey"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T GetInstance<T>(ExplicitArguments args, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("args", args);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            args.SetArg("name", instanceKey);

            try
            {
                return (T)DoGetInstance(typeof(T), args, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>({1}, '{2}')", typeof(T).GetFullName(), args, instanceKey);
                throw;
            }
        }

        /// <summary>
        /// Gets the default instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="args"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object GetInstance(Type pluginType, ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                return DoGetInstance(pluginType, args);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, {1})", pluginType.GetFullName(), args);
                throw;
            }
        }

        private object DoGetInstance(Type pluginType, ExplicitArguments args)
        {
            var defaultInstance = _pipelineGraph.Instances.GetDefault(pluginType);
            var requestedName = BuildSession.DEFAULT;

            return BuildInstanceWithArgs(pluginType, defaultInstance, args, requestedName);
        }

        /// <summary>
        /// Gets the named instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/>, <paramref name="args"/> or
        /// <paramref name="instanceKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object GetInstance(Type pluginType, ExplicitArguments args, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("args", args);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            args.SetArg("name", instanceKey);

            try
            {
                return DoGetInstance(pluginType, args, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, {1}, '{2}')", pluginType.GetFullName(), args, instanceKey);
                throw;
            }
        }

        private object DoGetInstance(Type pluginType, ExplicitArguments args, string instanceKey)
        {
            var namedInstance = _pipelineGraph.Instances.FindInstance(pluginType, instanceKey);
            return BuildInstanceWithArgs(pluginType, namedInstance, args, instanceKey);
        }

        /// <summary>
        /// Gets the default instance of <typeparamref name="T"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns the default value of <typeparamref name="T"/> if it is not known to
        /// the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The default instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T TryGetInstance<T>(ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                return (T)DoTryGetInstance(typeof(T), args);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance<{0}>({1})", typeof(T).GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Gets the named instance of <typeparamref name="T"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns the default value of <typeparamref name="T"/> if it is not known to
        /// the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> or <paramref name="instanceKey"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T TryGetInstance<T>(ExplicitArguments args, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("args", args);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return (T)DoTryGetInstance(typeof(T), args, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance<{0}>({1}, '{2}')", typeof(T).GetFullName(), args, instanceKey);
                throw;
            }
        }

        /// <summary>
        /// Gets the default instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns <see langword="null"/> if the named instance is not known to
        /// the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The default instance of <paramref name="pluginType"/> if resolved; <see langword="null"/>
        ///  otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="args"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object TryGetInstance(Type pluginType, ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                return DoTryGetInstance(pluginType, args);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0}, {1})", pluginType.GetFullName(), args);
                throw;
            }
        }

        private object DoTryGetInstance(Type pluginType, ExplicitArguments args)
        {
            return !_pipelineGraph.Instances.HasDefaultForPluginType(pluginType)
                ? null
                : DoGetInstance(pluginType, args);
        }

        /// <summary>
        /// Gets the named instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns <see langword="null"/> if the named instance is not known to
        /// the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <paramref name="pluginType"/> if resolved; <see langword="null"/>
        ///  otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/>, <paramref name="args"/> or
        /// <paramref name="instanceKey"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object TryGetInstance(Type pluginType, ExplicitArguments args, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("args", args);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return DoTryGetInstance(pluginType, args, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0}, {1}, '{2}')", pluginType.GetFullName(), args, instanceKey);
                throw;
            }
        }

        private object DoTryGetInstance(Type pluginType, ExplicitArguments args, string instanceKey)
        {
            return !_pipelineGraph.Instances.HasInstance(pluginType, instanceKey)
                ? null
                : DoGetInstance(pluginType, args, instanceKey);
        }

        /// <summary>
        /// Gets all configured instances of <paramref name="pluginType"/> using explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instances are to be resolved.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>All resolved instances of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="args"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public IEnumerable GetAllInstances(Type pluginType, ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                var session = new BuildSession(_pipelineGraph, BuildSession.DEFAULT, args);
                return session.GetAllInstances(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances({0}, {1})", pluginType.GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Gets all configured instances of <typeparamref name="T"/> using explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instances are to be resolved.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>All resolved instances of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public IEnumerable<T> GetAllInstances<T>(ExplicitArguments args)
        {
            ArgumentChecker.ThrowIfNull("args", args);
            assertNotDisposed();

            try
            {
                var session = new BuildSession(_pipelineGraph, BuildSession.DEFAULT, args);
                return session.GetAllInstances<T>();
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances<{0}>({1})", typeof(T).GetFullName(), args);
                throw;
            }
        }

        /// <summary>
        /// Creates or finds the default instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <returns>The default instance of <typeparamref name="T"/>.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T GetInstance<T>()
        {
            assertNotDisposed();

            try
            {
                return (T)DoGetInstance(typeof(T));
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance<{0}>()", typeof(T).GetFullName());
                throw;
            }
        }

        /// <summary>
        /// Creates or resolves all registered instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instances are to be created or resolved.</typeparam>
        /// <returns>All created or resolved instances of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public IEnumerable<T> GetAllInstances<T>()
        {
            assertNotDisposed();

            try
            {
                var session = new BuildSession(_pipelineGraph);
                return session.GetAllInstances<T>();
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetAllInstances<{0}>()", typeof(T).GetFullName());
                throw;
            }
        }

        /// <summary>
        /// Creates or finds the named instance of <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="instanceKey"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object GetInstance(Type pluginType, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return DoGetInstance(pluginType, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, '{1}')", pluginType.GetFullName(), instanceKey);
                throw;
            }
        }

        private object DoGetInstance(Type pluginType, string instanceKey)
        {
            return new BuildSession(_pipelineGraph, instanceKey).CreateInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the named instance of <paramref name="pluginType"/>. Returns <see langword="null"/> if
        /// the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <paramref name="pluginType"/> if resolved; <see langword="null"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="instanceKey"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object TryGetInstance(Type pluginType, string instanceKey)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return DoTryGetInstance(pluginType, instanceKey);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0}, '{1}')", pluginType.GetFullName(), instanceKey);
                throw;
            }
        }

        private object DoTryGetInstance(Type pluginType, string instanceKey)
        {
            return !_pipelineGraph.Instances.HasInstance(pluginType, instanceKey)
                ? null
                : GetInstance(pluginType, instanceKey);
        }

        /// <summary>
        /// Creates or finds the default instance of <paramref name="pluginType"/>. Returns <see langword="null"/> if
        /// <paramref name="pluginType"/> is not known to the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <returns>The default instance of <paramref name="pluginType"/> if resolved; <see langword="null"/>
        /// otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object TryGetInstance(Type pluginType)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            assertNotDisposed();

            try
            {
                return DoTryGetInstance(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance({0})", pluginType.GetFullName());
                throw;
            }
        }

        private object DoTryGetInstance(Type pluginType)
        {
            return !_pipelineGraph.Instances.HasDefaultForPluginType(pluginType)
                ? null
                : GetInstance(pluginType);
        }

        /// <summary>
        /// Creates or finds the default instance of <typeparamref name="T"/>. Returns the default value of
        /// <typeparamref name="T"/> if it is not known to the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <returns>The default instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T TryGetInstance<T>()
        {
            assertNotDisposed();

            try
            {
                return (T)(DoTryGetInstance(typeof(T)) ?? default(T));
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance<{0}>()", typeof(T).GetFullName());
                throw;
            }
        }

        /// <summary>
        /// The  <see cref="BuildUp"/> method takes in an already constructed object and uses Setter Injection to
        /// push in configured dependencies of that object.
        /// </summary>
        /// <param name="target">The object to inject properties to.</param>
        /// <exception cref="ArgumentNullException">If value is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public void BuildUp(object target)
        {
            ArgumentChecker.ThrowIfNull("target", target);
            assertNotDisposed();

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
        /// Creates or finds the named instance of <typeparamref name="T"/>. Returns the default value of
        /// <typeparamref name="T"/> if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="instanceKey"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="instanceKey"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public T TryGetInstance<T>(string instanceKey)
        {
            ArgumentChecker.ThrowIfNullOrEmptyString("instanceKey", instanceKey);
            assertNotDisposed();

            try
            {
                return (T)(DoTryGetInstance(typeof(T), instanceKey) ?? default(T));
            }
            catch (StructureMapException e)
            {
                e.Push("Container.TryGetInstance<{0}>('{1}')", typeof(T).GetFullName(), instanceKey);
                throw;
            }
        }

        /// <summary>
        /// Creates or finds the default instance of <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <returns>The default instance of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object GetInstance(Type pluginType)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            assertNotDisposed();

            try
            {
                return DoGetInstance(pluginType);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0})", pluginType.GetFullName());
                throw;
            }
        }

        private object DoGetInstance(Type pluginType)
        {
            return new BuildSession(_pipelineGraph).GetInstance(pluginType);
        }

        /// <summary>
        /// Creates a new instance of the requested type <paramref name="pluginType"/> using the supplied
        /// <see cref="Instance"/>. Mostly used internally.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instance">The instance of <see cref="Instance"/> used for creating of
        /// a <paramref name="pluginType"/> instance.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> or <paramref name="instance"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public object GetInstance(Type pluginType, Instance instance)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            ArgumentChecker.ThrowIfNull("instance", instance);
            assertNotDisposed();

            try
            {
                return DoGetInstance(pluginType, instance);
            }
            catch (StructureMapException e)
            {
                e.Push("Container.GetInstance({0}, Instance: {1})", pluginType.GetFullName(), instance.Description);
                throw;
            }
        }

        private object DoGetInstance(Type pluginType, Instance instance)
        {
            var session = new BuildSession(_pipelineGraph, instance.Name) { RootType = instance.ReturnedType };
            return session.FindObject(pluginType, instance);
        }

        /// <summary>
        /// Creates or resolves all registered instances of the <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instances are to be created or resolved.</param>
        /// <returns>All created or resolved instances of type <paramref name="pluginType"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        /// <exception cref="StructureMapException">If any other error occurs.</exception>
        public IEnumerable GetAllInstances(Type pluginType)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            assertNotDisposed();

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
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure">Additional configuration.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="configure"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void Configure(Action<ConfigurationExpression> configure)
        {
            ArgumentChecker.ThrowIfNull("configure", configure);
            assertNotDisposed();

            lock (_syncLock)
            {
                _pipelineGraph.Configure(configure);

                CorrectSingletoneLifecycleForChild(_pipelineGraph);

                if (Role == ContainerRole.Nested)
                {
                    _pipelineGraph.ValidateValidNestedScoping();
                }
            }
        }

        /// <summary> 
        /// Correct the Singleton lifecycle for child containers 
        /// </summary>
        private static void CorrectSingletoneLifecycleForChild(IPipelineGraph pipelineGraph)
        {
            if (pipelineGraph.Role == ContainerRole.ProfileOrChild)
            {
                var lifecycle = new ChildContainerSingletonLifecycle(pipelineGraph.ContainerCache);

                var singletons = pipelineGraph.Instances.ImmediateInstances()
                    .Where(x => x.Lifecycle is SingletonLifecycle);

                singletons
                    .Each(x => x.SetLifecycleTo(lifecycle));

                pipelineGraph.Instances.ImmediatePluginGraph.Families.ToArray().Where(x => x.Lifecycle is SingletonLifecycle)
                    .Each(x => x.SetLifecycleTo(lifecycle));
            }
        }

        /// <summary>
        /// Gets a new child container for the named profile using that profile's defaults with fallback to
        /// the original parent.
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        /// <returns>The created child container.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="profileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="profileName"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IContainer GetProfile(string profileName)
        {
            ArgumentChecker.ThrowIfNullOrEmptyString("profileName", profileName);
            assertNotDisposed();

            var pipeline = _pipelineGraph.Profiles.For(profileName);

            CorrectSingletoneLifecycleForChild(pipeline);

            return new Container(pipeline);
        }

        /// <summary>
        /// Creates a new, anonymous child container.
        /// </summary>
        /// <returns>The created child container.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IContainer CreateChildContainer()
        {
            assertNotDisposed();

            var pipeline = _pipelineGraph.Profiles.NewChild(_pipelineGraph.Instances.ImmediatePluginGraph);
            var childContainer = new Container(pipeline);
            _children.Add(childContainer);

            return childContainer;
        }

        /// <summary>
        /// The profile name of this container.
        /// </summary>
        public string ProfileName
        {
            get { return _pipelineGraph.Profile; }
        }

        /// <summary>
        /// Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        /// <param name="pluginType">Optional parameter to filter the results down to just this plugin type.</param>
        /// <param name="assembly">Optional parameter to filter the results down to only plugin types from this
        /// <see cref="Assembly"/>.</param>
        /// <param name="namespace">Optional parameter to filter the results down to only plugin types from this
        /// namespace.</param>
        /// <param name="typeName">Optional parameter to filter the results down to any plugin type whose name contains
        ///  this text.</param>
        /// <returns>The detailed report of the configuration.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public string WhatDoIHave(Type pluginType = null, Assembly assembly = null, string @namespace = null,
            string typeName = null)
        {
            assertNotDisposed();

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
        /// Returns a textual report of all the assembly scanners used to build up this Container
        /// </summary>
        /// <returns></returns>
        public string WhatDidIScan()
        {
            var scanners = Model.Scanners;

            var writer = new StringWriter();
            writer.WriteLine("All Scanners");
            writer.WriteLine("================================================================");


            scanners.Each(scanner =>
            {
                scanner.Describe(writer);

                writer.WriteLine();
                writer.WriteLine();
            });

            var failed = TypeRepository.FailedAssemblies();
            if (failed.Any())
            {
                writer.WriteLine();
                writer.WriteLine("Assemblies that failed in the call to Assembly.GetExportedTypes()");
                failed.Each(assem =>
                {
                    writer.WriteLine("* " + assem.Record.Name);
                });
            }
            else
            {
                writer.WriteLine("No problems were encountered in exporting types from Assemblies");
            }

            return writer.ToString();
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency of <typeparamref name="T"/> should be <paramref name="arg"/>.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public ExplicitArgsExpression With<T>(T arg)
        {
            assertNotDisposed();

            return new ExplicitArgsExpression(this).With(arg);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency of <paramref name="pluginType"/> should be <paramref name="arg"/>.
        /// </summary>
        /// <param name="pluginType">The argument type.</param>
        /// <param name="arg">The argument value.</param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public ExplicitArgsExpression With(Type pluginType, object arg)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            assertNotDisposed();

            return new ExplicitArgsExpression(this).With(pluginType, arg);
        }

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency or primitive argument with the designated name should be the next value.
        /// </summary>
        /// <param name="argName">The argument name.</param>
        /// <returns>The <see cref="IExplicitProperty"/> instance that could be used for setting the argument value.
        /// </returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IExplicitProperty With(string argName)
        {
            assertNotDisposed();

            return new ExplicitArgsExpression(this).With(argName);
        }

        /// <summary>
        /// Use with caution!  Does a full environment test of the configuration of this container.  Will try to create
        /// every configured instance and afterward calls any methods marked with
        /// <see cref="ValidationMethodAttribute"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void AssertConfigurationIsValid()
        {
            assertNotDisposed();
            PipelineGraphValidator.AssertNoErrors(_pipelineGraph);
        }

        /// <summary>
        /// Removes all configured instances of <typeparamref name="T"/> from the Container. Use with caution!
        /// </summary>
        /// <typeparam name="T">The type which instance to be removed.</typeparam>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void EjectAllInstancesOf<T>()
        {
            assertNotDisposed();
            _pipelineGraph.Ejector.EjectAllInstancesOf<T>();
        }

        /// <summary>
        /// Convenience method to request an object using an Open Generic Type and its parameter Types
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        /// <example>
        /// IFlattener flattener1 = container.ForGenericType(typeof (IFlattener&lt;&gt;))
        ///     .WithParameters(typeof (Address)).GetInstanceAs&lt;IFlattener&gt;();
        /// </example>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public OpenGenericTypeExpression ForGenericType(Type templateType)
        {
            assertNotDisposed();
            return new OpenGenericTypeExpression(templateType, this);
        }

        /// <summary>
        /// Shortcut syntax for using an object to find a service that handles that type of object by using
        /// an open generic type.
        /// </summary>
        /// <example>
        /// IHandler handler = container.ForObject(shipment)
        ///                        .GetClosedTypeOf(typeof (IHandler&lt;&gt;))
        ///                        .As&lt;IHandler&gt;();
        /// </example>
        /// <param name="subject"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public CloseGenericTypeExpression ForObject(object subject)
        {
            assertNotDisposed();
            return new CloseGenericTypeExpression(subject, this);
        }

        /// <summary>
        /// Starts a "Nested" Container for atomic, isolated access.
        /// </summary>
        /// <returns>The created nested container.</returns>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IContainer GetNestedContainer()
        {
            assertNotDisposed();
            var pipeline = _pipelineGraph.ToNestedGraph();
            return GetNestedContainer(pipeline);
        }

        /// <summary>
        /// Efficiently starts a "Nested" Container using some default services
        /// </summary>
        /// <param name="defaults"></param>
        /// <returns></returns>
        public IContainer GetNestedContainer(TypeArguments arguments)
        {
            assertNotDisposed();
            var pipeline = _pipelineGraph.ToNestedGraph(arguments);
            
            return GetNestedContainer(pipeline);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts a new "Nested" Container for atomic, isolated service location using that named profile's defaults.
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        /// <returns>The created nested container.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="profileName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="profileName"/> is an empty string.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public IContainer GetNestedContainer(string profileName)
        {
            ArgumentChecker.ThrowIfNullOrEmptyString("profileName", profileName);
            assertNotDisposed();

            var pipeline = _pipelineGraph.Profiles.For(profileName).ToNestedGraph();
            return GetNestedContainer(pipeline);
        }

        private IContainer GetNestedContainer(IPipelineGraph pipeline)
        {
            var container = new Container(pipeline)
            {
                Name = "Nested-" + Name
            };

            return container;
        }

        private bool _disposedLatch;
        private readonly ContainerRole _role;

        public void Dispose()
        {
            if (DisposalLock == DisposalLock.Ignore) return;

            if (DisposalLock == DisposalLock.ThrowOnDispose) throw new InvalidOperationException("This Container has DisposalLock = DisposalLock.ThrowOnDispose and cannot be disposed until the lock is cleared");

            if (_disposedLatch) return;
            _disposedLatch = true;

            _pipelineGraph.SafeDispose();
            _pipelineGraph = null;

            _children.Each(x => x.Dispose());
        }

        /// <summary>
        /// The name of the container. By default this is set to a random <see cref="Guid"/>. This is a convenience
        /// property to assist with debugging. Feel free to set to anything, as this is not used in any logic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// <typeparamref name="T"/>. Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios.
        /// </summary>
        /// <typeparam name="T">The type of the instance to inject.</typeparam>
        /// <param name="instance">The instance to inject.</param>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void Inject<T>(T instance) where T : class
        {
            Configure(x => x.For<T>().Use(instance));
        }

        /// <summary>
        /// Injects the given object into a Container as the default for the designated <paramref name="pluginType"/>.
        /// Mostly used for temporarily setting up return values of the Container to introduce mocks or stubs during
        /// automated testing scenarios.
        /// </summary>
        /// <param name="pluginType">The type of the instance to inject.</param>
        /// <param name="instance">The instance to inject.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void Inject(Type pluginType, object instance)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);

            Configure(x => x.For(pluginType).Use(instance));
        }

        private object BuildInstanceWithArgs(Type pluginType, Instance defaultInstance, ExplicitArguments args,
            string requestedName)
        {
            if (defaultInstance == null && pluginType.IsConcrete())
            {
                defaultInstance = new ConfiguredInstance(pluginType) {Name = requestedName};
            }

            var basicInstance = defaultInstance as IOverridableInstance;

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
        /// Starts a request for an instance or instances with explicitly configured arguments.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="action"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public ExplicitArgsExpression With(Action<IExplicitArgsExpression> action)
        {
            ArgumentChecker.ThrowIfNull("action", action);
            assertNotDisposed();

            var expression = new ExplicitArgsExpression(this);
            action(expression);

            return expression;
        }

        /// <summary>
        /// Sets the default instance for <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type of the instance to inject.</param>
        /// <param name="instance">The instance to inject.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="pluginType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the container is disposed.</exception>
        public void Inject(Type pluginType, Instance instance)
        {
            ArgumentChecker.ThrowIfNull("pluginType", pluginType);
            assertNotDisposed();

            Configure(x => x.For(pluginType).Use(instance));
        }

        /// <summary>
        /// Is this container the root, a profile or child, or a nested container?
        /// </summary>
        public ContainerRole Role
        {
            get { return _role; }
        }

        #region Nested type: GetInstanceAsExpression

        public interface GetInstanceAsExpression
        {
            T GetInstanceAs<T>();
        }

        #endregion Nested type: GetInstanceAsExpression

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
                return (T)_container.GetInstance(_pluginType);
            }

            #endregion GetInstanceAsExpression Members

            public GetInstanceAsExpression WithParameters(params Type[] parameterTypes)
            {
                _pluginType = _templateType.MakeGenericType(parameterTypes);
                return this;
            }
        }

        #endregion Nested type: OpenGenericTypeExpression

        public ITransientTracking TransientTracking
        {
            get { return _pipelineGraph.Transients; }
        }

        public void Release(object @object)
        {
            assertNotDisposed();
            TransientTracking.Release(@object);
        }

        public DisposalLock DisposalLock { get; set; } = DisposalLock.Unlocked;
    }
}