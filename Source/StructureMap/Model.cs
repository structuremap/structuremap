using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IModel
    {
        bool HasDefaultImplementationFor(Type pluginType);
        bool HasDefaultImplementationFor<T>();
        IEnumerable<PluginTypeConfiguration> PluginTypes { get; }
        IEnumerable<IInstance> InstancesOf(Type pluginType);
        IEnumerable<IInstance> InstancesOf<T>();
        bool HasImplementationsFor(Type pluginType);
        bool HasImplementationsFor<T>();
    }

    public class Model : TypeRules, IModel
    {
        private readonly PipelineGraph _graph;

        internal Model(PipelineGraph graph)
        {
            _graph = graph;
        }

        public bool HasDefaultImplementationFor(Type pluginType)
        {
            var family = PluginTypes.FirstOrDefault(x => x.PluginType == pluginType);
            return family == null ? false : family.Default != null;
        }

        public bool HasDefaultImplementationFor<T>()
        {
            return HasDefaultImplementationFor(typeof (T));
        }

        public IEnumerable<PluginTypeConfiguration> PluginTypes
        {
            get
            {
                return _graph.PluginTypes;
            }
        }

        public IEnumerable<IInstance> InstancesOf(Type pluginType)
        {
            return _graph.InstancesOf(pluginType);
        }

        public IEnumerable<IInstance> InstancesOf<T>()
        {
            return _graph.InstancesOf(typeof(T));
        }

        public bool HasImplementationsFor(Type pluginType)
        {
            return _graph.InstancesOf(pluginType).Count() > 0;
        }

        public bool HasImplementationsFor<T>()
        {
            return HasImplementationsFor(typeof(T));
        }
    }
}