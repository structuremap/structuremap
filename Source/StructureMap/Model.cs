using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Models the state of a Container or ObjectFactory.  Can be used to query for the 
    /// existence of types registered with StructureMap
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Access to all the <seealso cref="PluginTypeConfiguration">Plugin Type</seealso> registrations 
        /// </summary>
        IEnumerable<PluginTypeConfiguration> PluginTypes { get; }

        IEnumerable<IInstance> AllInstances { get; }

        /// <summary>
        /// Can StructureMap fulfill a request to ObjectFactory.GetInstance(pluginType) from the 
        /// current configuration.  This does not include concrete classes that could be auto-configured
        /// upon demand
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        bool HasDefaultImplementationFor(Type pluginType);

        /// <summary>
        /// Can StructureMap fulfill a request to ObjectFactory.GetInstance&lt;T&gt;() from the 
        /// current configuration.  This does not include concrete classes that could be auto-configured
        /// upon demand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasDefaultImplementationFor<T>();

        /// <summary>
        /// Queryable access to all of the <see cref="IInstance">IInstance</see> for a given PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        IEnumerable<IInstance> InstancesOf(Type pluginType);

        /// <summary>
        /// Queryable access to all of the <see cref="IInstance">IInstance</see> for a given PluginType
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInstance> InstancesOf<T>();
        
        /// <summary>
        /// Does the current container have existing configuration for the "pluginType"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        bool HasImplementationsFor(Type pluginType);

        /// <summary>
        /// Does the current container have existing configuration for the type T
        /// </summary>
        /// <returns></returns>
        bool HasImplementationsFor<T>();
    }

    public class Model : TypeRules, IModel
    {
        private readonly PipelineGraph _graph;

        internal Model(PipelineGraph graph)
        {
            _graph = graph;
        }

        #region IModel Members

        public bool HasDefaultImplementationFor(Type pluginType)
        {
            PluginTypeConfiguration family = PluginTypes.FirstOrDefault(x => x.PluginType == pluginType);
            return family == null ? false : family.Default != null;
        }

        public bool HasDefaultImplementationFor<T>()
        {
            return HasDefaultImplementationFor(typeof (T));
        }

        public IEnumerable<PluginTypeConfiguration> PluginTypes
        {
            get { return _graph.PluginTypes; }
        }

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
                foreach (var pluginType in PluginTypes)
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