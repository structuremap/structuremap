using NUnit.Framework;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class ArrayConstructorTester
    {
        [Test]
        public void BuildDecisionWithRules()
        {
            // May need to add a Plugin for Decision to Decision labelled "Default"
            DataMother.WriteDocument("FullTesting.XML");
            DataMother.WriteDocument("Array.xml");
            DataMother.WriteDocument("ObjectMother.config");

            XmlMementoSource source = new XmlFileMementoSource("Array.xml", string.Empty, "Decision");

            var container = new Container(x =>
            {
                x.AddConfigurationFromXmlFile("ObjectMother.config");
                x.ForRequestedType<Decision>().AddInstancesFrom(source);
            });

            container.GetInstance<Decision>("RedBlue").Rules.Length.ShouldEqual(2);

            var d2 = container.GetInstance(typeof (Decision), "GreenBluePurple").ShouldBeOfType<Decision>();
            d2.Rules.Length.ShouldEqual(3);

            d2.Rules[0].ShouldBeOfType<ColorRule>().Color.ShouldEqual("Green");
            d2.Rules[1].ShouldBeOfType<ColorRule>().Color.ShouldEqual("Blue");
            d2.Rules[2].ShouldBeOfType<ColorRule>().Color.ShouldEqual("Purple");
        }
    }
}