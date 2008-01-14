using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Testing.Configuration.Tokens.Properties
{
    [TestFixture]
    public class PrimitivePropertyTester
    {
        [Test]
        public void HappyPath()
        {
            string theProperty = "Prop1";
            string theValue = "red";

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty(theProperty, theValue);

            PropertyDefinition definition =
                new PropertyDefinition(theProperty, typeof (string),
                                       PropertyDefinitionType.Constructor, ArgumentType.Primitive);

            PrimitiveProperty property = new PrimitiveProperty(definition, memento);
            Assert.AreEqual(definition, property.Definition);
            Assert.AreEqual(theValue, property.Value);

            Assert.AreEqual(0, property.Problems.Length);
        }


        [Test]
        public void InvalidCast()
        {
            string theProperty = "Prop1";
            string theValue = "red";

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty(theProperty, theValue);

            PropertyDefinition definition =
                new PropertyDefinition(theProperty, typeof (double),
                                       PropertyDefinitionType.Constructor, ArgumentType.Primitive);

            PrimitiveProperty property = new PrimitiveProperty(definition, memento);

            Problem expected = new Problem(ConfigurationConstants.INVALID_PROPERTY_CAST, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }


        [Test]
        public void PropertyIsMissing()
        {
            string theProperty = "Prop1";

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

            PropertyDefinition definition =
                new PropertyDefinition(theProperty, typeof (string),
                                       PropertyDefinitionType.Constructor, ArgumentType.Primitive);

            PrimitiveProperty property = new PrimitiveProperty(definition, memento);
            Assert.AreEqual(definition, property.Definition);

            Problem expected = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }

        [Test]
        public void AcceptVisitor()
        {
            string theProperty = "Prop1";

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

            PropertyDefinition definition =
                new PropertyDefinition(theProperty, typeof (string),
                                       PropertyDefinitionType.Constructor, ArgumentType.Primitive);

            PrimitiveProperty property = new PrimitiveProperty(definition, memento);

            DynamicMock visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            visitorMock.Expect("HandlePrimitiveProperty", property);
            property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);
            visitorMock.Verify();
        }
    }
}