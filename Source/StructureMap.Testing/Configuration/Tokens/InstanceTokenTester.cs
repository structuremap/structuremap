using System;
using NMock;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;
using StructureMap.Graph;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.Tokens
{
    [TestFixture]
    public class InstanceTokenTester
    {
        private PluginGraphReport _report;
        private PluginToken _plugin;

        [SetUp]
        public void SetUp()
        {
            PluginGraphReport report = new PluginGraphReport();
            FamilyToken family = new FamilyToken(typeof (IGateway), "", new string[0]);

            report.AddFamily(family);

            _plugin = new PluginToken(new TypePath(typeof (StubbedGateway)), "concrete", DefinitionSource.Explicit);

            PropertyDefinition property =
                new PropertyDefinition("color", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);
            _plugin.AddPropertyDefinition(property);
            family.AddPlugin(_plugin);

            _report = report;
        }

        [Test]
        public void CreateInstanceTokenWithOnlyOnePrimitiveProperty()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty("color", "blue");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual("instance", instance.InstanceKey);
            Assert.AreEqual("concrete", instance.ConcreteKey);
            Assert.AreEqual(0, instance.Problems.Length);

            Assert.AreEqual(1, instance.Properties.Length);

            PrimitiveProperty property = (PrimitiveProperty) instance["color"];
            Assert.AreEqual("blue", property.Value);
        }


        [Test]
        public void CreateInstanceTokenWithTwoPrimitiveProperty()
        {
            _plugin.AddPropertyDefinition(
                new PropertyDefinition("name", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive));

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty("color", "blue");
            memento.SetProperty("name", "Bob");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual("instance", instance.InstanceKey);
            Assert.AreEqual("concrete", instance.ConcreteKey);
            Assert.AreEqual(0, instance.Problems.Length);

            Assert.AreEqual(2, instance.Properties.Length);

            PrimitiveProperty property = (PrimitiveProperty) instance["name"];
            Assert.AreEqual("Bob", property.Value);
        }


        [Test]
        public void CreateInstanceTokenWithAnEnumerationProperty()
        {
            PropertyDefinition definition =
                new PropertyDefinition("name", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Enumeration);
            definition.EnumerationValues = new string[] {"Bob"};
            _plugin.AddPropertyDefinition(definition);

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            memento.SetProperty("color", "blue");
            memento.SetProperty("name", "Bob");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual("instance", instance.InstanceKey);
            Assert.AreEqual("concrete", instance.ConcreteKey);
            Assert.AreEqual(0, instance.Problems.Length);

            Assert.AreEqual(2, instance.Properties.Length);

            EnumerationProperty property = (EnumerationProperty) instance["name"];
            Assert.AreEqual("Bob", property.Value);
        }

        [Test]
        public void InstanceTokenWithoutConcreteKey()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento(string.Empty, "instance");
            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Problem expected = new Problem(ConfigurationConstants.INVALID_PLUGIN, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, instance.Problems);
        }

        [Test]
        public void InstanceTokenWithoutAPlugin()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("SomethingReallyWrong", "instance");
            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Problem expected = new Problem(ConfigurationConstants.INVALID_PLUGIN, string.Empty);

            Assert.AreEqual(new Problem[] {expected}, instance.Problems);
        }

        [Test]
        public void ValidateInstanceTokenWithoutAnyPropertiesAndNoProblems()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual(0, instance.Problems.Length, "Asserting that there are no problems upfront");

            ValidationTarget target = new ValidationTarget(true);
            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("CreateObject", target, typeof (IGateway), memento);

            instance.Validate((IInstanceValidator) validatorMock.MockInstance);

            Assert.AreEqual(new Problem[0], instance.Problems);
            validatorMock.Verify();
            target.AssertValidationCall();
        }


        [Test]
        public void ValidateInstanceTokenWithoutAnyPropertiesAndValidationMethodThrowsAnException()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual(0, instance.Problems.Length, "Asserting that there are no problems upfront");

            ValidationTarget target = new ValidationTarget(false);
            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("CreateObject", target, typeof (IGateway), memento);

            instance.Validate((IInstanceValidator) validatorMock.MockInstance);

            validatorMock.Verify();
            target.AssertValidationCall();

            Problem expected = new Problem(ConfigurationConstants.VALIDATION_METHOD_FAILURE, string.Empty);
            Assert.AreEqual(new Problem[] {expected}, instance.Problems);
        }

        [Test]
        public void ValidateInstanceTokenWhenInstanceCannotBeCreated()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");

            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            Assert.AreEqual(0, instance.Problems.Length, "Asserting that there are no problems upfront");
            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndThrow("CreateObject", new ApplicationException("Bad"), typeof (IGateway), memento);

            instance.Validate((IInstanceValidator) validatorMock.MockInstance);

            validatorMock.Verify();

            Problem expected = new Problem(ConfigurationConstants.COULD_NOT_CREATE_INSTANCE, string.Empty);
            Assert.AreEqual(new Problem[] {expected}, instance.Problems);
        }

        [Test]
        public void InstanceTokenValidatePropagatesTheValidatorDownToTheProperties()
        {
            _plugin.AddPropertyDefinition(
                new PropertyDefinition("name", typeof (string), PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive));

            MemoryInstanceMemento memento = new MemoryInstanceMemento("concrete", "instance");
            InstanceToken instance = new InstanceToken(typeof (IGateway), _report, memento);

            MockProperty property1 = new MockProperty("prop1");
            MockProperty property2 = new MockProperty("prop2");
            MockProperty property3 = new MockProperty("prop3");

            instance.AddProperty(property1);
            instance.AddProperty(property2);
            instance.AddProperty(property3);

            DynamicMock validatorMock = new DynamicMock(typeof (IInstanceValidator));
            validatorMock.ExpectAndReturn("CreateObject", new object(), typeof (IGateway), memento);

            instance.Validate((IInstanceValidator) validatorMock.MockInstance);

            property1.Verify();
            property2.Verify();
            property3.Verify();
        }

        [Test]
        public void CreateInstanceTokenFromATemplatedMementoAllPropertiesAreAccountedFor()
        {
            TemplateToken template =
                new TemplateToken("Template1", "Concrete", new string[] {"prop1", "prop2", "prop3"});

            PluginGraphReport report = new PluginGraphReport();
            FamilyToken family = new FamilyToken(typeof (IGateway), "", new string[0]);
            report.AddFamily(family);
            family.AddTemplate(template);

            MemoryInstanceMemento memento = new MemoryInstanceMemento(string.Empty, "TheInstance");
            memento.SetTemplateKey(template.TemplateKey);
            ;
            memento.SetProperty("prop1", "value1");
            memento.SetProperty("prop2", "value2");
            memento.SetProperty("prop3", "value3");

            InstanceToken instance = new InstanceToken(typeof (IGateway), report, memento);

            Assert.AreEqual(memento.InstanceKey, instance.InstanceKey);
            Assert.AreEqual(template.TemplateKey, instance.TemplateKey);

            Assert.AreEqual(0, instance.Problems.Length);

            Assert.AreEqual(3, instance.Properties.Length);
            foreach (IProperty property in instance.Properties)
            {
                Assert.IsTrue(property is TemplateProperty);
            }
        }

        [Test]
        public void CreateInstanceTokenFromTemplatedMementoThatIsMissingATemplateProperty()
        {
            TemplateToken template =
                new TemplateToken("Template1", "Concrete", new string[] {"prop1", "prop2", "prop3"});

            PluginGraphReport report = new PluginGraphReport();
            FamilyToken family = new FamilyToken(typeof (IGateway), string.Empty, new string[0]);
            report.AddFamily(family);
            family.AddTemplate(template);

            MemoryInstanceMemento memento = new MemoryInstanceMemento(string.Empty, "TheInstance");
            memento.SetTemplateKey(template.TemplateKey);
            ;
            memento.SetProperty("prop1", "value1");
            memento.SetProperty("prop2", "value2");

            // prop3 is missing 
            //memento.SetProperty("prop3", "value3");


            InstanceToken instance = new InstanceToken(typeof (IGateway), report, memento);
            TemplateProperty property = (TemplateProperty) instance["prop3"];

            Problem problem = new Problem(ConfigurationConstants.MISSING_TEMPLATE_VALUE, "");
            Assert.AreEqual(new Problem[] {problem}, property.Problems);
        }
    }


    public class ValidationTarget
    {
        private readonly bool _success;
        private bool _validationWasCalled = false;

        public ValidationTarget(bool success)
        {
            _success = success;
        }

        [ValidationMethod]
        public void Validate()
        {
            _validationWasCalled = true;

            if (!_success)
            {
                throw new ApplicationException("Bad");
            }
        }

        public void AssertValidationCall()
        {
            Assert.IsTrue(_validationWasCalled);
        }
    }
}