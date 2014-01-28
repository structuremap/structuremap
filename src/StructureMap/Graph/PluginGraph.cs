using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Graph
{
    // TODO -- might eliminate the cache and use a dictionary directly
    // TODO -- go to reader writer locks

    /// <summary>
    ///   Models the runtime configuration of a StructureMap Container
    /// </summary>
    public class PluginGraph : IPluginGraph, IDisposable
    {
        private readonly Cache<Type, PluginFamily> _families;
        private readonly IList<IFamilyPolicy> _policies = new List<IFamilyPolicy>();

        private readonly List<Registry> _registries = new List<Registry>();
        private readonly LifecycleObjectCache _singletonCache = new LifecycleObjectCache();

        private readonly LightweightCache<string, PluginGraph> _profiles;

        /// <summary>
        /// Specifies interception, construction selection, and setter usage policies
        /// </summary>
        public readonly Policies Policies = new Policies();

        public PluginGraph()
        {
            _profiles =
                new LightweightCache<string, PluginGraph>(name => new PluginGraph {ProfileName = name, Parent = this});

            ProfileName = "DEFAULT";
            _families =
                new Cache<Type, PluginFamily>(
                    type => { return _policies.FirstValue(x => x.Build(type)) ?? new PluginFamily(type); });

            _families.OnAddition = family => family.Owner = this;
        }

        public PluginGraph Parent { get; set; }

        public PluginGraph(string profileName) : this()
        {
            ProfileName = profileName;
        }

        /// <summary>
        /// The profile name of this PluginGraph or "DEFAULT" if it is the top 
        /// </summary>
        public string ProfileName { get; private set; }

        /// <summary>
        /// The cache for all singleton scoped objects
        /// </summary>
        public LifecycleObjectCache SingletonCache
        {
            get { return _singletonCache; }
        }

        /// <summary>
        /// Fetch the PluginGraph for the named profile.  Will
        /// create a new one on the fly for unrecognized names.
        /// Is case sensitive
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PluginGraph Profile(string name)
        {
            return _profiles[name];
        }

        /// <summary>
        /// All the currently known profiles
        /// </summary>
        public IEnumerable<PluginGraph> Profiles
        {
            get { return _profiles; }
        }

        /// <summary>
        /// Add a new family policy that can create new PluginFamily's on demand
        /// when there is no pre-existing family
        /// </summary>
        /// <param name="policy"></param>
        public void AddFamilyPolicy(IFamilyPolicy policy)
        {
            _policies.Add(policy);
        }

        /// <summary>
        /// The list of Registry objects used to create this container
        /// </summary>
        public List<Registry> Registries
        {
            get { return _registries; }
        }

        /// <summary>
        /// Access to all the known PluginFamily members
        /// </summary>
        public Cache<Type, PluginFamily> Families
        {
            get { return _families; }
        }

        /// <summary>
        /// The top most PluginGraph.  If this is the root, will return itself.
        /// If a Profiled PluginGraph, returns its ultimate parent
        /// </summary>
        public PluginGraph Root
        {
            get { return Parent == null ? this : Parent.Root; }
        }

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        public virtual void AddType(Type pluginType, Type concreteType)
        {
            _families[pluginType].AddType(concreteType);
        }

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        /// <param name = "name"></param>
        public virtual void AddType(Type pluginType, Type concreteType, string name)
        {
            _families[pluginType].AddType(concreteType, name);
        }

        /// <summary>
        /// Adds a Registry by type.  Requires that the Registry class have a no argument
        /// public constructor
        /// </summary>
        /// <param name="type"></param>
        public void ImportRegistry(Type type)
        {
            if (Registries.Any(x => x.GetType() == type)) return;

            var registry = (Registry) Activator.CreateInstance(type);
            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public void AddFamily(PluginFamily family)
        {
            family.Owner = this;
            _families[family.PluginType] = family;
        }

        public void RemoveFamily(Type pluginType)
        {
            _families.Remove(pluginType);
        }

        public bool HasInstance(Type pluginType, string name)
        {
            if (!HasFamily(pluginType))
            {
                return false;
            }

            return _families[pluginType].GetInstance(name) != null;
        }

        public PluginFamily FindExistingOrCreateFamily(Type pluginType)
        {
            if (_families.Has(pluginType)) return _families[pluginType];

            var family = new PluginFamily(pluginType);
            _families[pluginType] = family;

            return family;
        }

        public bool HasFamily(Type pluginType)
        {
            if (_families.Has(pluginType)) return true;

            // TODO -- this needs better locking mechanics
            var newFamily = _policies.Where(x => x.AppliesToHasFamilyChecks).FirstValue(x => x.Build(pluginType));
            if (newFamily != null)
            {
                _families[pluginType] = newFamily;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Can this PluginGraph resolve a default instance
        /// for the pluginType?
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public bool HasDefaultForPluginType(Type pluginType)
        {
            if (!HasFamily(pluginType))
            {
                return false;
            }

            return Families[pluginType].GetDefaultInstance() != null;
        }

        /// <summary>
        /// Removes a PluginFamily from this PluginGraph
        /// and disposes that family and all of its Instance's
        /// </summary>
        /// <param name="pluginType"></param>
        public void EjectFamily(Type pluginType)
        {
            if (_families.Has(pluginType))
            {
                var family = _families[pluginType];

                family.SafeDispose();

                _families.Remove(pluginType);
            }
        }

        /// <summary>
        /// Use to iterate through each and every Instance held by this PluginGraph
        /// </summary>
        /// <param name="action"></param>
        public void EachInstance(Action<Type, Instance> action)
        {
            _families.Each(family => family.Instances.Each(i => action(family.PluginType, i)));
        }

        /// <summary>
        /// Find a named instance for a given PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Instance FindInstance(Type pluginType, string name)
        {
            if (!HasFamily(pluginType)) return null;

            return _families[pluginType].GetInstance(name) ?? _families[pluginType].MissingInstance;
        }

        /// <summary>
        /// Returns every instance in the PluginGraph for the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public IEnumerable<Instance> AllInstances(Type pluginType)
        {
            if (HasFamily(pluginType))
            {
                return _families[pluginType].Instances;
            }
            else
            {
                return Enumerable.Empty<Instance>();
            }
        }

        void IDisposable.Dispose()
        {
            _singletonCache.DisposeAndClear();

            _profiles.Each(x => x.SafeDispose());
            _profiles.Clear();

            var containerFamily = _families[typeof (IContainer)];
            _families.Remove(typeof (IContainer));
            containerFamily.RemoveAll();

            _families.Each(x => x.SafeDispose());
            _families.ClearAll();
        }
    }
}