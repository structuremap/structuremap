using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Query
{

    public interface IInstance
    {
        string Name { get; }

        /// <summary>
        /// The actual concrete type of this Instance.  Not every type of IInstance
        /// can determine the ConcreteType
        /// </summary>
        Type ConcreteType { get; }


        string Description { get; }
    }

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

        /// <summary>
        /// Find the concrete type for the default Instance of T.
        /// In other words, when I call Container.GetInstance(Type),
        /// what do I get?  May be indeterminate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Type DefaultTypeFor<T>();

        /// <summary>
        /// Find the concrete type for the default Instance of pluginType.
        /// In other words, when I call Container.GetInstance(Type),
        /// what do I get?  May be indeterminate
        /// </summary>
        /// <returns></returns>
        Type DefaultTypeFor(Type pluginType);
    }
}