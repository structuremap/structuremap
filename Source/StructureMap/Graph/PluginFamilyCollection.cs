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
        private readonly PluginGraph _pluginGraph;
        private readonly Dictionary<Type, PluginFamily> _pluginFamilies;

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
                    throw new StructureMapException(116, pluginType.FullName);
                }

                return _pluginFamilies[pluginType];
            }
        }


        public PluginFamily this[int index]
        {
            get
            {
                PluginFamily[] families = new PluginFamily[_pluginFamilies.Count];
                return families[index];
            }
        }

        public PluginFamily this[string pluginTypeName]
        {
            get
            {
                Type pluginType = Type.GetType(pluginTypeName);

                if (pluginType == null)
                {
                    foreach (KeyValuePair<Type, PluginFamily> pair in _pluginFamilies)
                    {
                        if (pair.Value.PluginType.FullName == pluginTypeName)
                        {
                            return pair.Value;
                        }
                    }

                    throw new ApplicationException("Could not find PluginFamily " + pluginTypeName);
                }
                else
                {
                    return this[pluginType];
                }
            }
        }

        public int Count
        {
            get { return _pluginFamilies.Count; }
        }


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



        IEnumerator<PluginFamily> IEnumerable<PluginFamily>.GetEnumerator()
        {
            return _pluginFamilies.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<PluginFamily>) this).GetEnumerator();
        }
    }
}