using System.Collections.Generic;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;

namespace StructureMap
{
    public class ConfigurationExpression : Registry
    {
        protected readonly GraphLog _log = new GraphLog();
        private readonly List<Registry> _registries = new List<Registry>();
        protected readonly ConfigurationParserBuilder _parserBuilder;

        internal ConfigurationExpression()
        {
            _parserBuilder = new ConfigurationParserBuilder(_log);
            _parserBuilder.IgnoreDefaultFile = true;
            _parserBuilder.PullConfigurationFromAppConfig = false;

            _registries.Add(this);
            
        }

        public void AddRegistry<T>() where T : Registry, new()
        {
            AddRegistry(new T());
        }

        public void AddRegistry(Registry registry)
        {
            _registries.Add(registry);
        }

        public void AddConfigurationFromXmlFile(string fileName)
        {
            _parserBuilder.IncludeFile(fileName);
        }

        public void AddConfigurationFromNode(XmlNode node)
        {
            _parserBuilder.IncludeNode(node, "Xml configuration");
        }

        public bool IncludeConfigurationFromConfigFile
        {
            set
            {
                _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value;
            }
            
        }

        internal PluginGraph BuildGraph()
        {
            var parsers = _parserBuilder.GetParsers();
            PluginGraphBuilder builder = new PluginGraphBuilder(parsers, _registries.ToArray(), _log);

            return builder.Build();
        }
    }
}