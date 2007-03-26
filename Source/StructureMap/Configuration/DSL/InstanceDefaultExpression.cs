using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
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

        public ProfileExpression Use(IMementoBuilder mementoBuilder)
        {
            _mementoBuilder = mementoBuilder;

            return _parent;
        }

        
    }
}
