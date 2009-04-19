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
        IBuildPolicy Policy { get; }
        Instance MissingInstance { get; set; }

        Instance[] AllInstances
        {
            get;
        }

        void AddInstance(Instance instance);
        Instance AddType<T>();

        [Obsolete("Return the list of Instances instead")]
        IList GetAllInstances(BuildSession session);

        object Build(BuildSession session, Instance instance);
        Instance FindInstance(string name);

        void ImportFrom(PluginFamily family);
        void EjectAllInstances();


    }
}