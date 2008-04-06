using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class PluginGraphBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument("FullTesting.XML");

            XmlDocument doc = new XmlDocument();
            doc.Load("StructureMap.config");
            XmlNode node = doc.DocumentElement.SelectSingleNode("//StructureMap");

            ConfigurationParser parser = new ConfigurationParser(node);

            PluginGraphBuilder builder = new PluginGraphBuilder(parser);
            graph = builder.Build();
        }

        #endregion

        private PluginGraph graph;

        public PluginGraphBuilderTester()
        {
        }


        [Test]
        public void AssemblyDeployTargets()
        {
            AssemblyGraph assem1 = graph.Assemblies["StructureMap.Testing.Widget"];
            AssemblyGraph assem2 = graph.Assemblies["StructureMap.Testing.Widget2"];

            Assert.AreEqual(3, assem1.DeploymentTargets.Length);
            Assert.IsTrue(assem1.IsDeployed("Client"));
            Assert.IsTrue(assem1.IsDeployed("Test"));
            Assert.IsTrue(assem1.IsDeployed("Server"));
            Assert.IsFalse(assem1.IsDeployed("Remote"));

            Assert.AreEqual(1, assem2.DeploymentTargets.Length);
            Assert.IsFalse(assem2.IsDeployed("Client"));
            Assert.IsFalse(assem2.IsDeployed("Test"));
            Assert.IsFalse(assem2.IsDeployed("Server"));
            Assert.IsTrue(assem2.IsDeployed("Remote"));
        }

        [Test]
        public void BuildsInterceptionChain()
        {
            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("SingletonIntercepterTest.xml");

            PluginFamily family = pluginGraph.PluginFamilies[typeof (Rule)];

            Assert.AreEqual(1, family.InterceptionChain.Count);
            Assert.IsTrue(family.InterceptionChain[0] is SingletonInterceptor);

            // The PluginFamily for IWidget has no intercepters configured
            PluginFamily widgetFamily = pluginGraph.PluginFamilies[typeof (IWidget)];
            Assert.AreEqual(0, widgetFamily.InterceptionChain.Count);
        }

        [Test]
        public void CanDefinedSourceBuildMemento()
        {
            PluginFamily family = graph.PluginFamilies[typeof (IWidget)];

            MementoSource source = family.Source;
            InstanceMemento memento = source.GetMemento("Red");

            Assert.IsNotNull(memento);
        }

        [Test]
        public void CanImpliedInlineSourceBuildMemento()
        {
            PluginFamily family = graph.PluginFamilies[typeof (Rule)];

            MementoSource source = family.Source;
            InstanceMemento memento = source.GetMemento("Red");

            Assert.IsNotNull(memento);
        }


        [Test]
        public void CanImpliedNOTInlineSourceBuildMemento()
        {
            PluginFamily family = graph.PluginFamilies[typeof (Parent)];

            MementoSource source = family.Source;
            InstanceMemento memento = source.GetMemento("Jerry");

            Assert.IsNotNull(memento);
        }

        [Test]
        public void CorrectlyBuildAMementoSourceWhenDefinedInConfiguration()
        {
            PluginFamily family = graph.PluginFamilies[typeof (IWidget)];

            XmlFileMementoSource source = family.Source as XmlFileMementoSource;
            Assert.IsNotNull(source);
        }

        [Test]
        public void ExplicitPluginFamilyDefinitionOverridesImplicitDefinition()
        {
            PluginGraph pluginGraph = DataMother.GetPluginGraph("ExplicitPluginFamilyOverridesImplicitPluginFamily.xml");

            PluginFamily family = pluginGraph.PluginFamilies[typeof (GrandChild)];
            Assert.AreEqual(DefinitionSource.Explicit, family.DefinitionSource);
            Assert.AreEqual("Fred", family.DefaultInstanceKey);
        }


        /*   Expected Families: GrandChild, Child, Parent, WidgetMaker from attributes
		 *       Rule & Column by configuration
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 */


        [Test]
        public void GotPluginFamiliesThatAreDefinedInConfigXml()
        {
            Assert.IsNotNull(graph.PluginFamilies[typeof (Rule)]);
            Assert.IsNotNull(graph.PluginFamilies[typeof (Column)]);

            PluginFamily family = graph.PluginFamilies[typeof (Rule)];
            Assert.AreEqual(DefinitionSource.Explicit, family.DefinitionSource);
        }

        [Test]
        public void GotPluginFamiliesThatAreMarkedByAttributes()
        {
            Assert.IsNotNull(graph.PluginFamilies[typeof (GrandChild)]);
            Assert.IsNotNull(graph.PluginFamilies[typeof (Child)]);
            Assert.IsNotNull(graph.PluginFamilies[typeof (Parent)]);
            Assert.IsNotNull(graph.PluginFamilies[typeof (WidgetMaker)]);

            PluginFamily family = graph.PluginFamilies[typeof (Child)];
            Assert.AreEqual(DefinitionSource.Implicit, family.DefinitionSource);
        }

        [Test]
        public void GotPluginSomeFamilies()
        {
            Assert.IsNotNull(graph.PluginFamilies);
            Assert.IsTrue(graph.PluginFamilies.Count > 0);

            foreach (PluginFamily family in graph.PluginFamilies)
            {
                Assert.IsNotNull(family);
            }
        }


        [Test]
        public void GotPluginsThatAreMarkedAsPluggable()
        {
            PluginFamily pluginFamily = graph.PluginFamilies[typeof (IWidget)];
            Plugin plugin = pluginFamily.Plugins[typeof (ColorWidget)];
            Assert.IsNotNull(plugin);
        }


        [Test]
        public void GotPluginThatIsAddedInConfigXml()
        {
            PluginFamily family = graph.PluginFamilies[typeof (IWidget)];
            Plugin plugin = family.Plugins[typeof (NotPluggableWidget)];
            Assert.IsNotNull(plugin);
            Assert.AreEqual("NotPluggable", plugin.ConcreteKey);

            // Just for fun, test with InstanceFactory too.
            InstanceFactory factory = new InstanceFactory(family, true);
            MemoryInstanceMemento memento = new MemoryInstanceMemento("NotPluggable", string.Empty);
            memento.SetProperty("name", "DorothyTheDinosaur");

            IWidget widget = (IWidget) factory.GetInstance(memento);
            Assert.IsNotNull(widget);
        }


        [Test]
        public void GotRightNumberOfPluginsForIWidget()
        {
            PluginFamily pluginFamily = graph.PluginFamilies[typeof (IWidget)];
            Assert.AreEqual(5, pluginFamily.Plugins.Count, "Should be 5 total");
        }


        [Test]
        public void GotRightNumberOfPluginsForMultipleAssemblies()
        {
            PluginFamily pluginFamily = graph.PluginFamilies[typeof (Rule)];
            Assert.AreEqual(5, pluginFamily.Plugins.Count, "Should be 5 total");
        }

        [Test]
        public void PicksUpTheDefaultProfileAttributeOnTheStructureMapNodeAndSetsTheProfile()
        {
            InstanceDefaultManager defaultManager =
                DataMother.GetDiagnosticPluginGraph("DefaultProfileConfig.xml").DefaultManager;

            Assert.AreEqual("Green", defaultManager.DefaultProfileName);
        }

        [Test]
        public void ReadScopeFromXmlConfiguration()
        {
            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("ScopeInFamily.xml");
            PluginFamily family = pluginGraph.PluginFamilies[typeof (Column)];

            Assert.AreEqual(1, family.InterceptionChain.Count);
            Assert.IsTrue(family.InterceptionChain[0] is ThreadLocalStorageInterceptor);

            // The PluginFamily for IWidget has no intercepters configured
            PluginFamily widgetFamily = pluginGraph.PluginFamilies[typeof (IWidget)];
            Assert.AreEqual(1, widgetFamily.InterceptionChain.Count);
            Assert.IsTrue(widgetFamily.InterceptionChain[0] is HttpContextItemInterceptor);
        }

        [Test]
        public void SetsTheDefaultInstanceKey()
        {
            PluginFamily family = graph.PluginFamilies[typeof (IWidget)];
            Assert.AreEqual("Red", family.DefaultInstanceKey);
        }
    }
}