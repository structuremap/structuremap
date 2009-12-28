using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Query
{
    public interface IPluginTypeConfiguration
    {
        Type PluginType { get; }

        /// <summary>
        /// The "instance" that will be used when Container.GetInstance(PluginType) is called.
        /// See <see cref="InstanceRef">InstanceRef</see> for more information
        /// </summary>
        InstanceRef Default { get; }

        /// <summary>
        /// The build "policy" for this PluginType.  Used by the WhatDoIHave() diagnostics methods
        /// </summary>
        string Lifecycle { get; }

        /// <summary>
        /// All of the <see cref="InstanceRef">InstanceRef</see>'s registered
        /// for this PluginType
        /// </summary>
        IEnumerable<InstanceRef> Instances { get; }

        /// <summary>
        /// Simply query to see if there are any implementations registered
        /// </summary>
        /// <returns></returns>
        bool HasImplementations();

        /// <summary>
        /// Ejects any instances of this instance from the current container
        /// and permanently removes the instance from the container configuration
        /// </summary>
        /// <param name="instance"></param>
        void EjectAndRemove(InstanceRef instance);

        /// <summary>
        /// Eject all instances of this PluginType from the current container,
        /// but leaves the lifecycle behavior
        /// </summary>
        void EjectAndRemoveAll();
    }

    public static class PluginTypeConfigurationExtensions
    {
        public static InstanceRef Find(this IPluginTypeConfiguration configuration, string instanceName)
        {
            return configuration.Instances.FirstOrDefault(x => x.Name == instanceName);
        }

        /// <summary>
        /// Ejects and removes all objects and the configuration for the named instance from the 
        /// container
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="instanceName"></param>
        public static void EjectAndRemove(this IPluginTypeConfiguration configuration, string instanceName)
        {
            configuration.EjectAndRemove(configuration.Find(instanceName));
        }

        /// <summary>
        /// Ejects and removes all objects and configuration for the instances that match the filter
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="filter"></param>
        public static void EjectAndRemove(this IPluginTypeConfiguration configuration, Func<InstanceRef, bool> filter)
        {
            configuration.Instances.Where(filter).Each(configuration.EjectAndRemove);
        }
    }
}