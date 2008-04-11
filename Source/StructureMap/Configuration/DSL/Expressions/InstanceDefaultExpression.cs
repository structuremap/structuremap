using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Use to express the instance of a PluginType for the containing Profile
    /// </summary>
    public class InstanceDefaultExpression
    {
        private readonly ProfileExpression _parent;
        private readonly Type _pluginType;
        private string _instanceKey = string.Empty;
        private Instance _instance;

        public InstanceDefaultExpression(Type pluginType, ProfileExpression parent)
        {
            _pluginType = pluginType;
            _parent = parent;
        }

        /// <summary>
        /// Use a named, preconfigured instance as the default instance for this profile 
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public ProfileExpression UseNamedInstance(string instanceKey)
        {
            _instanceKey = instanceKey;
            return _parent;
        }

        internal void Configure(Profile profile, PluginGraph graph)
        {
            if (!string.IsNullOrEmpty(_instanceKey))
            {
                InstanceDefault instanceDefault = new InstanceDefault(_pluginType, _instanceKey);
                profile.AddOverride(instanceDefault);
            }
            else if (_instance != null)
            {
                string defaultKey = Profile.InstanceKeyForProfile(profile.ProfileName);
                
                _instance.Name = defaultKey;
                graph.LocateOrCreateFamilyForType(_pluginType).AddInstance(_instance);

                InstanceDefault instanceDefault = new InstanceDefault(_pluginType, defaultKey);
                profile.AddOverride(instanceDefault);
            }
            else
            {
                throw new StructureMapException(304, TypePath.GetAssemblyQualifiedName(_pluginType));
            }
        }

        /// <summary>
        /// Define the default instance of the PluginType for the containing Profile
        /// </summary>
        /// <param name="mementoBuilder"></param>
        /// <returns></returns>
        public ProfileExpression Use(Instance instance)
        {
            _instance = instance;

            return _parent;
        }
    }
}