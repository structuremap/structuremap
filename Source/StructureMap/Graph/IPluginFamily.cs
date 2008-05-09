using System;
using StructureMap.Attributes;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public interface IPluginFamily
    {
        void AddMementoSource(MementoSource source);

        /// <summary>
        /// The InstanceKey of the default instance of the PluginFamily
        /// </summary>
        string DefaultInstanceKey { get; set; }

        /// <summary>
        /// The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        Type PluginType
        {
            get;
        }

        void SetScopeTo(InstanceScope scope);
        void AddInterceptor(IBuildInterceptor interceptor);
    }
}