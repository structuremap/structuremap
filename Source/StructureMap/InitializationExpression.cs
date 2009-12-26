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
        /// <summary>
        /// If true, makes the existence of the StructureMap.config mandatory.
        /// The default is false.
        /// </summary>
        bool UseDefaultStructureMapConfigFile { set; }

        /// <summary>
        /// If true, the StructureMap.config file will be ignored even if it exists.
        /// The default is false.
        /// </summary>
        bool IgnoreStructureMapConfig { set; }

        /// <summary>
        /// If true, directs StructureMap to look for configuration in the App.config.
        /// The default value is false.
        /// </summary>
        bool PullConfigurationFromAppConfig { set; }

        /// <summary>
        /// Designate the Default Profile.  This will be applied as soon as the 
        /// Container is initialized.
        /// </summary>
        string DefaultProfileName { get; set; }

        /// <summary>
        /// Imports configuration from an Xml file.  The fileName
        /// must point to an Xml file with valid StructureMap
        /// configuration
        /// </summary>
        /// <param name="fileName"></param>
        void AddConfigurationFromXmlFile(string fileName);

        /// <summary>
        /// Imports configuration directly from an XmlNode.  This
        /// method was intended for scenarios like Xml being embedded
        /// into an assembly.  The node must be a 'StructureMap' node
        /// </summary>
        /// <param name="node"></param>
        void AddConfigurationFromNode(XmlNode node);

        /// <summary>
        /// Creates and adds a Registry object of type T.  
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        void AddRegistry<T>() where T : Registry, new();

        /// <summary>
        /// Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        void AddRegistry(Registry registry);


        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> BuildInstancesOf<PLUGINTYPE>();

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  BuildInstancesOf()
        /// and ForRequestedType() are synonyms
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> ForRequestedType<PLUGINTYPE>();

        /// <summary>
        /// Expression Builder used to define policies for a PluginType including
        /// Scoping, the Default Instance, and interception.  This method is specifically
        /// meant for registering open generic types
        /// </summary>
        /// <returns></returns>
        GenericFamilyExpression ForRequestedType(Type pluginType);

        /// <summary>
        /// This method is a shortcut for specifying the default constructor and 
        /// setter arguments for a ConcreteType.  ForConcreteType is shorthand for:
        /// ForRequestedType[T]().TheDefault.Is.OfConcreteType[T].**************
        /// when the PluginType and ConcreteType are the same Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Registry.BuildWithExpression<T> ForConcreteType<T>();

        /// <summary>
        /// Adds an additional, non-Default Instance to the PluginType T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IsExpression<T> InstanceOf<T>();

        /// <summary>
        /// Adds an additional, non-Default Instance to the designated pluginType
        /// This method is mostly meant for open generic types
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        GenericIsExpression InstanceOf(Type pluginType);

        /// <summary>
        /// Expression Builder to define the defaults for a named Profile.  Each call
        /// to CreateProfile is additive.
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        ProfileExpression CreateProfile(string profileName);

        /// <summary>
        /// An alternative way to use CreateProfile that uses ProfileExpression
        /// as a Nested Closure.  This usage will result in cleaner code for 
        /// multiple declarations
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        void CreateProfile(string profileName, Action<ProfileExpression> action);

        /// <summary>
        /// Registers a new TypeInterceptor object with the Container
        /// </summary>
        /// <param name="interceptor"></param>
        void RegisterInterceptor(TypeInterceptor interceptor);

        /// <summary>
        /// Allows you to define a TypeInterceptor inline with Lambdas or anonymous delegates
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <example>
        /// IfTypeMatches( ... ).InterceptWith( o => new ObjectWrapper(o) );
        /// </example>
        MatchedTypeInterceptor IfTypeMatches(Predicate<Type> match);

        /// <summary>
        /// Expresses a single "Scanning" action
        /// </summary>
        /// <param name="action"></param>
        /// <example>
        /// Scan(x => {
        ///     x.Assembly("Foo.Services");
        ///     x.WithDefaultConventions();
        /// });
        /// </example>
        void Scan(Action<IAssemblyScanner> action);

        /// <summary>
        /// Directs StructureMap to always inject dependencies into any and all public Setter properties
        /// of the type PLUGINTYPE.
        /// </summary>
        /// <typeparam name="PLUGINTYPE"></typeparam>
        /// <returns></returns>
        CreatePluginFamilyExpression<PLUGINTYPE> FillAllPropertiesOfType<PLUGINTYPE>();

        /// <summary>
        /// Creates automatic "policies" for which public setters are considered mandatory
        /// properties by StructureMap that will be "setter injected" as part of the 
        /// construction process.
        /// </summary>
        /// <param name="action"></param>
        void SetAllProperties(Action<SetterConvention> action);
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
            set
            {
                _parserBuilder.UseAndEnforceExistenceOfDefaultFile = value;
                if (!value)
                {
                    _parserBuilder.IgnoreDefaultFile = true;
                }
            }
        }

        public bool IgnoreStructureMapConfig { set { _parserBuilder.IgnoreDefaultFile = value; } }

        public bool PullConfigurationFromAppConfig { set { _parserBuilder.PullConfigurationFromAppConfig = value; } }

        public string DefaultProfileName { get; set; }
    }
}