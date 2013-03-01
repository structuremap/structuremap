using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing
{
    [TestFixture]
    public class DictionaryAndArrayArgumentTester
    {
        [Test]
        public void Read_in_a_class_with_primitive_arrays()
        {
            string xml = @"
<Instance>
    <numbers Values='1,2,3'/>
    <strings Values='1,2,3'/>
</Instance>
";

            XmlElement element = DataMother.BuildDocument(xml).DocumentElement;
            element.SetAttribute("PluggedType", typeof (ClassWithStringAndIntArray).AssemblyQualifiedName);

            var memento = new XmlAttributeInstanceMemento(element);
            var graph = new PluginGraph();
            Instance instance = memento.ReadInstance(new SimplePluginFactory(), typeof (ClassWithStringAndIntArray));

            var theObject = (ClassWithStringAndIntArray) instance.Build(typeof (ClassWithStringAndIntArray),
                                                                        BuildSession.ForPluginGraph(graph));

            theObject.Numbers.ShouldEqual(new[] {1, 2, 3});
            theObject.Strings.ShouldEqual(new[] {"1", "2", "3"});

            Debug.WriteLine(theObject.GetType().AssemblyQualifiedName);
        }

        [Test]
        public void Read_in_a_dictionary_type_from_an_attribute_normalized_memento()
        {
            string xml =
                @"
<root>
    <dictionary>
        <Pair Key='color' Value='red'/>
        <Pair Key='state' Value='texas'/>
        <Pair Key='direction' Value='north'/>
    </dictionary>
</root>
";

            XmlElement element = DataMother.BuildDocument(xml).DocumentElement;
            element.SetAttribute("PluggedType", typeof (ClassWithDictionary).AssemblyQualifiedName);

            var memento = new XmlAttributeInstanceMemento(element);

            Instance instance = memento.ReadInstance(new SimplePluginFactory(), typeof(ClassWithDictionary));


            var theObject =
                (ClassWithDictionary) instance.Build(typeof (ClassWithDictionary), BuildSession.Empty());


            theObject.Dictionary["color"].ShouldEqual("red");
            theObject.Dictionary["state"].ShouldEqual("texas");
            theObject.Dictionary["direction"].ShouldEqual("north");
        }
    }

    public class ClassWithStringAndIntArray
    {
        private readonly int[] _numbers;
        private readonly string[] _strings;

        public ClassWithStringAndIntArray(int[] numbers, string[] strings)
        {
            _numbers = numbers;
            _strings = strings;
        }

        public int[] Numbers { get { return _numbers; } }

        public string[] Strings { get { return _strings; } }
    }

    public class ClassWithDictionary
    {
        private readonly IDictionary<string, string> _dictionary;

        public ClassWithDictionary(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }

        public IDictionary<string, string> Dictionary { get { return _dictionary; } }
    }

    public class SimplePluginFactory : IPluginFactory
    {
        public Plugin PluginFor(string name)
        {
            return PluginCache.GetPlugin(Type.GetType(name));
        }
    }
}