using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using StructureMap.AutoMocking;
using StructureMap.Graph;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class RhinoAutoMockerTester
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
        public void CanInjectAnArrayOfMockServices1()
        {
            var mocker = new RhinoAutoMocker<ClassWithArray>();

            IMockedService[] services = mocker.CreateMockArrayFor<IMockedService>(3);
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(3);
        }

        [Test]
        public void CanInjectAnArrayOfMockServices2()
        {
            var mocker = new RhinoAutoMocker<ClassWithArray>();

            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(0);
        }


        [Test]
        public void CanInjectAnArrayOfMockServices3()
        {
            var mocker = new RhinoAutoMocker<ClassWithArray>();

            IMockedService[] services = mocker.CreateMockArrayFor<IMockedService>(3);

            mocker.PartialMockTheClassUnderTest();
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(3);
        }

        [Test]
        public void CanInjectAnArrayOfMockServices4()
        {
            var mocker = new RhinoAutoMocker<ClassWithArray>();

            mocker.PartialMockTheClassUnderTest();
            ClassWithArray theClass = mocker.ClassUnderTest;

            theClass.Services.Length.ShouldEqual(0);
        }


        [Test]
        public void GetAFullMockForAServiceThatHasNotPreviouslyBeenRequested()
        {
            var service = _container.GetInstance<IMockedService>();
            service.ShouldBeOfType<IMockedObject>();
        }

        [Test]
        public void GetTheSameConcreteClassTwiceFromCreate()
        {
            var autoMocker = new RhinoAutoMocker<ConcreteClass>();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
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
        public void TheAutoMockerPushesInMocksAndAPreBuiltStubForAllOfTheConstructorArguments()
        {
            var autoMocker = new RhinoAutoMocker<ConcreteClass>();
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
            var autoMocker = new RhinoAutoMocker<ConcreteClass>();

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
            var mocker = new RhinoAutoMocker<ConcreteClass>();
            mocker.UseConcreteClassFor<ConcreteThing>();

            var thing = mocker.Get<ConcreteThing>();
            Assert.IsInstanceOfType(typeof (ConcreteThing), thing);

            Assert.AreSame(mocker.Get<IMockedService>(), thing.Service);
            Assert.AreSame(mocker.Get<IMockedService2>(), thing.Service2);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClass()
        {
            var autoMocker = new RhinoAutoMocker<ConcreteClass>();

            autoMocker.Get<IMockedService>().Expect(x => x.Name).Return("Jeremy");

            autoMocker.ClassUnderTest.Name.ShouldEqual("Jeremy");
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClassAsAPartialMockAndSetTheNameMethodUp()
        {
            var autoMocker = new RhinoAutoMocker<ConcreteClass>();

            autoMocker.PartialMockTheClassUnderTest();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            concreteClass.Expect(x => x.Name).Return("Max");
            concreteClass.Name.ShouldEqual("Max");
        }

        [Test]
        public void TheAutoMockerOptionallyPushesInMocksInReplayModeToAllowForAAAsyntax()
        {
            var autoMocker = new RhinoAutoMocker<ConcreteClass>(MockMode.AAA);

            autoMocker.ClassUnderTest.CallService();

            autoMocker.Get<IMockedService>().AssertWasCalled(s => s.Go());
        }

        public interface IAnotherService
        {
            
        }

    }
}