using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Testing.Configuration.Tokens
{
	public class MockProperty : Property
	{
		private readonly string _propertyName;
		private bool _validateWasCalled = false;

		public MockProperty(string propertyName)
			: base(new PropertyDefinition(propertyName, typeof(string), 
					PropertyDefinitionType.Constructor, ArgumentType.Primitive))
		{
			_propertyName = propertyName;
		}

		public override void Validate(IInstanceValidator validator)
		{
			_validateWasCalled = true;
		}

		public void Verify()
		{
			Assert.IsTrue(_validateWasCalled, "Validate(IInstanceValidator) was called");
		}
	}
}
