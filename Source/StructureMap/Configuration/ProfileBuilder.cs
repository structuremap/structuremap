using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class ProfileBuilder : IProfileBuilder
    {
        private static string _overriden_machine_name;

        public static void OverrideMachineName(string machineName)
        {
            _overriden_machine_name = machineName;
        }

        public static void ResetMachineName()
        {
            _overriden_machine_name = string.Empty;
        }

        private readonly string _machineName;
        private readonly PluginGraph _pluginGraph;
        private readonly ProfileManager _profileManager;
        private string _lastProfile;
        private bool _useMachineOverrides;


        public ProfileBuilder(PluginGraph pluginGraph, string machineName)
        {
            _pluginGraph = pluginGraph;
            _profileManager = pluginGraph.ProfileManager;
            _machineName = machineName;
        }


        public ProfileBuilder(PluginGraph pluginGraph)
            : this(pluginGraph, GetMachineName())
        {
        }

        #region IProfileBuilder Members

        public void AddProfile(string profileName)
        {
            _lastProfile = profileName;
        }

        public void OverrideProfile(TypePath typePath, string instanceKey)
        {
            _pluginGraph.Log.WithType(typePath, "while trying to add an override for a Profile", pluginType =>
            {
                ReferencedInstance instance = new ReferencedInstance(instanceKey);
                _pluginGraph.SetDefault(_lastProfile, pluginType, instance);
            });
        }

        public void AddMachine(string machineName, string profileName)
        {
            _useMachineOverrides = machineName == _machineName;

            if (_useMachineOverrides)
            {
                _profileManager.DefaultMachineProfileName = profileName;
            }
        }

        public void OverrideMachine(TypePath typePath, string instanceKey)
        {
            if (!_useMachineOverrides)
            {
                return;
            }

            _pluginGraph.Log.WithType(typePath, 
                "trying to configure a Machine Override",
                                      pluginType =>
                                      {
                                          ReferencedInstance instance = new ReferencedInstance(instanceKey);
                                          _profileManager.SetMachineDefault(pluginType, instance);
                                      });
        }

        public void SetDefaultProfileName(string profileName)
        {
            _profileManager.DefaultProfileName = profileName;
        }

        #endregion

        public static string GetMachineName()
        {
            if (!string.IsNullOrEmpty(_overriden_machine_name))
            {
                return _overriden_machine_name;
            }

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
    }
}