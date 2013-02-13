using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Graph;
using System.Linq;

namespace StructureMap
{
    /// <summary>
    ///   Reads configuration XML documents and builds the structures necessary to initialize
    ///   the Container/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder
    {
        private readonly IList<IPluginGraphConfiguration> _configurations = new List<IPluginGraphConfiguration>();
        private readonly IList<AssemblyScanner> _scanners = new List<AssemblyScanner>();
        private readonly PluginGraph _graph;

        public PluginGraphBuilder()
        {
            _graph = new PluginGraph();
        }

        public PluginGraphBuilder Add(IPluginGraphConfiguration configuration)
        {
            _configurations.Add(configuration);
            return this;
        }


        /// <summary>
        ///   Reads the configuration information and returns the PluginGraph definition of
        ///   plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        public PluginGraph Build()
        {
            _configurations.Each(x =>
            {
                // Change this to using the FubuCore.Description later
                _graph.Log.StartSource(x.ToString());
                x.Register(this);
                x.Configure(_graph);
            });

            var types = new TypePool(_graph);
            _scanners.Each(x => x.ScanForTypes(types, _graph));

            // TODO -- going to kill this later when Profile's are rewritten because they're stupid
            _graph.ProfileManager.Seal(_graph);

            return _graph;
        }

        public void AddScanner(AssemblyScanner scanner)
        {
            _scanners.Add(scanner);
        }


    }
}