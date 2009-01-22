using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using StructureMap.AutoMocking;
using StructureMap.Graph;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class RhinoAutoMockerTester : AutoMockerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PluginCache.ResetAll();

            _locator = new RhinoMocksAAAServiceLocator();
            _container = new AutoMockedContainer(_locator);
        }

        #endregion

        private RhinoMocksAAAServiceLocator _locator;
        private AutoMockedContainer _container;

        protected override AutoMocker<T> createAutoMocker<T>()
        {
            return new RhinoAutoMocker<T>();
        }

        protected override void setExpectation<T,TResult>(T mock, Expression<Func<T,TResult>> functionCall, TResult expectedResult)
        {
            mock.Expect(x => functionCall.Compile()(mock)).Return(expectedResult);
        }

        [Test]
        public void AutoFillAConcreteClassWithMocks()
        {
            var service = _container.GetInstance<IMockedService>();
            var service2 = _container.GetInstance<IMockedService2>();
            var service3 = _container.GetInstance<IMockedService3>();

            var concreteClass = _container.FillDependencies<ConcreteClass>();

            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }


        [Test]
        public void GetAFullMockForAServiceThatHasNotPreviouslyBeenRequested()
        {
            var service = _container.GetInstance<IMockedService>();
            service.ShouldBeOfType<IMockedObject>();
        }

        [Test]
        public void InjectAStubAndGetTheStubBack()
        {
            var stub = new StubService();
            _container.Inject<IMockedService>(stub);

            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
        }

        [Test]
        public void RequestTheServiceTwiceAndGetTheExactSameMockObject()
        {
            var service = _container.GetInstance<IMockedService>();
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
        }


        [Test]
        public void TheAutoMockerOptionallyPushesInMocksInReplayModeToAllowForAAAsyntax()
        {
            // This sets up a Rhino Auto Mocker in the Arrange, Act, Assert mode
            var autoMocker = new RhinoAutoMocker<ConcreteClass>(MockMode.AAA);

            // Act in the test
            ConcreteClass @class = autoMocker.ClassUnderTest;
            @class.CallService();

            // This retrieves the mock object for IMockedService
            autoMocker.Get<IMockedService>().AssertWasCalled(s => s.Go());
        }

    }
}