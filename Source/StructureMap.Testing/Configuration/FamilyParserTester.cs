using System;
using System.Xml;
using NMock;
using NUnit.Framework;
using StructureMap.Attributes;
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
            GraphBuilder builder = new GraphBuilder(new Registry[0]);
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

        private void assertThatTheFamilyPolicyIs<T>()
        {
            _parser.ParseFamily(_familyElement);

            PluginFamily family = _graph.FindFamily(thePluginType);
            Assert.IsInstanceOfType(typeof(T), family.Policy);
        }


        [Test]
        public void ScopeIsBlank()
        {
            assertThatTheFamilyPolicyIs<BuildPolicy>();
        }


        [Test]
        public void ScopeIsBlank2()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, "");
            assertThatTheFamilyPolicyIs<BuildPolicy>();
        }


        [Test]
        public void ScopeIsSingleton()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, InstanceScope.Singleton.ToString());
            assertThatTheFamilyPolicyIs<SingletonPolicy>();
        }


        [Test]
        public void ScopeIsThreadLocal()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, InstanceScope.ThreadLocal.ToString());
            assertThatTheFamilyPolicyIs<ThreadLocalStoragePolicy>();
        }
    }
}