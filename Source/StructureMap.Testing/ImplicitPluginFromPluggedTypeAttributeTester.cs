using System;
using System.Collections.Specialized;
using NUnit.Framework;
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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Type pluggedType = typeof (StubbedGateway);

            NameValueCollection values = new NameValueCollection();
            values.Add(XmlConstants.PLUGGED_TYPE, TypePath.GetAssemblyQualifiedName(pluggedType));
            _memento = new MemoryInstanceMemento(string.Empty, "Frank", values);

            _expectedPlugin = new Plugin(pluggedType);
        }

        #endregion

        private MemoryInstanceMemento _memento;
        private Plugin _expectedPlugin;

        [Test]
        public void CanBuildTheInstance()
        {
            GraphBuilder builder = new GraphBuilder(new Registry[0]);
            Type thePluginType = typeof (IGateway);
            PluginFamily family = builder.PluginGraph.FindFamily(thePluginType);
            family.DefaultInstanceKey = _memento.InstanceKey;

            family.AddInstance(_memento);

            PluginGraph graph = builder.PluginGraph;
            StructureMap.Container manager = new StructureMap.Container(graph);

            StubbedGateway gateway = (StubbedGateway) manager.GetInstance(typeof (IGateway), _memento.InstanceKey);

            Assert.IsNotNull(gateway);
        }


        [Test]
        public void RunThroughXml()
        {
            PluginGraph graph = DataMother.GetPluginGraph("PluggedTypeTest.xml");
            StructureMap.Container manager = new StructureMap.Container(graph);

            NotPluggableWidget widget = (NotPluggableWidget) manager.GetInstance(typeof (IWidget), "Me");
            Assert.AreEqual("Jeremy", widget.Name);
        }
    }
}