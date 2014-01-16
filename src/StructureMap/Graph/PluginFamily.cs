using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    /// <summary>
    ///     Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in
    ///     the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
    ///     Plugin’s implementing the CLR Type.
    /// </summary>
    public class PluginFamily : HasLifecycle, IDisposable
    {
        private readonly Cache<string, Instance> _instances = new Cache<string, Instance>(delegate { return null; });
        private readonly Type _pluginType;
        private Lazy<Instance> _defaultInstance;
        private Lazy<Instance> _fallBack = new Lazy<Instance>(() => null);


        public PluginFamily(Type pluginType)
        {
            _pluginType = pluginType;

            resetDefault();

            Attribute.GetCustomAttributes(_pluginType, typeof (FamilyAttribute), true).OfType<FamilyAttribute>()
                .Each(x => x.Alter(this));
        }

        public PluginGraph Owner { get; internal set; }

        public IEnumerable<Instance> Instances
        {
            get { return _instances.GetAll(); }
        }

        public bool IsGenericTemplate
        {
            get { return _pluginType.IsGenericTypeDefinition || _pluginType.ContainsGenericParameters; }
        }

        // TODO -- defensive check here to verify that the instance can be used against PluginType
        public Instance MissingInstance { get; set; }

        /// <summary>
        ///     The CLR Type that defines the "Plugin" interface for the PluginFamily
        /// </summary>
        public Type PluginType
        {
            get { return _pluginType; }
        }

        void IDisposable.Dispose()
        {
            _instances.Each(x => x.SafeDispose());
        }

        private void resetDefault()
        {
            _defaultInstance = new Lazy<Instance>(determineDefault);
            _fallBack = new Lazy<Instance>(() => null);
        }

        public void AddInstance(Instance instance)
        {
            instance.Parent = this;
            _instances[instance.Name] = instance;
        }

        // TODO -- re-evaluate this
        public void SetDefault(Func<Instance> defaultInstance)
        {
            _defaultInstance = new Lazy<Instance>(defaultInstance);
        }

        public void SetDefault(Instance instance)
        {
            AddInstance(instance);
            _defaultInstance = new Lazy<Instance>(() => instance);
        }

        public void SetFallback(Instance instance)
        {
            _fallBack = new Lazy<Instance>(() => instance);
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
            return _defaultInstance.Value ?? _fallBack.Value;
        }

        private Instance determineDefault()
        {
            if (_instances.Count == 1)
            {
                return _instances.Single();
            }

            if (_pluginType.IsConcrete() && Policies.CanBeAutoFilled(_pluginType))
            {
                var instance = new ConfiguredInstance(_pluginType);
                AddInstance(instance);

                return instance;
            }

            return null;
        }

        public PluginFamily CreateTemplatedClone(Type[] templateTypes)
        {
            var templatedType = _pluginType.MakeGenericType(templateTypes);
            var templatedFamily = new PluginFamily(templatedType);
            templatedFamily.copyLifecycle(this);

            _instances.GetAll().Select(x => {
                var clone = x.CloseType(templateTypes);
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
                .Where(x => x.ReturnedType.CanBeCastTo(templatedType))
                .Each(templatedFamily.AddInstance);

            return templatedFamily;
        }

        private bool hasType(Type concreteType)
        {
            return _instances.Any(x => x.ReturnedType == concreteType);
        }

        public void AddType(Type concreteType)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType))
            {
                AddType(concreteType, concreteType.AssemblyQualifiedName);
            }
        }

        public Policies Policies
        {
            get
            {
                if (Owner == null || Owner.Root == null) return new Policies();

                return Owner.Root.Policies;
            }
        }

        public void AddType(Type concreteType, string name)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType) && Policies.CanBeAutoFilled(concreteType))
            {
                AddInstance(new ConstructorInstance(concreteType, name));
            }
        }

        public void RemoveInstance(Instance instance)
        {
            _instances.Remove(instance.Name);
            instance.Parent = null;
            if (instance == GetDefaultInstance())
            {
                resetDefault();
            }
        }

        public void RemoveAll()
        {
            _instances.ClearAll();
            resetDefault();
        }
    }
}