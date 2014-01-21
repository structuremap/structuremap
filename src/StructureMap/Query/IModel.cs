using System;
using System.Collections.Generic;

namespace StructureMap.Query
{
    /// <summary>
    ///     Models the state of a Container or ObjectFactory.  Can be used to query for the
    ///     existence of types registered with StructureMap
    /// </summary>
    public interface IModel
    {
        /// <summary>
        ///     Access to all the <seealso cref="IPluginTypeConfiguration">Plugin Type</seealso> registrations
        /// </summary>
        IEnumerable<IPluginTypeConfiguration> PluginTypes { get; }

        /// <summary>
        ///     Direct access to the configuration model of this container
        /// </summary>
        IPipelineGraph Pipeline { get; }

        /// <summary>
        ///     All explicitly known Instance's in this container.  Other instances can be created during
        ///     the lifetime of the container
        /// </summary>
        IEnumerable<InstanceRef> AllInstances { get; }

        /// <summary>
        ///     Can StructureMap fulfill a request to ObjectFactory.GetInstance(pluginType) from the
        ///     current configuration.  This does not include concrete classes that could be auto-configured
        ///     upon demand
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        bool HasDefaultImplementationFor(Type pluginType);

        /// <summary>
        ///     Can StructureMap fulfill a request to ObjectFactory.GetInstance&lt;T&gt;() from the
        ///     current configuration.  This does not include concrete classes that could be auto-configured
        ///     upon demand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasDefaultImplementationFor<T>();

        /// <summary>
        ///     Queryable access to all of the <see cref="InstanceRef">InstanceRef</see> for a given PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        IEnumerable<InstanceRef> InstancesOf(Type pluginType);

        /// <summary>
        ///     Queryable access to all of the <see cref="InstanceRef">InstanceRef</see> for a given PluginType
        /// </summary>
        /// <returns></returns>
        IEnumerable<InstanceRef> InstancesOf<T>();

        /// <summary>
        ///     Does the current container have existing configuration for the "pluginType"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        bool HasImplementationsFor(Type pluginType);

        /// <summary>
        ///     Does the current container have existing configuration for the type T
        /// </summary>
        /// <returns></returns>
        bool HasImplementationsFor<T>();

        /// <summary>
        ///     Find the concrete type for the default Instance of T.
        ///     In other words, when I call Container.GetInstance(Type),
        ///     what do I get?  May be indeterminate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Type DefaultTypeFor<T>();

        /// <summary>
        ///     Find the concrete type for the default Instance of pluginType.
        ///     In other words, when I call Container.GetInstance(Type),
        ///     what do I get?  May be indeterminate
        /// </summary>
        /// <returns></returns>
        Type DefaultTypeFor(Type pluginType);

        /// <summary>
        ///     Retrieves the configuration for the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IPluginTypeConfiguration For<T>();

        /// <summary>
        ///     Retrieves the configuration for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IPluginTypeConfiguration For(Type type);

        /// <summary>
        ///     Eject all objects, configuration, and Plugin Types matching this filter
        /// </summary>
        /// <param name="filter"></param>
        void EjectAndRemoveTypes(Func<Type, bool> filter);

        /// <summary>
        ///     Eject all objects and configuration for any Plugin Type that matches this filter
        /// </summary>
        /// <param name="filter"></param>
        void EjectAndRemovePluginTypes(Func<Type, bool> filter);

        /// <summary>
        ///     Eject all objects and Instance configuration for this PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        void EjectAndRemove(Type pluginType);


        /// <summary>
        /// Eject all objects and Instance configuration for this PluginType
        /// </summary>
        void EjectAndRemove<TPluginType>();

        /// <summary>
        ///     Get each and every configured instance that could possibly
        ///     be cast to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAllPossible<T>() where T : class;
    }
}