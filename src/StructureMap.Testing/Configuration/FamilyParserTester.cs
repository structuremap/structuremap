using System;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class FamilyParserTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var builder = new GraphBuilder(new Registry[0]);
            _graph = builder.PluginGraph;

            _parser =
                new FamilyParser(builder,
                                 new XmlMementoCreator(XmlMementoStyle.NodeNormalized, XmlConstants.TYPE_ATTRIBUTE,
                                                       XmlConstants.ATTRIBUTE_STYLE));

            _document = new XmlDocument();
            _document.LoadXml("<PluginFamily />");
            _familyElement = _document.DocumentElement;

            thePluginType = typeof (IGateway);

            TypePath.WriteTypePathToXmlElement(thePluginType, _familyElement);
        }

        #endregion

        private FamilyParser _parser;
        private XmlDocument _document;
        private XmlElement _familyElement;
        private Type thePluginType;
        private PluginGraph _graph;

        private void assertThatTheFamilyLifecycleIs<T>()
        {
            _parser.ParseFamily(_familyElement);

            PluginFamily family = _graph.FindFamily(thePluginType);
            Assert.IsInstanceOfType(typeof (T), family.Lifecycle);
        }


        [Test]
        public void ScopeIsBlank()
        {
            _parser.ParseFamily(_familyElement);

            _graph.FindFamily(thePluginType).Lifecycle.ShouldBeNull();
        }


        [Test]
        public void ScopeIsBlank2()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE, "");
            _parser.ParseFamily(_familyElement);

            _graph.FindFamily(thePluginType).Lifecycle.ShouldBeNull();
        }


        [Test]
        public void ScopeIsSingleton()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE, InstanceScope.Singleton.ToString());
            assertThatTheFamilyLifecycleIs<SingletonLifecycle>();
        }


        [Test]
        public void ScopeIsThreadLocal()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE, InstanceScope.ThreadLocal.ToString());
            assertThatTheFamilyLifecycleIs<ThreadLocalStorageLifecycle>();
        }
    }
}