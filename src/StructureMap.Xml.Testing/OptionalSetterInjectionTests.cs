using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing;
using StructureMap.Testing.Widget;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing
{

    [TestFixture, Ignore("Until we rewrite the Xml config")]
    public class OptionalSetterInjectionTests
    {
        [Test]
        public void read_instance_from_xml_with_optional_setter_defined()
        {
            Debug.WriteLine(typeof(ClassWithDependency).AssemblyQualifiedName);

            PluginGraph graph =
                DataMother.BuildPluginGraphFromXml(
                    @"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance 
        PluginType='StructureMap.Xml.Testing.ClassWithDependency, StructureMap.Xml.Testing' 
        PluggedType='StructureMap.Xml.Testing.ClassWithDependency, StructureMap.Xml.Testing'>
        
        <Rule PluggedType='StructureMap.Testing.Widget.ColorRule, StructureMap.Testing.Widget' color='Red' />
    </DefaultInstance>
</StructureMap>

");

            var container = new Container(graph);

            container.GetInstance<ClassWithDependency>().Rule.IsType<ColorRule>().Color.ShouldEqual("Red");
        }


        [Test]
        public void read_instance_from_xml_with_optional_setter_not_defined()
        {
            Debug.WriteLine(typeof(ClassWithDependency).AssemblyQualifiedName);

            PluginGraph graph =
                DataMother.BuildPluginGraphFromXml(
                    @"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance 
        PluginType='StructureMap.Xml.Testing.ClassWithDependency, StructureMap.Xml.Testing' 
        PluggedType='StructureMap.Xml.Testing.ClassWithDependency, StructureMap.Xml.Testing'>

    </DefaultInstance>
</StructureMap>

");

            var container = new Container(graph);

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeNull();
        }

    }

    public class ClassWithDependency
    {
        public Rule Rule { get; set; }
        public Rule[] Rules { get; set; }
    }

}