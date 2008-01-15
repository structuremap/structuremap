using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.Tokens
{
    [TestFixture]
    public class GraphObjectTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ObjectMother.Reset();

            _visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            _visitor = (IConfigurationVisitor) _visitorMock.MockInstance;
            _report = ObjectMother.Report();
        }

        #endregion

        private DynamicMock _visitorMock;
        private IConfigurationVisitor _visitor;
        private PluginGraphReport _report;

        [Test]
        public void AssemblyTokenAcceptVisitor()
        {
            AssemblyToken token = new AssemblyToken();
            _visitorMock.Expect("HandleAssembly", token);

            token.AcceptVisitor(_visitor);

            _visitorMock.Verify();
        }

        [Test]
        public void AssemblyTokenChildren()
        {
            AssemblyToken token = new AssemblyToken();
            Assert.AreEqual(new GraphObject[0], token.Children);
        }

        [Test]
        public void FamilyTokenAcceptVisitor()
        {
            FamilyToken token = new FamilyToken(new TypePath(typeof (IGateway)), null, new string[0]);
            _visitorMock.Expect("HandleFamily", token);

            token.AcceptVisitor(_visitor);

            _visitorMock.Verify();
        }

        [Test]
        public void FamilyTokenChildren()
        {
            foreach (FamilyToken family in _report.Families)
            {
                int childCount = family.Plugins.Length + family.Interceptors.Length + family.Instances.Length;
                if (family.SourceInstance != null)
                {
                    childCount++;
                }

                Assert.AreEqual(childCount, family.Children.Length, family.PluginTypeName);
            }
        }


        [Test]
        public void InstanceAcceptVisitor()
        {
            InstanceToken token = new InstanceToken();
            _visitorMock.Expect("HandleInstance", token);
            token.AcceptVisitor(_visitor);
            _visitorMock.Verify();
        }

        [Test]
        public void InstanceChildren()
        {
            foreach (FamilyToken family in _report.Families)
            {
                foreach (InstanceToken instance in family.Instances)
                {
                    Assert.AreEqual(instance.Properties, instance.Children);
                }
            }
        }

        [Test]
        public void PluginAcceptVisitor()
        {
            PluginToken token = new PluginToken();
            _visitorMock.Expect("HandlePlugin", token);
            token.AcceptVisitor(_visitor);
            _visitorMock.Verify();
        }

        [Test]
        public void PluginChildren()
        {
            foreach (FamilyToken family in _report.Families)
            {
                foreach (PluginToken plugin in family.Plugins)
                {
                    Assert.AreEqual(plugin.Properties, plugin.Children);
                }
            }
        }


        [Test]
        public void PropertyDefinitionAcceptVisitor()
        {
            PropertyDefinition token = new PropertyDefinition();
            _visitorMock.Expect("HandlePropertyDefinition", token);
            token.AcceptVisitor(_visitor);
            _visitorMock.Verify();
        }
    }
}