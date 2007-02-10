using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public delegate void ConfigurePluginGraph(PluginGraph graph);

    public class DefaultExpression : IExpression
    {
        private readonly ConfigurePluginGraph _configure;

        public DefaultExpression(ConfigurePluginGraph configure)
        {
            _configure = configure;
        }

        public void Configure(PluginGraph graph)
        {
            _configure(graph);
        }
    }
}
