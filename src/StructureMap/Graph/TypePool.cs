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

        public TypePool(PluginGraph graph)
        {
            _types.OnMissing = assembly =>
            {
                try
                {
                    return assembly.GetExportedTypes();
                }
                catch (Exception ex)
                {
                    graph.Log.RegisterError(170, ex, assembly.FullName);
                    return new Type[0];
                }
            };
        }

        public IEnumerable<Type> For(IEnumerable<Assembly> assemblies, CompositeFilter<Type> filter)
        {
            return assemblies.SelectMany(x => _types[x].Where(filter.Matches));
        }
    }
}