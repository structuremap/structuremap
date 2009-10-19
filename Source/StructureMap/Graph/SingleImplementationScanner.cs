using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Graph
{
    public class SingleImplementationScanner : TypeRules, IHeavyweightTypeScanner
    {
        public void Process(PluginGraph graph, IEnumerable<TypeMap> typeMaps)
        {
            foreach(var map in typeMaps.Where(map => map.ConcreteTypes.Count == 1))
            {
                graph.AddType(map.PluginType, map.ConcreteTypes[0]);
            }
        }
    }
}