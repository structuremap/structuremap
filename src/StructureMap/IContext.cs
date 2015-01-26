using System;
using System.Collections.Generic;

namespace StructureMap
{
    public interface IContext
    {
        /// <summary>
        /// The requested instance name of the object graph
        /// </summary>
        string RequestedName { get; }

        /// <summary>
        /// The "BuildUp" method takes in an already constructed object
        /// and uses Setter Injection to push in configured dependencies
        /// of that object
        /// </summary>
        /// <param name="target"></param>
        void BuildUp(object target);

        /// <summary>
        /// Get the object of type T that is valid for this build session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>();

        /// <summary>
        /// Get the object of type T that is valid for this build session by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>(string name);

        object GetInstance(Type pluginType);

        /// <summary>
        /// Creates or finds the named instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Same as GetInstance, but can gracefully return null if 
        /// the Type does not already exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T TryGetInstance<T>() where T : class;

        /// <summary>
        /// Same as GetInstance(name), but can gracefully return null if 
        /// the Type and name does not already exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T TryGetInstance<T>(string name) where T : class;

        /// <summary>
        /// Creates or finds the default instance of the pluginType. Returns null if the pluginType is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        object TryGetInstance(Type pluginType);

        /// <summary>
        /// Creates or finds the named instance of the pluginType. Returns null if the named instance is not known to the container.
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object TryGetInstance(Type pluginType, string instanceKey);

        /// <summary>
        /// Gets all objects in the current object graph that can be cast
        /// to T that have already been created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> All<T>() where T : class;


        /// <summary>
        /// Creates/Resolves every configured instance of PlutinType T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAllInstances<T>();

        /// <summary>
        /// Creates or resolves all registered instances of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllInstances(Type pluginType);

        /// <summary>
        /// The type of the parent object.  Useful for constructing
        /// contextual logging dependencies
        /// </summary>
        Type ParentType { get; }

        /// <summary>
        /// The type of the requested object at the very top of the 
        /// object graph
        /// </summary>
        Type RootType { get; }
    }
}