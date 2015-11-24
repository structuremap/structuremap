using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public interface IConstructorSelector
    {
        ConstructorInfo Find(Type pluggedType, PluginGraph graph);
    }
}