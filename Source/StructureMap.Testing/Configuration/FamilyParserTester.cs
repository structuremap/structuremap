using System;
using System.Xml;
using NMock;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Graph;
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
            _builderMock = new DynamicMock(typeof (IGraphBuilder));
            _parser =
                new FamilyParser((IGraphBuilder) _builderMock.MockInstance,
                                 new XmlMementoCreator(XmlMementoStyle.NodeNormalized, XmlConstants.TYPE_ATTRIBUTE,
                                                       XmlConstants.ATTRIBUTE_STYLE));

            _document = new XmlDocument();
            _document.LoadXml("<PluginFamily />");
            _familyElement = _document.DocumentElement;

            Type type = typeof (IGateway);
            _typePath = new TypePath(type);

            TypePath.WriteTypePathToXmlElement(type, _familyElement);
        }

        #endregion

        private DynamicMock _builderMock;
        private FamilyParser _parser;
        private XmlDocument _document;
        private XmlElement _familyElement;
        private TypePath _typePath;


        [Test]
        public void ScopeIsBlank()
        {
            _builderMock.Expect("AddPluginFamily", _typePath, string.Empty, InstanceScope.PerRequest);

            _parser.ParseFamily(_familyElement);

            _builderMock.Verify();
        }


        [Test]
        public void ScopeIsBlank2()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, "");
            _builderMock.Expect("AddPluginFamily", _typePath, string.Empty, InstanceScope.PerRequest);

            _parser.ParseFamily(_familyElement);

            _builderMock.Verify();
        }


        [Test]
        public void ScopeIsSingleton()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, InstanceScope.Singleton.ToString());
            _builderMock.Expect("AddPluginFamily", _typePath, string.Empty, InstanceScope.Singleton);

            _parser.ParseFamily(_familyElement);

            _builderMock.Verify();
        }


        [Test]
        public void ScopeIsThreadLocal()
        {
            _familyElement.SetAttribute(XmlConstants.SCOPE_ATTRIBUTE, InstanceScope.ThreadLocal.ToString());
            _builderMock.Expect("AddPluginFamily", _typePath, string.Empty, InstanceScope.ThreadLocal);

            _parser.ParseFamily(_familyElement);

            _builderMock.Verify();
        }
    }
}