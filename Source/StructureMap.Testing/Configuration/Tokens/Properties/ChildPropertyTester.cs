using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.Tokens.Properties
{
    [TestFixture]
    public class ChildPropertyTester
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
            _mother.AddChildProperty(typeof (IGateway), THE_CONCRETE_KEY, THE_PROPERTY_NAME, typeof (IGateway));

            _report = _mother.Report;
        }

        [Test]
        public void InlineDefinitionChild()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = new MemoryInstanceMemento("Stubbed", "inner");
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            Assert.AreEqual(0, property.Problems.Length);
            Assert.IsNotNull(property.InnerInstance);
            Assert.AreEqual(ChildPropertyType.InlineDefinition, property.ChildType);
            Assert.AreEqual(string.Empty, property.ReferenceKey);

            InstanceToken innerInstance = property.InnerInstance;
            Assert.AreEqual("Stubbed", innerInstance.ConcreteKey);
            Assert.AreEqual("inner", innerInstance.InstanceKey);

            Assert.AreEqual(new GraphObject[] {innerInstance}, property.Children);
        }

        [Test]
        public void InlineDefinitionChildAcceptVisitor()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = new MemoryInstanceMemento("Stubbed", "inner");
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            visitorMock.Expect("HandleInlineChildProperty", property);
            property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);
            visitorMock.Verify();
        }

        [Test]
        public void ValidateInlineDefinitionChild()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = new MemoryInstanceMemento("Stubbed", "inner");
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            IInstanceValidator validator = (IInstanceValidator) validatorMock.MockInstance;
            validatorMock.ExpectAndReturn("CreateObject", new object(), typeof (IGateway), child);

            property.Validate(validator);

            validatorMock.Verify();
        }

        [Test]
        public void DefaultChild()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            Assert.AreEqual(0, property.Problems.Length);
            Assert.IsNull(property.InnerInstance);
            Assert.AreEqual(ChildPropertyType.Default, property.ChildType);
            Assert.AreEqual(string.Empty, property.ReferenceKey);

            Assert.AreEqual(0, property.Children.Length);
        }

        [Test]
        public void DefaultChildAcceptVisitor()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            visitorMock.Expect("HandleDefaultChildProperty", property);
            property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);
            visitorMock.Verify();
        }

        [Test]
        public void ValidateADefaultChildSuccessfully()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("HasDefaultInstance", true, typeof (IGateway));

            property.Validate((IInstanceValidator) validatorMock.MockInstance);

            Assert.AreEqual(0, property.Problems.Length);
            validatorMock.Verify();
        }


        [Test]
        public void ValidateADefaultChildWithNoDefaultConfigured()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateDefaultInstanceMemento();
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("HasDefaultInstance", false, typeof (IGateway));

            property.Validate((IInstanceValidator) validatorMock.MockInstance);

            validatorMock.Verify();

            Problem expected = new Problem(ConfigurationConstants.NO_DEFAULT_INSTANCE_CONFIGURED, string.Empty);
            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }

        [Test]
        public void ReferenceChild()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            string theReferenceKey = "Ref1";
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(theReferenceKey);
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            Assert.AreEqual(0, property.Problems.Length);
            Assert.IsNull(property.InnerInstance);
            Assert.AreEqual(ChildPropertyType.Reference, property.ChildType);
            Assert.AreEqual(theReferenceKey, property.ReferenceKey);

            Assert.AreEqual(0, property.Children.Length);
        }


        [Test]
        public void ReferenceChildAcceptVisitor()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            string theReferenceKey = "Ref1";
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(theReferenceKey);
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock visitorMock = new DynamicMock(typeof (IConfigurationVisitor));
            visitorMock.Expect("HandleReferenceChildProperty", property);
            property.AcceptVisitor((IConfigurationVisitor) visitorMock.MockInstance);
            visitorMock.Verify();
        }

        [Test]
        public void ReferenceChildValidatesSuccessfully()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            string theReferenceKey = "Ref1";
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(theReferenceKey);
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("InstanceExists", true, typeof (IGateway), theReferenceKey);

            property.Validate((IInstanceValidator) validatorMock.MockInstance);

            validatorMock.Verify();
            Assert.AreEqual(0, property.Problems.Length);
        }

        [Test]
        public void ReferenceChildValidateFails()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            string theReferenceKey = "Ref1";
            MemoryInstanceMemento child = MemoryInstanceMemento.CreateReferencedInstanceMemento(theReferenceKey);
            parent.AddChild(THE_PROPERTY_NAME, child);

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);

            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("InstanceExists", false, typeof (IGateway), theReferenceKey);

            property.Validate((IInstanceValidator) validatorMock.MockInstance);

            validatorMock.Verify();

            Problem expected = new Problem(ConfigurationConstants.NO_MATCHING_INSTANCE_CONFIGURED, string.Empty);
            Assert.AreEqual(new Problem[] {expected}, property.Problems);
        }

        [Test]
        public void NotDefinedChildShouldBeDefault()
        {
            MemoryInstanceMemento parent = new MemoryInstanceMemento(THE_CONCRETE_KEY, "decorated");
            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, parent);
            ChildProperty property = (ChildProperty) instance[THE_PROPERTY_NAME];

            Assert.IsNull(property.InnerInstance);
            Assert.AreEqual(ChildPropertyType.Default, property.ChildType);

            Assert.AreEqual(0, property.Problems.Length);
        }
    }
}