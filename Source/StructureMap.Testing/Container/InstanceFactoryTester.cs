using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class InstanceFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            string[] assemblyNames = new string[]
                {
                    "StructureMap.Testing.Widget",
                    "StructureMap.Testing.Widget2"
                };

            _ruleFactory = ObjectMother.CreateInstanceFactory(typeof (Rule), assemblyNames);
        }

        #endregion

        private InstanceFactory _ruleFactory;


        [Test]
        public void BuildRule1()
        {
            ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey("Rule1");

            Rule rule = (Rule) _ruleFactory.GetInstance(instance, null);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is Rule1);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleThatDoesNotExist()
        {
            Rule rule = (Rule) _ruleFactory.GetInstance(new ConfiguredInstance().WithConcreteKey("Invalid"), null);
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithABadValue()
        {
            ConfiguredInstance instance = (ConfiguredInstance) ComplexRule.GetInstance();
            instance.SetProperty("Int", "abc");
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance(instance, null);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithAMissingValue()
        {
            ConfiguredInstance instance = (ConfiguredInstance) ComplexRule.GetInstance();
            instance.RemoveKey("String");
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance(instance, null);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithInvalidInstanceKey()
        {
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance("NonExistentRule");
        }

        [Test]
        public void CanMakeAClassWithNoConstructorParametersWithoutADefinedMemento()
        {
            InstanceFactory factory = ObjectMother.CreateInstanceFactory(
                typeof (IGateway),
                new string[] {"StructureMap.Testing.Widget3"});

            DefaultGateway gateway = factory.GetInstance("Default") as DefaultGateway;
            Assert.IsNotNull(gateway);
        }

        [Test]
        public void CouldBuildInstanceFactory()
        {
            Assert.IsNotNull(_ruleFactory);
        }


        [Test]
        public void TestComplexRule()
        {
            Rule rule = (Rule) _ruleFactory.GetInstance(ComplexRule.GetInstance(), null);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is ComplexRule);
        }


    }
}