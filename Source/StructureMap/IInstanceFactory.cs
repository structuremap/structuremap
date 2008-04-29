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
        /// Returns an IList of all of the configured instances
        /// </summary>
        /// <returns></returns>
        IList GetAllInstances();

        void AddInstance(Instance instance);
        Instance AddType<T>();

        object ApplyInterception(object rawValue);
        object Build(Instance instance);
    }
}