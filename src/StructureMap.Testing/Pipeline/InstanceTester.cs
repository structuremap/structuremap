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