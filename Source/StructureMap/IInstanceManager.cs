using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IInstanceManager
    {
        InstanceDefaultManager DefaultManager { get; }
        T CreateInstance<T>(string instanceKey);
        T CreateInstance<T>();
        T FillDependencies<T>();
        object FillDependencies(Type type);
        void InjectStub<T>(T instance);
        IList<T> GetAllInstances<T>();
        void SetDefaultsToProfile(string profile);

        T CreateInstance<T>(Instance instance);

        /// <summary>
        /// Sets up the InstanceManager to return the object in the "stub" argument anytime
        /// any instance of the PluginType is requested
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        void InjectStub(Type pluginType, object stub);

        IList GetAllInstances(Type type);
        void AddInstance<T>(Instance instance);
        void AddInstance<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE;
        void AddDefaultInstance<PLUGINTYPE, CONCRETETYPE>();
        string WhatDoIHave();

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        void SetDefault(Type pluginType, Instance instance);

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        void SetDefault(Type pluginType, string instanceKey);

        /// <summary>
        /// Sets the default instance for the PluginType
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="instanceKey"></param>
        void SetDefault(string pluginTypeName, string instanceKey);

        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        object CreateInstance(Type pluginType);


        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        object CreateInstance(Type pluginType, Instance instance);

        /// <summary>
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object CreateInstance(Type pluginType, string instanceKey);

        PLUGINTYPE CreateInstance<PLUGINTYPE>(ExplicitArguments args);
        void Inject<PLUGINTYPE>(PLUGINTYPE instance);
    }
}