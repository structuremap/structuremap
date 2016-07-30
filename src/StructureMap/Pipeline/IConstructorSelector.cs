using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public interface IConstructorSelector
    {
        ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph);
    }
}