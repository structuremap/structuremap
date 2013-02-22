using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Util;
using StructureMap;

namespace StructureMap.Graph
{
    // TODO -- might eliminate the cache and use a dictionary directly
    // TODO -- go to reader writer locks

    /// <summary>
    ///   Models the runtime configuration of a StructureMap Container
    /// </summary>
    [Serializable]
    public class PluginGraph : IPluginGraph, IDisposable
    {
        private readonly Cache<Type, PluginFamily> _families;
        private readonly IList<IFamilyPolicy> _policies = new List<IFamilyPolicy>();

        private readonly InterceptorLibrary _interceptorLibrary = new InterceptorLibrary();
        
        private readonly List<Registry> _registries = new List<Registry>();
        private GraphLog _log = new GraphLog();
        private readonly LifecycleObjectCache _singletonCache = new LifecycleObjectCache();

        private readonly LightweightCache<string, PluginGraph> _profiles = new LightweightCache<string, PluginGraph>(name => new PluginGraph{ProfileName = name});  

        public PluginGraph()
        {
            ProfileName = "DEFAULT";

            _profiles = new LightweightCache<string, PluginGraph>(name => new PluginGraph{ProfileName = name});

            _families = new Cache<Type, PluginFamily>(type =>
            {
                return _policies.FirstValue(x => x.Build(type)) ?? new PluginFamily(type);
            });

            _families.OnAddition = family => family.Owner = this;


        }

        public PluginGraph(string profileName) : this()
        {
            ProfileName = profileName;
        }

        public string ProfileName { get; private set; }

        public LifecycleObjectCache SingletonCache
        {
            get { return _singletonCache; }
        }

        public static PluginGraph Empty()
        {
            return new PluginGraphBuilder().Build();
        }

        public PluginGraph Profile(string name)
        {
            return _profiles[name];
        }

        public IEnumerable<PluginGraph> Profiles
        {
            get { return _profiles; }
        } 

        public void AddFamilyPolicy(IFamilyPolicy policy)
        {
            _policies.Add(policy);
        }

        public List<Registry> Registries
        {
            get { return _registries; }
        }

        public GraphLog Log
        {
            get { return _log; }
            set { _log = value; }
        }

        // TODO -- do something tighter here later?
        public Cache<Type, PluginFamily> Families
        {
            get { return _families; }
        }

        public InterceptorLibrary InterceptorLibrary
        {
            get { return _interceptorLibrary; }
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
        ///   Add configuration to a PluginGraph with the Registry DSL
        /// </summary>
        /// <param name = "action"></param>
        public void Configure(Action<Registry> action)
        {
            var registry = new Registry();
            action(registry);

            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public void ImportRegistry(Type type)
        {
            if (Registries.Any(x => x.GetType() == type)) return;

            var registry = (Registry) Activator.CreateInstance(type);
            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public static PluginGraph BuildGraphFromAssembly(Assembly assembly)
        {
            var builder = new PluginGraphBuilder();
            var scanner = new AssemblyScanner();
            scanner.Assembly(assembly);

            builder.AddScanner(scanner);

            return builder.Build();
        }

        public void AddFamily(PluginFamily family)
        {
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

        public bool HasDefaultForPluginType(Type pluginType)
        {
            if (!HasFamily(pluginType))
            {
                return false;
            }

            return Families[pluginType].GetDefaultInstance() != null;
        }

        public void EjectFamily(Type pluginType)
        {
            if (_families.Has(pluginType))
            {
                var family = _families[pluginType];
                family.SafeDispose();

                _families.Remove(pluginType);
            }
        }

        public void EachInstance(Action<Type, Instance> action)
        {
            _families.Each(family =>
            {
                family.Instances.Each(i => action(family.PluginType, i));
            });
        }

        public Instance FindInstance(Type pluginType, string name)
        {
            if (!HasFamily(pluginType)) return null;

            return _families[pluginType].GetInstance(name) ?? _families[pluginType].MissingInstance;
        }

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
            _families.Remove(typeof(IContainer));
            containerFamily.RemoveAll();

            _families.Each(x => x.SafeDispose());
            _families.ClearAll();
        }
    }
}