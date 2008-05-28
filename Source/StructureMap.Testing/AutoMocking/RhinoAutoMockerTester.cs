using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using StructureMap.AutoMocking;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class RhinoAutoMockerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _locator = new RhinoMocksServiceLocator(_mocks);
            _container = new AutoMockedContainer(_locator);
        }

        #endregion

        private MockRepository _mocks;
        private RhinoMocksServiceLocator _locator;
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

        [Test]
        public void AutoFillAConcreteClassWithMocks()
        {
            IMockedService service = _container.GetInstance<IMockedService>();
            IMockedService2 service2 = _container.GetInstance<IMockedService2>();
            IMockedService3 service3 = _container.GetInstance<IMockedService3>();

            ConcreteClass concreteClass = _container.FillDependencies<ConcreteClass>();
            
            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void GetAFullMockForAServiceThatHasNotPreviouslyBeenRequested()
        {
            IMockedService service = _container.GetInstance<IMockedService>();


            Assert.IsNotNull(service);
            IMockedObject instance = (IMockedObject) service;
            Assert.AreSame(_mocks, instance.Repository);
        }

        [Test]
        public void GetTheSameConcreteClassTwiceFromCreate()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
            Assert.AreSame(concreteClass, autoMocker.ClassUnderTest);
        }

        [Test]
        public void InjectAStubAndGetTheStubBack()
        {
            StubService stub = new StubService();
            _container.InjectStub<IMockedService>(stub);

            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
            Assert.AreSame(stub, _container.GetInstance<IMockedService>());
        }

        [Test]
        public void RequestTheServiceTwiceAndGetTheExactSameMockObject()
        {
            IMockedService service = _container.GetInstance<IMockedService>();
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
            Assert.AreSame(service, _container.GetInstance<IMockedService>());
        }

        [Test]
        public void TheAutoMockerPushesInMocksAndAPreBuiltStubForAllOfTheConstructorArguments()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();
            StubService stub = new StubService();
            autoMocker.InjectStub<IMockedService>(stub);

            IMockedService2 service2 = autoMocker.Get<IMockedService2>();
            IMockedService3 service3 = autoMocker.Get<IMockedService3>();

            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(stub, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void TheAutoMockerPushesInMocksForAllOfTheConstructorArgumentsForAPartialMock()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();

            IMockedService service = autoMocker.Get<IMockedService>();
            IMockedService2 service2 = autoMocker.Get<IMockedService2>();
            IMockedService3 service3 = autoMocker.Get<IMockedService3>();

            autoMocker.PartialMockTheClassUnderTest();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void UseConcreteClassFor()
        {
            RhinoAutoMocker<ConcreteClass> mocker = new RhinoAutoMocker<ConcreteClass>();
            mocker.UseConcreteClassFor<ConcreteThing>();

            ConcreteThing thing = mocker.Get<ConcreteThing>();
            Assert.IsInstanceOfType(typeof (ConcreteThing), thing);

            Assert.AreSame(mocker.Get<IMockedService>(), thing.Service);
            Assert.AreSame(mocker.Get<IMockedService2>(), thing.Service2);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClass()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();

            using (autoMocker.Record())
            {
                Expect.Call(autoMocker.Get<IMockedService>().Name).Return("Jeremy");
            }

            Assert.AreEqual("Jeremy", autoMocker.ClassUnderTest.Name);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClassAsAPartialMockAndSetTheNameMethodUp()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();

            autoMocker.PartialMockTheClassUnderTest();
            ConcreteClass concreteClass = autoMocker.ClassUnderTest;

            using (autoMocker.Record())
            {
                Expect.Call(concreteClass.Name).Return("Max");
            }

            Assert.AreEqual("Max", concreteClass.Name);
        }
    }
}