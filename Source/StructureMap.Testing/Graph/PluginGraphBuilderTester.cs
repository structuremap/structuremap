using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
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

        [Test]
        public void BuildsInterceptionChain()
        {
            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("SingletonIntercepterTest.xml");

            PluginFamily family = pluginGraph.FindFamily(typeof (Rule));
            Assert.IsInstanceOfType(typeof (SingletonPolicy), family.Policy);

            // The PluginFamily for IWidget has no intercepters configured
            PluginFamily widgetFamily = pluginGraph.FindFamily(typeof (IWidget));
            Assert.IsInstanceOfType(typeof (BuildPolicy), widgetFamily.Policy);
        }

        [Test]
        public void CanDefinedSourceBuildMemento()
        {
            PluginFamily family = graph.FindFamily(typeof (IWidget));

            InstanceMemento memento = family.GetMemento("Red");

            Assert.IsNotNull(memento);
        }

        [Test]
        public void CanImpliedInlineSourceBuildMemento()
        {
            PluginFamily family = graph.FindFamily(typeof (Rule));

            InstanceMemento memento = family.GetMemento("Red");

            Assert.IsNotNull(memento);
        }


        [Test]
        public void CanImpliedNOTInlineSourceBuildMemento()
        {
            PluginFamily family = graph.FindFamily(typeof (Parent));

            InstanceMemento memento = family.GetMemento("Jerry");

            Assert.IsNotNull(memento);
        }

        [Test]
        public void ExplicitPluginFamilyDefinitionOverridesImplicitDefinition()
        {
            PluginGraph pluginGraph = DataMother.GetPluginGraph("ExplicitPluginFamilyOverridesImplicitPluginFamily.xml");

            PluginFamily family = pluginGraph.FindFamily(typeof (GrandChild));
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
            Assert.IsNotNull(graph.FindFamily(typeof (Rule)));
            Assert.IsNotNull(graph.FindFamily(typeof (Column)));

            PluginFamily family = graph.FindFamily(typeof (Rule));
        }

        [Test]
        public void GotPluginFamiliesThatAreMarkedByAttributes()
        {
            Assert.IsNotNull(graph.FindFamily(typeof (GrandChild)));
            Assert.IsNotNull(graph.FindFamily(typeof (Child)));
            Assert.IsNotNull(graph.FindFamily(typeof (Parent)));
            Assert.IsNotNull(graph.FindFamily(typeof (WidgetMaker)));

            PluginFamily family = graph.FindFamily(typeof (Child));
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
            Registry registry = new Registry();
            registry.ScanAssemblies().IncludeAssemblyContainingType<IWidget>();
            registry.BuildInstancesOf<IWidget>();
            PluginGraph pluginGraph = registry.Build();


            PluginFamily pluginFamily = pluginGraph.FindFamily(typeof (IWidget));
            Plugin plugin = pluginFamily.Plugins[typeof (ColorWidget)];
            Assert.IsNotNull(plugin);
        }


        [Test]
        public void GotPluginThatIsAddedInConfigXml()
        {
            PluginFamily family = graph.FindFamily(typeof (IWidget));
            Plugin plugin = family.Plugins[typeof (NotPluggableWidget)];
            Assert.IsNotNull(plugin);
            Assert.AreEqual("NotPluggable", plugin.ConcreteKey);
        }


        [Test]
        public void GotRightNumberOfPluginsForIWidget()
        {
            PluginFamily pluginFamily = graph.FindFamily(typeof (IWidget));
            Assert.AreEqual(5, pluginFamily.Plugins.Count, "Should be 5 total");
        }


        [Test]
        public void GotRightNumberOfPluginsForMultipleAssemblies()
        {
            PluginFamily pluginFamily = graph.FindFamily(typeof (Rule));
            Assert.AreEqual(5, pluginFamily.Plugins.Count, "Should be 5 total");
        }

        [Test]
        public void PicksUpTheDefaultProfileAttributeOnTheStructureMapNodeAndSetsTheProfile()
        {
            PluginGraph graph = DataMother.GetDiagnosticPluginGraph("DefaultProfileConfig.xml");
            Assert.AreEqual("Green", graph.ProfileManager.DefaultProfileName);
        }


        [Test]
        public void SetsTheDefaultInstanceKey()
        {
            PluginFamily family = graph.FindFamily(typeof (IWidget));
            Assert.AreEqual("Red", family.DefaultInstanceKey);
        }
    }
}