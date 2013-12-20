using System;
using System.Collections.Generic;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Pipeline;

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
            _graph = new PluginGraph();
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

            addCloseGenericPolicyTo(_graph);

            var funcInstance = new FactoryTemplate(typeof(LazyInstance<>));
            _graph.Families[typeof(Func<>)].SetDefault(funcInstance);
 
            _graph.Log.AssertFailures();

            return _graph;
        }

        private void addCloseGenericPolicyTo(PluginGraph graph)
        {
            var policy = new CloseGenericFamilyPolicy(graph);
            graph.AddFamilyPolicy(policy);

            graph.Profiles.Each(addCloseGenericPolicyTo);
        }

        public void RunConfigurations()
        {
            _configurations.Each(x => {
                // Change this to using the FubuCore.Description later
                _graph.Log.StartSource(x.ToString());
                x.Register(this);
                x.Configure(_graph);
            });

            var types = new TypePool(_graph);
            _scanners.Each(x => x.ScanForTypes(types, _graph));
        }

        public void AddScanner(AssemblyScanner scanner)
        {
            _scanners.Add(scanner);
        }
    }
}