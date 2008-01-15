using NUnit.Framework;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;
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
            _source = (MemoryMementoSource) _ruleFactory.Source;
        }

        #endregion

        private InstanceFactory _ruleFactory;
        private MemoryMementoSource _source;


        [Test]
        public void BuildRule1()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Rule1", string.Empty);

            Rule rule = (Rule) _ruleFactory.GetInstance(memento);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is Rule1);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleThatDoesNotExist()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Invalid", string.Empty);
            Rule rule = (Rule) _ruleFactory.GetInstance(memento);
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithABadValue()
        {
            MemoryInstanceMemento memento = (MemoryInstanceMemento) ComplexRule.GetMemento();
            memento.RemoveProperty("Int");
            memento.SetProperty("Int", "abc");
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance(memento);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithAMissingValue()
        {
            MemoryInstanceMemento memento = (MemoryInstanceMemento) ComplexRule.GetMemento();
            memento.RemoveProperty("String");
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance(memento);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithInvalidInstanceKey()
        {
            ComplexRule rule = (ComplexRule) _ruleFactory.GetInstance("NonExistentRule");
        }

        [Test]
        public void CanMakeAClassWithNoConstructorParametersADefaultMemento()
        {
            InstanceFactory factory = ObjectMother.CreateInstanceFactory(
                typeof (IGateway),
                new string[] {"StructureMap.Testing.Widget3"});

            factory.SetDefault("Stubbed");
            Assert.IsTrue(factory.GetInstance() is StubbedGateway);
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
        public void SetDefaultInstanceByString()
        {
            MemoryInstanceMemento red = new MemoryInstanceMemento("Color", "Red");
            red.SetProperty("Color", "Red");

            MemoryInstanceMemento blue = new MemoryInstanceMemento("Color", "Blue");
            blue.SetProperty("Color", "Blue");

            MemoryInstanceMemento orange = new MemoryInstanceMemento("Color", "Orange");
            orange.SetProperty("Color", "Orange");


            _source.AddMemento(red);
            _source.AddMemento(blue);
            _source.AddMemento(orange);

            _ruleFactory.SetDefault("Blue");
            ColorRule rule = _ruleFactory.GetInstance() as ColorRule;

            Assert.IsNotNull(rule);
            Assert.AreEqual("Blue", rule.Color);
        }

        [Test]
        public void TestComplexRule()
        {
            InstanceMemento memento = ComplexRule.GetMemento();

            Rule rule = (Rule) _ruleFactory.GetInstance(memento);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is ComplexRule);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToGetDefaultInstanceWithNoInstance()
        {
            ColorRule rule = _ruleFactory.GetInstance() as ColorRule;
        }

        [Test]
        public void UsesTheInterceptor()
        {
            PluginFamily family = new PluginFamily(typeof (IService));
            family.Plugins.Add(typeof (ColorService), "Color");
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Color", "Red");
            memento.SetProperty("color", "Red");
            family.AddInstance(memento);

            ColorService recordedService = null;

            StartupInterceptor<ColorService> interceptor = new StartupInterceptor<ColorService>(
                delegate(ColorService s) { recordedService = s; }
                );
            family.InstanceInterceptor = interceptor;


            InstanceFactory factory = new InstanceFactory(family, true);

            Assert.AreSame(factory.GetInstance("Red"), recordedService);
        }
    }
}