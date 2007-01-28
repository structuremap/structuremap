using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection class for PluginFamily's
    /// </summary>
    public class PluginFamilyCollection : PluginGraphObjectCollection
    {
        private Dictionary<Type, PluginFamily> _pluginFamilies;

        public PluginFamilyCollection(PluginGraph pluginGraph) : base(pluginGraph)
        {
            _pluginFamilies = new Dictionary<Type, PluginFamily>();
        }

        protected override ICollection innerCollection
        {
            get { return _pluginFamilies.Values; }
        }

        public PluginFamily Add(Type pluginType, string defaultInstanceKey)
        {
            PluginFamily family = new PluginFamily(pluginType, defaultInstanceKey);
            return Add(family);
        }


        public PluginFamily Add(Type pluginType, string defaultInstanceKey, MementoSource mementoSource)
        {
            PluginFamily family = new PluginFamily(pluginType, defaultInstanceKey, mementoSource);
            return Add(family);
        }

        public PluginFamily Add(PluginFamily family)
        {
            if (family.PluginTypeName.Contains("GrandChild"))
            {
                int x = 0;
            }

            verifyNotSealed();

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


        public PluginFamily this[TypePath pluginTypePath]
        {
            get
            {
                foreach (KeyValuePair<Type, PluginFamily> pair in _pluginFamilies)
                {
                    if (pluginTypePath.Matches(pair.Key))
                    {
                        return pair.Value;
                    }
                }

                return null;
            }
        }

        public PluginFamily this[int index]
        {
            get
            {
                PluginFamily[] families = new PluginFamily[_pluginFamilies.Count];
                CopyTo(families, 0);
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

        public void Remove(PluginFamily family)
        {
            _pluginFamilies.Remove(family.PluginType);
        }

        public void FilterByDeploymentTarget(string deploymentTarget)
        {
            foreach (PluginFamily family in this)
            {
                if (!family.IsDeployed(deploymentTarget))
                {
                    Remove(family);
                }
            }
        }

        public void RemoveImplicitChildren()
        {
            ArrayList families = new ArrayList(_pluginFamilies.Values);
            foreach (PluginFamily family in families)
            {
                if (family.DefinitionSource == DefinitionSource.Implicit)
                {
                    Remove(family);
                }
                else
                {
                    family.Plugins.RemoveImplicitChildren();
                }
            }
        }

        public bool Contains(Type pluginType)
        {
            return _pluginFamilies.ContainsKey(pluginType);
        }
    }
}