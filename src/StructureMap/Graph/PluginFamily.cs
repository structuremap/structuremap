using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly LightweightCache<string, Instance> _instances = new LightweightCache<string, Instance>(delegate { return null; });
        private readonly Type _pluginType;
        private Lazy<Instance> _defaultInstance;
        private Instance _missingInstance;


        public PluginFamily(Type pluginType)
        {
            _pluginType = pluginType;

            resetDefault();
            _pluginType.GetTypeInfo().ForAttribute<FamilyAttribute>(a => a.Alter(this));

        }

        /// <summary>
        /// The PluginGraph that "owns" this PluginFamily
        /// </summary>
        public PluginGraph Owner { get; internal set; }

        /// <summary>
        /// All the Instances held by this family
        /// </summary>
        public IEnumerable<Instance> Instances
        {
            get { return _instances.GetAll(); }
        }

        /// <summary>
        /// Does this PluginFamily represent an open generic type?
        /// </summary>
        public bool IsGenericTemplate
        {
            get { return _pluginType.GetTypeInfo().IsGenericTypeDefinition || _pluginType.GetTypeInfo().ContainsGenericParameters; }
        }

        /// <summary>
        /// Can be used to create an object for a named Instance that does not exist
        /// </summary>
        public Instance MissingInstance
        {
            get { return _missingInstance; }
            set
            {
                if (value != null)
                {
                    assertInstanceIsValidForThisPluginType(value);
                }

                _missingInstance = value;
            }
        }

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
            Fallback = null;
        }

        /// <summary>
        /// Add an additional Instance to this PluginFamily/PluginType
        /// </summary>
        /// <param name="instance"></param>
        public void AddInstance(Instance instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            assertInstanceIsValidForThisPluginType(instance);

            _instances[instance.Name] = instance;
        }

        private void assertInstanceIsValidForThisPluginType(Instance instance)
        {
            if (instance.ReturnedType == typeof (object)) return;

            

            if (instance.ReturnedType != null &&
                !instance.ReturnedType.CanBeCastTo(_pluginType))
            {
                throw new ArgumentOutOfRangeException(
                    "instance '{0}' with ReturnType {1} cannot be cast to {2}".ToFormat(instance.Description,
                        instance.ReturnedType.GetFullName(), _pluginType.GetFullName()));
            }
        }


        /// <summary>
        /// Sets the default Instance. 
        /// </summary>
        /// <param name="instance"></param>
        public void SetDefault(Instance instance)
        {
            AddInstance(instance);
            _defaultInstance = new Lazy<Instance>(() => instance);
        }

        /// <summary>
        /// The 'UseIfNone' instance to use if no default is set
        /// </summary>
        /// <value></value>
        public Instance Fallback
        {
            get; set;
        }

        /// <summary>
        /// Find a named instance for this PluginFamily
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Instance GetInstance(string name)
        {
            return _instances[name];
        }

        /// <summary>
        /// Determine the default instance if it can.  May return null.
        /// </summary>
        /// <returns></returns>
        public Instance GetDefaultInstance()
        {
            return _defaultInstance.Value ?? Fallback;
        }

        private Instance determineDefault()
        {
            if (_instances.Count == 1)
            {
                return _instances.Single();
            }

            // ONLY decide on a default Instance if there is none
            if (_instances.Count == 0 && _pluginType.IsConcrete())
            {
                var instance = new ConstructorInstance(_pluginType);
                AddInstance(instance);

                return instance;
            }

            return null;
        }

        /// <summary>
        /// If the PluginType is an open generic type, this method will create a 
        /// closed type copy of this PluginFamily
        /// </summary>
        /// <param name="templateTypes"></param>
        /// <returns></returns>
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

            if (MissingInstance != null)
            {
                templatedFamily.MissingInstance = MissingInstance.CloseType(templateTypes);
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

        /// <summary>
        /// Add a single concrete type as a new Instance with a derived name.
        /// Is idempotent.
        /// </summary>
        /// <param name="concreteType"></param>
        public void AddType(Type concreteType)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType))
            {
                AddType(concreteType, concreteType.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// The Policies from the root PluginGraph containing this PluginFamily
        /// or a default set of Policies if none supplied
        /// </summary>
        public Policies Policies
        {
            get
            {
                if (Owner == null || Owner.Root == null) return new Policies();

                return Owner.Root.Policies;
            }
        }

        /// <summary>
        /// Adds a new Instance for the concreteType with a name
        /// </summary>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        public void AddType(Type concreteType, string name)
        {
            if (!concreteType.CanBeCastTo(_pluginType)) return;

            if (!hasType(concreteType) && Policies.CanBeAutoFilled(concreteType))
            {
                var instance = new ConstructorInstance(concreteType);
                if (name.IsNotEmpty()) instance.Name = name;
                AddInstance(instance);
            }
        }

        /// <summary>
        /// completely removes an Instance from a PluginFamily
        /// </summary>
        /// <param name="instance"></param>
        public void RemoveInstance(Instance instance)
        {
            _instances.Remove(instance.Name);
            if (instance == GetDefaultInstance())
            {
                resetDefault();
            }
        }

        /// <summary>
        /// Removes all Instances and resets the default Instance determination
        /// </summary>
        public void RemoveAll()
        {
            _instances.ClearAll();
            resetDefault();
        }
    }
}