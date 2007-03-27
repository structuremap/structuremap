using System;
using System.Collections.Specialized;
using System.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ImplicitPluginFromPluggedTypeAttributeTester
    {
        private MemoryInstanceMemento _memento;
        private Plugin _expectedPlugin;

        [SetUp]
        public void SetUp()
        {
            Type pluggedType = typeof(StubbedGateway);

            NameValueCollection values = new NameValueCollection();
            values.Add(XmlConstants.PLUGGED_TYPE, TypePath.GetAssemblyQualifiedName(pluggedType));
            _memento = new MemoryInstanceMemento(string.Empty, "Frank", values);

            _expectedPlugin = Plugin.CreateImplicitPlugin(pluggedType);
        }

        [Test]
        public void InstanceMementoKnowsHowToBuildPluginForPluggedType()
        {
            Assert.AreEqual(_expectedPlugin.ConcreteKey, _memento.ConcreteKey);

            Plugin inferredPlugin = _memento.CreateInferredPlugin();
            Assert.AreEqual(_expectedPlugin, inferredPlugin);
        }

        [Test]
        public void NormalGraphBuilderHandlesTheInferredPlugin()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            TypePath pluginTypePath = new TypePath(typeof(IGateway));
            builder.AddPluginFamily(pluginTypePath, "", new string[0], InstanceScope.PerRequest);

            builder.RegisterMemento(pluginTypePath, _memento);

            PluginGraph graph = builder.CreatePluginGraph();

            Assert.IsTrue(graph.PluginFamilies.Contains(typeof(IGateway)));

            PluginFamily family = graph.PluginFamilies[typeof (IGateway)];
            Plugin plugin = family.Plugins[typeof (StubbedGateway)];
            Assert.AreEqual(_expectedPlugin, plugin);
        }


        [Test]
        public void CanBuildTheInstance()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            TypePath pluginTypePath = new TypePath(typeof(IGateway));
            builder.AddPluginFamily(pluginTypePath, _memento.InstanceKey, new string[0], InstanceScope.PerRequest);

            builder.RegisterMemento(pluginTypePath, _memento);

            PluginGraph graph = builder.CreatePluginGraph();
            InstanceManager manager = new InstanceManager(graph);

            StubbedGateway gateway = (StubbedGateway) manager.CreateInstance(typeof (IGateway), _memento.InstanceKey);
        
            Assert.IsNotNull(gateway);
        }

        [Test]
        public void RunThroughXml()
        {
            PluginGraph graph = DataMother.GetPluginGraph("PluggedTypeTest.xml");
            InstanceManager manager = new InstanceManager(graph);

            NotPluggableWidget widget = (NotPluggableWidget) manager.CreateInstance(typeof (IWidget), "Me");
            Assert.AreEqual("Jeremy", widget.Name);
        }



        
    }
}
