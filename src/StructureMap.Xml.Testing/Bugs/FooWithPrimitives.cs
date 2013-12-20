using System;

namespace StructureMap.Xml.Testing.Bugs
{
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
