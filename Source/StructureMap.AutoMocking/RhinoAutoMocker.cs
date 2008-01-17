using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks;
using StructureMap.Graph;

namespace StructureMap.AutoMocking
{
    public delegate void GenericVoidMethod<TARGETCLASS>(TARGETCLASS target);

    public delegate void VoidMethod();


    public class RhinoAutoMocker<TARGETCLASS> : MockRepository where TARGETCLASS : class
    {
        private readonly AutoMockedInstanceManager _manager;

        public RhinoAutoMocker()
        {
            RhinoMocksServiceLocator locator = new RhinoMocksServiceLocator(this);
            _manager = new AutoMockedInstanceManager(locator);
        }

        public void MockObjectFactory()
        {
            ObjectFactory.ReplaceManager(_manager);
        }

        public TARGETCLASS Create()
        {
            return _manager.FillDependencies<TARGETCLASS>();
        }

        public TARGETCLASS CreatePartialMocked()
        {
            return PartialMock<TARGETCLASS>(getConstructorArgs());
        }

        private object[] getConstructorArgs()
        {
            ConstructorInfo ctor = Plugin.GetGreediestConstructor(typeof (TARGETCLASS));
            List<object> list = new List<object>();
            foreach (ParameterInfo parameterInfo in ctor.GetParameters())
            {
                Type dependencyType = parameterInfo.ParameterType;
                object dependency = _manager.CreateInstance(dependencyType);
                list.Add(dependency);
            }

            return list.ToArray();
        }

        public T Service<T>()
        {
            return _manager.CreateInstance<T>();
        }

        public void InjectStub<T>(T stub)
        {
            _manager.InjectStub<T>(stub);
        }
    }
}