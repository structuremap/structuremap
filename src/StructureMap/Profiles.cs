using System.Collections.Concurrent;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap
{
    public class Profiles
    {
        private readonly PluginGraph _pluginGraph;
        private readonly IPipelineGraph _root;
        private readonly ConcurrentDictionary<string, IPipelineGraph> _profiles = new ConcurrentDictionary<string, IPipelineGraph>();

        public Profiles(PluginGraph pluginGraph, IPipelineGraph root)
        {
            _pluginGraph = pluginGraph;
            _root = root;
        }

        public IPipelineGraph NewChild(PluginGraph parent)
        {
            var childGraph = new PluginGraph
            {
                Parent = parent
            };

            var instances = new ComplexInstanceGraph(_root, childGraph, ContainerRole.ProfileOrChild);

            return new PipelineGraph(childGraph, instances, _root, _root.Singletons, _root.Transients);
        }

        public IEnumerable<IPipelineGraph> AllProfiles()
        {
            return _profiles.Values;
        }

        public IPipelineGraph For(string profileName)
        {
            return _profiles.GetOrAdd(profileName, key =>
            {
                var profileGraph = _pluginGraph.Profile(profileName);

                var instances = new ComplexInstanceGraph(_root, profileGraph, ContainerRole.ProfileOrChild);
                return new PipelineGraph(profileGraph, instances, _root, _root.Singletons, _root.Transients);
            });
        }
    }
}