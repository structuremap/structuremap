using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public class RootPipelineGraph : IPipelineGraph
    {
        private readonly PluginGraph _pluginGraph;
        private readonly IObjectCache _transientCache;

        public RootPipelineGraph(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            _transientCache = new NulloTransientCache();
        }

        public IObjectCache Singletons
        {
            get { return _pluginGraph.SingletonCache; }
        }

        public IObjectCache Transients
        {
            get { return _transientCache; }
        }

        public IPipelineGraph Root()
        {
            return this;
        }

        public InstanceInterceptor FindInterceptor(Type concreteType)
        {
            return _pluginGraph.InterceptorLibrary.FindInterceptor(concreteType);
        }

        public Instance GetDefault(Type pluginType)
        {
            return _pluginGraph.Families[pluginType].GetDefaultInstance();
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            return _pluginGraph.HasDefaultForPluginType(pluginType);
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            return _pluginGraph.HasInstance(pluginType, instanceKey);
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _pluginGraph.Families.Each(family => {
                family.Instances.Each(i => action(family.PluginType, i));
            });
        }

        public IEnumerable<Instance> GetAllInstances()
        {
            return _pluginGraph.Families.SelectMany(x => x.Instances);
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            return _pluginGraph.Families[pluginType].Instances;
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            return _pluginGraph.Families[pluginType].GetInstance(name);
        }

        public IPipelineGraph ForProfile(string profile)
        {
            throw new NotImplementedException();
        }

        public void ImportFrom(PluginGraph graph)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPluginTypeConfiguration> GetPluginTypes(IContainer container)
        {
            foreach (var family in _pluginGraph.Families)
            {
                if (family.IsGenericTemplate)
                {
                    yield return new GenericFamilyConfiguration(family);
                }
                else
                {
                    yield return new ClosedPluginTypeConfiguration(family, container, this);
                }
            }
        }

        public void EjectAllInstancesOf<T>()
        {
            _pluginGraph.EjectFamily(typeof(T));
        }

        public void Dispose()
        {
            _pluginGraph.EjectFamily(typeof (IContainer));
            _transientCache.DisposeAndClear();
        }

        public void Remove(Func<Type, bool> filter)
        {
            _pluginGraph.Families.Where(x => filter(x.PluginType)).Select(x => x.PluginType)
                        .ToArray().Each(x => _pluginGraph.EjectFamily(x));
        }

        public void Remove(Type pluginType)
        {
            _pluginGraph.EjectFamily(pluginType);
        }

        public IPipelineGraph ToNestedGraph()
        {
            throw new NotImplementedException();
        }
    }
}