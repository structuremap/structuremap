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
        private readonly Cache<string, Instance> _instances = new Cache<string, Instance>(delegate { return null; });
        private readonly List<InstanceMemento> _mementoList = new List<InstanceMemento>();
        private readonly Cache<string, Plugin> _pluggedTypes = new Cache<string, Plugin>();
        private readonly Type _pluginType;
        private IBuildPolicy _buildPolicy = new BuildPolicy();
        private string _defaultKey = string.Empty;
        private PluginGraph _parent;
        private IBuildPolicy _policy;

        public PluginFamily(Type pluginType)
            : this(pluginType, new PluginGraph())
        {
        }

        public PluginFamily(Type pluginType, PluginGraph parent)
        {
            _parent = parent;
            _pluginType = pluginType;

            PluginFamilyAttribute.ConfigureFamily(this);

            if (IsConcrete(pluginType))
            {
                Plugin plugin = PluginCache.GetPlugin(pluginType);
                if (plugin.CanBeCreated())
                {
                    AddPlugin(pluginType, Plugin.DEFAULT);
                }
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
            _instances[instance.Name] = instance;
        }


        // For testing
        public InstanceMemento GetMemento(string instanceKey)
        {
            return _mementoList.Find(m => m.InstanceKey == instanceKey);
        }


        public void AddTypes(List<Type> pluggedTypes)
        {
            pluggedTypes.ForEach(type => AddType(type));
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
            _pluggedTypes.Each((key, plugin) =>
            {
                if (plugin.CanBeAutoFilled && !hasInstanceWithPluggedType(plugin))
                {
                    ConfiguredInstance instance = new ConfiguredInstance(plugin.PluggedType).WithName(key);
                    AddInstance(instance);
                }
            });
        }

        private bool hasInstanceWithPluggedType(Plugin plugin)
        {
            return _instances.Exists(instance => instance.Matches(plugin));
        }

        public void EachInstance(Action<Instance> action)
        {
            _instances.Each(action);
        }

        public Instance GetInstance(string name)
        {
            return _instances[name];
        }

        public bool HasPlugin(Type pluggedType)
        {
            return _pluggedTypes.Exists(plugin => plugin.PluggedType == pluggedType);
        }

        private void assertPluggability(Type pluggedType)
        {
            if (!CanBeCast(_pluginType, pluggedType))
            {
                throw new StructureMapException(104, pluggedType, _pluginType);
            }

            if (!Constructor.HasConstructors(pluggedType))
            {
                throw new StructureMapException(180, pluggedType.AssemblyQualifiedName);
            }
        }

        public Plugin AddPlugin(Type pluggedType)
        {
            assertPluggability(pluggedType);

            Plugin plugin = PluginCache.GetPlugin(pluggedType);
            _pluggedTypes[plugin.ConcreteKey] = plugin;

            return plugin;
        }

        public Plugin AddPlugin(Type pluggedType, string key)
        {
            assertPluggability(pluggedType);

            Plugin plugin = PluginCache.GetPlugin(pluggedType);
            _pluggedTypes[key] = plugin;

            return plugin;
        }

        public Instance GetDefaultInstance()
        {
            return string.IsNullOrEmpty(_defaultKey) ? null : GetInstance(_defaultKey);
        }

        public Plugin FindPlugin(Type pluggedType)
        {
            return _pluggedTypes.Find(p => p.PluggedType == pluggedType);
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
            source._pluggedTypes.Each((key, plugin) => _pluggedTypes.Fill(key, plugin));
        }

        public Instance FirstInstance()
        {
            return _instances.First;
        }

        public Plugin FindPlugin(string concreteKey)
        {
            if (_pluggedTypes.Has(concreteKey))
            {
                return _pluggedTypes[concreteKey];
            }

            return null;
        }

        public bool HasPlugin(string concreteKey)
        {
            return _pluggedTypes.Has(concreteKey);
        }

        public PluginFamily CreateTemplatedClone(Type[] templateTypes)
        {
            Type templatedType = _pluginType.MakeGenericType(templateTypes);
            var templatedFamily = new PluginFamily(templatedType, Parent);
            templatedFamily.DefaultInstanceKey = DefaultInstanceKey;
            templatedFamily.Policy = Policy.Clone();


            // TODO -- Got a big problem here.  Intances need to be copied over
            EachInstance(i => { ((IDiagnosticInstance) i).AddTemplatedInstanceTo(templatedFamily, templateTypes); });

            // Need to attach the new PluginFamily to the old PluginGraph
            Parent.PluginFamilies.Add(templatedFamily);

            return templatedFamily;
        }

        public void AddType(Type concreteType)
        {
            if (!CanBeCast(_pluginType, concreteType)) return;

            if (FindPlugin(concreteType) == null)
            {
                AddPlugin(concreteType);
            }
        }

        public void AddType(Type concreteType, string name)
        {
            if (!CanBeCast(_pluginType, concreteType)) return;

            if (FindPlugin(name) == null)
            {
                AddPlugin(concreteType, name);
            }
        }

        public PluginTypeConfiguration GetConfiguration()
        {
            return new PluginTypeConfiguration
                       {
                           Default = GetDefaultInstance(),
                           PluginType = PluginType,
                           Policy = _buildPolicy,
                           Instances = Instances
                       };
        }

        #region properties

        public bool IsGenericTemplate
        {
            get { return _pluginType.IsGenericTypeDefinition || _pluginType.ContainsGenericParameters; }
        }

        public IBuildPolicy Policy
        {
            get { return _buildPolicy; }
            set { _policy = value; }
        }

        public int PluginCount
        {
            get { return _pluggedTypes.Count; }
        }

        public int InstanceCount
        {
            get { return _instances.Count; }
        }

        public IEnumerable<IInstance> Instances
        {
            get { return _instances.GetAll(); }
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

        #endregion
    }
}