using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection for Plugin objects
    /// </summary>
    public class PluginCollection : IEnumerable<Plugin>
    {
        private readonly PluginFamily _family;
        private readonly Dictionary<Type, Plugin> _plugins = new Dictionary<Type, Plugin>();

        public PluginCollection(PluginFamily family)
        {
            _family = family;
        }

        public Plugin[] All
        {
            get
            {
                Plugin[] returnValue = new Plugin[_plugins.Count];
                _plugins.Values.CopyTo(returnValue, 0);

                return returnValue;
            }
        }

        public int Count
        {
            get { return _plugins.Count; }
        }

        /// <summary>
        /// Gets a Plugin by its PluggedType
        /// </summary>
        /// <param name="PluggedType"></param>
        /// <returns></returns>
        public Plugin this[Type PluggedType]
        {
            get
            {
                return _plugins[PluggedType];
            }
        }

        /// <summary>
        /// Retrieves a Plugin by its ConcreteKey
        /// </summary>
        /// <param name="concreteKey"></param>
        /// <returns></returns>
        public Plugin this[string concreteKey]
        {
            get
            {
                foreach (KeyValuePair<Type, Plugin> pair in _plugins)
                {
                    if (pair.Value.ConcreteKey == concreteKey)
                    {
                        return pair.Value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Adds a new Plugin by the PluggedType
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <param name="concreteKey"></param>
        // TODO -- not wild about this method.
        [Obsolete("Get rid of this")]
        public void Add(Type pluggedType, string concreteKey)
        {
            Plugin plugin = new Plugin(pluggedType, concreteKey);
            Add(plugin);
        }

        public void Add(Plugin plugin)
        {
            if (_plugins.ContainsKey(plugin.PluggedType))
            {
                Plugin peer = this[plugin.PluggedType];
                peer.MergeSetters(plugin);

                // Last ConcreteKey wins
                peer.ConcreteKey = plugin.ConcreteKey;

                return;
            }


            // Reject if the PluggedType cannot be upcast to the PluginType
            if (!TypeRules.CanBeCast(_family.PluginType, plugin.PluggedType))
            {
                // TODO -- get this logged
                throw new StructureMapException(114, plugin.PluggedType.FullName, _family.PluginType.AssemblyQualifiedName);
            }

            _plugins.Add(plugin.PluggedType, plugin);
        }

        /// <summary>
        /// Does the PluginFamily contain a Plugin
        /// </summary>
        /// <param name="concreteKey"></param>
        /// <returns></returns>
        public bool HasPlugin(string concreteKey)
        {
            return this[concreteKey] != null;
        }


        public void Remove(string concreteKey)
        {
            Plugin plugin = this[concreteKey];
            _plugins.Remove(plugin.PluggedType);
        }

        public Plugin FindOrCreate(Type pluggedType, bool createDefaultInstanceOfType)
        {
            Plugin plugin = new Plugin(pluggedType);
            Add(plugin);

            return plugin;
        }

        IEnumerator<Plugin> IEnumerable<Plugin>.GetEnumerator()
        {
            return _plugins.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Plugin>) this).GetEnumerator();
        }

        public List<Plugin> FindAutoFillablePlugins()
        {
            List<Plugin> list = new List<Plugin>();
            foreach (Plugin plugin in _plugins.Values)
            {
                if (plugin.CanBeAutoFilled)
                {
                    list.Add(plugin);
                }
            }

            return list;
        }

        public bool HasPlugin(Type pluggedType)
        {
            return _plugins.ContainsKey(pluggedType);
        }
    }
}