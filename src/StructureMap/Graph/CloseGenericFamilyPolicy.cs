using System;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

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
            if (_graph.IsRunningConfigure) return null;

            if (!type.GetTypeInfo().IsGenericType) return null;

            var basicType = type.GetGenericTypeDefinition();

            if (!_graph.Families.Has(basicType))
            {
                // RIGHT HERE: do the connections thing HERE!
                var connectingTypes = _graph.ConnectedConcretions.Where(x => x.CanBeCastTo(type)).ToArray();
                if (connectingTypes.Any())
                {
                    var family = new PluginFamily(type);
                    connectingTypes.Each(family.AddType);

                    return family;
                }

                return _graph.Families.ToArray().FirstOrDefault(x => type.GetTypeInfo().IsAssignableFrom(x.PluginType.GetTypeInfo()));
            }

            var basicFamily = _graph.Families[basicType];
            var templatedParameterTypes = type.GetGenericArguments();

            return basicFamily.CreateTemplatedClone(templatedParameterTypes.ToArray());
        }

        public bool AppliesToHasFamilyChecks
        {
            get { return true; }
        }

        public bool Matches(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType) return false;

            var basicType = type.GetGenericTypeDefinition();
            return _graph.Families.Has(basicType);
        }
    }
}