using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Util;

namespace StructureMap
{
    public class Profiles
    {
        private readonly IPipelineGraph _root;
        private readonly Cache<string, IPipelineGraph> _profiles;

        public Profiles(PluginGraph pluginGraph, IPipelineGraph root)
        {
            _root = root;
            _profiles = new Cache<string, IPipelineGraph>(name => {
                var profileGraph = pluginGraph.Profile(name);

                var instances = new ComplexInstanceGraph(root, profileGraph, ContainerRole.ProfileOrChild);
                return new PipelineGraph(profileGraph, instances, root, root.Singletons, root.Transients);
            });
        }

        public IPipelineGraph NewChild()
        {
            var childGraph = new PluginGraph();
            var instances = new ComplexInstanceGraph(_root, childGraph, ContainerRole.ProfileOrChild);
        
            return new PipelineGraph(childGraph, instances, _root, _root.Singletons, _root.Transients);
        }

        public IEnumerable<IPipelineGraph> AllProfiles()
        {
            return _profiles;
        }

        public IPipelineGraph For(string profileName)
        {
            return _profiles[profileName];
        }
    }
}