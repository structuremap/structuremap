using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class ProfileManager
    {
        private readonly Profile _default = new Profile("");
        private readonly Profile _machineProfile = new Profile("MACHINE");
        private readonly Dictionary<string, Profile> _profiles = new Dictionary<string, Profile>();
        private Profile _currentProfile;
        private string _defaultMachineProfileName;
        private string _defaultProfileName;


        public ProfileManager()
        {
            _currentProfile = _default;
        }

        public string DefaultMachineProfileName
        {
            get { return _defaultMachineProfileName; }
            set { _defaultMachineProfileName = value; }
        }

        public string DefaultProfileName
        {
            get { return _defaultProfileName; }
            set { _defaultProfileName = value; }
        }

        public string CurrentProfile
        {
            get { return _currentProfile.Name; }
            set
            {
                // TODO:  Profile cannot be found

                if (string.IsNullOrEmpty(value))
                {
                    _currentProfile = _default;
                }
                else
                {
                    _currentProfile = getProfile(value);
                    _default.FillAllTypesInto(_currentProfile);
                }
            }
        }


        public Instance GetDefault(Type pluginType, string profileName)
        {
            Profile profile = getProfile(profileName);
            return profile.GetDefault(pluginType);
        }

        public void SetDefault(string profileName, Type pluginType, Instance instance)
        {
            Profile profile = getProfile(profileName);
            profile.SetDefault(pluginType, instance);
        }

        private Profile getProfile(string profileName)
        {
            if (!_profiles.ContainsKey(profileName))
            {
                Profile profile = new Profile(profileName);
                _profiles.Add(profileName, profile);
            }

            return _profiles[profileName];
        }

        public Instance GetMachineDefault(Type pluginType)
        {
            return _machineProfile.GetDefault(pluginType);
        }

        public void SetMachineDefault(Type pluginType, Instance instance)
        {
            _machineProfile.SetDefault(pluginType, instance);
        }

        public void SetDefault(Type pluginType, Instance instance)
        {
            _default.SetDefault(pluginType, instance);
        }

        public Instance GetDefault(Type pluginType)
        {
            return _currentProfile.GetDefault(pluginType);
        }

        public void Seal(PluginGraph graph)
        {
            findMasterInstances(graph);

            setProfileDefaults();

            processMachineDefaults(graph);

            findFamilyDefaults(graph);

            backfillProfiles();

            _currentProfile = _default;
        }

        private void backfillProfiles()
        {
            foreach (KeyValuePair<string, Profile> pair in _profiles)
            {
                _default.FillAllTypesInto(pair.Value);
            }
        }

        private void setProfileDefaults()
        {
            if (string.IsNullOrEmpty(_defaultProfileName))
            {
                return;
            }

            // TODO:  What if Profile doesn't exist?
            Profile profile = getProfile(_defaultProfileName);
            profile.FillAllTypesInto(_default);
        }

        private void findFamilyDefaults(PluginGraph graph)
        {
            foreach (PluginFamily family in graph.PluginFamilies)
            {
                findDefaultFromPluginFamily(family);
            }
        }

        private void findMasterInstances(PluginGraph graph)
        {
            _machineProfile.FindMasterInstances(graph);
            foreach (KeyValuePair<string, Profile> pair in _profiles)
            {
                pair.Value.FindMasterInstances(graph);
            }
        }

        private void processMachineDefaults(PluginGraph graph)
        {
            _machineProfile.FillAllTypesInto(_default);

            if (!string.IsNullOrEmpty(_defaultMachineProfileName))
            {
                // TODO:  Machine profile name cannot be found
                Profile profile = getProfile(_defaultMachineProfileName);
                profile.FillAllTypesInto(_default);
            }
        }

        private void findDefaultFromPluginFamily(PluginFamily family)
        {
            // TODO:  Sad path here if the default instance key cannot be found
            // TODO:  Pull inside of PluginFamily itself
            if (string.IsNullOrEmpty(family.DefaultInstanceKey))
            {
                return;
            }

            Instance defaultInstance = family.GetInstance(family.DefaultInstanceKey);
            _default.FillTypeInto(family.PluginType, defaultInstance);
        }

        public void CopyDefaults(Type basicType, Type templatedType)
        {
            _default.CopyDefault(basicType, templatedType);
            foreach (KeyValuePair<string, Profile> pair in _profiles)
            {
                pair.Value.CopyDefault(basicType, templatedType);
            }
        }
    }
}