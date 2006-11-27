using System;

namespace StructureMap.Configuration
{
	public interface IInstanceValidator
	{
		object CreateObject(Type pluginType, InstanceMemento memento);
		bool HasDefaultInstance(Type pluginType);
		bool InstanceExists(Type pluginType, string instanceKey);
	}
}
