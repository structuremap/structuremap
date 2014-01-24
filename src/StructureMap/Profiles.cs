using StructureMap.Graph;
using StructureMap.Util;

namespace StructureMap
{
    public class Profiles
    {
        private readonly Cache<string, IPipelineGraph> _profiles;

        public Profiles(PluginGraph pluginGraph, IPipelineGraph root)
        {
            _profiles = new Cache<string, IPipelineGraph>(name => {
                var profileGraph = pluginGraph.Profile(name);

                var instances = new ComplexInstanceGraph(root, profileGraph, ContainerRole.ProfileOrChild);
                return new PipelineGraph(profileGraph, instances, root, root.Singletons, root.Transients);
            });
        }

        public IPipelineGraph For(string profileName)
        {
            return _profiles[profileName];
        }
    }
}