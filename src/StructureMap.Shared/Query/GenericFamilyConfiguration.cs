using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    public class GenericFamilyConfiguration : IPluginTypeConfiguration, IFamily
    {
        private readonly PluginFamily _family;
        private readonly IPipelineGraph _pipeline;
        private readonly PluginGraph _graph;

        public GenericFamilyConfiguration(PluginFamily family, IPipelineGraph pipeline)
        {
            _family = family;
            _pipeline = pipeline;
            _graph = family.Owner;
        }

        public IPipelineGraph Pipeline
        {
            get { return _pipeline; }
        }

        public string ProfileName
        {
            get { return _graph.ProfileName; }
        }

        void IFamily.Eject(Instance instance)
        {
        }

        object IFamily.Build(Instance instance)
        {
            return null;
        }

        bool IFamily.HasBeenCreated(Instance instance)
        {
            return false;
        }

        public Type PluginType
        {
            get { return _family.PluginType; }
        }

        /// <summary>
        /// The "instance" that will be used when Container.GetInstance(PluginType) is called.
        /// See <see cref="InstanceRef">InstanceRef</see> for more information
        /// </summary>
        public InstanceRef Default
        {
            get
            {
                var defaultInstance = _family.GetDefaultInstance();
                return defaultInstance == null ? null : new InstanceRef(defaultInstance, this);
            }
        }

        /// <summary>
        /// The build "policy" for this PluginType.  Used by the WhatDoIHave() diagnostics methods
        /// </summary>
        public ILifecycle Lifecycle
        {
            get { return _family.Lifecycle ?? Lifecycles.Transient; }
        }

        /// <summary>
        /// All of the <see cref="InstanceRef">InstanceRef</see>'s registered
        /// for this PluginType
        /// </summary>
        public IEnumerable<InstanceRef> Instances
        {
            get { return _family.Instances.Select(x => new InstanceRef(x, this)).ToArray(); }
        }

        /// <summary>
        /// Simply query to see if there are any implementations registered
        /// </summary>
        /// <returns></returns>
        public bool HasImplementations()
        {
            return _family.Instances.Any();
        }

        public void EjectAndRemove(InstanceRef instance)
        {
            if (instance == null) return;
            EjectAndRemove(instance.Instance);
        }

        public void EjectAndRemove(Instance instance)
        {
            if (instance == null) return;
            _family.RemoveInstance(instance);
        }

        public void EjectAndRemoveAll()
        {
            _family.RemoveAll();
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