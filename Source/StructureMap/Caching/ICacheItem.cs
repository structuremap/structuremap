using System;

namespace StructureMap.Caching
{
    public interface ICacheItem
    {
        object Value { get; set; }
        int Accesses { get; }
        DateTime Created { get; }
        DateTime LastAccessed { get; }
        object Key { get; }
    }
}