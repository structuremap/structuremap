using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Graph.Interceptors
{
    [TestFixture]
    public class CompoundInterceptorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void CallAllTheInterceptors()
        {
            MockRepository mocks = new MockRepository();
            InstanceInterceptor interceptor1 = mocks.CreateMock<InstanceInterceptor>();
            InstanceInterceptor interceptor2 = mocks.CreateMock<InstanceInterceptor>();
            InstanceInterceptor interceptor3 = mocks.CreateMock<InstanceInterceptor>();
            InstanceInterceptor interceptor4 = mocks.CreateMock<InstanceInterceptor>();

            Expect.Call(interceptor1.Process("0")).Return("1");
            Expect.Call(interceptor2.Process("1")).Return("2");
            Expect.Call(interceptor3.Process("2")).Return("3");
            Expect.Call(interceptor4.Process("3")).Return("4");

            mocks.ReplayAll();
            CompoundInterceptor compoundInterceptor = new CompoundInterceptor(new InstanceInterceptor[]
                                                                                  {
                                                                                      interceptor1,
                                                                                      interceptor2,
                                                                                      interceptor3,
                                                                                      interceptor4
                                                                                  });

            Assert.AreEqual("4", compoundInterceptor.Process("0"));
            mocks.VerifyAll();
        }
    }
}