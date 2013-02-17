using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class InstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Build_the_InstanceToken()
        {
            var instance = new InstanceUnderTest();
            instance.Name = "name of instance";
            IDiagnosticInstance diagnosticInstance = instance;

            InstanceToken token = diagnosticInstance.CreateToken();

            Assert.AreEqual(instance.Name, token.Name);
            Assert.AreEqual("InstanceUnderTest", token.Description);
        }

        [Test]
        public void default_scope_is_PerRequest()
        {
            var i1 = new ConfiguredInstance(GetType()).Named("foo");
            i1.Lifecycle.ShouldBeNull();  // TODO -- this is going to change
        }

        [Test]
        public void can_set_scope_directly_on_the_instance()
        {
            var i1 = new ConfiguredInstance(GetType()).Named("foo");
            i1.SetScopeTo(InstanceScope.ThreadLocal);

            i1.Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }

        [Test]
        public void does_override_the_scope_of_the_parent()
        {
            var family = new PluginFamily(GetType());
            family.SetScopeTo(InstanceScope.Singleton);

            var i1 = new ConfiguredInstance(GetType()).Named("foo");
            i1.SetScopeTo(InstanceScope.ThreadLocal);

            family.AddInstance(i1);

            i1.Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }

        [Test]
        public void uses_parent_lifecycle_if_none_is_set_on_instance()
        {
            var family = new PluginFamily(GetType());
            family.SetScopeTo(InstanceScope.Singleton);

            var i1 = new ConfiguredInstance(GetType()).Named("foo");

            family.AddInstance(i1);

            i1.Lifecycle.ShouldBeOfType<SingletonLifecycle>();
        }

        [Test]
        public void still_chooses_PerRequest_if_nothing_is_selected_on_either_family_or_instance()
        {
            var family = new PluginFamily(GetType());

            var i1 = new ConfiguredInstance(GetType()).Named("foo");

            family.AddInstance(i1);

            i1.Lifecycle.ShouldBeNull(); // TODO -- will change later
        }


        [Test]
        public void instance_key_is_predictable()
        {
            var i1 = new ConfiguredInstance(GetType()).Named("foo");
            var i2 = new ConfiguredInstance(GetType()).Named("bar");

            i1.InstanceKey(GetType()).ShouldEqual(i1.InstanceKey(GetType()));
            i2.InstanceKey(GetType()).ShouldEqual(i2.InstanceKey(GetType()));
            i1.InstanceKey(GetType()).ShouldNotEqual(i2.InstanceKey(GetType()));
            i1.InstanceKey(typeof(InstanceUnderTest)).ShouldNotEqual(i1.InstanceKey(GetType()));
        }

    }

    public class InstanceUnderTest : Instance
    {
        public object TheInstanceThatWasBuilt = new object();


        protected override object build(Type pluginType, BuildSession session)
        {
            return TheInstanceThatWasBuilt;
        }

        protected override string getDescription()
        {
            return "InstanceUnderTest";
        }
    }
}