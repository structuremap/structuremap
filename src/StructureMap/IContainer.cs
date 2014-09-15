using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    /// <summary>
    /// The main "container" object that implements the Service Locator pattern
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Provides queryable access to the configured PluginType's and Instances of this Container
        /// </summary>
        IModel Model { get; }

        /// <summary>
        /// Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Creates or finds the default instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType);

        /// <summary>
        /// Creates a new instance of the requested type using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, Instance instance);

        /// <summary>
        /// Creates or finds the named instance of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        T GetInstance<T>(string instanceKey);

        /// <summary>
        /// Creates or finds the default instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>();

        /// <summary>
        /// Creates a new instance of the requested type T using the supplied Instance.  Mostly used internally
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        T GetInstance<T>(Instance instance);

        /// <summary>
        /// Creates or resolves all registered instances of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        IEnumerable GetAllInstances(Type pluginType);

        /// <summary>
        /// Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object TryGetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        object TryGetInstance(Type pluginType);

        /// <summary>
        /// Creates or finds the default instance of type T. Returns the default value of T if it is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T TryGetInstance<T>();


        /// <summary>
        /// Creates or finds the named instance of type T. Returns the default value of T if the named instance is not known to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T TryGetInstance<T>(string instanceKey);


        /// <summary>
        /// Gets all configured instances of type T using explicitly configured arguments from the "args"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable<T> GetAllInstances<T>(ExplicitArguments args);

        /// <summary>
        /// Gets the default instance of type T using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable GetAllInstances(Type type, ExplicitArguments args);

        /// <summary>
        /// Gets the default instance of T, but built with the overridden
        /// arguments from args
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetInstance<T>(ExplicitArguments args);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        ExplicitArgsExpression With<T>(T arg);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency or primitive argument
        /// with the designated name should be the next value.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        IExplicitProperty With(string argName);

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, ExplicitArguments args);

        /// <summary>
        /// Gets the named instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, ExplicitArguments args, string name);

        /// <summary>
        /// Removes all configured instances of type T from the Container.  Use with caution!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void EjectAllInstancesOf<T>();

        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object
        /// </summary>
        /// <param name="target"></param>
        void BuildUp(object target);

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
        Container.OpenGenericTypeExpression ForGenericType(Type templateType);

        /// <summary>
        /// Gets the named instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetInstance<T>(ExplicitArguments args, string name);

        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured arguments.  Specifies that any dependency
        /// of type T should be "arg"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        ExplicitArgsExpression With(Type pluginType, object arg);


        /// <summary>
        /// Starts a request for an instance or instances with explicitly configured
        /// arguments
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ExplicitArgsExpression With(Action<IExplicitArgsExpression> action);

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
        CloseGenericTypeExpression ForObject(object subject);




        /// <summary>
        /// Used to add additional configuration to a Container *after* the initialization.
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<ConfigurationExpression> configure);

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// TPluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="instance"></param>
        void Inject<TPluginType>(TPluginType instance) where TPluginType : class;

        /// <summary>
        /// Injects the given object into a Container as the default for the designated
        /// pluginType.  Mostly used for temporarily setting up return values of the Container
        /// to introduce mocks or stubs during automated testing scenarios
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="object"></param>
        void Inject(Type pluginType, object @object);

        /// <summary>
        /// Gets a new child container for the named profile using that profile's defaults with
        /// fallback to the original parent
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        IContainer GetProfile(string profileName);


        /// <summary>
        /// Returns a report detailing the complete configuration of all PluginTypes and Instances
        /// </summary>
        /// <returns></returns>
        /// <param name="pluginType">Optional parameter to filter the results down to just this plugin type</param>
        /// <param name="assembly">Optional parameter to filter the results down to only plugin types from this Assembly</param>
        /// <param name="@namespace">Optional parameter to filter the results down to only plugin types from this namespace</param>
        /// <param name="typeName">Optional parameter to filter the results down to any plugin type whose name contains this text</param>
        string WhatDoIHave(Type pluginType = null, Assembly assembly = null, string @namespace = null,
            string typeName = null);


        /// <summary>
        /// Use with caution!  Does a full environment test of the configuration of this container.  Will try to create every configured
        /// instance and afterward calls any methods marked with the [ValidationMethod] attribute
        /// </summary>
        void AssertConfigurationIsValid();


        /// <summary>
        /// Starts a "Nested" Container for atomic, isolated access
        /// </summary>
        /// <returns></returns>
        IContainer GetNestedContainer();

        /// <summary>
        /// Starts a new "Nested" Container for atomic, isolated service location.  Opens 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        IContainer GetNestedContainer(string profileName);

        /// <summary>
        /// The name of the container. By default this is set to 
        /// a random Guid. This is a convience property to 
        /// assist with debugging. Feel free to set to anything,
        /// as this is not used in any logic.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Is this container the root, a profile or child, or a nested container?
        /// </summary>
        ContainerRole Role { get; }

        /// <summary>
        /// The profile name of this container
        /// </summary>
        string ProfileName { get; }

        /// <summary>
        /// Creates a new, anonymous child container
        /// </summary>
        /// <returns></returns>
        IContainer CreateChildContainer();
    }


}