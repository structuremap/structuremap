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
    public class PluginFamily : TypeRules, IPluginFamily
    {
        private readonly List<InstanceMemento> _mementoList = new List<InstanceMemento>();
        private readonly PluginCollection _plugins;
        private string _defaultKey = string.Empty;
        private PluginGraph _parent;
        private readonly List<Instance> _instances = new List<Instance>();
        private IBuildPolicy _buildPolicy = new BuildPolicy();
        private readonly Type _pluginType;

        private readonly Predicate<Type> _explicitlyMarkedPluginFilter;
        private readonly Predicate<Type> _implicitPluginFilter;
        private Predicate<Type> _pluginFilter;
        private IBuildPolicy _policy;

        public PluginFamily(Type pluginType)
        {
            _pluginType = pluginType;
            _plugins = new PluginCollection(this);

            PluginFamilyAttribute.ConfigureFamily(this);

            _explicitlyMarkedPluginFilter = delegate(Type type) { return TypeRules.IsExplicitlyMarkedAsPlugin(PluginType, type); };
            _implicitPluginFilter = delegate(Type type) { return TypeRules.CanBeCast(PluginType, type); };
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

        public void AddInstance(InstanceMemento memento)
        {
            _mementoList.Add(memento);
        }

        public void AddInstance(Instance instance)
        {
            _instances.Add(instance);
        }

        // TODO -- eliminate this.  Move to GraphBuilder, and wrap error handling around it
        public void AddMementoSource(MementoSource source)
        {
            _mementoList.AddRange(source.GetAllMementos());
        }

        // For testing
        public InstanceMemento GetMemento(string instanceKey)
        {
            return _mementoList.Find(delegate(InstanceMemento m) { return m.InstanceKey == instanceKey; });
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

        public bool IsGenericTemplate
        {
            get { return _pluginType.IsGenericTypeDefinition; }
        }

        public bool SearchForImplicitPlugins
        {
            get
            {
                return ReferenceEquals(_pluginFilter, _implicitPluginFilter);
            }
            set
            {
                _pluginFilter = value ? _implicitPluginFilter : _explicitlyMarkedPluginFilter;
            }
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

        #endregion

        public void Seal()
        {
            _mementoList.ForEach(delegate(InstanceMemento memento)
            {
                Instance instance = memento.ReadInstance(Parent, _pluginType);
                _instances.Add(instance);
            });

            discoverImplicitInstances();

            if (_instances.Count == 1)
            {
                _defaultKey = _instances[0].Name;
            }
        }

        private void discoverImplicitInstances()
        {
            // TODO:  Apply some 3.5 lambda magic.  Maybe move to PluginCollection
            List<Plugin> list = _plugins.FindAutoFillablePlugins();
            list.RemoveAll(delegate(Plugin plugin)
                               {
                                   return _instances.Exists(delegate(Instance instance)
                                                         {
                                                             return instance.Matches(plugin);
                                                         });
                               });

            foreach (Plugin plugin in list)
            {
                AddInstance(plugin.CreateImplicitInstance());
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

        public void AddInterceptor(IBuildInterceptor interceptor)
        {
            interceptor.InnerPolicy = _buildPolicy;
            _buildPolicy = interceptor;
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

        public void AddPlugin(Type pluggedType)
        {
            if (!HasPlugin(pluggedType))
            {
                _plugins.Add(new Plugin(pluggedType));
            }
        }

        public Plugin AddPlugin(Type pluggedType, string key)
        {
            Plugin plugin = new Plugin(pluggedType, key);
            _plugins.Add(plugin);

            return plugin;
        }

        public void AddPlugin(Plugin plugin)
        {
            _plugins.Add(plugin);
        }

        public Instance GetDefaultInstance()
        {
            return string.IsNullOrEmpty(_defaultKey) ? null : GetInstance(_defaultKey);
        }
    }
}