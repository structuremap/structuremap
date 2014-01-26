using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class TypePool
    {
        private readonly Cache<Assembly, Type[]> _types = new Cache<Assembly, Type[]>();

        public TypePool()
        {
            _types.OnMissing = assembly => assembly.GetExportedTypes();
        }

        public IEnumerable<Type> For(IEnumerable<Assembly> assemblies, CompositeFilter<Type> filter)
        {
            return assemblies.SelectMany(x => _types[x].Where(filter.Matches));
        }
    }
}