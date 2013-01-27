using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    /// <summary>
    /// Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in 
    /// the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
    /// Plugin’s implementing the CLR Type.
    /// </summary>
    public class PluginFamily : IPluginFamily
    {
        private readonly Cache<string, Instance> _instances = new Cache<string, Instance>(delegate { return null; });
        private readonly List<InstanceMemento> _mementoList = new List<InstanceMemento>();
        private readonly Cache<string, Plugin> _TPluggedTypes = new Cache<string, Plugin>();
        private readonly Type _pluginType;
        private string _defaultKey = string.Empty;
        private ILifecycle _lifecycle;
        private PluginGraph _parent;

    	public PluginFamily(Type pluginType)
            : this(pluginType, new PluginGraph())
        {
        }

        public PluginFamily(Type pluginType, PluginGraph parent)
        {
            _parent = parent;
            _pluginType = pluginType;

            PluginFamilyAttribute.ConfigureFamily(this);

        }

    	public InstanceScope? Scope { get; private set; }

    	public PluginGraph Parent { get { return _parent; } set { _parent = value; } }
        public IEnumerable<Instance> Instances { get { return _instances.GetAll(); } }

        #region IPluginFamily Members

        public void AddMementoSource(MementoSource source)
        {
            _mementoList.AddRange(source.GetAllMementos());
        }

        public void SetScopeTo(InstanceScope scope)
        {
        	Scope = scope;
        	if (scope == InstanceScope.Transient)
            {
                _lifecycle = null;
                return;
            }

            _lifecycle = Lifecycles.GetLifecycle(scope);
        }

        #endregion

        public void SetScopeTo(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public ILifecycle Lifecycle { get { return _lifecycle; } }

        public void AddInstance(InstanceMemento memento)
        {
            _mementoList.Add(memento);
        }

        public void AddInstance(Instance instance)
        {
            _instances[instance.Name] = instance;
        }

        public void SetDefault(Instance instance)
        {
            AddInstance(instance);
            DefaultInstanceKey = instance.Name;
        }


        // For testing
        public InstanceMemento GetMemento(string instanceKey)
        {
            return _mementoList.Find(m => m.InstanceKey == instanceKey);
        }


        public void AddTypes(List<Type> TPluggedTypes)
        {
            TPluggedTypes.ForEach(type => AddType(type));
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

            if (_pluginType.IsConcrete() && PluginCache.GetPlugin(_pluginType).CanBeAutoFilled)
            {
                MissingInstance = new ConfiguredInstance(_pluginType);
            }


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
            _TPluggedTypes.Each((key, plugin) =>
            {
                if (!plugin.CanBeAutoFilled) return;

                if (hasInstanceWithTPluggedType(plugin)) return;

                ConfiguredInstance instance = new ConfiguredInstance(plugin.TPluggedType).Named(key);
                FillInstance(instance);
            });
        }

        private void FillInstance(Instance instance)
        {
            _instances.Fill(instance.Name, instance);
        }

        private bool hasInstanceWithTPluggedType(Plugin plugin)
        {
            return _instances.Exists(instance => instance.Matches(plugin));
        }


        public Instance GetInstance(string name)
        {
            return _instances[name];
        }

        public bool HasPlugin(Type TPluggedType)
        {
            return _TPluggedTypes.Exists(plugin => plugin.TPluggedType == TPluggedType);
        }

        private void assertPluggability(Type TPluggedType)
        {
            if (!TPluggedType.CanBeCastTo(_pluginType))
            {
                throw new StructureMapException(104, TPluggedType, _pluginType);
            }

            if (!Constructor.HasConstructors(TPluggedType))
            {
                throw new StructureMapException(180, TPluggedType.AssemblyQualifiedName);
            }
        }

        public Plugin AddPlugin(Type TPluggedType)
        {
            assertPluggability(TPluggedType);

            Plugin plugin = PluginCache.GetPlugin(TPluggedType);
            _TPluggedTypes[plugin.ConcreteKey] = plugin;

            return plugin;
        }

        public Plugin AddPlugin(Type TPluggedType, string key)
        {
            assertPluggability(TPluggedType);

            Plugin plugin = PluginCache.GetPlugin(TPluggedType);
            _TPluggedTypes[key] = plugin;

            return plugin;
        }

        public Instance GetDefaultInstance()
        {
            return string.IsNullOrEmpty(_defaultKey) ? null : GetInstance(_defaultKey);
        }

        public Plugin FindPlugin(Type TPluggedType)
        {
            return _TPluggedTypes.Find(p => p.TPluggedType == TPluggedType);
        }

        public void AddDefaultMemento(InstanceMemento memento)
        {
            if (string.IsNullOrEmpty(memento.InstanceKey))
            {
                memento.InstanceKey = "DefaultInstanceOf" + PluginType.AssemblyQualifiedName;
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
            if (source.Lifecycle != null)
            {
                SetScopeTo(source.Lifecycle);
            }
            
            source.Instances.Each(instance => _instances.Fill(instance.Name, instance));

            source._TPluggedTypes.Each((key, plugin) => _TPluggedTypes.Fill(key, plugin));

            if (source.MissingInstance != null)
            {
                MissingInstance = source.MissingInstance;
            }
        }

        public Instance FirstInstance()
        {
            return _instances.First;
        }

        public Plugin FindPlugin(string concreteKey)
        {
            if (_TPluggedTypes.Has(concreteKey))
            {
                return _TPluggedTypes[concreteKey];
            }

            return null;
        }

        public bool HasPlugin(string concreteKey)
        {
            return _TPluggedTypes.Has(concreteKey);
        }

        public PluginFamily CreateTemplatedClone(Type[] templateTypes)
        {
            Type templatedType = _pluginType.MakeGenericType(templateTypes);
            var templatedFamily = new PluginFamily(templatedType, Parent);
            templatedFamily.DefaultInstanceKey = DefaultInstanceKey;
            templatedFamily._lifecycle = _lifecycle;

            _instances.GetAll().Select(x =>
            {
                Instance clone = x.CloseType(templateTypes);
                if (clone == null) return null;

                clone.Name = x.Name;
                return clone;
            }).Where(x => x != null).Each(templatedFamily.AddInstance);

            //Are there instances that close the templatedtype straight away?
            _instances.GetAll()
                .Where(x => x.ConcreteType.CanBeCastTo(templatedType))
                .Each(templatedFamily.AddInstance);

            // Need to attach the new PluginFamily to the old PluginGraph
            Parent.PluginFamilies.Add(templatedFamily);


            return templatedFamily;
        }

        private bool hasType(Type concreteType)
        {
            return FindPlugin(concreteType) != null || _instances.Any(x => x.ConcreteType == concreteType);
        }

        public void AddType(Type concreteType)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType))
            {
                AddPlugin(concreteType);
            }
        }

        public void AddType(Type concreteType, string name)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType))
            {
                AddPlugin(concreteType, name);
            }
        }

        public void ForInstance(string name, Action<Instance> action)
        {
            _instances.WithValue(name, action);
        }

        public void RemoveInstance(Instance instance)
        {
            _instances.Remove(instance.Name);
            if (_defaultKey == instance.Name)
            {
                _defaultKey = null;
            }
        }

        public void RemoveAll()
        {
            _instances.Clear();
            _mementoList.Clear();
            _defaultKey = null;
        }

        #region properties

        public bool IsGenericTemplate { get { return _pluginType.IsGenericTypeDefinition || _pluginType.ContainsGenericParameters; } }


        public int PluginCount { get { return _TPluggedTypes.Count; } }

        public int InstanceCount { get { return _instances.Count; } }

        public Instance MissingInstance { get; set; }

        /// <summary>
        /// The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        public Type PluginType { get { return _pluginType; } }

        /// <summary>
        /// The InstanceKey of the default instance of the PluginFamily
        /// </summary>
        public string DefaultInstanceKey { get { return _defaultKey; } set { _defaultKey = value ?? string.Empty; } }

        #endregion
    }
}
