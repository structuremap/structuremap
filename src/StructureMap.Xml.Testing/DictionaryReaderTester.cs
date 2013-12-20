using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using StructureMap.Configuration.Xml;
using StructureMap.Pipeline;
using StructureMap.Testing;
using StructureMap.Testing.Pipeline;
using StructureMap.Xml.Testing.TestData;

namespace StructureMap.Xml.Testing
{
    [TestFixture]
    public class DictionaryReaderTester
    {
        [Test]
        public void Can_process_a_dictionary()
        {
            var reader = new DictionaryReader();
            reader.CanProcess(typeof (IDictionary<string, string>)).ShouldBeTrue();
            reader.CanProcess(typeof (IDictionary<int, string>)).ShouldBeTrue();
            reader.CanProcess(typeof (IDictionary<string, int>)).ShouldBeTrue();
            reader.CanProcess(typeof (Dictionary<string, string>)).ShouldBeTrue();
            reader.CanProcess(typeof (Dictionary<int, string>)).ShouldBeTrue();
            reader.CanProcess(typeof (Dictionary<string, int>)).ShouldBeTrue();
        }

        [Test]
        public void Can_process_NameValueCollection()
        {
            new DictionaryReader().CanProcess(typeof (NameValueCollection)).ShouldBeTrue();
        }

        [Test]
        public void Read_a_string_int_dictionary2()
        {
            string xml =
                @"
<node>
    <Pair Key='color' Value='1'/>
    <Pair Key='state' Value='2'/>
    <Pair Key='direction' Value='3'/>
</node>
";

            Instance instance = new DictionaryReader().Read(DataMother.BuildDocument(xml).DocumentElement,
                                                            typeof (Dictionary<string, int>));
            instance.ShouldBeOfType(typeof (SerializedInstance));

            var collection =
                (Dictionary<string, int>) instance.Build(typeof (Dictionary<string, int>), new StubBuildSession());

            collection["color"].ShouldEqual(1);
            collection["state"].ShouldEqual(2);
            collection["direction"].ShouldEqual(3);
        }

        [Test]
        public void Read_a_string_string_dictionary()
        {
            string xml =
                @"
<node>
    <Pair Key='color' Value='red'/>
    <Pair Key='state' Value='texas'/>
    <Pair Key='direction' Value='north'/>
</node>
";

            Instance instance = new DictionaryReader().Read(DataMother.BuildDocument(xml).DocumentElement,
                                                            typeof (IDictionary<string, string>));
            instance.ShouldBeOfType(typeof (SerializedInstance));

            var collection =
                (IDictionary<string, string>)
                instance.Build(typeof (IDictionary<string, string>), new StubBuildSession());

            collection["color"].ShouldEqual("red");
            collection["state"].ShouldEqual("texas");
            collection["direction"].ShouldEqual("north");
        }


        [Test]
        public void Read_a_string_string_dictionary2()
        {
            string xml =
                @"
<node>
    <Pair Key='color' Value='red'/>
    <Pair Key='state' Value='texas'/>
    <Pair Key='direction' Value='north'/>
</node>
";

            Instance instance = new DictionaryReader().Read(DataMother.BuildDocument(xml).DocumentElement,
                                                            typeof (Dictionary<string, string>));
            instance.ShouldBeOfType(typeof (SerializedInstance));

            var collection =
                (Dictionary<string, string>) instance.Build(typeof (Dictionary<string, string>), new StubBuildSession());

            collection["color"].ShouldEqual("red");
            collection["state"].ShouldEqual("texas");
            collection["direction"].ShouldEqual("north");
        }

        [Test]
        public void Read_an_instance_for_NameValueCollection()
        {
            string xml =
                @"
<node>
    <Pair Key='color' Value='red'/>
    <Pair Key='state' Value='texas'/>
    <Pair Key='direction' Value='north'/>
</node>
";

            Instance instance = new DictionaryReader().Read(DataMother.BuildDocument(xml).DocumentElement,
                                                            typeof (NameValueCollection));
            instance.ShouldBeOfType(typeof (SerializedInstance));

            var collection = (NameValueCollection) instance.Build(typeof (NameValueCollection), new StubBuildSession());

            collection["color"].ShouldEqual("red");
            collection["state"].ShouldEqual("texas");
            collection["direction"].ShouldEqual("north");
        }


        [Test]
        public void Read_an_int_string_dictionary()
        {
            string xml =
                @"
<node>
    <Pair Key='1' Value='red'/>
    <Pair Key='2' Value='texas'/>
    <Pair Key='3' Value='north'/>
</node>
";

            Instance instance = new DictionaryReader().Read(DataMother.BuildDocument(xml).DocumentElement,
                                                            typeof (IDictionary<int, string>));
            instance.ShouldBeOfType(typeof (SerializedInstance));

            var collection =
                (IDictionary<int, string>) instance.Build(typeof (IDictionary<int, string>), new StubBuildSession());

            collection[1].ShouldEqual("red");
            collection[2].ShouldEqual("texas");
            collection[3].ShouldEqual("north");
        }
    }
}