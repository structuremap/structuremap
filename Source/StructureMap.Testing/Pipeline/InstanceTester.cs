using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Testing.Container.Interceptors;

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

        [Test]
        public void Instance_Build_Calls_into_its_Interceptor()
        {
            MockRepository mocks = new MockRepository();
            InstanceInterceptor interceptor = mocks.CreateMock<InstanceInterceptor>();

            InstanceUnderTest instanceUnderTest = new InstanceUnderTest();
            instanceUnderTest.Interceptor = interceptor;

            object objectReturnedByInterceptor = new object();
            using (mocks.Record())
            {
                Expect.Call(interceptor.Process(instanceUnderTest.TheInstanceThatWasBuilt)).Return(objectReturnedByInterceptor);
            }

            using (mocks.Playback())
            {
                Assert.AreEqual(objectReturnedByInterceptor, instanceUnderTest.Build<object>(null));
            }
        }

        #endregion
    }

    public class InstanceUnderTest : Instance
    {
        public object TheInstanceThatWasBuilt = new object();

        public override void Diagnose<T>(StructureMap.Pipeline.IInstanceCreator creator, IInstanceDiagnostics diagnostics)
        {
            throw new System.NotImplementedException();
        }

        public override void Describe<T>(IInstanceDiagnostics diagnostics)
        {
            throw new System.NotImplementedException();
        }

        protected override T build<T>(StructureMap.Pipeline.IInstanceCreator creator)
        {
            return (T) TheInstanceThatWasBuilt;
        }
    }
}