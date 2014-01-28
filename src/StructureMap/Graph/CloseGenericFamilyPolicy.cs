using System;
using System.Linq;

namespace StructureMap.Graph
{
    // Covered by testing integrated w/ the container
    public class CloseGenericFamilyPolicy : IFamilyPolicy
    {
        private readonly PluginGraph _graph;

        public CloseGenericFamilyPolicy(PluginGraph graph)
        {
            _graph = graph;
        }

        public PluginFamily Build(Type type)
        {
            if (!type.IsGenericType) return null;

            var basicType = type.GetGenericTypeDefinition();
            if (!_graph.Families.Has(basicType))
            {
                return _graph.Families.ToArray().FirstOrDefault(x => type.IsAssignableFrom(x.PluginType));
            }

            var basicFamily = _graph.Families[basicType];
            var templatedParameterTypes = type.GetGenericArguments();

            return basicFamily.CreateTemplatedClone(templatedParameterTypes);
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }

        public bool Matches(Type type)
        {
            if (!type.IsGenericType) return false;

            var basicType = type.GetGenericTypeDefinition();
            return _graph.Families.Has(basicType);
        }
    }
}