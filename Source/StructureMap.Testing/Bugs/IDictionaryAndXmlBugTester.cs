using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class IDictionaryAndXmlBugTester
    {
        [Test]
        public void read_a_dictionary_from_configuration()
        {
            string className = typeof (ClassWithDictionaryInCtor).AssemblyQualifiedName;
            string xml =
                @"
<StructureMap>
    <DefaultInstance PluginType='{0}' PluggedType='{0}'>
        <Property Name='values'>
            <Pair Key='color' Value='red'/>
            <Pair Key='state' Value='texas'/>
            <Pair Key='direction' Value='north'/>
        </Property>
    </DefaultInstance>
</StructureMap>
"
                    .ToFormat(className);

            Container container = DataMother.BuildContainerForXml(xml);

            var dict = container.GetInstance<ClassWithDictionaryInCtor>();
            dict.Values["color"].ShouldEqual("red");
        }


        [Test]
        public void
            read_a_dictionary_from_configuration_in_attribute_style_that_is_missing_a_dictionary_and_blow_up_with_good_message
            ()
        {
            string className = typeof (ClassWithDictionaryInCtor).AssemblyQualifiedName;
            string xml =
                @"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance PluginType='{0}' PluggedType='{0}'></DefaultInstance>
</StructureMap>
"
                    .ToFormat(className);


            try
            {
                Container container = DataMother.BuildContainerForXml(xml);
                container.GetInstance<ClassWithDictionaryInCtor>();
            }
            catch (StructureMapException e)
            {
                e.ErrorCode.ShouldEqual(202);
            }
        }


        [Test]
        public void
            read_a_dictionary_from_configuration_in_attribute_style_that_is_missing_a_dictionary_for_an_optional_setter()
        {
            string className = typeof (ClassWithDictionaryProperty).AssemblyQualifiedName;
            string xml =
                @"
<StructureMap MementoStyle='Attribute'>
    <DefaultInstance PluginType='{0}' PluggedType='{0}'></DefaultInstance>
</StructureMap>
"
                    .ToFormat(className);


            Container container = DataMother.BuildContainerForXml(xml);
            container.GetInstance<ClassWithDictionaryProperty>().ShouldNotBeNull();
        }

        [Test]
        public void read_a_dictionary_from_configuration_when_the_property_is_missing_on_mandatory_setter()
        {
            string className = typeof (ClassWithDictionaryProperty2).AssemblyQualifiedName;
            string xml =
                @"
<StructureMap>
    <DefaultInstance PluginType='{0}' PluggedType='{0}'></DefaultInstance>
</StructureMap>
"
                    .ToFormat(className);


            try
            {
                Container container = DataMother.BuildContainerForXml(xml);
                container.GetInstance<ClassWithDictionaryProperty2>();
            }
            catch (StructureMapException e)
            {
                e.ErrorCode.ShouldEqual(202);
            }
        }

    }

    public class ClassWithDictionaryInCtor
    {
        private readonly IDictionary<string, string> _values;

        public ClassWithDictionaryInCtor(IDictionary<string, string> values)
        {
            _values = values;
        }

        public IDictionary<string, string> Values
        {
            get { return _values; }
        }
    }

    public class ClassWithDictionaryProperty
    {
        public IDictionary<string, string> Values { get; set; }
    }

    public class ClassWithDictionaryProperty2
    {
        [SetterProperty]
        public IDictionary<string, string> Values { get; set; }
    }
}