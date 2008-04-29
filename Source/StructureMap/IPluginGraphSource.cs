using System;
using StructureMap.Graph;

namespace StructureMap
{
    [Obsolete] public interface IPluginGraphSource
    {
        /// <summary>
        /// Reads the configuration information and returns the PluginGraph definition of
        /// plugin families and plugin's
        /// </summary>
        /// <returns></returns>
        PluginGraph Build();
    }
}