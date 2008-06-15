using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Graph
{
    /// <summary>
    /// Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in 
    /// the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
    /// Plugin’s implementing the CLR Type.
    /// </summary>
    public class PluginFamily : TypeRules, IPluginFamily
    {
        private readonly Predicate<Type> _explicitlyMarkedPluginFilter;
        private readonly Predicate<Type> _implicitPluginFilter;
        private readonly Cache<string, Instance> _instances = new Cache<string, Instance>(delegate { return null; });
        private readonly List<InstanceMemento> _mementoList = new List<InstanceMemento>();
        private readonly PluginCollection _plugins;
        private readonly Type _pluginType;
        private IBuildPolicy _buildPolicy = new BuildPolicy();
        private string _defaultKey = string.Empty;
        private PluginGraph _parent;
        private Predicate<Type> _pluginFilter;
        private IBuildPolicy _policy;

        public PluginFamily(Type pluginType)
            : this(pluginType, new PluginGraph())
        {
        }

        public PluginFamily(Type pluginType, PluginGraph parent)
        {
            _parent = parent;
            _pluginType = pluginType;
            _plugins = new PluginCollection(this);

            PluginFamilyAttribute.ConfigureFamily(this);

            _explicitlyMarkedPluginFilter = (type => IsExplicitlyMarkedAsPlugin(PluginType, type));
            _implicitPluginFilter = (type => CanBeCast(PluginType, type));
            _pluginFilter = _explicitlyMarkedPluginFilter;

            if (IsConcrete(pluginType))
            {
                Plugin plugin = new Plugin(pluginType, Plugin.DEFAULT);
                Plugins.Add(plugin);
            }
        }


        public PluginGraph Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        #region IPluginFamily Members

        public void AddMementoSource(MementoSource source)
        {
            _mementoList.AddRange(source.GetAllMementos());
        }

        public void SetScopeTo(InstanceScope scope)
        {
            switch (scope)
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

        public void AddInterceptor(IBuildInterceptor interceptor)
        {
            interceptor.InnerPolicy = _buildPolicy;
            _buildPolicy = interceptor;
        }

        #endregion

        public void AddInstance(InstanceMemento memento)
        {
            _mementoList.Add(memento);
        }

        public void AddInstance(Instance instance)
        {
            _instances.Store(instance.Name, instance);
        }


        // For testing
        public InstanceMemento GetMemento(string instanceKey)
        {
            return _mementoList.Find(m => m.InstanceKey == instanceKey);
        }


        public void Seal()
        {
            _mementoList.ForEach(memento => _parent.Log.Try(() =>
            {
                Instance instance = memento.ReadInstance(Parent, _pluginType);
                AddInstance(instance);
            }).AndLogAnyErrors());

            discoverImplicitInstances();

            validatePluggabilityOfInstances();

            if (_instances.Count == 1)
            {
                _defaultKey = _instances.First.Name;
            }
        }

        private void validatePluggabilityOfInstances()
        {
            _instances.Each(instance =>
            {
                IDiagnosticInstance diagnosticInstance = instance;

                _parent.Log.Try(() => diagnosticInstance.Preprocess(this))
                    .AndReportErrorAs(104, diagnosticInstance.CreateToken(), _pluginType);


                if (!diagnosticInstance.CanBePartOfPluginFamily(this))
                {
                    _parent.Log.RegisterError(104, diagnosticInstance.CreateToken(), _pluginType);
                }
            });
        }

        private void discoverImplicitInstances()
        {
            // TODO:  Apply some 3.5 lambda magic.  Maybe move to PluginCollection
            List<Plugin> list = _plugins.FindAutoFillablePlugins();
            list.RemoveAll(
                plugin => _instances.Exists(instance => instance.Matches(plugin)));

            foreach (Plugin plugin in list)
            {
                AddInstance(plugin.CreateImplicitInstance());
            }
        }

        public void EachInstance(Action<Instance> action)
        {
            _instances.Each(action);
        }

        public Instance GetInstance(string name)
        {
            return _instances.Retrieve(name);
        }


        public void AnalyzeTypeForPlugin(Type pluggedType)
        {
            if (_pluginFilter(pluggedType))
            {
                if (!HasPlugin(pluggedType))
                {
                    Plugin plugin = new Plugin(pluggedType);
                    _plugins.Add(plugin);
                }
            }
        }

        public bool HasPlugin(Type pluggedType)
        {
            return _plugins.HasPlugin(pluggedType);
        }

        public Plugin AddPlugin(Type pluggedType)
        {
            if (!HasPlugin(pluggedType))
            {
                Plugin plugin = new Plugin(pluggedType);
                AddPlugin(plugin);

                return plugin;
            }

            return Plugins[pluggedType];
        }

        public Plugin AddPlugin(Type pluggedType, string key)
        {
            Plugin plugin = new Plugin(pluggedType, key);
            AddPlugin(plugin);

            return plugin;
        }

        public void AddPlugin(Plugin plugin)
        {
            if (_plugins.HasPlugin(plugin.ConcreteKey))
            {
                _parent.Log.RegisterError(113, plugin.ConcreteKey, _pluginType);
            }


            _plugins.Add(plugin);
        }

        public Instance GetDefaultInstance()
        {
            return string.IsNullOrEmpty(_defaultKey) ? null : GetInstance(_defaultKey);
        }

        #region properties

        public PluginCollection Plugins
        {
            get { return _plugins; }
        }

        public bool IsGenericTemplate
        {
            get { return _pluginType.IsGenericTypeDefinition; }
        }

        public bool SearchForImplicitPlugins
        {
            get { return ReferenceEquals(_pluginFilter, _implicitPluginFilter); }
            set { _pluginFilter = value ? _implicitPluginFilter : _explicitlyMarkedPluginFilter; }
        }

        public IBuildPolicy Policy
        {
            get { return _buildPolicy; }
            set { _policy = value; }
        }

        public int PluginCount
        {
            get { return _plugins.Count; }
        }

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

        public int InstanceCount
        {
            get { return _instances.Count; }
        }

        #endregion

        public Plugin FindPlugin(Type pluggedType)
        {
            if (HasPlugin(pluggedType))
            {
                return Plugins[pluggedType];
            }
            else
            {
                Plugin plugin = new Plugin(pluggedType);
                Plugins.Add(plugin);

                return plugin;
            }
        }

        public void AddDefaultMemento(InstanceMemento memento)
        {
            if (string.IsNullOrEmpty(memento.InstanceKey))
            {
                memento.InstanceKey = "DefaultInstanceOf" + TypePath.GetAssemblyQualifiedName(PluginType);
            }

            AddInstance(memento);
            DefaultInstanceKey = memento.InstanceKey;
        }

        public void FillDefault(Profile profile)
        {
            if (string.IsNullOrEmpty(DefaultInstanceKey))
            {
                return;
            }

            Instance defaultInstance = GetInstance(DefaultInstanceKey);
            if (defaultInstance == null)
            {
                Parent.Log.RegisterError(210, DefaultInstanceKey, PluginType);
            }

            profile.FillTypeInto(PluginType, defaultInstance);
        }

        public void ImportFrom(PluginFamily source)
        {
            source.EachInstance(instance => _instances.Fill(instance.Name, instance));

            foreach (Plugin plugin in source.Plugins)
            {
                Plugins.Fill(plugin);
            }
        }

        public Instance FirstInstance()
        {
            return _instances.First;
        }
    }
}