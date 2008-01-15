using System;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    public class InstanceValidator : IInstanceValidator
    {
        private readonly Profile _defaultProfile;
        private readonly PluginGraph _pluginGraph;
        private InstanceManager _instanceManager;

        public InstanceValidator(PluginGraph pluginGraph, Profile defaultProfile, InstanceManager instanceManager)
        {
            if (defaultProfile == null)
            {
                throw new ArgumentNullException("defaultProfile", "Cannot be null");
            }

            _pluginGraph = pluginGraph;
            _defaultProfile = defaultProfile;
            _instanceManager = instanceManager;
        }

        #region IInstanceValidator Members

        public object CreateObject(Type pluginType, InstanceMemento memento)
        {
            return _instanceManager.CreateInstance(pluginType, memento);
        }

        public bool HasDefaultInstance(Type pluginType)
        {
            return _defaultProfile.HasOverride(TypePath.GetAssemblyQualifiedName(pluginType));
        }

        public bool InstanceExists(Type pluginType, string instanceKey)
        {
            bool returnValue = false;

            try
            {
                PluginFamily family = _pluginGraph.PluginFamilies[pluginType];
                InstanceMemento memento = family.Source.GetMemento(instanceKey);
                if (memento != null)
                {
                    returnValue = true;
                }
            }
            catch (Exception)
            {
                returnValue = false;
            }

            return returnValue;
        }

        #endregion
    }
}