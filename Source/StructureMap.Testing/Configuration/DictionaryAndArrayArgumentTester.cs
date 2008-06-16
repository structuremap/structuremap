using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class DictionaryAndArrayArgumentTester
    {
        [Test]
        public void Read_in_a_dictionary_type_from_an_attribute_normalized_memento()
        {
            string xml = @"
<root>
    <dictionary>
        <Pair Key='color' Value='red'/>
        <Pair Key='state' Value='texas'/>
        <Pair Key='direction' Value='north'/>
    </dictionary>
</root>
";

            var element = DataMother.BuildDocument(xml).DocumentElement;
            element.SetAttribute("PluggedType", TypePath.GetAssemblyQualifiedName(typeof (ClassWithDictionary)));

            XmlAttributeInstanceMemento memento = new XmlAttributeInstanceMemento(element);

            Instance instance = memento.ReadInstance(new PluginGraph(), typeof (ClassWithDictionary));


            ClassWithDictionary theObject =
                (ClassWithDictionary) instance.Build(typeof(ClassWithDictionary), new BuildSession(new PluginGraph()));


            theObject.Dictionary["color"].ShouldEqual("red");
            theObject.Dictionary["state"].ShouldEqual("texas");
            theObject.Dictionary["direction"].ShouldEqual("north");
        }

        [Test]
        public void Read_in_a_dictionary_type_from_a_node_normalized_memento()
        {
            string xml = @"
<root>
    <Property Name='dictionary'>
        <Pair Key='color' Value='red'/>
        <Pair Key='state' Value='texas'/>
        <Pair Key='direction' Value='north'/>
    </Property>
</root>
";

            var element = DataMother.BuildDocument(xml).DocumentElement;
            element.SetAttribute("PluggedType", TypePath.GetAssemblyQualifiedName(typeof(ClassWithDictionary)));

            XmlNodeInstanceMemento memento = new XmlNodeInstanceMemento(element, "Type", "Key");

            Instance instance = memento.ReadInstance(new PluginGraph(), typeof(ClassWithDictionary));


            ClassWithDictionary theObject =
                (ClassWithDictionary)instance.Build(typeof(ClassWithDictionary), new BuildSession(new PluginGraph()));


            theObject.Dictionary["color"].ShouldEqual("red");
            theObject.Dictionary["state"].ShouldEqual("texas");
            theObject.Dictionary["direction"].ShouldEqual("north");
        }

        [Test]
        public void Read_in_a_class_with_primitive_arrays()
        {
            string xml = @"
<root>
    <numbers Values='1,2,3'/>
    <strings Values='1,2,3'/>
</root>
";

            var element = DataMother.BuildDocument(xml).DocumentElement;
            element.SetAttribute("PluggedType", TypePath.GetAssemblyQualifiedName(typeof(ClassWithStringAndIntArray)));

            XmlAttributeInstanceMemento memento = new XmlAttributeInstanceMemento(element);
            PluginGraph graph = new PluginGraph();
            Instance instance = memento.ReadInstance(graph, typeof(ClassWithStringAndIntArray));

            ClassWithStringAndIntArray theObject = (ClassWithStringAndIntArray) instance.Build(typeof (ClassWithStringAndIntArray),
                                                                                               new BuildSession(graph));

            theObject.Numbers.ShouldEqual(new int[] {1, 2, 3});
            theObject.Strings.ShouldEqual(new string[] {"1", "2", "3"});
        }
    }

    public class ClassWithStringAndIntArray
    {
        private int[] _numbers;
        private string[] _strings;

        public ClassWithStringAndIntArray(int[] numbers, string[] strings)
        {
            _numbers = numbers;
            _strings = strings;
        }

        public int[] Numbers
        {
            get { return _numbers; }
        }

        public string[] Strings
        {
            get { return _strings; }
        }
    }

    public class ClassWithDictionary
    {
        private readonly IDictionary<string, string> _dictionary;

        public ClassWithDictionary(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        public IDictionary<string, string> Dictionary
        {
            get { return _dictionary; }
        }
    }
}
