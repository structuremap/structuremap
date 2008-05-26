using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
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
            instance = new ConfiguredInstance();
        }

        #endregion

        private ConfiguredInstance instance;


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
        public void Create_description_if_has_pluggedType_and_plugged_type_has_arguments()
        {
            ConfiguredInstance instance = new ConfiguredInstance(typeof(ColorService));
            TestUtility.AssertDescriptionIs(instance, "Configured " + TypePath.GetAssemblyQualifiedName(typeof(ColorService)));
        }

        [Test]
        public void Create_description_if_has_only_concrete_key()
        {
            ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey("Concrete");
            TestUtility.AssertDescriptionIs(instance, "Configured 'Concrete'");
        }

        [Test]
        public void Create_description_if_has_plugged_type_and_plugged_type_has_no_arguments()
        {
            ConfiguredInstance instance = new ConfiguredInstance(GetType());
            TestUtility.AssertDescriptionIs(instance, TypePath.GetAssemblyQualifiedName(GetType()));
        }


        [Test]
        public void AttachDependencies_should_find_the_InstanceBuilder_by_ConcreteKey_if_PluggedType_does_not_exists()
        {
            MockRepository mocks = new MockRepository();
            IBuildSession session = mocks.CreateMock<IBuildSession>();
            InstanceBuilder builder = mocks.CreateMock<InstanceBuilder>();
            string theConcreteKey = "something";

            Type thePluginType = typeof (IGateway);

            using (mocks.Record())
            {
                Expect.Call(session.FindBuilderByType(thePluginType, null)).Return(null);
                Expect.Call(session.FindBuilderByConcreteKey(thePluginType, theConcreteKey)).Return(builder);
                Expect.Call(builder.BuildInstance(null, null)).Return(new object());
                LastCall.IgnoreArguments();
            }

            using (mocks.Playback())
            {
                ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey(theConcreteKey);
                instance.Build(thePluginType, session);
            }
        }

        [Test]
        public void Build_happy_path()
        {
            MockRepository mocks = new MockRepository();
            InstanceBuilder builder = mocks.CreateMock<InstanceBuilder>();
            IBuildSession session = mocks.CreateMock<IBuildSession>();
            object theObjectBuilt = new object();

            ConfiguredInstance instance = new ConfiguredInstance();


            using (mocks.Record())
            {
                Expect.Call(builder.BuildInstance(instance, session)).Return(theObjectBuilt);
            }

            using (mocks.Playback())
            {
                object actualObject = instance.Build(GetType(), session, builder);
                Assert.AreSame(theObjectBuilt, actualObject);
            }
        }

        [Test]
        public void CanBePartOfPluginFamily_is_false_if_the_plugin_cannot_be_found()
        {
            PluginFamily family = new PluginFamily(typeof (IService));
            family.AddPlugin(typeof (ColorService), "Color");

            ConfiguredInstance instance = new ConfiguredInstance();
            instance.ConcreteKey = "Color";

            IDiagnosticInstance diagnosticInstance = instance;

            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family));

            instance.ConcreteKey = "something else";
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

        [Test]
        public void GetProperty_happy_path()
        {
            instance.SetProperty("Color", "Red")
                .SetProperty("Age", "34");

            IConfiguredInstance configuredInstance = instance;

            Assert.AreEqual("Red", configuredInstance.GetProperty("Color"));
            Assert.AreEqual("34", configuredInstance.GetProperty("Age"));

            instance.SetProperty("Color", "Blue");
            Assert.AreEqual("Blue", configuredInstance.GetProperty("Color"));
        }

        [Test]
        public void Property_cannot_be_found_so_throw_205()
        {
            try
            {
                IConfiguredInstance configuredInstance = instance;
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
            MockRepository mocks = new MockRepository();
            IBuildSession session = mocks.DynamicMock<IBuildSession>();

            Type thePluginType = typeof (IGateway);
            Type thePluggedType = GetType();
            InstanceBuilder builder = mocks.CreateMock<InstanceBuilder>();

            ConfiguredInstance instance = new ConfiguredInstance(thePluggedType);

            using (mocks.Record())
            {
                Expect.Call(session.FindBuilderByType(thePluginType, thePluggedType)).Return(builder);
                Expect.Call(builder.BuildInstance(instance, session)).Return(new object());
            }

            using (mocks.Playback())
            {
                instance.Build(thePluginType, session);
            }
        }

        [Test]
        public void Trying_to_build_with_an_InvalidCastException_will_throw_error_206()
        {
            MockRepository mocks = new MockRepository();
            InstanceBuilder builder = mocks.CreateMock<InstanceBuilder>();
            Expect.Call(builder.BuildInstance(null, null)).Throw(new InvalidCastException());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(206, delegate()
                                                 {
                                                     ConfiguredInstance instance = new ConfiguredInstance();
                                                     instance.Build(GetType(), new StubBuildSession(), builder);
                                                 });
        }

        [Test]
        public void Trying_to_build_with_an_unknown_exception_will_throw_error_207()
        {
            MockRepository mocks = new MockRepository();
            InstanceBuilder builder = mocks.CreateMock<InstanceBuilder>();
            Expect.Call(builder.BuildInstance(null, null)).Throw(new Exception());
            LastCall.IgnoreArguments();
            mocks.Replay(builder);

            assertActionThrowsErrorCode(207, delegate()
                                                 {
                                                     ConfiguredInstance instance = new ConfiguredInstance();
                                                     instance.Build(GetType(), new StubBuildSession(), builder);
                                                 });
        }

        [Test]
        public void Trying_to_build_without_an_InstanceBuilder_throws_exception()
        {
            assertActionThrowsErrorCode(201, delegate()
                                                 {
                                                     string theConcreteKey = "the concrete key";
                                                     ConfiguredInstance instance =
                                                         new ConfiguredInstance(GetType()).WithConcreteKey(
                                                             theConcreteKey);

                                                     instance.Build(GetType(), null, null);
                                                 });
        }

        [Test]
        public void Can_be_plugged_in_by_concrete_key()
        {
            ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey("Color");
            PluginFamily family = new PluginFamily(typeof(IWidget));
            family.AddPlugin(typeof (ColorWidget), "Color");

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

        [Test]
        public void Can_be_plugged_in_if_there_is_a_plugged_type_and_the_plugged_type_can_be_cast_to_the_plugintype()
        {
            ConfiguredInstance instance = new ConfiguredInstance().UsingConcreteType<ColorWidget>();
            PluginFamily family = new PluginFamily(typeof(IWidget));

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

        [Test]
        public void Can_NOT_be_plugged_in_if_no_plugged_type_and_concrete_key_cannot_be_found_in_family()
        {
            ConfiguredInstance instance = new ConfiguredInstance().WithConcreteKey("SomethingThatDoesNotExist");
            PluginFamily family = new PluginFamily(typeof(IWidget));

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family));            
        }

        [Test]
        public void Can_NOT_be_plugged_in_if_plugged_type_cannot_be_cast_to_the_plugin_type()
        {
            ConfiguredInstance instance = new ConfiguredInstance().UsingConcreteType<ColorRule>();
            PluginFamily family = new PluginFamily(typeof(IWidget));

            IDiagnosticInstance diagnosticInstance = instance;
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family));
        }

    }
}