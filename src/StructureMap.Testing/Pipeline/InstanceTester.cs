using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Diagnostics;
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