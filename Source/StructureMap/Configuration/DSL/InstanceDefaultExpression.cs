using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    /// <summary>
    /// Use to express the instance of a PluginType for the containing Profile
    /// </summary>
    public class InstanceDefaultExpression
    {
        private readonly Type _pluginType;
        private readonly ProfileExpression _parent;
        private string _instanceKey = string.Empty;
        private IMementoBuilder _mementoBuilder;

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
            else if (_mementoBuilder != null)
            {
                string defaultKey = Profile.InstanceKeyForProfile(profile.ProfileName);
                InstanceMemento memento = _mementoBuilder.BuildMemento(graph);
                memento.InstanceKey = defaultKey;

                graph.PluginFamilies[_pluginType].AddInstance(memento);

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
        public ProfileExpression Use(IMementoBuilder mementoBuilder)
        {
            _mementoBuilder = mementoBuilder;

            return _parent;
        }
    }
}