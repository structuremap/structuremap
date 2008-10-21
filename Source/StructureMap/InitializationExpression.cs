using System;
using System.Xml;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap
{
    public interface IInitializationExpression
    {
        // Directives on how to treat the StructureMap.config file
        bool UseDefaultStructureMapConfigFile { set; }
        bool IgnoreStructureMapConfig { set; }

        // Xml configuration from the App.Config file
        bool PullConfigurationFromAppConfig { set; }

        // Ancillary sources of Xml configuration
        void AddConfigurationFromXmlFile(string fileName);
        void AddConfigurationFromNode(XmlNode node);

        // Specifying Registry's
        void AddRegistry<T>() where T : Registry, new();
        void AddRegistry(Registry registry);

        
        string DefaultProfileName { get; set; }

        // The Registry DSL
        CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>();
        CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>();
        GenericFamilyExpression ForRequestedType(Type pluginType);
        Registry.BuildWithExpression<T> ForConcreteType<T>();

        IsExpression<T> InstanceOf<T>();
        GenericIsExpression InstanceOf(Type pluginType);

        ProfileExpression CreateProfile(string profileName);
        void CreateProfile(string profileName, Action<ProfileExpression> action);

        void RegisterInterceptor(TypeInterceptor interceptor);
        MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match);

        void Scan(Action<IAssemblyScanner> action);

        CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>();
    }

    public class InitializationExpression : ConfigurationExpression, IInitializationExpression
    {
        internal InitializationExpression()
        {
            _parserBuilder.IgnoreDefaultFile = false;
            DefaultProfileName = string.Empty;
        }

        public bool UseDefaultStructureMapConfigFile
        {
            set { _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value; }
        }

        public bool IgnoreStructureMapConfig
        {
            set { _parserBuilder.IgnoreDefaultFile = value; }
        }

        public bool PullConfigurationFromAppConfig
        {
            set { _parserBuilder.PullConfigurationFromAppConfig = value; }
        }

        public string DefaultProfileName { get; set; }
    }
}