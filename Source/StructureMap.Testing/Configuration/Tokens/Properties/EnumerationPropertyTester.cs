using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Testing.Configuration.Tokens.Properties
{
	[TestFixture]
	public class EnumerationPropertyTester
	{
		private PropertyDefinition _definition;

		[SetUp]
		public void SetUp()
		{
			_definition = new PropertyDefinition("order", "OrderEnum", PropertyDefinitionType.Constructor, ArgumentType.Enumeration);
			_definition.EnumerationValues = new string[]{"First", "Second", "Third"};
		}

		[Test]
		public void HappyPath()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
			memento.SetProperty("order", "First");

			EnumerationProperty property = new EnumerationProperty(_definition, memento);

			Assert.AreEqual(_definition, property.Definition);
			Assert.AreEqual("First", property.Value);

			Assert.AreEqual(0, property.Problems.Length);
		}

		[Test]
		public void AcceptVisitor()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
			memento.SetProperty("order", "First");

			EnumerationProperty property = new EnumerationProperty(_definition, memento);

			DynamicMock visitorMock = new DynamicMock(typeof(IConfigurationVisitor));
			visitorMock.Expect("HandleEnumerationProperty", property);
			property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);
			visitorMock.Verify();
		}


		[Test]
		public void PropertyIsMissing()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

			EnumerationProperty property = new EnumerationProperty(_definition, memento);

			Problem expected = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, string.Empty);
			
			Assert.AreEqual(new Problem[]{expected}, property.Problems);
		}

		[Test]
		public void MementoValueIsNotAnEnumerationValue()
		{
			MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
			memento.SetProperty("order", "Wrong");

			EnumerationProperty property = new EnumerationProperty(_definition, memento);

			Problem expected = new Problem(ConfigurationConstants.INVALID_ENUMERATION_VALUE, string.Empty);
			
			Assert.AreEqual(new Problem[]{expected}, property.Problems);
		}
	}


}
