using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// Metadata describing the registration of a PluginType
    /// </summary>
    public class PluginTypeConfiguration
    {
        public Type PluginType { get; set; }

        /// <summary>
        /// The "instance" that will be used when Container.GetInstance(PluginType) is called.
        /// See <see cref="StructureMap.Pipeline.IInstance">IInstance</see> for more information
        /// </summary>
        public IInstance Default { get; set; }

        /// <summary>
        /// The build "policy" for this PluginType.  Used by the WhatDoIHave() diagnostics methods
        /// </summary>
        public ILifecycle Lifecycle { get; set; }

        /// <summary>
        /// All of the <see cref="StructureMap.Pipeline.IInstance">IInstance</see>'s registered
        /// for this PluginType
        /// </summary>
        public IEnumerable<IInstance> Instances { get; set; }
    }
}