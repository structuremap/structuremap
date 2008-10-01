using System;
using System.Collections.Generic;
using StructureMap.Diagnostics;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class ProfileManager
    {
        private readonly Profile _default = new Profile("");
        private readonly Profile _machineProfile = new Profile("MACHINE");
        private readonly Dictionary<string, Profile> _profiles = new Dictionary<string, Profile>();
        private Profile _currentProfile = new Profile(string.Empty);
        private string _defaultMachineProfileName;
        private string _defaultProfileName;
        private object _locker = new object();


        public ProfileManager()
        {

        }

        private void setProfile(Profile profile)
        {
            lock (_locker)
            {
                _currentProfile = new Profile(profile.Name);
                profile.FillAllTypesInto(_currentProfile);

                if (!ReferenceEquals(profile, _default))
                {
                    _default.FillAllTypesInto(_currentProfile);
                }
            }
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
                if (string.IsNullOrEmpty(value))
                {
                    setProfile(_default);
                }
                else
                {
                    validateHasProfile(value);

                    Profile profile = getProfile(value);
                    setProfile(profile);
                }
            }
        }

        private void validateHasProfile(string profileName)
        {
            if (!_profiles.ContainsKey(profileName))
            {
                throw new StructureMapException(280, profileName);
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
            if (profileName == _defaultProfileName || profileName == _defaultMachineProfileName)
            {
                _default.FillTypeInto(pluginType, instance);
                _currentProfile.FillTypeInto(pluginType, instance);
            }
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
            _currentProfile.SetDefault(pluginType, instance);
            _default.FillTypeInto(pluginType, instance);
        }

        public Instance GetDefault(Type pluginType)
        {
            return _currentProfile.GetDefault(pluginType);
        }

        public void Seal(PluginGraph graph)
        {
            findMasterInstances(graph);

            setProfileDefaults(graph.Log);

            processMachineDefaults(graph);

            findFamilyDefaults(graph);

            backfillProfiles();

            setProfile(_default);
        }

        private void backfillProfiles()
        {
            foreach (KeyValuePair<string, Profile> pair in _profiles)
            {
                _default.FillAllTypesInto(pair.Value);
            }
        }

        private void setProfileDefaults(GraphLog log)
        {
            if (string.IsNullOrEmpty(_defaultProfileName))
            {
                return;
            }

            if (!_profiles.ContainsKey(_defaultProfileName))
            {
                log.RegisterError(280, _defaultProfileName);
            }

            Profile profile = getProfile(_defaultProfileName);
            profile.FillAllTypesInto(_default);
        }

        private void findFamilyDefaults(PluginGraph graph)
        {
            foreach (PluginFamily family in graph.PluginFamilies)
            {
                family.FillDefault(_default);
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
                if (!_profiles.ContainsKey(_defaultMachineProfileName))
                {
                    graph.Log.RegisterError(280, _defaultMachineProfileName);
                }
                Profile profile = getProfile(_defaultMachineProfileName);
                profile.FillAllTypesInto(_default);
            }
        }

        public void CopyDefaults(Type basicType, Type templatedType, PluginFamily family)
        {
            _default.CopyDefault(basicType, templatedType, family);
            foreach (KeyValuePair<string, Profile> pair in _profiles)
            {
                pair.Value.CopyDefault(basicType, templatedType, family);
            }

            if (!string.IsNullOrEmpty(CurrentProfile))
            {
                Profile theSourceProfile = getProfile(CurrentProfile);
                theSourceProfile.FillAllTypesInto(_currentProfile);
            }
            
            _default.FillAllTypesInto(_currentProfile);
            
        }

        public void ImportFrom(ProfileManager source)
        {
            foreach (KeyValuePair<string, Profile> pair in source._profiles)
            {
                Profile fromProfile = pair.Value;
                Profile toProfile = getProfile(pair.Key);

                fromProfile.Merge(toProfile);
            }

            source._default.Merge(_default);

            

            setProfileDefaults(new GraphLog());

            CurrentProfile = CurrentProfile;
        }
    }
}