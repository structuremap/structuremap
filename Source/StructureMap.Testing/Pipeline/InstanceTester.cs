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
        public void Instance_Build_Calls_into_its_Interceptor()
        {
            MockRepository mocks = new MockRepository();
            InstanceInterceptor interceptor = mocks.CreateMock<InstanceInterceptor>();
            IBuildSession buildSession = mocks.CreateMock<IBuildSession>();


            InstanceUnderTest instanceUnderTest = new InstanceUnderTest();
            instanceUnderTest.Interceptor = interceptor;

            object objectReturnedByInterceptor = new object();
            
            using (mocks.Record())
            {
                Expect.Call(interceptor.Process(instanceUnderTest.TheInstanceThatWasBuilt)).Return(objectReturnedByInterceptor);
            }

            using (mocks.Playback())
            {
                Assert.AreEqual(objectReturnedByInterceptor, instanceUnderTest.Build(typeof (object), buildSession));
            }
        }

        [Test]
        public void Build_the_InstanceToken()
        {
            InstanceUnderTest instance = new InstanceUnderTest();
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


        protected override object build(Type pluginType, IBuildSession session)
        {
            return TheInstanceThatWasBuilt;
        }

        protected override string getDescription()
        {
            return "InstanceUnderTest";
        }
    }
}