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
        private readonly Type _pluginType;
        private ILifecycle _lifecycle;
        private PluginGraph _parent;
        private Instance _default;

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

        public void AddInstance(Instance instance)
        {
            _instances[instance.Name] = instance;
        }

        public void SetDefault(Instance instance)
        {
            AddInstance(instance);
            _default = instance;
        }

        public void AddTypes(List<Type> pluggedTypes)
        {
            pluggedTypes.ForEach(AddType);
        }

        public void Seal()
        {
            validatePluggabilityOfInstances();

            if (_pluginType.IsConcrete() && PluginCache.GetPlugin(_pluginType).CanBeAutoFilled)
            {
                MissingInstance = new ConfiguredInstance(_pluginType);
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

        public Instance GetInstance(string name)
        {
            return _instances[name];
        }

        // TODO -- make this a bit smarter
        public Instance GetDefaultInstance()
        {
            if (_default != null) return _default;

            if (_instances.Count == 1)
            {
                return _instances.Single();
            }

            return null;
        }

        [Obsolete("Gonna make this go away")]
        public void FillDefault(Profile profile)
        {
            var defaultInstance = GetDefaultInstance();
            if (defaultInstance != null)
            {
                profile.FillTypeInto(PluginType, defaultInstance);
            }
        }


        public void ImportFrom(PluginFamily source)
        {
            if (source.Lifecycle != null)
            {
                SetScopeTo(source.Lifecycle);
            }
            
            source.Instances.Each(instance => _instances.Fill(instance.Name, instance));

            if (source.MissingInstance != null)
            {
                MissingInstance = source.MissingInstance;
            }
        }

        public Instance FirstInstance()
        {
            return _instances.First;
        }

        public PluginFamily CreateTemplatedClone(Type[] templateTypes)
        {
            Type templatedType = _pluginType.MakeGenericType(templateTypes);
            var templatedFamily = new PluginFamily(templatedType, Parent);
            templatedFamily._lifecycle = _lifecycle;

            _instances.GetAll().Select(x =>
            {
                Instance clone = x.CloseType(templateTypes);
                if (clone == null) return null;

                clone.Name = x.Name;
                return clone;
            }).Where(x => x != null).Each(templatedFamily.AddInstance);

            if (GetDefaultInstance() != null)
            {
                var defaultKey = GetDefaultInstance().Name;
                var @default = templatedFamily.Instances.FirstOrDefault(x => x.Name == defaultKey);
                if (@default != null)
                {
                    templatedFamily.SetDefault(@default);
                }
            }

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
            return _instances.Any(x => x.ConcreteType == concreteType);
        }

        public void AddType(Type concreteType)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType))
            {
                var plugin = PluginCache.GetPlugin(concreteType);
                AddType(concreteType, plugin.ConcreteKey ?? concreteType.AssemblyQualifiedName);
            }
        }

        public void AddType(Type concreteType, string name)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType) && PluginCache.GetPlugin(concreteType).CanBeAutoFilled)
            {
                AddInstance(new ConstructorInstance(concreteType, name));
            }
        }

        public void ForInstance(string name, Action<Instance> action)
        {
            _instances.WithValue(name, action);
        }

        public void RemoveInstance(Instance instance)
        {
            _instances.Remove(instance.Name);
            if (_default == instance)
            {
                _default = null;
            }
        }

        public void RemoveAll()
        {
            _instances.Clear();
            _default = null;
        }

        public bool IsGenericTemplate { get { return _pluginType.IsGenericTypeDefinition || _pluginType.ContainsGenericParameters; } }

        public int InstanceCount { get { return _instances.Count; } }

        public Instance MissingInstance { get; set; }

        /// <summary>
        /// The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        public Type PluginType { get { return _pluginType; } }

        /// <summary>
        /// Primarily for TESTING
        /// </summary>
        /// <param name="defaultKey"></param>
        public void SetDefaultKey(string defaultKey)
        {
            var instance = _instances.FirstOrDefault(x => x.Name == defaultKey);
            if (instance == null)
            {
                throw new ArgumentOutOfRangeException("Could not find an instance with name " + defaultKey);
            }

            SetDefault(instance);
        }
    }
}
