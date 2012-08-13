using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class OverwritingOfPrimitiveProperties
    {
        public const string XML_FILENAME = "BUG_OverwritingOfPrimitiveProperties.xml";

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument(XML_FILENAME);
        }

        [Test]
        public void Test()
        {   
            ObjectFactory.Initialize(x =>
            {
                x.AddConfigurationFromXmlFile(XML_FILENAME);
            });

            IFooWithPrimitives foo = ObjectFactory.GetInstance<IFooWithPrimitives>();
            Assert.IsNotNull(foo);
            Assert.AreEqual("Test123", foo.TestValue);
            Assert.IsTrue(foo.IsTest);
        }
    }

    public interface IFooWithPrimitives
    {
        bool IsTest { get; }
        string TestValue { get; }
    }

    public class FooWithPrimitives : IFooWithPrimitives
    {
        public FooWithPrimitives(String name) 
        {
            _testString = name;
        }

        private string _testString = string.Empty;
        private bool _testBool = true;

        public bool IsTest
        {
            get { return _testBool; }
            set { _testBool = value; }
        }

        public string TestValue
        {
            get { return _testString; }
            set { _testString = value; }
        }

    }
}
