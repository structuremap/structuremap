using System;
using System.IO;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;

namespace StructureMap.Query
{
    /// <summary>
    /// A diagnostic wrapper around registered Instance's 
    /// </summary>
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

        /// <summary>
        /// The lifecycle of this specific Instance
        /// </summary>
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
        /// *Only* ejects the cached object built by this Instance
        /// from its lifecycle if it already exists.  
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

        /// <summary>
        /// Returns the real object represented by this Instance
        /// resolved by the underlying Container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            return _family.Build(_instance) as T;
        }

        /// <summary>
        /// Has the object already been created and 
        /// cached in its Lifecycle?  Mostly useful
        /// for Singleton's
        /// </summary>
        /// <returns></returns>
        public bool ObjectHasBeenCreated()
        {
            return _family.HasBeenCreated(_instance);
        }

        /// <summary>
        /// Creates the textual representation of the 'BuildPlan'
        /// for this Instance
        /// </summary>
        /// <param name="maxLevels">Limits the number of recursive levels for visualizing dependencies.  The default is 0 for a shallow representation</param>
        /// <returns></returns>
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