using System;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing
{
    public class ObjectMother
    {
        private static InstanceManager _instanceManager;
        private static PluginGraph _pluginGraph;

        static ObjectMother()
        {
            Reset();
        }

        private ObjectMother()
        {
        }


        public static void Reset()
        {
            DataMother.WriteDocument("FullTesting.XML");

            XmlDocument document = DataMother.GetXmlDocument("ObjectMother.config");
            ConfigurationParser parser = new ConfigurationParser(document.DocumentElement);
            PluginGraphBuilder builder = new PluginGraphBuilder(parser);

            _pluginGraph = builder.Build();

            _instanceManager = new InstanceManager(_pluginGraph);
        }


        public static PluginFamily GetPluginFamily(Type pluginType)
        {
            return _pluginGraph.FindFamily(pluginType);
        }

        public static Plugin GetPlugin(Type pluginType, string concreteKey)
        {
            PluginFamily family = GetPluginFamily(pluginType);
            return family.Plugins[concreteKey];
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

        public static InstanceFactory Factory<T>()
        {
            return new InstanceFactory(new PluginFamily(typeof (T)));
        }
    }
}