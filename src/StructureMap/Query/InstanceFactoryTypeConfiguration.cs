using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class InstanceFactoryTypeConfiguration : IPluginTypeConfiguration, IFamily
    {
        private readonly IContainer _container;
        private readonly PipelineGraph _graph;
        private readonly Type _pluginType;

        public InstanceFactoryTypeConfiguration(Type pluginType, IContainer container, PipelineGraph graph)
        {
            _pluginType = pluginType;
            _container = container;
            _graph = graph;
        }

        private IInstanceFactory factory { get { return _graph.ForType(_pluginType); } }

        void IFamily.Eject(Instance instance)
        {
            cacheForInstance(instance).Eject(_pluginType, instance);
            instance.SafeDispose();
        }

        private IObjectCache cacheForInstance(Instance instance)
        {
            return instance.Lifecycle.FindCache(_graph);
        }

        object IFamily.Build(Instance instance)
        {
            // It's important to use this overload to get it to 
            // respect the lifecycle.  I think.  It works, so leave
            // it alone.
            return _container.GetInstance(_pluginType, instance.Name);
        }

        bool IFamily.HasBeenCreated(Instance instance)
        {
            return cacheForInstance(instance).Has(_pluginType, instance);
        }

        public Type PluginType { get { return _pluginType; } }

        /// <summary>
        /// The "instance" that will be used when Container.GetInstance(PluginType) is called.
        /// See <see cref="InstanceRef">InstanceRef</see> for more information
        /// </summary>
        public InstanceRef Default
        {
            get
            {
                Instance instance = _graph.GetDefault(_pluginType);
                return toRef(instance);
            }
        }

        /// <summary>
        /// The build "policy" for this PluginType.  Used by the WhatDoIHave() diagnostics methods
        /// </summary>
        public string Lifecycle { get { return factory.Lifecycle.ToName(); } }

        /// <summary>
        /// All of the <see cref="InstanceRef">InstanceRef</see>'s registered
        /// for this PluginType
        /// </summary>
        public IEnumerable<InstanceRef> Instances { get { return factory.AllInstances.Select(x => toRef(x)).ToArray(); } }

        /// <summary>
        /// Simply query to see if there are any implementations registered
        /// </summary>
        /// <returns></returns>
        public bool HasImplementations()
        {
            return factory.AllInstances.Any();
        }

        public void EjectAndRemove(InstanceRef instance)
        {
            instance.EjectObject();
            _graph.Remove(_pluginType, instance.Instance);
        }

        public void EjectAndRemoveAll()
        {
            _graph.EjectAllInstancesOf(_pluginType);
        }


        private InstanceRef toRef(Instance instance)
        {
            if (instance == null) return null;

            return new InstanceRef(instance, this);
        }
    }
}