using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class Model : IModel
    {
        private readonly PipelineGraph _graph;
        private readonly IContainer _container;

        internal Model(PipelineGraph graph, IContainer container)
        {
            _graph = graph;
            _container = container;
        }

        #region IModel Members

        public bool HasDefaultImplementationFor(Type pluginType)
        {
            return findForFamily(pluginType, f => f.Default != null);
        }

        private T findForFamily<T>(Type pluginType, Func<PluginTypeConfiguration, T> func)
        {
            PluginTypeConfiguration family = PluginTypes.FirstOrDefault(x => x.PluginType == pluginType);
            return family == null ? default(T) : func(family);
        }

        public bool HasDefaultImplementationFor<T>()
        {
            return HasDefaultImplementationFor(typeof (T));
        }

        public Type DefaultTypeFor<T>()
        {
            return DefaultTypeFor(typeof (T));
        }

        public Type DefaultTypeFor(Type pluginType)
        {
            return findForFamily(pluginType, f => f.Default == null ? null : f.Default.ConcreteType);
        }

        public IEnumerable<PluginTypeConfiguration> PluginTypes { get { return _graph.PluginTypes; } }

        public IEnumerable<IInstance> InstancesOf(Type pluginType)
        {
            return _graph.InstancesOf(pluginType);
        }

        public IEnumerable<IInstance> InstancesOf<T>()
        {
            return _graph.InstancesOf(typeof (T));
        }

        public bool HasImplementationsFor(Type pluginType)
        {
            return _graph.InstancesOf(pluginType).Count() > 0;
        }

        public bool HasImplementationsFor<T>()
        {
            return HasImplementationsFor(typeof (T));
        }

        public IEnumerable<IInstance> AllInstances
        {
            get
            {
                foreach (PluginTypeConfiguration pluginType in PluginTypes)
                {
                    foreach (IInstance instance in pluginType.Instances)
                    {
                        yield return instance;
                    }
                }
            }
        }

        #endregion
    }
}