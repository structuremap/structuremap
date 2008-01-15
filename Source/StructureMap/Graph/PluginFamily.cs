using System;
using StructureMap.Interceptors;
using StructureMap.Source;

namespace StructureMap.Graph
{
    /// <summary>
    /// Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in 
    /// the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
    /// Plugin’s implementing the CLR Type.
    /// </summary>
    public class PluginFamily : Deployable
    {
        #region statics

        public static PluginFamily CreateAutoFilledPluginFamily(Type pluginType)
        {
            Plugin plugin = Plugin.CreateAutofilledPlugin(pluginType);

            PluginFamily family = new PluginFamily(pluginType);
            family.DefinitionSource = DefinitionSource.Implicit;

            family.Plugins.Add(plugin);
            family.DefaultInstanceKey = plugin.ConcreteKey;

            return family;
        }

        #endregion

        public const string CONCRETE_KEY = "CONCRETE";
        private readonly PluginCollection _plugins;
        private bool _canUseUnMarkedPlugins = false;
        private string _defaultKey = string.Empty;
        private DefinitionSource _definitionSource = DefinitionSource.Implicit;
        private InstanceInterceptor _instanceInterceptor = new NulloInterceptor();
        private InterceptionChain _interceptionChain;
        private Type _pluginType;
        private string _pluginTypeName;
        private MementoSource _source;

        #region constructors

        public PluginFamily(Type pluginType, string defaultInstanceKey)
        {
            _pluginType = pluginType;
            _pluginTypeName = TypePath.GetAssemblyQualifiedName(_pluginType);
            _defaultKey = defaultInstanceKey;
            _plugins = new PluginCollection(this);

            _interceptionChain = new InterceptionChain();

            Source = new MemoryMementoSource();
        }

        public PluginFamily(Type pluginType, string defaultInstanceKey, MementoSource source)
            : this(pluginType, defaultInstanceKey)
        {
            Source = source;
            _definitionSource = DefinitionSource.Explicit;
            _pluginTypeName = TypePath.GetAssemblyQualifiedName(_pluginType);
        }

        /// <summary>
        /// Testing constructor
        /// </summary>
        /// <param name="pluginType"></param>
        public PluginFamily(Type pluginType) :
            this(pluginType, PluginFamilyAttribute.GetDefaultKey(pluginType))
        {
            if (PluginFamilyAttribute.IsMarkedAsSingleton(pluginType))
            {
                InterceptionChain.AddInterceptor(new SingletonInterceptor());
            }
        }


        /// <summary>
        /// Troubleshooting constructor to find potential problems with a PluginFamily's
        /// configuration
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultKey"></param>
        public PluginFamily(TypePath path, string defaultKey)
        {
            _plugins = new PluginCollection(this);
            _pluginTypeName = path.AssemblyQualifiedName;
            _interceptionChain = new InterceptionChain();
            initializeExplicit(path, defaultKey);

            Source = new MemoryMementoSource();
        }


        private void initializeExplicit(TypePath path, string defaultKey)
        {
            try
            {
                _pluginType = path.FindType();
                _pluginTypeName = TypePath.GetAssemblyQualifiedName(_pluginType);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(103, ex, path.ClassName, path.AssemblyName);
            }

            _defaultKey = defaultKey;
        }

        #endregion

        public InstanceInterceptor InstanceInterceptor
        {
            get { return _instanceInterceptor; }
            set { _instanceInterceptor = value; }
        }

        // This code sucks.  What's going on here?
        public PluginFamily CreateTemplatedClone(params Type[] templateTypes)
        {
            Type templatedType = _pluginType.MakeGenericType(templateTypes);
            PluginFamily templatedFamily = new PluginFamily(templatedType);
            templatedFamily._defaultKey = _defaultKey;
            templatedFamily._source = new MemoryMementoSource();
            templatedFamily._definitionSource = _definitionSource;

            foreach (InstanceFactoryInterceptor interceptor in _interceptionChain)
            {
                InstanceFactoryInterceptor clonedInterceptor = (InstanceFactoryInterceptor) interceptor.Clone();
                templatedFamily.InterceptionChain.AddInterceptor(clonedInterceptor);
            }

            foreach (Plugin plugin in _plugins)
            {
                if (isOfCorrectGenericType(plugin, templateTypes))
                {
                    Plugin templatedPlugin = plugin.CreateTemplatedClone(templateTypes);
                    templatedFamily.Plugins.Add(templatedPlugin);
                    foreach (InstanceMemento memento in _source.GetAllMementos())
                    {
                        if (memento.ConcreteKey == plugin.ConcreteKey)
                        {
                            templatedFamily._source.AddExternalMemento(memento);
                        }
                    }
                }
            }

            return templatedFamily;
        }

        // TODO:  Move this around
        private bool isOfCorrectGenericType(Plugin plugin, params Type[] templateTypes)
        {
            bool isValid = true;

            Type interfaceType = plugin.PluggedType.GetInterface(_pluginType.Name);
            if (interfaceType == null)
            {
                interfaceType = plugin.PluggedType.BaseType;
            }
            Type[] pluginArgs = _pluginType.GetGenericArguments();
            Type[] pluggableArgs = interfaceType.GetGenericArguments();

            if (templateTypes.Length != pluginArgs.Length &&
                pluginArgs.Length != pluggableArgs.Length)
            {
                return false;
            }

            for (int i = 0; i < templateTypes.Length; i++)
            {
                isValid &= templateTypes[i] == pluggableArgs[i] ||
                           pluginArgs[i].IsGenericParameter &&
                           pluggableArgs[i].IsGenericParameter;
            }
            return isValid;
        }

        /// <summary>
        /// Finds Plugin's that match the PluginType from the assembly and add to the internal
        /// collection of Plugin's 
        /// </summary>
        /// <param name="assembly"></param>
        public Plugin[] FindPlugins(AssemblyGraph assembly)
        {
            Predicate<Type> pluggedTypeFilter =
                delegate(Type type) { return Plugin.IsAnExplicitPlugin(PluginType, type); };

            if (_canUseUnMarkedPlugins)
            {
                pluggedTypeFilter = delegate(Type type) { return Plugin.CanBeCast(PluginType, type); };
            }

            Plugin[] plugins = assembly.FindPlugins(pluggedTypeFilter);

            foreach (Plugin plugin in plugins)
            {
                _plugins.Add(plugin);
            }

            return plugins;
        }


        /// <summary>
        /// Removes any Implicitly defined Plugin and/or Instance from the PluginFamily
        /// </summary>
        public void RemoveImplicitChildren()
        {
            _plugins.RemoveImplicitChildren();
        }

        public void AddInstance(InstanceMemento memento)
        {
            _source.AddExternalMemento(memento);
        }

        #region properties

        /// <summary>
        /// The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        public Type PluginType
        {
            get { return _pluginType; }
        }

        /// <summary>
        /// The InstanceKey of the default instance of the PluginFamily
        /// </summary>
        public string DefaultInstanceKey
        {
            get { return _defaultKey; }
            set { _defaultKey = value ?? string.Empty; }
        }

        /// <summary>
        /// Denotes the source or the definition for this Plugin.  Implicit means the
        /// Plugin is defined by a [Pluggable] attribute on the PluggedType.  Explicit
        /// means the Plugin was defined in the StructureMap.config file.
        /// </summary>
        public DefinitionSource DefinitionSource
        {
            get { return _definitionSource; }
            set { _definitionSource = value; }
        }

        /// <summary>
        /// The MementoSource that fetches InstanceMemento definitions for the PluginFamily
        /// </summary>
        public MementoSource Source
        {
            get { return _source; }
            set
            {
                _source = value;

                if (_source != null)
                {
                    _source.Family = this;
                }
            }
        }

        public InterceptionChain InterceptionChain
        {
            get { return _interceptionChain; }
            set { _interceptionChain = value; }
        }

        public PluginCollection Plugins
        {
            get { return _plugins; }
        }

        public string PluginTypeName
        {
            get { return _pluginTypeName; }
        }

        public string SourceDescription
        {
            get { return Source.Description; }
        }

        public bool IsGeneric
        {
            get { return _pluginType.IsGenericType; }
        }


        public bool CanUseUnMarkedPlugins
        {
            get { return _canUseUnMarkedPlugins; }
            set { _canUseUnMarkedPlugins = value; }
        }

        #endregion
    }
}