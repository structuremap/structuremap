using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Rhino.Mocks.Interfaces;
using StructureMap.Graph;
using System.Linq;
using StructureMap.TypeRules;

namespace StructureMap.AutoMocking
{
    public interface IAutoMocker<TTargetClass> where TTargetClass : class
    {
        /// <summary>
        ///Gets an instance of the ClassUnderTest with mock objects (or stubs) pushed in for all of its dependencies
        /// </summary>
        TTargetClass ClassUnderTest { get; }

        /// <summary>
        /// Accesses the underlying AutoMockedContainer
        /// </summary>
        AutoMockedContainer Container { get; }

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
    /// <typeparam name="TTargetClass"></typeparam>
    public class AutoMocker<TTargetClass> : IAutoMocker<TTargetClass> where TTargetClass : class
    {
        private TTargetClass _classUnderTest;
        protected AutoMockedContainer _container;
        protected ServiceLocator _serviceLocator;

        /// <summary>
        ///Gets an instance of the ClassUnderTest with mock objects (or stubs) pushed in for all of its dependencies
        /// </summary>
        public TTargetClass ClassUnderTest
        {
            get
            {
                if (_classUnderTest == null)
                {
                    _classUnderTest = _container.GetInstance<TTargetClass>();
                }

                return _classUnderTest;
            }
        }

        /// <summary>
        /// Accesses the underlying AutoMockedContainer
        /// </summary>
        public AutoMockedContainer Container { get { return _container; } }

        /// <summary>
        /// Calling this method will immediately create a "Partial" mock
        /// for the ClassUnderTest using the "Greediest" constructor.
        /// </summary>
        public void PartialMockTheClassUnderTest()
        {
            _classUnderTest = _serviceLocator.PartialMock<TTargetClass>(getConstructorArgs());
        }

        /// <summary>
        /// Gets the mock object for type T that would be injected into the constructor function
        /// of the ClassUnderTest
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
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
                    x.For(typeof (T)).Add(t);
                }
            });
        }

        private object[] getConstructorArgs()
        {
            ConstructorInfo ctor = Constructor.GetGreediestConstructor(typeof (TTargetClass));
            var list = new List<object>();
            foreach (ParameterInfo parameterInfo in ctor.GetParameters())
            {
                Type dependencyType = parameterInfo.ParameterType;

                if (dependencyType.IsArray)
                {
                    var builder = typeof (ArrayBuilder<>).CloseAndBuildAs<IEnumerableBuilder>(_container, dependencyType.GetElementType());
                    list.Add(builder.ToEnumerable());
                }
                else if (dependencyType.Closes(typeof (IEnumerable<>)))
                {
                    var @interface = dependencyType.FindFirstInterfaceThatCloses(typeof (IEnumerable<>));
                    var elementType = @interface.GetGenericArguments().First();

                    var builder = typeof (EnumerableBuilder<>).CloseAndBuildAs<IEnumerableBuilder>(_container, elementType);
                    list.Add(builder.ToEnumerable());
                }
                else
                {
                    object dependency = _container.GetInstance(dependencyType);
                    list.Add(dependency);
                }
            }

            return list.ToArray();
        }




    }

    public interface IEnumerableBuilder
    {
        object ToEnumerable();
    }

    public class ArrayBuilder<T> : IEnumerableBuilder
    {
        private readonly IContainer _container;

        public ArrayBuilder(IContainer container)
        {
            _container = container;
        }

        public object ToEnumerable()
        {
            return _container.GetAllInstances<T>().ToArray();
        }
    }

    public class EnumerableBuilder<T> : IEnumerableBuilder
    {
        private readonly IContainer _container;

        public EnumerableBuilder(IContainer container)
        {
            _container = container;
        }

        public object ToEnumerable()
        {
            return _container.GetAllInstances<T>();
        }
    }


}