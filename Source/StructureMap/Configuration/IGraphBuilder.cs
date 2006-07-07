using StructureMap.Attributes;

namespace StructureMap.Graph.Configuration
{
	public interface IGraphBuilder
	{
		void AddAssembly(string assemblyName, string[] deployableTargets);
		
		void StartFamilies();
		
		void AddPluginFamily(TypePath typePath, string defaultKey, string[] deploymentTargets, InstanceScope scope);
		void AttachSource(string pluginTypeName, InstanceMemento sourceMemento);
		void AttachSource(string pluginTypeName, MementoSource source);
		Plugin AddPlugin(string pluginTypeName, TypePath pluginPath, string concreteKey);
		SetterProperty AddSetter(string pluginTypeName, string concreteKey, string setterName);
		void AddInterceptor(string pluginTypeName, InstanceMemento interceptorMemento);
		
		void FinishFamilies();
		
		PluginGraph CreatePluginGraph();
		PluginGraph SystemGraph { get; }
		InstanceDefaultManager DefaultManager {get;}

		void RegisterMemento(string pluginTypeName, InstanceMemento memento);

		PluginGraph PluginGraph { get; }
	}
}
