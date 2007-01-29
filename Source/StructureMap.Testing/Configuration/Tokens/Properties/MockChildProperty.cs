using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Testing.Configuration.Tokens.Properties
{
    public class MockChildProperty : ChildProperty
    {
        private bool _validateWasCalled = false;

        public MockChildProperty() : base(new PropertyDefinition())
        {
        }

        public override void Validate(IInstanceValidator validator)
        {
            _validateWasCalled = true;
        }

        public void Verify()
        {
            Assert.IsTrue(_validateWasCalled, "Validate was called");
        }
    }
}