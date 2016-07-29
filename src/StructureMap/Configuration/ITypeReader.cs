using System;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public interface ITypeReader<T>
    {
        bool CanProcess(Type pluginType);
        Instance Read(T node, Type pluginType);
    }
}