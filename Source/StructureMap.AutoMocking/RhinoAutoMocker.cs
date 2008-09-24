using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks;
using StructureMap.Graph;

namespace StructureMap.AutoMocking
{
    public delegate void GenericVoidMethod<TARGETCLASS>(TARGETCLASS target);

    public delegate void VoidMethod();

    // Note that it subclasses the RhinoMocks.MockRepository class
    public class RhinoAutoMocker<TARGETCLASS> : MockRepository where TARGETCLASS : class
    {
        private readonly AutoMockedContainer _container;
        private TARGETCLASS _classUnderTest;

        public RhinoAutoMocker()
        {
            RhinoMocksServiceLocator locator = new RhinoMocksServiceLocator(this);
            _container = new AutoMockedContainer(locator);
        }

        // Replaces the inner Container in ObjectFactory with the mocked
        // Container from the auto mocking container.  This will make ObjectFactory
        // return mocks for everything.  Use cautiously!!!!!!!!!!!!!!!

        // Gets the ClassUnderTest with mock objects (or stubs) pushed in
        // for all of its dependencies
        public TARGETCLASS ClassUnderTest
        {
            get
            {
                if (_classUnderTest == null)
                {
                    _classUnderTest = _container.FillDependencies<TARGETCLASS>();
                }

                return _classUnderTest;
            }
        }

        public void MockObjectFactory()
        {
            ObjectFactory.ReplaceManager(_container);
        }

        // I find it useful from time to time to use partial mocks for the ClassUnderTest
        // Especially in Presenter testing
        public void PartialMockTheClassUnderTest()
        {
            _classUnderTest = PartialMock<TARGETCLASS>(getConstructorArgs());
        }

        private object[] getConstructorArgs()
        {
            ConstructorInfo ctor = Constructor.GetGreediestConstructor(typeof (TARGETCLASS));
            List<object> list = new List<object>();
            foreach (ParameterInfo parameterInfo in ctor.GetParameters())
            {
                Type dependencyType = parameterInfo.ParameterType;

                if (dependencyType.IsArray)
                {
                    var values = _container.GetAllInstances(dependencyType.GetElementType());
                    var array = Array.CreateInstance(dependencyType.GetElementType(), values.Count);
                    values.CopyTo(array, 0);

                    list.Add(array);
                    
                }
                else
                {
                    object dependency = _container.GetInstance(dependencyType);
                    list.Add(dependency);
                }


            }

            return list.ToArray();
        }

        // Get one of the mock objects that are injected into the constructor function
        // of the ClassUnderTest
        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        // Set the auto mocking container to use a Stub for Type T
        public void InjectStub<T>(T stub)
        {
            _container.Inject<T>(stub);
        }

        public void Inject(Type pluginType, object stub)
        {
            _container.Inject(pluginType, stub);
        }

        public void Inject<T>(T target)
        {
            _container.Inject<T>(target);
        }

        public T AddAdditionalMockFor<T>()
        {
            T mock = DynamicMock<T>();
            _container.Configure(r => r.InstanceOf<T>().Is.Object(mock));

            return mock;
        }

        // So that Aaron Jensen can use his concrete HubService object
        // Construct whatever T is with all mocks, and make sure that the
        // ClassUnderTest gets built with a concrete T
        public void UseConcreteClassFor<T>()
        {
            T concreteClass = _container.FillDependencies<T>();
            _container.Inject(concreteClass);
        }

        public AutoMockedContainer Container
        {
            get { return _container; }
        }

        public T[] CreateMockArrayFor<T>(int count)
        {
            T[] returnValue = new T[count];
            
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = DynamicMock<T>();
            }

            InjectArray<T>(returnValue);

            return returnValue;
        }

        public void InjectArray<T>(T[] stubs)
        {
            _container.EjectAllInstancesOf<T>();
            _container.Configure(x =>
            {
                foreach (var t in stubs)
                {
                    x.InstanceOf<T>().Is.Object(t);
                }
            });
        }
    }
}