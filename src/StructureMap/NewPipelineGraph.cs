using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Query;

namespace StructureMap
{
    public class NewPipelineGraph : IPipelineGraph
    {
        private readonly PluginGraph _pluginGraph;

        public NewPipelineGraph(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
        }

        public Instance GetDefault(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public bool HasDefaultForPluginType(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public bool HasInstance(Type pluginType, string instanceKey)
        {
            throw new NotImplementedException();
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instance> GetAllInstances()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instance> GetAllInstances(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            throw new NotImplementedException();
        }

        public bool IsUnique(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public IObjectCache FindCache(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public IPipelineGraph ForProfile(string profile)
        {
            throw new NotImplementedException();
        }

        public string CurrentProfile
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public MissingFactoryFunction OnMissingFactory
        {
            set { throw new NotImplementedException(); }
        }

        public void ImportFrom(PluginGraph graph)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPluginTypeConfiguration> GetPluginTypes(IContainer container)
        {
            throw new NotImplementedException();
        }

        public void EjectAllInstancesOf<T>()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(Func<Type, bool> filter)
        {
            throw new NotImplementedException();
        }

        public void Remove(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public IPipelineGraph ToNestedGraph()
        {
            throw new NotImplementedException();
        }
    }
}