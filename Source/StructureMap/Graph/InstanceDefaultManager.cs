using System;
using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Contains the logic rules to determine the default instances for a PluginGraph and/or
    /// InstanceManager for any combination of profile and machine name.
    /// </summary>
    [Serializable]
    public class InstanceDefaultManager
    {
        private string _defaultProfileName = string.Empty;
        private List<InstanceDefault> _defaults;
        private Dictionary<string, MachineOverride> _machineOverrides;
        private Dictionary<string, Profile> _profiles;

        public InstanceDefaultManager() : base()
        {
            _defaults = new List<InstanceDefault>();
            _machineOverrides = new Dictionary<string, MachineOverride>();
            _profiles = new Dictionary<string, Profile>();
        }

        /// <summary>
        /// If defined, sets the default Profile to be used if no other profile 
        /// is requested
        /// </summary>
        public string DefaultProfileName
        {
            get { return _defaultProfileName; }
            set { _defaultProfileName = value == null ? string.Empty : value; }
        }

        public Profile[] Profiles
        {
            get
            {
                Profile[] returnValue = new Profile[_profiles.Count];
                _profiles.Values.CopyTo(returnValue, 0);
                Array.Sort(returnValue);
                return returnValue;
            }
        }

        public MachineOverride[] MachineOverrides
        {
            get
            {
                MachineOverride[] returnValue = new MachineOverride[_machineOverrides.Count];
                _machineOverrides.Values.CopyTo(returnValue, 0);
                Array.Sort(returnValue);
                return returnValue;
            }
        }

        public static string GetMachineName()
        {
            string machineName = string.Empty;
            try
            {
                machineName = Environment.MachineName.ToUpper();
            }
            finally
            {
            }

            return machineName;
        }

        public void ReadDefaultsFromPluginGraph(PluginGraph graph)
        {
            _defaults = new List<InstanceDefault>();

            foreach (PluginFamily family in graph.PluginFamilies)
            {
                InstanceDefault instanceDefault = new InstanceDefault(family.PluginType, family.DefaultInstanceKey);
                _defaults.Add(instanceDefault);
            }
        }


        /// <summary>
        /// Adds the InstanceDefault from a PluginFamily
        /// </summary>
        /// <param name="instanceDefault"></param>
        public void AddPluginFamilyDefault(InstanceDefault instanceDefault)
        {
            _defaults.Add(instanceDefault);
        }

        /// <summary>
        /// Adds the InstanceDefault from a PluginFamily
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="defaultKey"></param>
        public void AddPluginFamilyDefault(string pluginTypeName, string defaultKey)
        {
            if (defaultKey == null)
            {
                defaultKey = string.Empty;
            }

            AddPluginFamilyDefault(new InstanceDefault(pluginTypeName, defaultKey));
        }


        /// <summary>
        /// Register a MachineOverride
        /// </summary>
        /// <param name="machine"></param>
        public void AddMachineOverride(MachineOverride machine)
        {
            _machineOverrides.Add(machine.MachineName, machine);
        }

        /// <summary>
        /// Register a Profile
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(Profile profile)
        {
            _profiles.Add(profile.ProfileName, profile);
        }

        /// <summary>
        /// Fetches the named Profile
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public Profile GetProfile(string profileName)
        {
            if (!_profiles.ContainsKey(profileName))
            {
                return null;
            }

            return _profiles[profileName];
        }

        /// <summary>
        /// Fetches the named MachineOverride
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public MachineOverride GetMachineOverride(string machineName)
        {
            if (_machineOverrides.ContainsKey(machineName))
            {
                return _machineOverrides[machineName];
            }
            else
            {
                return new MachineOverride(machineName);
            }
        }

        private Profile findCurrentProfile(string profileName)
        {
            bool profileNameIsBlank = profileName == string.Empty || profileName == null;
            string profileToFind = profileNameIsBlank ? _defaultProfileName : profileName;

            if (_profiles.ContainsKey(profileToFind))
            {
                return _profiles[profileToFind];
            }
            else
            {
                return new Profile(profileToFind);
            }
        }

        /// <summary>
        /// Determines the default instance key for each plugin type using machine and/or
        /// profile overrides.  Used internally by <see cref="ObjectFactory"/> to set instance
        /// defaults at runtime
        /// </summary>
        /// <param name="machineName">The machine (computer) name.</param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public Profile CalculateDefaults(string machineName, string profileName)
        {
            Profile answer = new Profile("Defaults");
            MachineOverride machine = GetMachineOverride(machineName);
            Profile profile = findCurrentProfile(profileName);

            foreach (InstanceDefault instance in _defaults)
            {
                answer.AddOverride((InstanceDefault) instance.Clone());

                // Machine specific override
                if (machine.HasOverride(instance.PluginTypeName))
                {
                    answer.AddOverride(instance.PluginTypeName, machine[instance.PluginTypeName]);
                }

                // Profile specific override
                if (profile.HasOverride(instance.PluginTypeName))
                {
                    answer.AddOverride(instance.PluginTypeName, profile[instance.PluginTypeName]);
                }
            }

            return answer;
        }

        /// <summary>
        /// Returns the defaults for the current machine name and the default profile
        /// </summary>
        /// <returns></returns>
        public Profile CalculateDefaults()
        {
            string profileName = DefaultProfileName;
            string machinceName = GetMachineName();

            return CalculateDefaults(machinceName, profileName);
        }

        /// <summary>
        /// Determines ONLY overriden defaults.  Used by the Deployment NAnt task to 
        /// filter a PluginGraph prior to deploying a subset of the StructureMap.config
        /// file
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public Profile CalculateOverridenDefaults(string machineName, string profileName)
        {
            MachineOverride machine = GetMachineOverride(machineName);
            Profile profile = findCurrentProfile(profileName);

            Profile answer = new Profile("Defaults");
            foreach (InstanceDefault instance in machine.Defaults)
            {
                answer.AddOverride((InstanceDefault) instance.Clone());
            }

            foreach (InstanceDefault instance in profile.Defaults)
            {
                answer.AddOverride((InstanceDefault) instance.Clone());
            }

            return answer;
        }

        public void ClearMachineOverrides()
        {
            _machineOverrides.Clear();
        }

        public void ClearProfiles()
        {
            _profiles.Clear();
        }

        public string[] GetMachineNames()
        {
            string[] names = new string[_machineOverrides.Count];
            int i = 0;
            foreach (MachineOverride machine in _machineOverrides.Values)
            {
                names[i++] = machine.MachineName;
            }

            return names;
        }

        public string[] GetProfileNames()
        {
            string[] names = new string[_profiles.Count];
            int i = 0;
            foreach (Profile profile in _profiles.Values)
            {
                names[i++] = profile.ProfileName;
            }

            return names;
        }
    }
}