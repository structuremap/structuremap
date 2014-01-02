using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
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
            registry.Scan(x => {
                x.Assembly("StructureMap.Testing.Widget");
                x.Assembly("StructureMap.Testing.Widget2");
            });

            var graph = registry.Build();

            _session = BuildSession.ForPluginGraph(graph);
        }

        #endregion

        private BuildSession _session;


        private void assertActionThrowsErrorCode(int errorCode, Action action)
        {
            try
            {
                action();

                Assert.Fail("Should have thrown StructureMapException");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(errorCode, ex.ErrorCode);
            }
        }

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
        public void BuildRule1()
        {
            var instance = new ConfiguredInstance(typeof (Rule1));

            var rule = (Rule) instance.Build(typeof (Rule), _session);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is Rule1);
        }


        [Test]
        public void BuildRuleWithAMissingValue()
        {
            var instance = ComplexRule.GetInstance();
            instance.Dependencies.RemoveByName("String");

            Exception<StructureMapException>.ShouldBeThrownBy(
                () => { var rule = (ComplexRule) ((Instance) instance).Build(typeof (Rule), _session); });
        }

        [Test]
        public void Can_be_plugged_in_if_there_is_a_plugged_type_and_the_plugged_type_can_be_cast_to_the_plugintype()
        {
            var instance = new ConfiguredInstance(typeof (ColorWidget));
            var family = new PluginFamily(typeof (IWidget));

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

        [Test]
        public void Can_NOT_be_plugged_in_if_plugged_type_cannot_be_cast_to_the_plugin_type()
        {
            var instance = new ConfiguredInstance(typeof (ColorRule));
            var family = new PluginFamily(typeof (IWidget));

            instance.As<IDiagnosticInstance>().CanBePartOfPluginFamily(family).ShouldBeFalse();
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


            Assert.AreEqual("Red", configuredInstance.Dependencies.Get("color"));
            Assert.AreEqual(34, configuredInstance.Dependencies.Get("Age"));

            instance.Ctor<string>("color").Is("Blue");
            Assert.AreEqual("Blue", configuredInstance.Dependencies.Get("color"));
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
            var instance = (ConfiguredInstance) ComplexRule.GetInstance();

            var rule = (Rule) instance.Build(typeof (Rule), _session);
            Assert.IsNotNull(rule);
            Assert.IsTrue(rule is ComplexRule);
        }

        [Test]
        public void Trying_to_build_with_an_InvalidCastException_will_throw_error_206()
        {
            var mocks = new MockRepository();
            var builder = mocks.StrictMock<IBuildPlan>();
            Expect.Call(builder.Build(null)).Throw(new InvalidCastException());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(206, delegate {
                var instance = new ConfiguredInstance(GetType());
                instance.Build(GetType(), new StubBuildSession(), builder);
            });
        }

        [Test]
        public void Trying_to_build_with_an_unknown_exception_will_throw_error_207()
        {
            var mocks = new MockRepository();
            var builder = mocks.StrictMock<IBuildPlan>();
            Expect.Call(builder.Build(null)).Throw(new Exception());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(207, delegate {
                var instance = new ConfiguredInstance(GetType());
                instance.Build(GetType(), new StubBuildSession(), builder);
            });
        }

        [Test]
        public void use_the_setter_function()
        {
            var theRule = new ARule();

            var container =
                new Container(
                    x => {
                        x.For(typeof (ClassWithDependency)).Use(typeof (ClassWithDependency)).Setter<Rule>().Is(
                            theRule);
                    });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeTheSameAs(theRule);
        }
    }
}