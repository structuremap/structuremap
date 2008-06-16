using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class PrimitiveeArrayReaderTester
    {
        [Test]
        public void CanProcess_an_array_of_primitive_types()
        {
            PrimitiveArrayReader reader = new PrimitiveArrayReader();

            reader.CanProcess(typeof(string)).ShouldBeFalse();
            reader.CanProcess(typeof(IWidget[])).ShouldBeFalse();

            reader.CanProcess(typeof(string[])).ShouldBeTrue();
            reader.CanProcess(typeof(int[])).ShouldBeTrue();
            reader.CanProcess(typeof(double[])).ShouldBeTrue();
        }

        private object parseNode(string xml, Type pluginType)
        {
            XmlElement element = DataMother.BuildDocument(xml).DocumentElement;
            PrimitiveArrayReader reader = new PrimitiveArrayReader();
            Instance instance =  reader.Read(element, pluginType);

            return instance.Build(pluginType, new StubBuildSession());
        }

        [Test]
        public void Parse_a_string_array_with_the_default_concatenator()
        {
            parseNode("<node Values='a,b,c,d'></node>", typeof (string[])).ShouldEqual(new string[] {"a", "b", "c", "d"});
        }


        [Test]
        public void Parse_a_string_array_with_overridden_concatenator()
        {
            parseNode("<node Values='a,b,c,d' Concatenator=';'></node>", typeof(string[])).ShouldEqual(new string[] { "a,b,c,d" });
            parseNode("<node Values='a;b;c;d' Concatenator=';'></node>", typeof(string[])).ShouldEqual(new string[] { "a", "b", "c", "d" });
        }


        [Test]
        public void Parse_a_string_array_with_the_default_concatenator_and_deal_with_leading_or_trailing_spaces()
        {
            parseNode("<node Values='a , b,c,d'></node>", typeof(string[])).ShouldEqual(new string[] { "a", "b", "c", "d" });
        }


        [Test]
        public void Parse_an_int_array_with_the_default_concatenator()
        {
            parseNode("<node Values='1,2,3,4'></node>", typeof(int[])).ShouldEqual(new int[] { 1, 2, 3, 4 });
        }
    }
}
