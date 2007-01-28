using System;
using System.Collections;
using System.Runtime.CompilerServices;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.Graph
{
    /// <summary>
    /// Models the machine-level overrides for default instances per plugin type.
    /// </summary>
    [Serializable]
    public class MachineOverride : GraphObject
    {
        private string _machineName;
        private Profile _profile = new Profile(string.Empty);
        private Hashtable _defaults;

        public MachineOverride(string machineName, Profile profile)
            : this(machineName)
        {
            if (profile != null)
            {
                _profile = profile;
            }
        }

        public MachineOverride(string machineName) : base()
        {
            _machineName = machineName;
            _defaults = new Hashtable();
        }

        public string MachineName
        {
            get { return _machineName; }
        }

        /// <summary>
        /// Registers an override for the default instance of a certain plugin type.
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="defaultKey"></param>
        public void AddMachineOverride(string pluginTypeName, string defaultKey)
        {
            InstanceDefault instanceDefault = new InstanceDefault(pluginTypeName, defaultKey);
            _defaults.Add(pluginTypeName, instanceDefault);
        }

        /// <summary>
        /// Determines if the MachineOverride instance has an overriden default for the plugin type
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <returns></returns>
        public bool HasOverride(string pluginTypeName)
        {
            return (_defaults.ContainsKey(pluginTypeName) || _profile.HasOverride(pluginTypeName));
        }

        /// <summary>
        /// Finds the default key for a plugin type
        /// </summary>
        [IndexerName("DefaultKey")]
        public string this[string pluginTypeName]
        {
            get
            {
                if (_profile.HasOverride(pluginTypeName))
                {
                    return _profile[pluginTypeName];
                }
                else
                {
                    return ((InstanceDefault) _defaults[pluginTypeName]).DefaultKey;
                }
            }
        }

        /// <summary>
        /// If the MachineOverride has a Profile, returns the profile name
        /// </summary>
        public string ProfileName
        {
            get { return _profile == null ? string.Empty : _profile.ProfileName; }
        }


        public InstanceDefault[] Defaults
        {
            get
            {
                InstanceDefault[] profileDefaults = _profile.Defaults;
                Hashtable defaultHash = new Hashtable();
                foreach (InstanceDefault instance in profileDefaults)
                {
                    defaultHash.Add(instance.PluginTypeName, instance);
                }

                foreach (InstanceDefault instance in _defaults.Values)
                {
                    if (!defaultHash.ContainsKey(instance.PluginTypeName))
                    {
                        defaultHash.Add(instance.PluginTypeName, instance);
                    }
                }

                InstanceDefault[] returnValue = new InstanceDefault[defaultHash.Count];
                defaultHash.Values.CopyTo(returnValue, 0);
                Array.Sort(returnValue);

                return returnValue;
            }
        }

        /// <summary>
        /// Filters instance defaults for plugin types that are no longer contained by
        /// the PluginGraph
        /// </summary>
        /// <param name="report"></param>
        public void FilterOutNonExistentPluginTypes(PluginGraphReport report)
        {
            foreach (InstanceDefault instanceDefault in Defaults)
            {
                FamilyToken family = report.FindFamily(instanceDefault.PluginTypeName);
                if (family == null)
                {
                    _defaults.Remove(instanceDefault.PluginTypeName);
                }
            }
        }


        public InstanceDefault[] InnerDefaults
        {
            get
            {
                InstanceDefault[] returnValue = new InstanceDefault[_defaults.Count];
                _defaults.Values.CopyTo(returnValue, 0);
                Array.Sort(returnValue);
                return returnValue;
            }
        }


        protected override string key
        {
            get { return MachineName; }
        }
    }
}