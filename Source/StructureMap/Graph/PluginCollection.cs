using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection for Plugin objects
    /// </summary>
    public class PluginCollection : PluginGraphObjectCollection
    {
        private readonly PluginFamily _family;
        private Dictionary<string, Plugin> _plugins = new Dictionary<string, Plugin>();

        public PluginCollection(PluginFamily family) : base(null)
        {
            _family = family;
        }

        protected override ICollection innerCollection
        {
            get { return _plugins.Values; }
        }

        public void Add(TypePath path, string concreteKey)
        {
            Plugin plugin = new Plugin(path, concreteKey);
            Add(plugin);
        }

        /// <summary>
        /// Adds a new Plugin by the PluggedType
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <param name="concreteKey"></param>
        // TODO -- not wild about this method.
        public void Add(Type pluggedType, string concreteKey)
        {
            Plugin plugin = Plugin.CreateExplicitPlugin(pluggedType, concreteKey, string.Empty);
            Add(plugin);
        }

        public void Add(Plugin plugin)
        {
            // Reject if a duplicate ConcreteKey
            if (_plugins.ContainsKey(plugin.ConcreteKey))
            {
                // Don't duplicate
                Plugin peer = this[plugin.ConcreteKey];
                if (peer.PluggedType == plugin.PluggedType)
                {
                    return;
                }

                throw new StructureMapException(113, plugin.ConcreteKey, _family.PluginTypeName);
            }

            // Reject if the PluggedType cannot be upcast to the PluginType
            if (!Plugin.CanBeCast(_family.PluginType, plugin.PluggedType))
            {
                throw new StructureMapException(114, plugin.PluggedType.FullName, _family.PluginTypeName);
            }

            InstanceMemento memento = plugin.CreateImplicitMemento();
            if (memento != null)
            {
                _family.Source.AddExternalMemento(memento);
            }

            _plugins.Add(plugin.ConcreteKey, plugin);
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
                Plugin returnValue = null;

                foreach (Plugin plugin in _plugins.Values)
                {
                    if (plugin.PluggedType.Equals(PluggedType))
                    {
                        returnValue = plugin;
                        break;
                    }
                }

                return returnValue;
            }
        }

        public Plugin this[int index]
        {
            get
            {
                ArrayList list = new ArrayList(this);
                return (Plugin) list[index];
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
                if (_plugins.ContainsKey(concreteKey))
                {
                    return _plugins[concreteKey] as Plugin;
                }

                string msg = string.Format(
                    "Plugin *{0}* for PluginFamily *{1}* does not exist",
                    concreteKey,
                    _family.PluginTypeName);

                throw new ApplicationException(msg);
            }
        }

        /// <summary>
        /// Does the PluginFamily contain a Plugin
        /// </summary>
        /// <param name="concreteKey"></param>
        /// <returns></returns>
        public bool HasPlugin(string concreteKey)
        {
            return _plugins.ContainsKey(concreteKey);
        }


        public void Remove(string concreteKey)
        {
            _plugins.Remove(concreteKey);
        }

        public void RemoveImplicitChildren()
        {
            foreach (Plugin plugin in this)
            {
                if (plugin.DefinitionSource == DefinitionSource.Implicit)
                {
                    Remove(plugin.ConcreteKey);
                }
            }
        }
    }
}