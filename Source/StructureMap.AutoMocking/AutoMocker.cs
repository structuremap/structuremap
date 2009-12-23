using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Graph;
using System.Linq;
using StructureMap.Pipeline;

namespace StructureMap.AutoMocking
{
    public interface IAutoMocker<TARGETCLASS> where TARGETCLASS : class
    {
        /// <summary>
        ///Gets an instance of the ClassUnderTest with mock objects (or stubs) pushed in for all of its dependencies
        /// </summary>
        TARGETCLASS ClassUnderTest { get; }

        /// <summary>
        /// Accesses the underlying AutoMockedContainer
        /// </summary>
        AutoMockedContainer Container { get; }

        /// <summary>
        /// Use this with EXTREME caution.  This will replace the active "Container" in accessed
        /// by ObjectFactory with the AutoMockedContainer from this instance
        /// </summary>
        void MockObjectFactory();

        /// <summary>
        /// Calling this method will immediately create a "Partial" mock
        /// for the ClassUnderTest using the "Greediest" constructor.
        /// </summary>
        void PartialMockTheClassUnderTest();

        /// <summary>
        /// Gets the mock object for type T that would be injected into the constructor function
        /// of the ClassUnderTest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        /// <summary>
        /// Method to specify the exact object that will be used for 
        /// "pluginType."  Useful for stub objects and/or static mocks
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        void Inject(Type pluginType, object stub);

        /// <summary>
        /// Method to specify the exact object that will be used for 
        /// "pluginType."  Useful for stub objects and/or static mocks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        void Inject<T>(T target);

        /// <summary>
        /// Adds an additional mock object for a given T
        /// Useful for array arguments to the ClassUnderTest
        /// object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T AddAdditionalMockFor<T>() where T : class;

        /// <summary>
        /// So that Aaron Jensen can use his concrete HubService object
        /// Construct whatever T is with all mocks, and make sure that the
        /// ClassUnderTest gets built with a concrete T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void UseConcreteClassFor<T>();

        /// <summary>
        /// Creates, returns, and registers an array of mock objects for type T.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        T[] CreateMockArrayFor<T>(int count) where T : class;

        /// <summary>
        /// Allows you to "inject" an array of known objects for an 
        /// argument of type T[] in the ClassUnderTest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stubs"></param>
        void InjectArray<T>(T[] stubs);
    }

    /// <summary>
    /// The Auto Mocking Container for StructureMap
    /// </summary>
    /// <typeparam name="TARGETCLASS"></typeparam>
    public class AutoMocker<TARGETCLASS> : IAutoMocker<TARGETCLASS> where TARGETCLASS : class
    {
        protected AutoMockedContainer _container;
        private TARGETCLASS _classUnderTest;
        protected ServiceLocator _serviceLocator;

        /// <summary>
        ///Gets an instance of the ClassUnderTest with mock objects (or stubs) pushed in for all of its dependencies
        /// </summary>
        public TARGETCLASS ClassUnderTest
        {
            get
            {
                if (_classUnderTest == null)
                {
                    _classUnderTest = _container.GetInstance<TARGETCLASS>();
                }

                return _classUnderTest;
            }
        }

        /// <summary>
        /// Accesses the underlying AutoMockedContainer
        /// </summary>
        public AutoMockedContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Use this with EXTREME caution.  This will replace the active "Container" in accessed
        /// by ObjectFactory with the AutoMockedContainer from this instance
        /// </summary>
        public void MockObjectFactory()
        {
            ObjectFactory.ReplaceManager(_container);
        }

        /// <summary>
        /// Calling this method will immediately create a "Partial" mock
        /// for the ClassUnderTest using the "Greediest" constructor.
        /// </summary>
        public void PartialMockTheClassUnderTest()
        {
            _classUnderTest = _serviceLocator.PartialMock<TARGETCLASS>(getConstructorArgs());
        }

        private object[] getConstructorArgs()
        {
            ConstructorInfo ctor = Constructor.GetGreediestConstructor(typeof (TARGETCLASS));
            var list = new List<object>();
            foreach (ParameterInfo parameterInfo in ctor.GetParameters())
            {
                Type dependencyType = parameterInfo.ParameterType;

                if (dependencyType.IsArray)
                {
                    IList values = _container.GetAllInstances(dependencyType.GetElementType());
                    Array array = Array.CreateInstance(dependencyType.GetElementType(), values.Count);
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

        /// <summary>
        /// Gets the mock object for type T that would be injected into the constructor function
        /// of the ClassUnderTest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            if (!_container.Model.HasDefaultImplementationFor(typeof(T)))
            {
                _container.Inject<T>(_serviceLocator.Service<T>());
            }

            return _container.GetInstance<T>();
        }

        /// <summary>
        /// Method to specify the exact object that will be used for 
        /// "pluginType."  Useful for stub objects and/or static mocks
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="stub"></param>
        public void Inject(Type pluginType, object stub)
        {
            _container.Inject(pluginType, stub);
        }

        /// <summary>
        /// Method to specify the exact object that will be used for 
        /// "pluginType."  Useful for stub objects and/or static mocks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void Inject<T>(T target)
        {
            _container.Inject(target);
        }

        /// <summary>
        /// Adds an additional mock object for a given T
        /// Useful for array arguments to the ClassUnderTest
        /// object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddAdditionalMockFor<T>() where T : class
        {
            var mock = _serviceLocator.Service<T>();
            _container.Configure(r => r.For(typeof (T)).Add(mock));

            return mock;
        }

        /// <summary>
        /// So that Aaron Jensen can use his concrete HubService object
        /// Construct whatever T is with all mocks, and make sure that the
        /// ClassUnderTest gets built with a concrete T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseConcreteClassFor<T>()
        {
            var concreteClass = _container.GetInstance<T>();
            _container.Inject(concreteClass);
        }

        /// <summary>
        /// Creates, returns, and registers an array of mock objects for type T.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] CreateMockArrayFor<T>(int count) where T : class
        {
            var returnValue = new T[count];

            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = _serviceLocator.Service<T>();
            }

            InjectArray(returnValue);

            return returnValue;
        }

        /// <summary>
        /// Allows you to "inject" an array of known objects for an 
        /// argument of type T[] in the ClassUnderTest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stubs"></param>
        public void InjectArray<T>(T[] stubs)
        {
            _container.EjectAllInstancesOf<T>();
            _container.Configure(x =>
            {
                foreach (T t in stubs)
                {
                    x.For(typeof(T)).Add(t);
                }
            });
        }
    }
}