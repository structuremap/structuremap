using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline.Lazy;

namespace StructureMap
{
    /// <summary>
    ///     Reads configuration XML documents and builds the structures necessary to initialize
    ///     the Container/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder
    {
        private readonly IList<IPluginGraphConfiguration> _configurations = new List<IPluginGraphConfiguration>();
        private readonly PluginGraph _graph;
        private readonly IList<AssemblyScanner> _scanners = new List<AssemblyScanner>();

        public PluginGraphBuilder()
        {
            _graph = PluginGraph.CreateRoot();
        }

        public PluginGraphBuilder(PluginGraph graph)
        {
            _graph = graph;
        }

        public PluginGraphBuilder Add(IPluginGraphConfiguration configuration)
        {
            _configurations.Add(configuration);
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
            _configurations.Each(x => {
                x.Register(this);
                x.Configure(_graph);
            });

            _scanners.Each(x => x.ScanForTypes(_graph));

            // Recursive scanning
            if (_graph.QueuedRegistries.Any())
            {
                var builder = new PluginGraphBuilder(_graph);
                while (_graph.QueuedRegistries.Any())
                {
                    var registry = _graph.QueuedRegistries.Dequeue();
                    builder.Add(registry);
                }

                builder.Build();
            }
        }

        public void AddScanner(AssemblyScanner scanner)
        {
            _scanners.Fill(scanner);
        }
    }
}