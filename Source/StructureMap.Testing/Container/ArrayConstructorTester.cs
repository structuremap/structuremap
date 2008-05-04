using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
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
            DataMother.WriteDocument("FullTesting.XML");
            DataMother.WriteDocument("Array.xml");

            PluginGraph graph = DataMother.GetPluginGraph("ObjectMother.config");

            XmlMementoSource source = new XmlFileMementoSource("Array.xml", string.Empty, "Decision");

            PluginFamily family = graph.FindFamily(typeof(Decision));
            family.AddMementoSource(source);            

            family.Plugins.Add(typeof (Decision), "Default");
            
            graph.Seal();

            InstanceManager manager = new InstanceManager(graph);

            Decision d1 = (Decision) manager.CreateInstance(typeof (Decision), "RedBlue");
            Assert.IsNotNull(d1);
            Assert.AreEqual(2, d1.Rules.Length, "2 Rules");

            Decision d2 = (Decision) manager.CreateInstance(typeof (Decision), "GreenBluePurple");
            Assert.IsNotNull(d2);
            Assert.AreEqual(3, d2.Rules.Length, "3 Rules");


            Assert.AreEqual("Green", ((ColorRule) d2.Rules[0]).Color);
            Assert.AreEqual("Blue", ((ColorRule) d2.Rules[1]).Color);
            Assert.AreEqual("Purple", ((ColorRule) d2.Rules[2]).Color);
        }
    }
}