using System;
using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    public interface IProfileBuilder
    {
        void AddProfile(string profileName);
        void OverrideProfile(TypePath typePath, string instanceKey);
        void AddMachine(string machineName, string profileName);
        void OverrideMachine(TypePath typePath, string instanceKey);
        void SetDefaultProfileName(string profileName);
    }


    public interface IGraphBuilder
    {
        PluginGraph SystemGraph { get; }
        PluginGraph PluginGraph { get; }
        void AddAssembly(string assemblyName);

        void StartFamilies();
        void FinishFamilies();
        PluginGraph CreatePluginGraph();

        void AddPluginFamily(TypePath typePath, string defaultKey, InstanceScope scope);
        void AttachSource(TypePath pluginTypePath, InstanceMemento sourceMemento);
        void AttachSource(TypePath pluginTypePath, MementoSource source);
        Plugin AddPlugin(TypePath pluginTypePath, TypePath pluginPath, string concreteKey);
        SetterProperty AddSetter(TypePath pluginTypePath, string concreteKey, string setterName);
        void AddInterceptor(TypePath pluginTypePath, InstanceMemento interceptorMemento);

        void RegisterMemento(TypePath pluginTypePath, InstanceMemento memento);

        IProfileBuilder GetProfileBuilder();
    }
}