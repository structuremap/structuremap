using System;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing
{
    public class ObjectMother
    {
        private static PluginGraph _pluginGraph;
        private static InstanceManager _instanceManager;
        private static InstanceDefaultManager _instanceDefaultManager;
        private static PluginGraphReport _report;

        static ObjectMother()
        {
            Reset();
        }


        public static void Reset()
        {
            DataMother.WriteDocument("FullTesting.XML");

            XmlDocument document = DataMother.GetXmlDocument("ObjectMother.config");
            ConfigurationParser parser = new ConfigurationParser(document.DocumentElement);
            PluginGraphBuilder builder = new PluginGraphBuilder(parser);

            _pluginGraph = builder.BuildDiagnosticPluginGraph();
            _report = builder.Report;

            _instanceManager = new InstanceManager(_pluginGraph);
            _instanceDefaultManager = _pluginGraph.DefaultManager;
           
        }


        public static PluginGraphReport Report()
        {
            return _report;
        }

        private ObjectMother()
        {
        }

        public static InstanceDefaultManager GetInstanceDefaultManager()
        {
            return _instanceDefaultManager;
        }

        public static MachineOverride GetMachineOverride(string machineName)
        {
            return GetInstanceDefaultManager().GetMachineOverride(machineName);
        }


        public static PluginFamily GetPluginFamily(Type pluginType)
        {
            return _pluginGraph.PluginFamilies[pluginType];
        }

        public static Plugin GetPlugin(Type pluginType, string concreteKey)
        {
            PluginFamily family = GetPluginFamily(pluginType);
            return family.Plugins[concreteKey];
        }


        public static Profile GetProfile(string profileName)
        {
            InstanceDefaultManager defaultManager = GetInstanceDefaultManager();
            return defaultManager.GetProfile(profileName);
        }

        public static PluginGraph GetPluginGraph()
        {
            return _pluginGraph;
        }

        public static PluginGraph CreateSealedPluginGraph(string[] assemblyNames)
        {
            PluginGraph pluginGraph = createPluginGraphFromAssemblyNames(assemblyNames);

            pluginGraph.Seal();
            return pluginGraph;
        }

        private static PluginGraph createPluginGraphFromAssemblyNames(string[] assemblyNames)
        {
            PluginGraph pluginGraph = new PluginGraph();

            foreach (string assemblyName in assemblyNames)
            {
                pluginGraph.Assemblies.Add(assemblyName);
            }
            return pluginGraph;
        }

        public static InstanceFactory CreateInstanceFactory(Type pluginType, string[] assemblyNames)
        {
            PluginGraph pluginGraph = createPluginGraphFromAssemblyNames(assemblyNames);
            pluginGraph.PluginFamilies.Add(pluginType, string.Empty);
            pluginGraph.Seal();

            PluginFamily family = pluginGraph.PluginFamilies[pluginType];
            return new InstanceFactory(family, false);
        }
    }
}