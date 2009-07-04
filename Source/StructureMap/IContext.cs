using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IContext
    {
        /// <summary>
        /// Gets a reference to the <see cref="BuildStack">BuildStack</see> for this build session
        /// </summary>
        BuildStack BuildStack { get; }

        /// <summary>
        /// The concrete type of the immediate parent object in the object graph
        /// </summary>
        Type ParentType { get; }

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

        /// <summary>
        /// Gets the root "frame" of the object request
        /// </summary>
        BuildFrame Root { get; }

        /// <summary>
        /// The requested instance name of the object graph
        /// </summary>
        string RequestedName { get; }

        /// <summary>
        /// Register a default object for the given PluginType that will
        /// be used throughout the rest of the current object request
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="defaultObject"></param>
        void RegisterDefault(Type pluginType, object defaultObject);

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
        /// Gets all objects in the current object graph that can be cast
        /// to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> All<T>() where T : class;
    }
}