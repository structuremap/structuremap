using System;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ConfigurationParserCollectionTester
    {
        private ConfigurationParserCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new ConfigurationParserCollection();
        }


        public void assertParserIdList(params string[] expected)
        {
            Array.Sort(expected);
            ConfigurationParser[] parsers = _collection.GetParsers();
            Converter<ConfigurationParser, string> converter = delegate(ConfigurationParser parser) { return parser.Id; };

            string[] actuals = Array.ConvertAll<ConfigurationParser, string>(parsers, converter);
            Array.Sort(actuals);

            Assert.AreEqual(expected, actuals);
        }

        [Test]
        public void SimpleDefaultConfigurationParser()
        {
            _collection.UseDefaultFile = true;
            assertParserIdList("Main");
        }

        [Test]
        public void DoNotUseDefaultAndUseADifferentFile()
        {
            _collection.UseDefaultFile = false;
            DataMother.WriteDocument("GenericsTesting.xml");

            _collection.IncludeFile("GenericsTesting.xml");
            assertParserIdList("Generics");
        }

        [Test, ExpectedException(typeof(StructureMapException), "StructureMap Exception Code:  100\nExpected file \"StructureMap.config\" cannot be opened at DoesNotExist.xml")]
        public void FileDoesNotExist()
        {
            _collection.UseDefaultFile = false;
            _collection.IncludeFile("DoesNotExist.xml");
            _collection.GetParsers();
        }

        [Test]
        public void GetIncludes()
        {
            DataMother.WriteDocument("Include1.xml");
            DataMother.WriteDocument("Include2.xml");
            DataMother.WriteDocument("Master.xml");

            _collection.UseDefaultFile = false;
            _collection.IncludeFile("Master.xml");

            assertParserIdList("Include1", "Include2", "Master");
        }

        [Test]
        public void UseDefaultIsTrueUponConstruction()
        {
            Assert.IsTrue(_collection.UseDefaultFile);
        }

        [Test]
        public void GetMultiples()
        {
            DataMother.WriteDocument("Include1.xml");
            DataMother.WriteDocument("Include2.xml");
            DataMother.WriteDocument("Master.xml");

            DataMother.WriteDocument("GenericsTesting.xml");

            _collection.IncludeFile("GenericsTesting.xml");
            _collection.UseDefaultFile = true;
            _collection.IncludeFile("Master.xml");

            assertParserIdList("Generics", "Include1", "Include2", "Main", "Master");
        }
    }
}
