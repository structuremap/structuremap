using System;
using System.Collections;

namespace StructureMap
{
    /// <summary>
    /// Interface for a "Factory" pattern class that creates object instances of the PluginType
    /// </summary>
    public interface IInstanceFactory
    {
        /// <summary>
        /// Establishes a reference to the parent InstanceManager
        /// </summary>
        /// <param name="instanceManager"></param>
        void SetInstanceManager(InstanceManager instanceManager);

        /// <summary>
        /// The CLR System.Type that the IInstanceManager builds instances  
        /// </summary>
        Type PluginType { get; }

        /// <summary>
        /// Creates an object instance for the InstanceKey
        /// </summary>
        /// <param name="InstanceKey">The named instance</param>
        /// <returns></returns>
        object GetInstance(string InstanceKey);

        /// <summary>
        /// Creates an object instance directly from the Memento
        /// </summary>
        /// <param name="Memento">A representation of an object instance</param>
        /// <returns></returns>
        object GetInstance(InstanceMemento Memento);

        /// <summary>
        /// Creates a new object instance of the default instance memento
        /// </summary>
        /// <returns></returns>
        object GetInstance();

        /// <summary>
        /// Returns an array of objects, one for each InstanceMemento passed in
        /// </summary>
        /// <param name="Mementos"></param>
        /// <returns>An array of InstanceMemento's to build out into objects</returns>
        Array GetArray(InstanceMemento[] Mementos);

        /// <summary>
        /// Sets the default instance 
        /// </summary>
        /// <param name="InstanceKey"></param>
        void SetDefault(string InstanceKey);

        /// <summary>
        /// Makes the InstanceMemento the basis of the default instance
        /// </summary>
        /// <param name="Memento"></param>
        void SetDefault(InstanceMemento Memento);


        /// <summary>
        /// The InstanceKey of the default instance built by this IInstanceFactory
        /// </summary>
        string DefaultInstanceKey { get; }

        /// <summary>
        /// Returns an IList of all of the configured instances
        /// </summary>
        /// <returns></returns>
        IList GetAllInstances();

        void AddInstance(InstanceMemento memento);
        InstanceMemento AddType<T>();
    }
}