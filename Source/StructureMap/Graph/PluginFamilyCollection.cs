using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection class for PluginFamily's
    /// </summary>
    public class PluginFamilyCollection : IEnumerable<PluginFamily>
    {
        private readonly Dictionary<Type, PluginFamily> _pluginFamilies;
        private readonly PluginGraph _pluginGraph;

        public PluginFamilyCollection(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            _pluginFamilies = new Dictionary<Type, PluginFamily>();
        }

        public PluginFamily this[Type pluginType]
        {
            get
            {
                if (!_pluginFamilies.ContainsKey(pluginType))
                {
                    var family = new PluginFamily(pluginType, _pluginGraph);
                    Add(family);
                }

                return _pluginFamilies[pluginType];
            }
        }

        public int Count
        {
            get { return _pluginFamilies.Count; }
        }

        public IEnumerable<PluginFamily> All
        {
            get
            {
                var families = new PluginFamily[_pluginFamilies.Count];
                _pluginFamilies.Values.CopyTo(families, 0);
                return families;
            }
        }

        #region IEnumerable<PluginFamily> Members

        IEnumerator<PluginFamily> IEnumerable<PluginFamily>.GetEnumerator()
        {
            return _pluginFamilies.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<PluginFamily>) this).GetEnumerator();
        }

        #endregion

        public PluginFamily Add(PluginFamily family)
        {
            family.Parent = _pluginGraph;

            Type key = family.PluginType;
            if (_pluginFamilies.ContainsKey(key))
            {
                _pluginFamilies[key] = family;
            }
            else
            {
                _pluginFamilies.Add(key, family);
            }

            return family;
        }

        public void Remove(PluginFamily family)
        {
            _pluginFamilies.Remove(family.PluginType);
        }

        public bool Contains(Type pluginType)
        {
            return _pluginFamilies.ContainsKey(pluginType);
        }

        public bool Contains<T>()
        {
            return Contains(typeof (T));
        }

        public void Each(Action<PluginFamily> action)
        {
            foreach (PluginFamily family in All)
            {
                action(family);
            }
        }
    }
}