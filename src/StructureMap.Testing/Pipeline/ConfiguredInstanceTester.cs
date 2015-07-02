using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ConfiguredInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new Registry();
            registry.For<Rule>();
            registry.Scan(x =>
            {
                x.Assembly("StructureMap.Testing.Widget");
                x.Assembly("StructureMap.Testing.Widget2");
            });

            var graph = registry.Build();

            _session = BuildSession.ForPluginGraph(graph);
        }

        #endregion

        private BuildSession _session;


        public List<string> prop1 { get; set; }
        public IGateway[] gateways { get; set; }

        public class UsesGateways
        {
            private readonly IGateway[] _gateways;

            public UsesGateways(IGateway[] gateways)
            {
                _gateways = gateways;
            }
        }

        public class ClassThatTakesAnything
        {
            public ClassThatTakesAnything(string anything)
            {
            }
        }

        [Test]
        public void can_build_a_simple_class_type()
        {
            var instance = new ConfiguredInstance(typeof (Rule1));
            var rule = instance.Build<Rule>(_session);
            rule.ShouldNotBeNull();
            rule.ShouldBeOfType<Rule1>();
        }


        [Test]
        public void build_with_a_missing_dependency_throws_configuration_exception()
        {
            var instance = ComplexRule.GetInstance();
            instance.Dependencies.RemoveByName("String");

            Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(
                () => { instance.As<Instance>().Build<Rule>(_session); });
        }

        [Test]
        public void can_use_a_configured_instance_with_generic_template_type_and_arguments()
        {
            var instance = new ConfiguredInstance(typeof (Service2<>), typeof (string));
            var container = new Container();

            container.GetInstance<IService<string>>(instance).ShouldBeOfType(typeof (Service2<string>));
        }


        [Test]
        public void setter_with_primitive_happy_path()
        {
            var instance = new ConfiguredInstance(typeof (ColorRule))
                .Ctor<string>("color").Is("Red").Setter<int>("Age").Is(34);

            IConfiguredInstance configuredInstance = instance;


            configuredInstance.Dependencies.Get("color").ShouldBe("Red");
            configuredInstance.Dependencies.Get("Age").ShouldBe(34);

            instance.Ctor<string>("color").Is("Blue");
            configuredInstance.Dependencies.Get("color").ShouldBe("Blue");
        }

        [Test]
        public void HasProperty_for_child()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.Dependencies.Has("prop1").ShouldBeFalse();

            instance.Setter<List<string>>("prop1").IsNamedInstance("something");
            configuredInstance.Dependencies.Has("prop1").ShouldBeTrue();
        }


        [Test]
        public void HasProperty_for_child_array()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.Dependencies.Has("prop1").ShouldBeFalse();

            instance.EnumerableOf<IGateway>("prop1").Contains(new DefaultInstance());
            configuredInstance.Dependencies.Has("prop1").ShouldBeTrue();
        }


        [Test]
        public void TestComplexRule()
        {
            var instance = ComplexRule.GetInstance().As<Instance>();

            var rule = instance.Build<Rule>(_session);
            rule.ShouldNotBeNull();
            rule.ShouldBeOfType<ComplexRule>();
        }


        [Test]
        public void build_fails_with_StructureMapException_adds_context()
        {
            var instance = new ConfiguredInstance(typeof (ClassThatBlowsUp));

            var actual =
                Exception<StructureMapBuildException>.ShouldBeThrownBy(() => { instance.Build<ClassThatBlowsUp>(); });

            actual.Message.ShouldContain(instance.Description);
            actual.ShouldBeOfType<StructureMapBuildException>();
        }

        public class ClassThatBlowsUp
        {
            public ClassThatBlowsUp()
            {
                throw new StructureMapBuildException("I blew up all over myself");
            }
        }

        [Test]
        public void use_the_setter_function()
        {
            var theRule = new ARule();

            var container =
                new Container(
                    x =>
                    {
                        x.For(typeof (ClassWithDependency)).Use(typeof (ClassWithDependency)).Setter<Rule>().Is(
                            theRule);
                    });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeTheSameAs(theRule);
        }
    }
}