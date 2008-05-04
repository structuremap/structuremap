using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using StructureMap.Attributes;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    /// <summary>
    /// Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in 
    /// the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
    /// Plugin’s implementing the CLR Type.
    /// </summary>
    public class PluginFamily
    {
        #region statics

        [Obsolete]
        public static PluginFamily CreateAutoFilledPluginFamily(Type pluginType)
        {
            Plugin plugin = Plugin.CreateAutofilledPlugin(pluginType);

            PluginFamily family = new PluginFamily(pluginType);

            family.Plugins.Add(plugin);
            family.DefaultInstanceKey = plugin.ConcreteKey;

            return family;
        }

        #endregion

        public const string CONCRETE_KEY = "CONCRETE";
        private readonly List<InstanceMemento> _mementoList = new List<InstanceMemento>();
        private readonly PluginCollection _plugins;
        private bool _canUseUnMarkedPlugins = false;
        private string _defaultKey = string.Empty;
        private InstanceInterceptor _instanceInterceptor = new NulloInterceptor();
        private PluginGraph _parent;
        private Type _pluginType;
        private string _pluginTypeName;
        private List<Instance> _instances = new List<Instance>();
        private IBuildPolicy _buildPolicy = new BuildPolicy();

        #region constructors

        public PluginFamily(Type pluginType, string defaultInstanceKey)
        {
            _pluginType = pluginType;
            _pluginTypeName = TypePath.GetAssemblyQualifiedName(_pluginType);
            _defaultKey = defaultInstanceKey;
            _plugins = new PluginCollection(this);
        }

        // TODO:  Need to unit test the scope from the attribute
        /// <summary>
        /// Testing constructor
        /// </summary>
        /// <param name="pluginType"></param>
        public PluginFamily(Type pluginType) :
            this(pluginType, PluginFamilyAttribute.GetDefaultKey(pluginType))
        {
            // TODO -- Merge functionality with PluginFamilyAttribute
            PluginFamilyAttribute attribute = PluginFamilyAttribute.GetAttribute(pluginType);
            if (attribute != null)
            {
                SetScopeTo(attribute.Scope);
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
            initializeExplicit(path, defaultKey);
        }


        public PluginGraph Parent
        {
            get { return _parent; }
            set { _parent = value; }
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

        // TODO:  This code sucks.  What's going on here?
        public PluginFamily CreateTemplatedClone(params Type[] templateTypes)
        {
            Type templatedType = _pluginType.MakeGenericType(templateTypes);
            PluginFamily templatedFamily = new PluginFamily(templatedType);
            templatedFamily._defaultKey = _defaultKey;
            templatedFamily.Parent = Parent;
            templatedFamily._buildPolicy = _buildPolicy.Clone();

            // Add Plugins
            foreach (Plugin plugin in _plugins)
            {
                if (plugin.CanBePluggedIntoGenericType(_pluginType, templateTypes))
                {
                    Plugin templatedPlugin = plugin.CreateTemplatedClone(templateTypes);
                    templatedFamily.Plugins.Add(templatedPlugin);
                }
            }

            // TODO -- Got a big problem here.  Intances need to be copied over
            foreach (IDiagnosticInstance instance in GetAllInstances())
            {
                if (instance.CanBePartOfPluginFamily(templatedFamily))
                {
                    templatedFamily.AddInstance((Instance)instance);
                }
            }

            // Need to attach the new PluginFamily to the old PluginGraph
            Parent.PluginFamilies.Add(templatedFamily);

            return templatedFamily;
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

        public void AddInstance(InstanceMemento memento)
        {
            _mementoList.Add(memento);
        }

        public void AddInstance(Instance instance)
        {
            _instances.Add(instance);
        }

        public void AddMementoSource(MementoSource source)
        {
            _mementoList.AddRange(source.GetAllMementos());
        }

        public InstanceMemento[] GetAllMementos()
        {
            return _mementoList.ToArray();
        }

        // For testing
        public InstanceMemento GetMemento(string instanceKey)
        {
            return _mementoList.Find(delegate(InstanceMemento m) { return m.InstanceKey == instanceKey; });
        }

        public void DiscoverImplicitInstances()
        {
            List<Plugin> list = _plugins.FindAutoFillablePlugins();
            foreach (InstanceMemento memento in _mementoList)
            {
                Plugin plugin = memento.FindPlugin(this);
                list.Remove(plugin);
            }

            foreach (Plugin plugin in list)
            {
                AddInstance(plugin.CreateImplicitMemento());
            }
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

        public PluginCollection Plugins
        {
            get { return _plugins; }
        }

        public string PluginTypeName
        {
            get { return _pluginTypeName; }
        }

        public bool IsGenericTemplate
        {
            get { return _pluginType.IsGenericTypeDefinition; }
        }

        public bool IsGenericType
        {
            get { return _pluginType.IsGenericType; }
        }


        public bool CanUseUnMarkedPlugins
        {
            get { return _canUseUnMarkedPlugins; }
            set { _canUseUnMarkedPlugins = value; }
        }

        public IBuildPolicy Policy
        {
            get { return _buildPolicy; }
        }

        #endregion

        public void Seal()
        {
            foreach (InstanceMemento memento in _mementoList)
            {
                Instance instance = memento.ReadInstance(Parent, _pluginType);
                _instances.Add(instance);
            }
        }

        public Instance[] GetAllInstances()
        {
            return _instances.ToArray();
        }

        public Instance GetInstance(string name)
        {
            return _instances.Find(delegate(Instance i) { return i.Name == name; });
        }

        public void SetScopeTo(InstanceScope scope)
        {
            switch(scope)
            {
                case InstanceScope.Singleton:
                    AddInterceptor(new SingletonPolicy());
                    break;

                case InstanceScope.HttpContext:
                    AddInterceptor(new HttpContextBuildPolicy());
                    break;

                case InstanceScope.ThreadLocal:
                    AddInterceptor(new ThreadLocalStoragePolicy());
                    break;

                case InstanceScope.Hybrid:
                    AddInterceptor(new HybridBuildPolicy());
                    break;
            }
        }

        public void AddInterceptor(IInstanceInterceptor interceptor)
        {
            interceptor.InnerPolicy = _buildPolicy;
            _buildPolicy = interceptor;
        }


    }
}