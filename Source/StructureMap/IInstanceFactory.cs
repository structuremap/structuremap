using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Interface for a "Factory" pattern class that creates object instances of the PluginType
    /// </summary>
    public interface IInstanceFactory
    {
        Type PluginType { get; }
        IEnumerable<IInstance> Instances { get; }
        Instance MissingInstance { get; set; }

        Instance[] AllInstances
        {
            get;
        }

        void AddInstance(Instance instance);
        Instance AddType<T>();

        Instance FindInstance(string name);

        void ImportFrom(PluginFamily family);

        [Obsolete("Kill!!!!")]
        void EjectAllInstances();

        ILifecycle Lifecycle {get; }


    }
}