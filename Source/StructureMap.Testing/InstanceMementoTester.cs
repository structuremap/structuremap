using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class InstanceMementoTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void InterceptorTest()
        {
            MockRepository mocks = new MockRepository();

            IService servicePassedIntoInterceptor = null;
            StartupInterceptor<IService> interceptor = new StartupInterceptor<IService>(
                delegate(IService s) { servicePassedIntoInterceptor = s; });
            InstanceMemento memento = mocks.PartialMock<InstanceMemento>();
            memento.Interceptor = interceptor;


            IInstanceCreator instanceCreator = mocks.CreateMock<IInstanceCreator>();

            ColorService realService = new ColorService("Red");
            Expect.Call(instanceCreator.BuildInstance(memento)).Return(realService);

            mocks.ReplayAll();

            Assert.AreSame(realService, memento.Build(instanceCreator));
            Assert.AreSame(realService, servicePassedIntoInterceptor);
        }
    }
}