using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Diagnostics;
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
    public class PluginFamily : HasScope, IDisposable
    {
        private readonly Cache<string, Instance> _instances = new Cache<string, Instance>(delegate { return null; });
        private readonly Type _pluginType;
        private Lazy<Instance> _defaultInstance; 

        public PluginFamily(Type pluginType)
        {
            _pluginType = pluginType;

            // TODO -- like to clean this up so it happens more at runtime
            if (_pluginType.IsConcrete() && PluginCache.GetPlugin(_pluginType).CanBeAutoFilled)
            {
                MissingInstance = new ConfiguredInstance(_pluginType);
            }

            resetDefault();

            Attribute.GetCustomAttributes(typeof (FamilyAttribute), true).OfType<FamilyAttribute>()
                .Each(x => x.Alter(this));
        }

        private void resetDefault()
        {
            _defaultInstance = new Lazy<Instance>(determineDefault);
        }

        public IEnumerable<Instance> Instances { get { return _instances.GetAll(); } }

        public void AddInstance(Instance instance)
        {
            _instances[instance.Name] = instance;
            instance.Parent = this;
        }

        public void SetDefault(Func<Instance> defaultInstance)
        {
            _defaultInstance = new Lazy<Instance>(defaultInstance);
        }

        public void SetDefault(Instance instance)
        {
            AddInstance(instance);
            _defaultInstance = new Lazy<Instance>(() => instance);
        }

        public void AddTypes(List<Type> pluggedTypes)
        {
            pluggedTypes.ForEach(AddType);
        }

        public Instance GetInstance(string name)
        {
            return _instances[name];
        }

        public Instance GetDefaultInstance()
        {
            return _defaultInstance.Value;
        }

        private Instance determineDefault()
        {
            if (_instances.Count == 1)
            {
                return _instances.Single();
            }

            if (_pluginType.IsConcrete() && PluginCache.GetPlugin(_pluginType).CanBeAutoFilled)
            {
                return new ConfiguredInstance(_pluginType);
            }

            return null;
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
            var templatedFamily = new PluginFamily(templatedType);
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
            resetDefault();
        }

        public void RemoveAll()
        {
            _instances.Clear();
            resetDefault();
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

        void IDisposable.Dispose()
        {
            _instances.Each(x => x.SafeDispose());
        }
    }
}
