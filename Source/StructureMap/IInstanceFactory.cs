using System;
using System.Collections;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Interface for a "Factory" pattern class that creates object instances of the PluginType
    /// </summary>
    public interface IInstanceFactory
    {
        /// <summary>
        /// The CLR System.Type that the IInstanceManager builds instances  
        /// </summary>
        Type PluginType { get; }

        /// <summary>
        /// The InstanceKey of the default instance built by this IInstanceFactory
        /// </summary>
        string DefaultInstanceKey { get; }

        /// <summary>
        /// Establishes a reference to the parent InstanceManager
        /// </summary>
        /// <param name="instanceManager"></param>
        void SetInstanceManager(InstanceManager instanceManager);

        /// <summary>
        /// Creates an object instance for the InstanceKey
        /// </summary>
        /// <param name="InstanceKey">The named instance</param>
        /// <returns></returns>
        object GetInstance(string InstanceKey);

        /// <summary>
        /// Creates an object instance directly from the Memento
        /// </summary>
        /// <param name="instance">A representation of an object instance</param>
        /// <returns></returns>
        object GetInstance(IConfiguredInstance instance, IInstanceCreator instanceCreator);

        /// <summary>
        /// Creates a new object instance of the default instance memento
        /// </summary>
        /// <returns></returns>
        object GetInstance();

        /// <summary>
        /// Sets the default instance 
        /// </summary>
        /// <param name="InstanceKey"></param>
        void SetDefault(string InstanceKey);

        /// <summary>
        /// Makes the InstanceMemento the basis of the default instance
        /// </summary>
        /// <param name="instance"></param>
        void SetDefault(Instance instance);


        /// <summary>
        /// Returns an IList of all of the configured instances
        /// </summary>
        /// <returns></returns>
        IList GetAllInstances();

        void AddInstance(Instance instance);
        Instance AddType<T>();
        Instance GetDefault();
        object ApplyInterception(object rawValue);
    }
}