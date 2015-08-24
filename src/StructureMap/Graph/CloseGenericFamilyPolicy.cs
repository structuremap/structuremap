using System;
using System.Linq;
using System.Threading;
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
            if (!type.GetTypeInfo().IsGenericType) return null;

            var basicType = type.GetGenericTypeDefinition();

            if (!_graph.Families.Has(basicType))
            {
                try
                {
                    return tryToConnect(type);
                }
                catch (Exception)
                {
                    // TODO: HATE, HATE, HATE this. Beat later with the immutable types on
                    // the PluginGraph.Families
                    return tryToConnect(type);
                }
                
            }

            var basicFamily = _graph.Families[basicType];
            var templatedParameterTypes = type.GetGenericArguments();

            return basicFamily.CreateTemplatedClone(templatedParameterTypes.ToArray());
        }

        private PluginFamily tryToConnect(Type type)
        {
            // RIGHT HERE: do the connections thing HERE!
            var connectingTypes = _graph.ConnectedConcretions.ToArray().Where(x => x.CanBeCastTo(type)).ToArray();
            if (connectingTypes.Any())
            {
                var family = new PluginFamily(type);
                connectingTypes.Each(family.AddType);

                return family;
            }

            // This is a problem right here. Need this to be exposed
            return _graph.Families.ToArray()
                .FirstOrDefault(x => type.GetTypeInfo().IsAssignableFrom(x.PluginType.GetTypeInfo()));
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