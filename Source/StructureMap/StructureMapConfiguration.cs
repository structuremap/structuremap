using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Verification;

namespace StructureMap
{
    public static class StructureMapConfiguration
    {
        private const string CONFIG_FILE_NAME = "StructureMap.config";
        private static ConfigurationParserCollection _collection;
        private static Registry _registry;
        private static List<Registry> _registries;
        private static StartUp _startUp;
        private static bool _pullConfigurationFromAppConfig;

        static StructureMapConfiguration()
        {
            ResetAll();
        }


        /// <summary>
        /// Returns the path to the StructureMap.config file
        /// </summary>
        /// <returns></returns>
        public static string GetStructureMapConfigurationPath()
        {
            string basePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string configPath = Path.Combine(basePath, CONFIG_FILE_NAME);

            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(basePath, "bin");
                configPath = Path.Combine(configPath, CONFIG_FILE_NAME);

                if (!File.Exists(configPath))
                {
                    configPath = Path.Combine(basePath, @"..\" + CONFIG_FILE_NAME);
                }
            }

            return configPath;
        }

        /// <summary>
        /// Clears StructureMapConfiguration of all configuration options.  Returns StructureMap
        /// to only using the default StructureMap.config file for configuration.
        /// </summary>
        public static void ResetAll()
        {
            _collection = new ConfigurationParserCollection();
            _registry = new Registry();
            _registries = new List<Registry>();
            _registries.Add(_registry);
            _startUp = null;
            _pullConfigurationFromAppConfig = false;
        }

        /// <summary>
        /// Builds a PluginGraph object for the current configuration.  Used by ObjectFactory.
        /// </summary>
        /// <returns></returns>
        public static PluginGraph GetPluginGraph()
        {
            if (_startUp != null)
            {
                _startUp.RunDiagnostics();
            }

            PluginGraphBuilder builder = createBuilder();
            return builder.Build();
        }

        private static PluginGraphBuilder createBuilder()
        {
            if (_pullConfigurationFromAppConfig)
            {
                IList<XmlNode> appConfigNodes = StructureMapConfigurationSection.GetStructureMapConfiguration();
                foreach (XmlNode appConfigNode in appConfigNodes)
                {
                    _collection.IncludeNode(
                        delegate() { return appConfigNode; });
                }
            }

            ConfigurationParser[] parsers = _collection.GetParsers();
            return new PluginGraphBuilder(parsers, _registries.ToArray());
        }

        /// <summary>
        /// Creates a PluginGraphReport that details the current configuration along with any problems found with the configuration.  
        /// The PluginGraphReport can be used to troubleshoot problems with the StructureMap configuration.
        /// </summary>
        /// <returns></returns>
        public static PluginGraphReport GetDiagnosticReport()
        {
            PluginGraphBuilder builder = createBuilder();
            return builder.Report;
        }

        /// <summary>
        /// Directs StructureMap to include Xml configuration information from a separate file
        /// </summary>
        /// <param name="filename"></param>
        public static void IncludeConfigurationFromFile(string filename)
        {
            _collection.IncludeFile(filename);
        }

        /// <summary>
        /// Register a FetchNodeDelegate delegate to retrieve a &lt;StructureMap&gt;
        /// node to include Xml configuration
        /// </summary>
        /// <param name="fetcher"></param>
        public static void IncludeConfigurationFrom(FetchNodeDelegate fetcher)
        {
            _collection.IncludeNode(fetcher);
        }

        /// <summary>
        /// Programmatically adds a &lt;StructureMap&gt; node containing Xml configuration
        /// </summary>
        /// <param name="node"></param>
        public static void IncludeConfigurationFromNode(XmlNode node)
        {
            _collection.IncludeNode(
                delegate { return node; }
                );
        }

        /// <summary>
        /// Flag to enable or disable the usage of the default StructureMap.config
        /// If set to false, StructureMap will not look for a StructureMap.config file
        /// </summary>
        public static bool UseDefaultStructureMapConfigFile
        {
            get { return _collection.UseDefaultFile; }
            set { _collection.UseDefaultFile = value; }
        }

        public static bool PullConfigurationFromAppConfig
        {
            get { return _pullConfigurationFromAppConfig; }
            set { _pullConfigurationFromAppConfig = value; }
        }

        /// <summary>
        /// Programmatically determine Assembly's to be scanned for attribute configuration
        /// </summary>
        /// <returns></returns>
        public static ScanAssembliesExpression ScanAssemblies()
        {
            ScanAssembliesExpression expression = new ScanAssembliesExpression();
            _registry.addExpression(expression);

            return expression;
        }

        /// <summary>
        /// Direct StructureMap to create instances of Type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE">The Type to build</typeparam>
        /// <returns></returns>
        public static CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>()
        {
            return _registry.BuildInstancesOf<PLUGINTYPE>();
        }

        /// <summary>
        /// Adds a new configured instance of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InstanceExpression.InstanceTypeExpression AddInstanceOf<T>()
        {
            return _registry.AddInstanceOf<T>();
        }


        /// <summary>
        /// Adds a preconfigured instance of Type T to StructureMap.  When this instance is requested,
        /// StructureMap will always return the original object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static LiteralExpression<T> AddInstanceOf<T>(T target)
        {
            return _registry.AddInstanceOf(target);
        }

        /// <summary>
        /// Adds a Prototype (GoF) instance of Type T.  The actual prototype object must implement the
        /// ICloneable interface.  When this instance of T is requested, StructureMap will
        /// return a cloned copy of the originally registered prototype object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prototype"></param>
        /// <returns></returns>
        public static PrototypeExpression<T> AddPrototypeInstanceOf<T>(T prototype)
        {
            return _registry.AddPrototypeInstanceOf(prototype);
        }

        /// <summary>
        /// Starts the definition of a configuration Profile. 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static ProfileExpression CreateProfile(string profileName)
        {
            return _registry.CreateProfile(profileName);
        }

        /// <summary>
        /// Directs StructureMap to use a Registry class to construct the
        /// PluginGraph
        /// </summary>
        /// <param name="registry"></param>
        public static void AddRegistry(Registry registry)
        {
            _registries.Add(registry);
        }

        /// <summary>
        /// Controls the reporting and diagnostics of StructureMap on 
        /// startup
        /// </summary>
        /// <returns></returns>
        public static IStartUp OnStartUp()
        {
            if (_startUp == null)
            {
                _startUp = new StartUp();
            }

            return _startUp;
        }

        public static void TheDefaultProfileIs(string profileName)
        {
            DefaultProfileExpression expression = new DefaultProfileExpression(profileName);
            _registry.addExpression(expression);
        }

        internal class DefaultProfileExpression : IExpression
        {
            private readonly string _profileName;

            public DefaultProfileExpression(string profileName)
            {
                _profileName = profileName;
            }

            public void Configure(PluginGraph graph)
            {
                graph.DefaultManager.DefaultProfileName = _profileName;
            }
        }
    }
}