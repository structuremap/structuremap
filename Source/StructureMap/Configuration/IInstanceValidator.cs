namespace StructureMap.Configuration
{
	public interface IInstanceValidator
	{
		object CreateObject(string pluginTypeName, InstanceMemento memento);
		bool HasDefaultInstance(string pluginTypeName);
		bool InstanceExists(string pluginTypeName, string instanceKey);
	}
}
