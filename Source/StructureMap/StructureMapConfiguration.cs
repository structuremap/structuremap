using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap
{
    public static class StructureMapConfiguration
    {
        private const string CONFIG_FILE_NAME = "StructureMap.config";
        private static GraphLog _log;
        private static List<Registry> _registries;
        private static Registry _registry;
        private static ConfigurationParserBuilder _parserBuilder;
        private static bool _sealed = false;
       

        static StructureMapConfiguration()
        {
            ResetAll();
        }

        private static IConfigurationParserBuilder parserBuilder
        {
            get
            {
                return _parserBuilder;
            }
        }

        private static Registry registry
        {
            get
            {
                return _registry;
            }
        }

        /// <summary>
        /// Flag to enable or disable the usage of the default StructureMap.config
        /// If set to false, StructureMap will not look for a StructureMap.config file
        /// </summary>
        public static bool UseDefaultStructureMapConfigFile
        {
            get { return parserBuilder.UseAndEnforceExistenceOfDefaultFile; }
            set { parserBuilder.UseAndEnforceExistenceOfDefaultFile = value; }
        }


        public static bool IgnoreStructureMapConfig
        {
            get { return parserBuilder.IgnoreDefaultFile; }
            set { parserBuilder.IgnoreDefaultFile = value; }
        }

        public static bool PullConfigurationFromAppConfig
        {
            get { return parserBuilder.PullConfigurationFromAppConfig; }
            set { parserBuilder.PullConfigurationFromAppConfig = value; }
        }

        /// <summary>
        /// Programmatically adds a &lt;StructureMap&gt; node containing Xml configuration
        /// </summary>
        /// <param name="node"></param>
        /// <param name="description">A description of this node source for troubleshooting purposes</param>
        public static void IncludeConfigurationFromNode(XmlNode node, string description)
        {
            parserBuilder.IncludeNode(node, string.Empty);
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
            PluginCache.ResetAll();

            _sealed = false;

            _log = new GraphLog();
            _parserBuilder = new ConfigurationParserBuilder(_log);
            _registry = new Registry();
            _registries = new List<Registry>();
            _registries.Add(_registry);
            UseDefaultStructureMapConfigFile = false;
            IgnoreStructureMapConfig = false;

            PluginCache.ResetAll();
            ObjectFactory.Reset();
        }

        public static void RegisterInterceptor(TypeInterceptor interceptor)
        {
            registry.RegisterInterceptor(interceptor);
        }

        /// <summary>
        /// Builds a PluginGraph object for the current configuration.  Used by ObjectFactory.
        /// </summary>
        /// <returns></returns>
        internal static PluginGraph GetPluginGraph()
        {
            ConfigurationParser[] parsers = _parserBuilder.GetParsers();
            
            PluginGraphBuilder pluginGraphBuilder = new PluginGraphBuilder(parsers, _registries.ToArray(), _log);
            return pluginGraphBuilder.Build();
        }

        /// <summary>
        /// Directs StructureMap to include Xml configuration information from a separate file
        /// </summary>
        /// <param name="filename"></param>
        public static void IncludeConfigurationFromFile(string filename)
        {
            parserBuilder.IncludeFile(filename);
        }



        /// <summary>
        /// Programmatically determine Assembly's to be scanned for attribute configuration
        /// </summary>
        /// <returns></returns>
        public static ScanAssembliesExpression ScanAssemblies()
        {
            return registry.ScanAssemblies();
        }

        /// <summary>
        /// Direct StructureMap to create instances of Type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE">The Type to build</typeparam>
        /// <returns></returns>
        public static CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>()
        {
            return registry.BuildInstancesOf<PLUGINTYPE>();
        }

        /// <summary>
        /// Direct StructureMap to create instances of Type T
        /// </summary>
        /// <typeparam name="PLUGINTYPE">The Type to build</typeparam>
        /// <returns></returns>
        public static CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>()
        {
            return registry.BuildInstancesOf<PLUGINTYPE>();
        }

        public static GenericFamilyExpression ForRequestedType(Type pluginType)
        {
            return registry.ForRequestedType(pluginType);
        }

        /// <summary>
        /// Adds a new configured instance of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete]
        public static Registry.ConfiguredInstanceExpression<T> AddInstanceOf<T>()
        {
            return registry.AddInstanceOf<T>();
        }

        [Obsolete]
        public static void AddInstanceOf<T>(Func<T> func)
        {
            registry.AddInstanceOf<T>(new ConstructorInstance<T>(func));
        }

        [Obsolete]
        public static void AddInstanceOf<T>(Instance instance)
        {
            registry.ForRequestedType<T>().AddInstance(instance);
        }


        /// <summary>
        /// Adds a preconfigured instance of Type T to StructureMap.  When this instance is requested,
        /// StructureMap will always return the original object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        [Obsolete]
        public static LiteralInstance AddInstanceOf<T>(T target)
        {
            return registry.AddInstanceOf(target);
        }

        /// <summary>
        /// Starts the definition of a configuration Profile. 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static ProfileExpression CreateProfile(string profileName)
        {
            return registry.CreateProfile(profileName);
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

        public static void TheDefaultProfileIs(string profileName)
        {
            registry.addExpression(graph => graph.ProfileManager.DefaultProfileName = profileName);
        }


        internal static void Seal()
        {
            _sealed = true;
        }

        internal static void Unseal()
        {
            _sealed = false;
        }
    }
}