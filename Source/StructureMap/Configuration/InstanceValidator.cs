using System;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
	public class InstanceValidator : IInstanceValidator
	{
		private readonly PluginGraph _pluginGraph;
		private readonly Profile _defaultProfile;
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

		public object CreateObject(string pluginTypeName, InstanceMemento memento)
		{
			return _instanceManager.CreateInstance(pluginTypeName, memento);
		}

		public bool HasDefaultInstance(string pluginTypeName)
		{
			return _defaultProfile.HasOverride(pluginTypeName);
		}

		public bool InstanceExists(string pluginTypeName, string instanceKey)
		{
			bool returnValue = false;

			try
			{
				PluginFamily family = _pluginGraph.PluginFamilies[pluginTypeName];
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
	}
}
