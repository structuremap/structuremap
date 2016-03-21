using Shouldly;
using StructureMap.AutoMocking;
using System;
using System.Linq.Expressions;
using Xunit;

namespace StructureMap.Testing.AutoMocking.Moq
{
    public abstract class AutoMockerTester
    {
        protected abstract AutoMocker<T> createAutoMocker<T>() where T : class;

        protected abstract void setExpectation<T, TResult>(T mock, Expression<Func<T, TResult>> functionCall,
            TResult expectedResult) where T : class;

        public class ConcreteThingWithNoConstructor
        {
        }

        public class ConcreteThing
        {
            private readonly IMockedService _service;
            private readonly IMockedService2 _service2;

            public ConcreteThing(IMockedService service, IMockedService2 service2)
            {
                _service = service;
                _service2 = service2;
            }

            public IMockedService Service
            {
                get { return _service; }
            }

            public IMockedService2 Service2
            {
                get { return _service2; }
            }
        }

        public class ConcreteClass
        {
            private readonly IMockedService _service;
            private readonly IMockedService2 _service2;
            private readonly IMockedService3 _service3;

            public ConcreteClass(IMockedService service, IMockedService2 service2, IMockedService3 service3)
            {
                _service = service;
                _service2 = service2;
                _service3 = service3;
            }

            public virtual string Name
            {
                get { return _service.Name; }
            }

            public IMockedService Service
            {
                get { return _service; }
            }

            public IMockedService2 Service2
            {
                get { return _service2; }
            }

            public IMockedService3 Service3
            {
                get { return _service3; }
            }

            public void CallService()
            {
                _service.Go();
            }
        }

        public interface IMockedService
        {
            string Name { get; }

            void Go();
        }

        public interface IMockedService2
        {
            void Go();
        }

        public interface IMockedService3
        {
            void Go();
        }

        public class StubService : IMockedService
        {
            private readonly string _name;

            public StubService()
            {
            }

            public StubService(string name)
            {
                _name = name;
            }

            #region IMockedService Members

            public string Name
            {
                get { return _name; }
            }

            public void Go()
            {
                throw new NotImplementedException();
            }

            #endregion IMockedService Members
        }

        public class ClassWithArray
        {
            private readonly IMockedService[] _services;

            public ClassWithArray(IMockedService[] services)
            {
                _services = services;
            }

            public IMockedService[] Services
            {
                get { return _services; }
            }
        }

        public interface IAnotherService
        {
        }

        [Fact]
        public void CanInjectAnArrayOfMockServices1()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            var services = mocker.CreateMockArrayFor<IMockedService>(3);
            var theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldBe(3);
        }

        [Fact]
        public void CanInjectAnArrayOfMockServices3()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            var services = mocker.CreateMockArrayFor<IMockedService>(3);

            mocker.PartialMockTheClassUnderTest();
            var theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldBe(3);
        }

        [Fact]
        public void GetTheSameConcreteClassTwiceFromCreate()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            var concreteClass = autoMocker.ClassUnderTest;

            concreteClass.ShouldBeTheSameAs(autoMocker.ClassUnderTest);
            concreteClass.ShouldBeTheSameAs(autoMocker.ClassUnderTest);
            concreteClass.ShouldBeTheSameAs(autoMocker.ClassUnderTest);
        }

        [Fact]
        public void TheAutoMockerPushesInMocksAndAPreBuiltStubForAllOfTheConstructorArguments()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            var stub = new StubService();
            autoMocker.Inject<IMockedService>(stub);

            var service2 = autoMocker.Get<IMockedService2>();
            var service3 = autoMocker.Get<IMockedService3>();

            var concreteClass = autoMocker.ClassUnderTest;

            stub.ShouldBeTheSameAs(concreteClass.Service);
            service2.ShouldBeTheSameAs(concreteClass.Service2);
            service3.ShouldBeTheSameAs(concreteClass.Service3);
        }

        [Fact]
        public void TheAutoMockerPushesInMocksForAllOfTheConstructorArgumentsForAPartialMock()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();

            var service = autoMocker.Get<IMockedService>();
            var service2 = autoMocker.Get<IMockedService2>();
            var service3 = autoMocker.Get<IMockedService3>();

            autoMocker.PartialMockTheClassUnderTest();
            var concreteClass = autoMocker.ClassUnderTest;

            service.ShouldBeTheSameAs(concreteClass.Service);
            service2.ShouldBeTheSameAs(concreteClass.Service2);
            service3.ShouldBeTheSameAs(concreteClass.Service3);
        }

        [Fact]
        public void UseConcreteClassFor()
        {
            var mocker = createAutoMocker<ConcreteClass>();
            mocker.UseConcreteClassFor<ConcreteThing>();

            var thing = mocker.Get<ConcreteThing>();
            thing.ShouldBeOfType<ConcreteThing>();

            mocker.Get<IMockedService>().ShouldBeTheSameAs(thing.Service);
            mocker.Get<IMockedService2>().ShouldBeTheSameAs(thing.Service2);
        }

        [Fact]
        public void UseTheAutoMockerToStartUpTheConcreteClass()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            setExpectation(autoMocker.Get<IMockedService>(), x => x.Name, "Jeremy");
            autoMocker.ClassUnderTest.Name.ShouldBe("Jeremy");
        }

        [Fact]
        public void UseTheAutoMockerToStartUpTheConcreteClassAsAPartialMockAndSetTheNameMethodUp()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();

            autoMocker.PartialMockTheClassUnderTest();
            var concreteClass = autoMocker.ClassUnderTest;
            setExpectation(concreteClass, x => x.Name, "Max");
            concreteClass.Name.ShouldBe("Max");
        }

        [Fact]
        public void GetTheConcreteTypeFromTheMockerWhenTypeHasNoConstructorArguments()
        {
            var autoMocker = createAutoMocker<ConcreteThingWithNoConstructor>();

            var thing = autoMocker.ClassUnderTest;

            thing.GetType().ShouldBe(typeof(ConcreteThingWithNoConstructor));
        }
    }
}