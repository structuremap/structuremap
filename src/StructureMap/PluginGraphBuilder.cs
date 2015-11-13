using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    ///     Reads configuration XML documents and builds the structures necessary to initialize
    ///     the Container/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder
    {
        private readonly PluginGraph _graph;

        public PluginGraphBuilder()
        {
            _graph = PluginGraph.CreateRoot();
        }

        public PluginGraphBuilder(PluginGraph graph)
        {
            _graph = graph;
        }

        public PluginGraphBuilder Add(Registry registry)
        {
            _graph.ImportRegistry(registry);
            return this;
        }


        /// <summary>
        ///     Reads the configuration information and returns the PluginGraph definition of
        ///     Plugin families and Plugin's
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            RunConfigurations();

            // TEMP!
            _graph.addCloseGenericPolicyTo();
            _graph.Profiles.Each(x => x.addCloseGenericPolicyTo());

            return _graph;
        }

        public void RunConfigurations()
        {
            var scanning = new List<Task<Registry>>();

            // Recursive scanning
            while (_graph.QueuedRegistries.Any())
            {
                var registry = _graph.QueuedRegistries.Dequeue();
                _graph.Registries.Add(registry);
                scanning.AddRange(registry.Scanners.Where(x => x.HasAssemblies()).Select(x => x.ScanForTypes()));

                registry.Configure(_graph);
            }

            if (scanning.Any())
            {
                Task.WaitAll(scanning.ToArray());

                scanning.Each(x => _graph.ImportRegistry(x.Result));
            }


            if (_graph.QueuedRegistries.Any())
            {
                RunConfigurations();
            }
        }
    }
}