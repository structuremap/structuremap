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


        // All of these need to DIE!
        //void AddPluginFamily(Type pluginType, string defaultKey, InstanceScope scope);
        void AttachSource(Type pluginType, InstanceMemento sourceMemento);
        void AttachSource(Type pluginType, MementoSource source);
        Plugin AddPlugin(Type pluginType, TypePath pluginPath, string concreteKey);
        SetterProperty AddSetter(Type pluginType, string concreteKey, string setterName);
        void AddInterceptor(Type pluginType, InstanceMemento interceptorMemento);
        void RegisterMemento(Type pluginType, InstanceMemento memento);



        IProfileBuilder GetProfileBuilder();

        void ConfigureFamily(TypePath pluginTypePath, Action<PluginFamily> action);
    }
}