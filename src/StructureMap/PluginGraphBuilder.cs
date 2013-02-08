using System.Collections.Generic;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    ///   Reads configuration XML documents and builds the structures necessary to initialize
    ///   the Container/IInstanceFactory/InstanceBuilder/ObjectInstanceActivator objects
    /// </summary>
    public class PluginGraphBuilder
    {
        private readonly IList<IPluginGraphConfiguration> _configurations = new List<IPluginGraphConfiguration>();
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
                x.Configure(_graph);
            });

            _graph.Seal();

            return _graph;
        }
    }
}