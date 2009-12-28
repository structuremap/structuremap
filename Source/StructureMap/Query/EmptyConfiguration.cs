using System;
using System.Collections.Generic;

namespace StructureMap.Query
{
    public class EmptyConfiguration : IPluginTypeConfiguration
    {
        private readonly Type _pluginType;

        public EmptyConfiguration(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public Type PluginType { get { return _pluginType; } }

        /// <summary>
        /// The "instance" that will be used when Container.GetInstance(PluginType) is called.
        /// See <see cref="InstanceRef">InstanceRef</see> for more information
        /// </summary>
        public InstanceRef Default { get { return null; } }

        /// <summary>
        /// The build "policy" for this PluginType.  Used by the WhatDoIHave() diagnostics methods
        /// </summary>
        public string Lifecycle { get { return null; } }

        /// <summary>
        /// All of the <see cref="InstanceRef">InstanceRef</see>'s registered
        /// for this PluginType
        /// </summary>
        public IEnumerable<InstanceRef> Instances { get { return new InstanceRef[0]; } }

        /// <summary>
        /// Simply query to see if there are any implementations registered
        /// </summary>
        /// <returns></returns>
        public bool HasImplementations()
        {
            return false;
        }

        public void EjectAndRemove(InstanceRef instance)
        {
        }

    }
}