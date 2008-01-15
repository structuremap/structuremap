using StructureMap.Attributes;

namespace StructureMap.Graph.Configuration
{
    public interface IGraphBuilder
    {
        PluginGraph SystemGraph { get; }
        InstanceDefaultManager DefaultManager { get; }
        PluginGraph PluginGraph { get; }
        void AddAssembly(string assemblyName, string[] deployableTargets);

        void StartFamilies();

        void AddPluginFamily(TypePath typePath, string defaultKey, string[] deploymentTargets, InstanceScope scope);
        void AttachSource(TypePath pluginTypePath, InstanceMemento sourceMemento);
        void AttachSource(TypePath pluginTypePath, MementoSource source);
        Plugin AddPlugin(TypePath pluginTypePath, TypePath pluginPath, string concreteKey);
        SetterProperty AddSetter(TypePath pluginTypePath, string concreteKey, string setterName);
        void AddInterceptor(TypePath pluginTypePath, InstanceMemento interceptorMemento);

        void FinishFamilies();

        PluginGraph CreatePluginGraph();

        void RegisterMemento(TypePath pluginTypePath, InstanceMemento memento);

        void AddProfile(string profileName);
        void OverrideProfile(string fullTypeName, string instanceKey);
        void AddMachine(string machineName, string profileName);
        void OverrideMachine(string fullTypeName, string instanceKey);
        TypePath LocateOrCreateFamilyForType(string fullName);
    }
}