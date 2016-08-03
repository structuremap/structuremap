using StructureMap.Pipeline;
using StructureMap.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace StructureMap
{
    /// <summary>
    /// The main "container" object that implements the Service Locator pattern.
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of this Container.
        /// </summary>
        IModel Model { get; }

        /// <summary>
        /// Creates or finds the default instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <returns>The default instance of <typeparamref name="T"/>.</returns>
        T GetInstance<T>();

        /// <summary>
        /// Creates or finds the named instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <typeparamref name="T"/>.</returns>
        T GetInstance<T>(string instanceKey);

        /// <summary>
        /// Creates a new instance of the requested type <typeparamref name="T"/> using the supplied
        /// <see cref="Instance"/>. Mostly used internally.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instance">The instance of <see cref="Instance"/> used for creating of
        /// a <typeparamref name="T"/> instance.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        T GetInstance<T>(Instance instance);

        /// <summary>
        /// Creates or finds the default instance of <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <returns>The default instance of <paramref name="pluginType"/>.</returns>
        object GetInstance(Type pluginType);

        /// <summary>
        /// Creates or finds the named instance of <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <paramref name="pluginType"/>.</returns>
        object GetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Creates a new instance of the requested type <paramref name="pluginType"/> using the supplied
        /// <see cref="Instance"/>. Mostly used internally.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instance">The instance of <see cref="Instance"/> used for creating of
        /// a <paramref name="pluginType"/> instance.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        object GetInstance(Type pluginType, Instance instance);

        /// <summary>
        /// Creates or resolves all registered instances of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type which instances are to be created or resolved.</typeparam>
        /// <returns>All created or resolved instances of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        /// Creates or resolves all registered instances of the <paramref name="pluginType"/>.
        /// </summary>
        /// <param name="pluginType">The type which instances are to be created or resolved.</param>
        /// <returns>All created or resolved instances of type <paramref name="pluginType"/>.</returns>
        IEnumerable GetAllInstances(Type pluginType);

        /// <summary>
        /// Creates or finds the default instance of <typeparamref name="T"/>. Returns the default value of
        /// <typeparamref name="T"/> if it is not known to the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <returns>The default instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        T TryGetInstance<T>();

        /// <summary>
        /// Creates or finds the named instance of <typeparamref name="T"/>. Returns the default value of
        /// <typeparamref name="T"/> if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created or found.</typeparam>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        T TryGetInstance<T>(string instanceKey);

        /// <summary>
        /// Creates or finds the default instance of <paramref name="pluginType"/>. Returns <see langword="null"/> if
        /// <paramref name="pluginType"/> is not known to the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <returns>The default instance of <paramref name="pluginType"/> if resolved; <see langword="null"/> otherwise.
        /// </returns>
        object TryGetInstance(Type pluginType);

        /// <summary>
        /// Creates or finds the named instance of <paramref name="pluginType"/>. Returns <see langword="null"/> if
        /// the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created or found.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The named instance of <paramref name="pluginType"/> if resolved; <see langword="null"/> otherwise.
        /// </returns>
        object TryGetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Gets all configured instances of <typeparamref name="T"/> using explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instances are to be resolved.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>All resolved instances of <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetAllInstances<T>(ExplicitArguments args);

        /// <summary>
        /// Gets all configured instances of <paramref name="pluginType"/> using explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instances are to be resolved.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>All resolved instances of <paramref name="pluginType"/>.</returns>
        IEnumerable GetAllInstances(Type pluginType, ExplicitArguments args);

        /// <summary>
        /// Gets the default instance of <typeparamref name="T"/>, but built with the overridden arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        T GetInstance<T>(ExplicitArguments args);

        /// <summary>
        /// Gets the named instance of <typeparamref name="T"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The created instance of <typeparamref name="T"/>.</returns>
        T GetInstance<T>(ExplicitArguments args, string instanceKey);

        /// <summary>
        /// Gets the default instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        object GetInstance(Type pluginType, ExplicitArguments args);

        /// <summary>
        /// Gets the named instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <param name="instanceKey">The name of the instance.</param>
        /// <returns>The created instance of <paramref name="pluginType"/>.</returns>
        object GetInstance(Type pluginType, ExplicitArguments args, string instanceKey);

        /// <summary>
        /// Gets the default instance of <typeparamref name="T"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns the default value of <typeparamref name="T"/> if it is not known to
        /// the container.
        /// </summary>
        /// <typeparam name="T">The type which instance is to be created.</typeparam>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The default instance of <typeparamref name="T"/> if resolved; the default value of
        /// <typeparamref name="T"/> otherwise.</returns>
        T TryGetInstance<T>(ExplicitArguments args);

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
        T TryGetInstance<T>(ExplicitArguments args, string instanceKey);

        /// <summary>
        /// Gets the default instance of <paramref name="pluginType"/> using the explicitly configured arguments from
        /// <paramref name="args"/>. Returns <see langword="null"/> if the named instance is not known to
        /// the container.
        /// </summary>
        /// <param name="pluginType">The type which instance is to be created.</param>
        /// <param name="args">The explicitly configured parameters to use for construction.</param>
        /// <returns>The default instance of <paramref name="pluginType"/> if resolved; <see langword="null"/>
        ///  otherwise.</returns>
        object TryGetInstance(Type pluginType, ExplicitArguments args);

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
        object TryGetInstance(Type pluginType, ExplicitArguments args, string instanceKey);

        /// <summary>
        /// Removes all configured instances of <typeparamref name="T"/> from the Container. Use with caution!
        /// </summary>
        /// <typeparam name="T">The type which instance to be removed.</typeparam>
        void EjectAllInstancesOf<T>();

        /// <summary>
        /// The  <see cref="BuildUp"/> method takes in an already constructed object and uses Setter Injection to
        /// push in configured dependencies of that object.
        /// </summary>
        /// <param name="target">The object to inject properties to.</param>
        void BuildUp(object target);

        /// <summary>
        /// Convenience method to request an object using an Open Generic Type and its parameter Types
        /// </summary>
        /// <param name="templateType"></param>
        /// <returns></returns>
        /// <example>
        /// IFlattener flattener1 = container.ForGenericType(typeof (IFlattener&lt;&gt;))
        ///     .WithParameters(typeof (Address)).GetInstanceAs&lt;IFlattener&gt;();
        /// </example>
        Container.OpenGenericTypeExpression ForGenericType(Type templateType);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency of <paramref name="pluginType"/> should be <paramref name="arg"/>.
        /// </summary>
        /// <param name="pluginType">The argument type.</param>
        /// <param name="arg">The argument value.</param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        ExplicitArgsExpression With(Type pluginType, object arg);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        ExplicitArgsExpression With(Action<IExplicitArgsExpression> action);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency of <typeparamref name="T"/> should be <paramref name="arg"/>.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="arg">The argument value.</param>
        /// <returns>The <see cref="ExplicitArgsExpression"/> instance that could be used for setting more explicitly
        /// configured arguments and use them for creating instances.</returns>
        ExplicitArgsExpression With<T>(T arg);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments. Specifies that any
        /// dependency or primitive argument with the designated name should be the next value.
        /// </summary>
        /// <param name="argName">The argument name.</param>
        /// <returns>The <see cref="IExplicitProperty"/> instance that could be used for setting the argument value.
        /// </returns>
        IExplicitProperty With(string argName);

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
        CloseGenericTypeExpression ForObject(object subject);

        /// <summary>
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure">Additional configuration.</param>
        void Configure(Action<ConfigurationExpression> configure);

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// <typeparamref name="T"/>. Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios.
        /// </summary>
        /// <typeparam name="T">The type of the instance to inject.</typeparam>
        /// <param name="instance">The instance to inject.</param>
        void Inject<T>(T instance) where T : class;

        /// <summary>
        /// Injects the given object into a Container as the default for the designated <paramref name="pluginType"/>.
        /// Mostly used for temporarily setting up return values of the Container to introduce mocks or stubs during
        /// automated testing scenarios.
        /// </summary>
        /// <param name="pluginType">The type of the instance to inject.</param>
        /// <param name="instance">The instance to inject.</param>
        void Inject(Type pluginType, object instance);

        /// <summary>
        /// Gets a new child container for the named profile using that profile's defaults with fallback to
        /// the original parent.
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        /// <returns>The created child container.</returns>
        IContainer GetProfile(string profileName);

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
        string WhatDoIHave(Type pluginType = null, Assembly assembly = null, string @namespace = null,
            string typeName = null);

        /// <summary>
        /// Returns a textual report of all the assembly scanners used to build up this Container
        /// </summary>
        /// <returns></returns>
        string WhatDidIScan();

        /// <summary>
        /// Use with caution!  Does a full environment test of the configuration of this container.  Will try to create
        /// every configured instance and afterward calls any methods marked with
        /// <see cref="ValidationMethodAttribute"/>.
        /// </summary>
        void AssertConfigurationIsValid();

        /// <summary>
        /// Starts a "Nested" Container for atomic, isolated access.
        /// </summary>
        /// <returns>The created nested container.</returns>
        IContainer GetNestedContainer();

        /// <summary>
        /// Starts a new "Nested" Container for atomic, isolated service location using that named profile's defaults.
        /// </summary>
        /// <param name="profileName">The profile name.</param>
        /// <returns>The created nested container.</returns>
        IContainer GetNestedContainer(string profileName);

        /// <summary>
        /// The name of the container. By default this is set to a random <see cref="Guid"/>. This is a convenience
        /// property to assist with debugging. Feel free to set to anything, as this is not used in any logic.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Is this container the root, a profile or child, or a nested container?
        /// </summary>
        ContainerRole Role { get; }

        /// <summary>
        /// The profile name of this container.
        /// </summary>
        string ProfileName { get; }

        /// <summary>
        /// Creates a new, anonymous child container.
        /// </summary>
        /// <returns>The created child container.</returns>
        IContainer CreateChildContainer();

        /// <summary>
        /// Query or manipulate StructureMap's tracking of transient objects created by this Container. Use with
        /// caution.
        /// </summary>
        ITransientTracking TransientTracking { get; }

        /// <summary>
        /// If explicit transient tracking is turned on, calling this method will call Dispose() on a transient
        /// scoped object that was previously created by this <see cref="IContainer"/> and remove it from its tracking.
        /// </summary>
        /// <param name="object">The object to dispose.</param>
        void Release(object @object);


        /// <summary>
        /// Govern the behavior on Dispose() to prevent applications from 
        /// being prematurely disposed
        /// </summary>
        DisposalLock DisposalLock { get; set; }

        /// <summary>
        /// Efficiently starts a "Nested" Container using some default services
        /// </summary>
        /// <param name="defaults"></param>
        /// <returns></returns>
        IContainer GetNestedContainer(TypeArguments arguments);
    }



    public enum DisposalLock
    {
        /// <summary>
        /// If a user calls IContainer.Dispose(), ignore the request
        /// </summary>
        Ignore,

        /// <summary>
        /// Default "just dispose the container" behavior
        /// </summary>
        Unlocked,

        /// <summary>
        /// Throws an InvalidOperationException when Dispose() is called
        /// </summary>
        ThrowOnDispose
    }
}