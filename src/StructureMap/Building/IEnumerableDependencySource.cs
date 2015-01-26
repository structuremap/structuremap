using System;
using System.Collections.Generic;

namespace StructureMap.Building
{
    public interface IEnumerableDependencySource
    {
        Type ItemType { get; }
        IEnumerable<IDependencySource> Items { get; }
        string Description { get; }
        Type ReturnedType { get; }
    }
}