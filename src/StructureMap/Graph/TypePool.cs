using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class TypePool
    {
        private readonly LightweightCache<Assembly, Type[]> _types = new LightweightCache<Assembly, Type[]>();

        public TypePool()
        {
            _types.OnMissing = assembly => assembly.GetExportedTypes().ToArray();
        }

        public IEnumerable<Type> For(IEnumerable<Assembly> assemblies, CompositeFilter<Type> filter)
        {
            return assemblies.SelectMany(x => _types[x].Where(filter.Matches));
        }
    }
}