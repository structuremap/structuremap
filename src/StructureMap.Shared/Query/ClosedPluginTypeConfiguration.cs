using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class ClosedPluginTypeConfiguration : IPluginTypeConfiguration, IFamily
    {
        private readonly PluginFamily _family;
        private readonly IPipelineGraph _pipelineGraph;

        public ClosedPluginTypeConfiguration(PluginFamily family, IPipelineGraph pipelineGraph)
        {
            _family = family;
            _pipelineGraph = pipelineGraph;
        }

        public void Eject(Instance instance)
        {
            _pipelineGraph.Ejector.RemoveFromLifecycle(_family.PluginType, instance);
        }

        public object Build(Instance instance)
        {
            return new Container(_pipelineGraph).GetInstance(_family.PluginType, instance);
        }

        public bool HasBeenCreated(Instance instance)
        {
            return
                _pipelineGraph.DetermineLifecycle(_family.PluginType, instance)
                    .FindCache(_pipelineGraph)
                    .Has(_family.PluginType, instance);
        }

        public string ProfileName
        {
            get { return _pipelineGraph.Profile; }
        }

        public IPipelineGraph Pipeline
        {
            get { return _pipelineGraph; }
        }

        Type IFamily.PluginType
        {
            get { return _family.PluginType; }
        }

        Type IPluginTypeConfiguration.PluginType
        {
            get { return _family.PluginType; }
        }

        public InstanceRef Default
        {
            get
            {
                var instance = _family.GetDefaultInstance();
                return instance == null ? null : new InstanceRef(instance, this);
            }
        }

        public ILifecycle Lifecycle
        {
            get { return _pipelineGraph.Instances.DefaultLifecycleFor(_family.PluginType) ?? Lifecycles.Transient; }
        }

        public IEnumerable<InstanceRef> Instances
        {
            get
            {
                foreach (var instance in _family.Instances)
                {
                    yield return new InstanceRef(instance, this);
                }
            }
        }

        public bool HasImplementations()
        {
            return _family.Instances.Any();
        }

        public void EjectAndRemove(Instance instance)
        {
            _pipelineGraph.Ejector.RemoveCompletely(_family.PluginType, instance);
        }

        public void EjectAndRemove(InstanceRef instance)
        {
            _pipelineGraph.Ejector.RemoveCompletely(_family.PluginType, instance.Instance);
        }

        public void EjectAndRemoveAll()
        {
            Instances.ToArray().Each(EjectAndRemove);
        }

        public InstanceRef Fallback
        {
            get
            {
                return _family.Fallback == null ? null : new InstanceRef(_family.Fallback, this);
            }
        }

        public InstanceRef MissingNamedInstance
        {
            get
            {
                return _family.MissingInstance == null ? null : new InstanceRef(_family.MissingInstance, this);
            }
        }
    }
}