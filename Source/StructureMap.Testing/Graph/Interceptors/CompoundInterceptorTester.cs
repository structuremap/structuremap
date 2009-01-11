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
            var mocks = new MockRepository();
            var interceptor1 = mocks.StrictMock<InstanceInterceptor>();
            var interceptor2 = mocks.StrictMock<InstanceInterceptor>();
            var interceptor3 = mocks.StrictMock<InstanceInterceptor>();
            var interceptor4 = mocks.StrictMock<InstanceInterceptor>();

            Expect.Call(interceptor1.Process("0", null)).Return("1");
            Expect.Call(interceptor2.Process("1", null)).Return("2");
            Expect.Call(interceptor3.Process("2", null)).Return("3");
            Expect.Call(interceptor4.Process("3", null)).Return("4");

            mocks.ReplayAll();
            var compoundInterceptor = new CompoundInterceptor(new[]
                                                                  {
                                                                      interceptor1,
                                                                      interceptor2,
                                                                      interceptor3,
                                                                      interceptor4
                                                                  });

            Assert.AreEqual("4", compoundInterceptor.Process("0", null));
            mocks.VerifyAll();
        }
    }
}