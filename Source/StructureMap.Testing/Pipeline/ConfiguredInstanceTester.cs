using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
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
            registry.BuildInstancesOf<Rule>();
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


        [Test]
        public void Build_happy_path()
        {
            var mocks = new MockRepository();
            var builder = mocks.StrictMock<InstanceBuilder>();
            var session = mocks.StrictMock<BuildSession>();
            var theObjectBuilt = new object();

            var instance = new ConfiguredInstance(GetType());


            using (mocks.Record())
            {
                Expect.Call(builder.BuildInstance(instance, session)).Return(theObjectBuilt);
            }

            using (mocks.Playback())
            {
                object actualObject = ((IConfiguredInstance) instance).Build(GetType(), session, builder);
                Assert.AreSame(theObjectBuilt, actualObject);
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
        public void BuildRuleWithABadValue()
        {
            var instance = (ConfiguredInstance) ComplexRule.GetInstance();

            instance.WithProperty("Int").EqualTo("abc");
            var rule = (ComplexRule) instance.Build(typeof (Rule), _session);
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

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

        [Test]
        public void can_use_a_configured_instance_with_generic_template_type_and_arguments()
        {
            var instance = new ConfiguredInstance(typeof (Service2<>), typeof (string));
            var container = new Container();

            container.GetInstance<IService<string>>(instance).ShouldBeOfType(typeof (Service2<string>));
        }

        [Test]
        public void Create_description_if_has_plugged_type_and_plugged_type_has_no_arguments()
        {
            var instance = new ConfiguredInstance(GetType());
            TestUtility.AssertDescriptionIs(instance, TypePath.GetAssemblyQualifiedName(GetType()));
        }

        [Test]
        public void Create_description_if_has_pluggedType_and_plugged_type_has_arguments()
        {
            var instance = new ConfiguredInstance(typeof (ColorService));
            TestUtility.AssertDescriptionIs(instance,
                                            "Configured " + TypePath.GetAssemblyQualifiedName(typeof (ColorService)));
        }

        [Test]
        public void get_the_concrete_type_from_diagnostic_instance()
        {
            var instance = new ConfiguredInstance(typeof (ColorRule)) as IDiagnosticInstance;
            instance.ConcreteType.ShouldEqual(typeof (ColorRule));
        }


        [Test]
        public void GetProperty_happy_path()
        {
            ConfiguredInstance instance = new ConfiguredInstance(typeof (ColorRule))
                .WithProperty("Color").EqualTo("Red").WithProperty("Age").EqualTo("34");

            IConfiguredInstance configuredInstance = instance;

            Assert.AreEqual("Red", configuredInstance.GetProperty("Color"));
            Assert.AreEqual("34", configuredInstance.GetProperty("Age"));

            instance.WithProperty("Color").EqualTo("Blue");
            Assert.AreEqual("Blue", configuredInstance.GetProperty("Color"));
        }

        [Test]
        public void HasProperty_for_child()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("prop1").ShouldBeFalse();

            instance.Child("prop1").IsNamedInstance("something");
            configuredInstance.HasProperty("prop1").ShouldBeTrue();
        }


        [Test]
        public void HasProperty_for_child_array()
        {
            var instance = new ConfiguredInstance(GetType());

            IConfiguredInstance configuredInstance = instance;
            configuredInstance.HasProperty("prop1").ShouldBeFalse();

            instance.ChildArray<IGateway[]>("prop1").Contains(new DefaultInstance());
            configuredInstance.HasProperty("prop1").ShouldBeTrue();
        }

        [Test]
        public void Property_cannot_be_found_so_throw_205()
        {
            try
            {
                IConfiguredInstance configuredInstance = new ConfiguredInstance(GetType());
                configuredInstance.GetProperty("anything");
                Assert.Fail("Did not throw exception");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(205, ex.ErrorCode);
            }
        }

        [Test]
        public void Should_find_the_InstanceBuilder_by_PluggedType_if_it_exists()
        {
            var mocks = new MockRepository();
            var session = mocks.DynamicMock<BuildSession>();

            Type thePluginType = typeof (IGateway);
            Type thePluggedType = GetType();
            var builder = mocks.StrictMock<InstanceBuilder>();

            var instance = new ConfiguredInstance(thePluggedType);

            using (mocks.Record())
            {
                PluginCache.Store(thePluggedType, builder);
                Expect.Call(builder.BuildInstance(instance, session)).Return(new object());
            }

            using (mocks.Playback())
            {
                instance.Build(thePluginType, session);
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
            var builder = mocks.StrictMock<InstanceBuilder>();
            Expect.Call(builder.BuildInstance(null, null)).Throw(new InvalidCastException());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(206, delegate
            {
                IConfiguredInstance instance = new ConfiguredInstance(GetType());
                instance.Build(GetType(), new StubBuildSession(), builder);
            });
        }

        [Test]
        public void Trying_to_build_with_an_unknown_exception_will_throw_error_207()
        {
            var mocks = new MockRepository();
            var builder = mocks.StrictMock<InstanceBuilder>();
            Expect.Call(builder.BuildInstance(null, null)).Throw(new Exception());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(207, delegate
            {
                IConfiguredInstance instance = new ConfiguredInstance(GetType());
                instance.Build(GetType(), new StubBuildSession(), builder);
            });
        }
    }
}