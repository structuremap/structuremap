using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using StructureMap.AutoMocking;
using StructureMap.TypeRules;
using System;
using System.Linq.Expressions;
using Xunit;

namespace StructureMap.Testing.AutoMocking
{
    public class RhinoAutoMockerTester : AutoMockerTester
    {
        private readonly AutoMockedContainer container;

        public RhinoAutoMockerTester()
        {
            var locator = new RhinoMocksAAAServiceLocator();
            container = new AutoMockedContainer(locator);
        }

        protected override AutoMocker<T> createAutoMocker<T>()
        {
            return new RhinoAutoMocker<T>();
        }

        protected override void setExpectation<T, TResult>(T mock, Expression<Func<T, TResult>> functionCall,
            TResult expectedResult)
        {
            mock.Expect(x => functionCall.Compile()(mock)).Return(expectedResult);
        }

        [Fact]
        public void AutoFillAConcreteClassWithMocks()
        {
            var service = container.GetInstance<IMockedService>();
            var service2 = container.GetInstance<IMockedService2>();
            var service3 = container.GetInstance<IMockedService3>();

            var concreteClass = container.GetInstance<ConcreteClass>();

            service.ShouldBeTheSameAs(concreteClass.Service);
            service2.ShouldBeTheSameAs(concreteClass.Service2);
            service3.ShouldBeTheSameAs(concreteClass.Service3);
        }

        [Fact]
        public void GetAFullMockForAServiceThatHasNotPreviouslyBeenRequested()
        {
            var service = container.GetInstance<IMockedService>();
            service.GetType().CanBeCastTo<IMockedObject>().ShouldBeTrue();
        }

        [Fact]
        public void InjectAStubAndGetTheStubBack()
        {
            var stub = new StubService();
            container.Inject<IMockedService>(stub);

            stub.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
            stub.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
            stub.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
        }

        [Fact]
        public void RequestTheServiceTwiceAndGetTheExactSameMockObject()
        {
            var service = container.GetInstance<IMockedService>();
            service.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
            service.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
            service.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
            service.ShouldBeTheSameAs(container.GetInstance<IMockedService>());
        }

        [Fact]
        public void TheAutoMockerOptionallyPushesInMocksInReplayModeToAllowForAAAsyntax()
        {
            // This sets up a Rhino Auto Mocker in the Arrange, Act, Assert mode
            var autoMocker = new RhinoAutoMocker<ConcreteClass>(MockMode.AAA);

            // Act in the test
            var @class = autoMocker.ClassUnderTest;
            @class.CallService();

            // This retrieves the mock object for IMockedService
            autoMocker.Get<IMockedService>().AssertWasCalled(s => s.Go());
        }

        [Fact]
        public void use_a_mock_object_for_concrete_class_dependency()
        {
            var autoMocker = new RhinoAutoMocker<ClassThatUsesConcreteDependency>();
            autoMocker.UseMockForType<ConcreteDependency>();

            autoMocker.ClassUnderTest.Dependency.Go();

            autoMocker.Get<ConcreteDependency>().AssertWasCalled(x => x.Go());
        }
    }

    public class ClassThatUsesConcreteDependency
    {
        private readonly ConcreteDependency _dependency;

        public ClassThatUsesConcreteDependency(ConcreteDependency dependency)
        {
            _dependency = dependency;
        }

        public ConcreteDependency Dependency
        {
            get { return _dependency; }
        }
    }

    public class ConcreteDependency
    {
        public virtual void Go()
        {
        }
    }
}