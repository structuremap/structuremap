using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class ProfileBuilder : IProfileBuilder
    {
        private readonly PluginGraph _pluginGraph;
        private readonly ProfileManager _profileManager;
        private string _lastProfile;


        public ProfileBuilder(PluginGraph pluginGraph)
        {
            _pluginGraph = pluginGraph;
            _profileManager = pluginGraph.ProfileManager;
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
                var instance = new ReferencedInstance(instanceKey);
                _pluginGraph.SetDefault(_lastProfile, pluginType, instance);
            });
        }

        public void SetDefaultProfileName(string profileName)
        {
            _profileManager.DefaultProfileName = profileName;
        }

        #endregion

    }
}