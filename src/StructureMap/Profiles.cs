using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

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
            var childGraph = parent.NewChild();

            var instances = new ComplexInstanceGraph(_root, childGraph, ContainerRole.ProfileOrChild);

            var transientTracking = parent.TransientTracking == TransientTracking.DefaultNotTrackedAtRoot ? _root.Transients : new TrackingTransientCache();
            return new PipelineGraph(childGraph, instances, _root, _root.Singletons, transientTracking);
        }

        public IEnumerable<IPipelineGraph> AllProfiles()
        {
            return _profiles.Values;
        }

        public IPipelineGraph For(string profileName, object syncLock)
        {
            if (!_profiles.ContainsKey(profileName))
            {
                lock (syncLock)
                {
                    if (!_profiles.ContainsKey(profileName))
                    {
                        var profileGraph = _pluginGraph.Profile(profileName);

                        var instances = new ComplexInstanceGraph(_root, profileGraph, ContainerRole.ProfileOrChild);
                        var pipeline = new PipelineGraph(profileGraph, instances, _root, _root.Singletons, _root.Transients);

                        Container.CorrectSingletonLifecycleForChild(pipeline);

                        _profiles[profileName] = pipeline;
                    }
                }
            }

            return _profiles[profileName];



        }


    }
}