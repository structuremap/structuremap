using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Graph
{
    /// <summary>
    /// A collection of InstanceDefault's overriding the default instances
    /// </summary>
    [Serializable]
    public class Profile : GraphObject
    {
        private Dictionary<string, InstanceDefault> _defaults;
        private string _profileName;

        public Profile(string profileName)
        {
            _profileName = profileName;
            _defaults = new Dictionary<string, InstanceDefault>();
        }

        public string ProfileName
        {
            get { return _profileName; }
            set { _profileName = value; }
        }

        [IndexerName("DefaultInstanceKey")]
        public string this[string pluginTypeName]
        {
            get
            {
                if (_defaults.ContainsKey(pluginTypeName))
                {
                    return (_defaults[pluginTypeName]).DefaultKey;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int Count
        {
            get { return _defaults.Count; }
        }

        public InstanceDefault[] Defaults
        {
            get
            {
                InstanceDefault[] defaults = getDefaultArray();
                Array.Sort(defaults);

                return defaults;
            }
        }

        protected override string key
        {
            get { return ProfileName; }
        }


        public bool OverridesPluginType(string pluginTypeName)
        {
            return _defaults.ContainsKey(pluginTypeName);
        }

        public void AddOverride(InstanceDefault instanceDefault)
        {
            if (_defaults.ContainsKey(instanceDefault.PluginTypeName))
            {
                _defaults[instanceDefault.PluginTypeName] = instanceDefault;
            }
            else
            {
                _defaults.Add(instanceDefault.PluginTypeName, instanceDefault);
            }
        }

        public void AddOverride(string pluginTypeName, string defaultKey)
        {
            AddOverride(new InstanceDefault(pluginTypeName, defaultKey));
        }

        public void RemoveOverride(string pluginTypeName)
        {
            _defaults.Remove(pluginTypeName);
        }

        public bool HasOverride(string pluginTypeName)
        {
            return _defaults.ContainsKey(pluginTypeName);
        }


        private InstanceDefault[] getDefaultArray()
        {
            InstanceDefault[] defaults = new InstanceDefault[_defaults.Count];
            _defaults.Values.CopyTo(defaults, 0);
            return defaults;
        }


        public void FilterOutNonExistentPluginTypes(PluginGraphReport report)
        {
            foreach (InstanceDefault instanceDefault in getDefaultArray())
            {
                FamilyToken family = report.FindFamily(instanceDefault.PluginTypeName);
                if (family == null)
                {
                    RemoveOverride(instanceDefault.PluginTypeName);
                }
            }
        }


        public void FilterBlanks()
        {
            foreach (InstanceDefault instanceDefault in getDefaultArray())
            {
                if (instanceDefault.DefaultKey == string.Empty)
                {
                    _defaults.Remove(instanceDefault.PluginTypeName);
                }
            }
        }

        public static string InstanceKeyForProfile(string profileName)
        {
            return profileName + "-Instance";
        }
    }
}