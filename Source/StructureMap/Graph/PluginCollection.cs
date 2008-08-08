using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Util;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection for Plugin objects
    /// </summary>
    public class PluginCollection : IEnumerable<Plugin>
    {
        private readonly PluginFamily _family;
        private readonly Cache<Type, Plugin> _plugins;

        public PluginCollection(PluginFamily family)
        {
            _family = family;
            _plugins = new Cache<Type, Plugin>(t =>
            {
                // Reject if the PluggedType cannot be upcast to the PluginType
                if (!TypeRules.CanBeCast(_family.PluginType, t))
                {
                    throw new StructureMapException(104, t, _family.PluginType);
                }

                return new Plugin(t);
            });
        }

        public Plugin[] All
        {
            get
            {
                return _plugins.GetAll();
            }
        }

        public int Count
        {
            get { return _plugins.Count; }
        }

        /// <summary>
        /// Gets a Plugin by its pluggedType
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <returns></returns>
        public Plugin this[Type pluggedType]
        {
            get { return _plugins.Retrieve(pluggedType); }
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
                return _plugins.Find(plugin => plugin.ConcreteKey == concreteKey);
            }
        }

        #region IEnumerable<Plugin> Members

        IEnumerator<Plugin> IEnumerable<Plugin>.GetEnumerator()
        {
            return _plugins.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<Plugin>) this).GetEnumerator();
        }

        #endregion


        public void Add(Plugin plugin)
        {
            if (_plugins.Has(plugin.PluggedType))
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
                throw new StructureMapException(104, plugin.PluggedType, _family.PluginType);
            }


            _plugins.Store(plugin.PluggedType, plugin);
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

        public List<Plugin> FindAutoFillablePlugins()
        {
            List<Plugin> list = new List<Plugin>();
            _plugins.Each(plugin =>
            {
                if (plugin.CanBeAutoFilled)
                {
                    list.Add(plugin);
                }
            });

            return list;
        }

        public bool HasPlugin(Type pluggedType)
        {
            return _plugins.Has(pluggedType);
        }

        public void Fill(Plugin plugin)
        {
            _plugins.Fill(plugin.PluggedType, plugin);
        }
    }
}