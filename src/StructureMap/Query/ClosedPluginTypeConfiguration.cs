using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class ClosedPluginTypeConfiguration : IPluginTypeConfiguration, IFamily
    {
        private readonly PluginFamily _family;
        private readonly IContainer _container;
        private readonly IPipelineGraph _pipelineGraph;

        public ClosedPluginTypeConfiguration(PluginFamily family, IContainer container, IPipelineGraph pipelineGraph)
        {
            _family = family;
            _container = container;
            _pipelineGraph = pipelineGraph;
        }

        Type IPluginTypeConfiguration.PluginType
        {
            get { return _family.PluginType; }
        }

        public void Eject(Instance instance)
        {
            throw new NotImplementedException();
        }

        public object Build(Instance instance)
        {
            throw new NotImplementedException();
        }

        public bool HasBeenCreated(Instance instance)
        {
            throw new NotImplementedException();
        }

        public InstanceRef Default { get; private set; }
        public string Lifecycle { get; private set; }
        public IEnumerable<InstanceRef> Instances { get; private set; }
        public bool HasImplementations()
        {
            throw new NotImplementedException();
        }

        public void EjectAndRemove(InstanceRef instance)
        {
            throw new NotImplementedException();
        }

        public void EjectAndRemoveAll()
        {
            throw new NotImplementedException();
        }

        Type IFamily.PluginType { get; private set; }
    }
}