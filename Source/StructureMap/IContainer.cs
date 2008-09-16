using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IContainer
    {
        T GetInstance<T>(string instanceKey);
        T GetInstance<T>();
        T FillDependencies<T>();
        object FillDependencies(Type type);
        
        IList<T> GetAllInstances<T>();
        

        T GetInstance<T>(Instance instance);

        IList GetAllInstances(Type type);

        void Configure(Action<Registry> configure);
        void Inject<PLUGINTYPE>(PLUGINTYPE instance);
        void Inject(Type pluginType, object stub);
        void Inject<T>(string name, T instance);

        void SetDefault(Type pluginType, string instanceKey);
        void SetDefault(Type pluginType, Instance instance);
        void SetDefault<T>(Instance instance);
        void SetDefault<PLUGINTYPE, CONCRETETYPE>() where CONCRETETYPE : PLUGINTYPE;
        void SetDefaultsToProfile(string profile);

        string WhatDoIHave();


        

        /// <summary>
        /// Creates a new object instance of the requested type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType);


        /// <summary>
        /// Creates a new instance of the requested type using the InstanceMemento.  Mostly used from other
        /// classes to link children members
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, Instance instance);

        /// <summary>
        /// Creates the named instance of the PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        object GetInstance(Type pluginType, string instanceKey);

        PLUGINTYPE GetInstance<PLUGINTYPE>(ExplicitArguments args);

        ExplicitArgsExpression With<T>(T arg);
        IExplicitProperty With(string argName);
        void AssertConfigurationIsValid();
        object GetInstance(Type type, ExplicitArguments args);
        PluginGraph PluginGraph { get; }
    }
}