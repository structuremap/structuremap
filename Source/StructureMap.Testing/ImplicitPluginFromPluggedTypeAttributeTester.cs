using System;
using System.Collections.Specialized;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ImplicitPluginFromPluggedTypeAttributeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Type pluggedType = typeof (StubbedGateway);

            NameValueCollection values = new NameValueCollection();
            values.Add(XmlConstants.PLUGGED_TYPE, TypePath.GetAssemblyQualifiedName(pluggedType));
            _memento = new MemoryInstanceMemento(string.Empty, "Frank", values);

            _expectedPlugin = Plugin.CreateImplicitPlugin(pluggedType);
        }

        #endregion

        private MemoryInstanceMemento _memento;
        private Plugin _expectedPlugin;

        [Test]
        public void CanBuildTheInstance()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            TypePath pluginTypePath = new TypePath(typeof (IGateway));
            builder.AddPluginFamily(pluginTypePath, _memento.InstanceKey, InstanceScope.PerRequest);

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