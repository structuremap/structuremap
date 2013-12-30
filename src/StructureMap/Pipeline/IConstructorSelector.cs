using System;
using System.Reflection;

namespace StructureMap.Pipeline
{
    public interface IConstructorSelector
    {
        ConstructorInfo Find(Type pluggedType);
    }
}