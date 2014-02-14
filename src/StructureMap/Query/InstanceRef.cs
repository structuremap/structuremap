using System;
using System.IO;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    // TODO -- add Xml comments here
    public class InstanceRef
    {
        private readonly IFamily _family;
        private readonly Instance _instance;

        public InstanceRef(Instance instance, IFamily family)
        {
            _instance = instance;
            _family = family;
        }

        internal Instance Instance
        {
            get { return _instance; }
        }

        public ILifecycle Lifecycle
        {
            get
            {
                return _instance.DetermineLifecycle(_family.Lifecycle);
            }
        }

        public string Name
        {
            get { return _instance.Name; }
        }

        /// <summary>
        ///     The actual concrete type of this Instance.  Not every type of IInstance
        ///     can determine the ConcreteType
        /// </summary>
        public Type ReturnedType
        {
            get { return _instance.ReturnedType; }
        }


        public string Description
        {
            get { return _instance.Description; }
        }

        public Type PluginType
        {
            get { return _family.PluginType; }
        }

        /// <summary>
        /// Only ejects a cached version of the object built by this Instance
        /// from its lifecycle.  
        /// </summary>
        public void EjectObject()
        {
            _family.Eject(_instance);
        }

        /// <summary>
        /// Ejects any cached version of the object built by this Instance
        /// and removes the configured Instance completely from this Container
        /// </summary>
        public void EjectAndRemove()
        {
            _family.EjectAndRemove(_instance);
        }

        public T Get<T>() where T : class
        {
            return _family.Build(_instance) as T;
        }

        public bool ObjectHasBeenCreated()
        {
            return _family.HasBeenCreated(_instance);
        }

        public string DescribeBuildPlan(int maxLevels = 0)
        {
            var visualizer = new BuildPlanVisualizer(_family.Pipeline, levels: maxLevels);
            visualizer.Instance(_family.PluginType, Instance);

            var writer = new StringWriter();
            visualizer.Write(writer);

            return writer.ToString();
        }
    }
}