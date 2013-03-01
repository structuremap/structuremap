using StructureMap.Graph;

namespace StructureMap.Configuration
{
    /// <summary>
    /// Interact with the <see cref="PluginGraph"/>.
    /// <see cref="ConfigurationExpression.ConfigurePluginGraph"/> and <see cref="ConfigurationExpression.RegisterPluginGraphConfiguration"/>
    /// </summary>
    public interface IPluginGraphConfiguration
    {
        /// <summary>
        /// Configure an already built <see cref="PluginGraph"/>
        /// </summary>
        void Configure(PluginGraph graph);

        /// <summary>
        /// Interact with a <see cref="PluginGraphBuilder"/>.
        /// </summary>
        void Register(PluginGraphBuilder builder);
    }

}