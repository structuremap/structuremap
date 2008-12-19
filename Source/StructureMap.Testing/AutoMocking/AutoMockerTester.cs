using System;
using System.Linq.Expressions;
using NUnit.Framework;
using StructureMap.AutoMocking;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public abstract class AutoMockerTester
    {
        protected abstract AutoMocker<T> createAutoMocker<T>() where T : class;
        protected abstract void setExpectation<T, TResult>(T mock, Expression<Func<T, TResult>> functionCall, TResult expectedResult) where T : class;

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

            #endregion
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

        [Test]
        public void CanInjectAnArrayOfMockServices1()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            IMockedService[] services = mocker.CreateMockArrayFor<IMockedService>(3);
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(3);
        }

        [Test]
        public void CanInjectAnArrayOfMockServices2()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(0);
        }


        [Test]
        public void CanInjectAnArrayOfMockServices3()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            IMockedService[] services = mocker.CreateMockArrayFor<IMockedService>(3);

            mocker.PartialMockTheClassUnderTest();
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(3);
        }

        [Test]
        public void CanInjectAnArrayOfMockServices4()
        {
            var mocker = createAutoMocker<ClassWithArray>();

            mocker.PartialMockTheClassUnderTest();
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(0);
        }


        [Test]
        public void GetTheSameConcreteClassTwiceFromCreate()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
        }

        [Test]
        public void TheAutoMockerPushesInMocksAndAPreBuiltStubForAllOfTheConstructorArguments()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            var stub = new StubService();
            autoMocker.Inject<IMockedService>(stub);

            var service2 = autoMocker.Get<IMockedService2>();
            var service3 = autoMocker.Get<IMockedService3>();

            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(stub, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void TheAutoMockerPushesInMocksForAllOfTheConstructorArgumentsForAPartialMock()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();

            var service = autoMocker.Get<IMockedService>();
            var service2 = autoMocker.Get<IMockedService2>();
            var service3 = autoMocker.Get<IMockedService3>();

            autoMocker.PartialMockTheClassUnderTest();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void UseConcreteClassFor()
        {
            var mocker = createAutoMocker<ConcreteClass>();
            mocker.UseConcreteClassFor<ConcreteThing>();

            var thing = mocker.Get<ConcreteThing>();
            Assert.IsInstanceOfType(typeof (ConcreteThing), thing);

            Assert.AreSame(mocker.Get<IMockedService>(), thing.Service);
            Assert.AreSame(mocker.Get<IMockedService2>(), thing.Service2);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClass()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();
            setExpectation(autoMocker.Get<IMockedService>(), x=> x.Name, "Jeremy");
            autoMocker.ClassUnderTest.Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClassAsAPartialMockAndSetTheNameMethodUp()
        {
            var autoMocker = createAutoMocker<ConcreteClass>();

            autoMocker.PartialMockTheClassUnderTest();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;
            setExpectation(concreteClass, x=> x.Name, "Max");
            concreteClass.Name.ShouldEqual("Max");
        }

        public interface IAnotherService
        {
            
        }

    }
}