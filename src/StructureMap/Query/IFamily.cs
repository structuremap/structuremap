using System;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public interface IFamily
    {
        Type PluginType { get; }
        void Eject(Instance instance);
        object Build(Instance instance);
        bool HasBeenCreated(Instance instance);
    }
}