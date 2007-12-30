using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using StructureMap.AutoMocking;

namespace StructureMap.Testing.AutoMocking
{
    [TestFixture]
    public class RhinoAutoMockerTester
    {
        private MockRepository _mocks;
        private RhinoMocksServiceLocator _locator;
        private AutoMockedInstanceManager _instanceManager;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _locator = new RhinoMocksServiceLocator(_mocks);
            _instanceManager = new AutoMockedInstanceManager(_locator);
        }

        [Test]
        public void GetAFullMockForAServiceThatHasNotPreviouslyBeenRequested()
        {
            IMockedService service = _instanceManager.CreateInstance<IMockedService>();

            
            Assert.IsNotNull(service);
            IMockedObject instance = (IMockedObject)service;
            Assert.AreSame(_mocks, instance.Repository);
        }

        [Test]
        public void RequestTheServiceTwiceAndGetTheExactSameMockObject()
        {
            IMockedService service = _instanceManager.CreateInstance<IMockedService>();
            Assert.AreSame(service, _instanceManager.CreateInstance<IMockedService>());
            Assert.AreSame(service, _instanceManager.CreateInstance<IMockedService>());
            Assert.AreSame(service, _instanceManager.CreateInstance<IMockedService>());
            Assert.AreSame(service, _instanceManager.CreateInstance<IMockedService>());
        }

        [Test]
        public void InjectAStubAndGetTheStubBack()
        {
            StubService stub = new StubService();
            _instanceManager.InjectStub<IMockedService>(stub);

            Assert.AreSame(stub, _instanceManager.CreateInstance<IMockedService>());
            Assert.AreSame(stub, _instanceManager.CreateInstance<IMockedService>());
            Assert.AreSame(stub, _instanceManager.CreateInstance<IMockedService>());
        }

        [Test]
        public void AutoFillAConcreteClassWithMocks()
        {
            IMockedService service = _instanceManager.CreateInstance<IMockedService>();
            IMockedService2 service2 = _instanceManager.CreateInstance<IMockedService2>();
            IMockedService3 service3 = _instanceManager.CreateInstance<IMockedService3>();

            ConcreteClass concreteClass = _instanceManager.FillDependencies<ConcreteClass>();
            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClass()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();
            
            using (autoMocker.Record())
            {
                Expect.Call(autoMocker.Service<IMockedService>().Name).Return("Jeremy");
            }

            Assert.AreEqual("Jeremy", autoMocker.Create().Name);
        }

        [Test]
        public void UseTheAutoMockerToStartUpTheConcreteClassAsAPartialMockAndSetTheNameMethodUp()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();

            ConcreteClass concreteClass = autoMocker.CreatePartialMocked();

            using (autoMocker.Record())
            {
                Expect.Call(concreteClass.Name).Return("Max");
            }

            Assert.AreEqual("Max", concreteClass.Name);
        }

        [Test]
        public void TheAutoMockerPushesInMocksForAllOfTheConstructorArgumentsForAPartialMock()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();

            IMockedService service = autoMocker.Service<IMockedService>();
            IMockedService2 service2 = autoMocker.Service<IMockedService2>();
            IMockedService3 service3 = autoMocker.Service<IMockedService3>();

            ConcreteClass concreteClass = autoMocker.CreatePartialMocked();

            Assert.AreSame(service, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
        }


        [Test]
        public void TheAutoMockerPushesInMocksAndAPreBuiltStubForAllOfTheConstructorArguments()
        {
            RhinoAutoMocker<ConcreteClass> autoMocker = new RhinoAutoMocker<ConcreteClass>();
            StubService stub = new StubService();
            autoMocker.InjectStub<IMockedService>(stub);
            
            IMockedService2 service2 = autoMocker.Service<IMockedService2>();
            IMockedService3 service3 = autoMocker.Service<IMockedService3>();

            ConcreteClass concreteClass = autoMocker.Create();

            Assert.AreSame(stub, concreteClass.Service);
            Assert.AreSame(service2, concreteClass.Service2);
            Assert.AreSame(service3, concreteClass.Service3);
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
                get
                {
                    return _service.Name;
                }
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
            void Go();
            string Name { get;}
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


            public string Name
            {
                get { return _name; }
            }

            public void Go()
            {
                throw new NotImplementedException();
            }
        }
    }


}
