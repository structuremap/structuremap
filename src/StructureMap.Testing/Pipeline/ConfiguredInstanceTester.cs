using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Construction;
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
            registry.Scan(x =>
            {
                x.Assembly("StructureMap.Testing.Widget");
                x.Assembly("StructureMap.Testing.Widget2");
            });

            PluginGraph graph = registry.Build();

            var pipelineGraph = new PipelineGraph(graph);
            _session = new BuildSession(pipelineGraph, graph.InterceptorLibrary);
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



        [Test, ExpectedException(typeof (StructureMapException))]
        public void BuildRuleWithAMissingValue()
        {
            var instance = (IStructuredInstance) ComplexRule.GetInstance();
            instance.RemoveKey("String");

            var rule = (ComplexRule) ((Instance) instance).Build(typeof (Rule), _session);
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
            ConfiguredInstance instance = new ConfiguredInstance(typeof (ColorRule))
                .Ctor<string>("color").Is("Red").Setter<int>("Age").Is(34);

            IConfiguredInstance configuredInstance = instance;


            Assert.AreEqual("Red", configuredInstance.GetProperty("color"));
            Assert.AreEqual("34", configuredInstance.GetProperty("Age"));

            instance.Ctor<string>("color").Is("Blue");
            Assert.AreEqual("Blue", configuredInstance.GetProperty("color"));
        }

        [Test]
        public void HasProperty_for_child()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("prop1", null).ShouldBeFalse();

            instance.Setter<List<string>>("prop1").IsNamedInstance("something");
            configuredInstance.HasProperty("prop1", null).ShouldBeTrue();
        }


        [Test]
        public void HasProperty_for_child_array()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("prop1", null).ShouldBeFalse();

            instance.EnumerableOf<IGateway>("prop1").Contains(new DefaultInstance());
            configuredInstance.HasProperty("prop1", null).ShouldBeTrue();
        }

        [Test]
        public void HasProperty_for_child_array_when_property_name_is_inferred()
        {
            var instance = new ConfiguredInstance(typeof (UsesGateways));

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("gateways", null).ShouldBeFalse();

            instance.EnumerableOf<IGateway>().Contains(new DefaultInstance());
            configuredInstance.HasProperty("gateways", null).ShouldBeTrue();
        }

        [Test]
        public void HasProperty_for_generic_child_array_when_property_name_is_inferred()
        {
            var instance = new ConfiguredInstance(typeof (UsesGateways));

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("gateways", null).ShouldBeFalse();

            instance.EnumerableOf<IGateway>().Contains(new DefaultInstance());
            configuredInstance.HasProperty("gateways", null).ShouldBeTrue();
        }

        [Test]
        public void Property_cannot_be_found_so_throw_205()
        {
            try
            {
                IConfiguredInstance configuredInstance = new ConfiguredInstance(typeof (ClassThatTakesAnything));
                configuredInstance.Get<string>("anything", new StubBuildSession());
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(205, ex.ErrorCode);
            }
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
            var builder = mocks.StrictMock<IInstanceBuilder>();
            Expect.Call(builder.BuildInstance(null)).Throw(new InvalidCastException());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(206, delegate
            {
                var instance = new ConfiguredInstance(GetType());
                instance.Build(GetType(), new StubBuildSession(), builder);
            });
        }

        [Test]
        public void Trying_to_build_with_an_unknown_exception_will_throw_error_207()
        {
            var mocks = new MockRepository();
            var builder = mocks.StrictMock<IInstanceBuilder>();
            Expect.Call(builder.BuildInstance(null)).Throw(new Exception());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(207, delegate
            {
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
                    x =>
                    {
                        x.For(typeof (ClassWithDependency)).Use(typeof (ClassWithDependency)).Setter<Rule>().Is(
                            theRule);
                    });

            container.GetInstance<ClassWithDependency>().Rule.ShouldBeTheSameAs(theRule);
        }
    }
}