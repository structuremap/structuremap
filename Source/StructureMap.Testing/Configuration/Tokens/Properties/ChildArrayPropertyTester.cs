using System;
using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.Tokens.Properties
{
    [TestFixture]
    public class ChildArrayPropertyTester
    {
        private const string THE_CONCRETE_KEY = "Decorated";
        private const string THE_PROPERTY_NAME = "innerGateway";

        private ReportObjectMother _mother;
        private PluginGraphReport _report;

        [SetUp]
        public void SetUp()
        {
            _mother = new ReportObjectMother();
            _mother.AddFamily(typeof (IGateway));
            _mother.AddPlugin(typeof (IGateway), typeof (StubbedGateway), "Stubbed");

            _mother.AddPlugin(typeof (IGateway), typeof (DecoratedGateway), THE_CONCRETE_KEY);

            _mother.AddChildArrayProperty(typeof (IGateway), THE_CONCRETE_KEY, THE_PROPERTY_NAME, typeof (IGateway));


            _report = _mother.Report;
        }

        [Test]
        public void CreateChildren()
        {
            ChildArrayProperty property = createChildArrayProperty();

            Assert.AreEqual(4, property.InnerProperties.Length);

            Assert.AreEqual(ChildPropertyType.Default, property[0].ChildType);
            Assert.AreEqual(ChildPropertyType.Reference, property[1].ChildType);
            Assert.AreEqual(ChildPropertyType.Reference, property[2].ChildType);
            Assert.AreEqual(ChildPropertyType.Reference, property[3].ChildType);
        }

        private ChildArrayProperty createChildArrayProperty()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            parent.AddChildArray(THE_PROPERTY_NAME, new InstanceMemento[]
                                                        {
                                                            MemoryInstanceMemento.CreateDefaultInstanceMemento(),
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref1")
                                                            ,
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref2")
                                                            ,
                                                            MemoryInstanceMemento.CreateReferencedInstanceMemento("Ref3")
                                                        });


            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            return (ChildArrayProperty) instance[THE_PROPERTY_NAME];
        }

        [Test]
        public void AcceptVisitor()
        {
            ChildArrayProperty property = createChildArrayProperty();

            DynamicMock visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            visitorMock.Expect("HandleChildArrayProperty", property);
            property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);

            visitorMock.Verify();
        }

        [Test]
        public void Children()
        {
            ChildArrayProperty property = createChildArrayProperty();
            Assert.AreEqual(property.InnerProperties, property.Children);
        }

        [Test]
        public void ChildArrayPropertyPropagatesTheValidateCallToTheInnerProperties()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            parent.AddChildArray(THE_PROPERTY_NAME, new InstanceMemento[0]);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildArrayProperty property = (ChildArrayProperty) instance[THE_PROPERTY_NAME];

            MockChildProperty child1 = new MockChildProperty();
            MockChildProperty child2 = new MockChildProperty();
            MockChildProperty child3 = new MockChildProperty();

            property.AddChildProperty(child1);
            property.AddChildProperty(child2);
            property.AddChildProperty(child3);

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            IInstanceValidator validator = (IInstanceValidator) validatorMock.MockInstance;

            property.Validate(validator);

            child1.Verify();
            child2.Verify();
            child3.Verify();
        }


        [Test]
        public void ChildArrayIsNotDefinedInTheMemento()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildArrayProperty property = (ChildArrayProperty) instance[THE_PROPERTY_NAME];

            Problem expected = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }


        [Test]
        public void ChildArrayIsNotDefinedInTheMementoWhenMementoThrowsException()
        {
            PropertyDefinition definition = _report.FindPlugin(typeof (IGateway), THE_CONCRETE_KEY)[THE_PROPERTY_NAME];

            DynamicMock mementoMock = new DynamicMock(typeof (InstanceMemento));
            mementoMock.ExpectAndThrow("GetChildrenArray", new ApplicationException("Bad"), THE_PROPERTY_NAME);

            ChildArrayProperty property =
                new ChildArrayProperty(definition, (InstanceMemento) mementoMock.MockInstance, _report);

            Problem expected = new Problem(ConfigurationConstants.MEMENTO_PROPERTY_IS_MISSING, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }
    }
}