using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ArrayConstructorTester
    {
        #region Setup/Teardown

        [TearDown]
        public void TearDown()
        {
            ObjectMother.Reset();
        }

        #endregion

        [Test]
        public void BuildDecisionWithRules()
        {
            // May need to add a Plugin for Decision to Decision labelled "Default"

            DataMother.WriteDocument("FullTesting.XML");
            DataMother.WriteDocument("Array.xml");
            DataMother.WriteDocument("ObjectMother.config");

            Registry registry = new Registry();
            XmlMementoSource source = new XmlFileMementoSource("Array.xml", string.Empty, "Decision");
            registry.ForRequestedType<Decision>().AddInstancesFrom(source);

            PluginGraphBuilder builder =
                new PluginGraphBuilder(new ConfigurationParser[] {ConfigurationParser.FromFile("ObjectMother.config")},
                                       new Registry[] {registry}, new GraphLog());

            PluginGraph graph = builder.Build();

            Container manager = new Container(graph);

            Decision d1 = manager.GetInstance<Decision>("RedBlue");
            Assert.IsNotNull(d1);
            Assert.AreEqual(2, d1.Rules.Length, "2 Rules");

            Decision d2 = (Decision) manager.GetInstance(typeof (Decision), "GreenBluePurple");
            Assert.IsNotNull(d2);
            Assert.AreEqual(3, d2.Rules.Length, "3 Rules");


            Assert.AreEqual("Green", ((ColorRule) d2.Rules[0]).Color);
            Assert.AreEqual("Blue", ((ColorRule) d2.Rules[1]).Color);
            Assert.AreEqual("Purple", ((ColorRule) d2.Rules[2]).Color);
        }
    }
}