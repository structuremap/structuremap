using System;
using System.Collections.Generic;

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
    }
}